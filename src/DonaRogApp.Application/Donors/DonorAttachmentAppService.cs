using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DonaRogApp.Application.Contracts.Donors;
using DonaRogApp.Application.Contracts.Donors.Dto;
using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Application.Donors
{
    [Authorize(DonaRogAppPermissions.Donors.Default)]
    public class DonorAttachmentAppService : ApplicationService, IDonorAttachmentAppService
    {
        private const string DonorAttachmentContainerName = "donor-attachments";
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

        private readonly IRepository<DonorAttachment, Guid> _attachmentRepository;
        private readonly IRepository<Donor, Guid> _donorRepository;
        private readonly IBlobContainer _blobContainer;

        public DonorAttachmentAppService(
            IRepository<DonorAttachment, Guid> attachmentRepository,
            IRepository<Donor, Guid> donorRepository,
            IBlobContainer blobContainer)
        {
            _attachmentRepository = attachmentRepository;
            _donorRepository = donorRepository;
            _blobContainer = blobContainer;
        }

        public async Task<ListResultDto<DonorAttachmentDto>> GetListByDonorAsync(Guid donorId)
        {
            var attachments = await _attachmentRepository.GetListAsync(x => x.DonorId == donorId);
            
            var dtos = attachments
                .OrderBy(x => x.DisplayOrder)
                .ThenByDescending(x => x.CreationTime)
                .Select(MapToDto)
                .ToList();

            return new ListResultDto<DonorAttachmentDto>(dtos);
        }

        public async Task<DonorAttachmentDto> GetAsync(Guid id)
        {
            var attachment = await _attachmentRepository.GetAsync(id);
            return MapToDto(attachment);
        }

        [Authorize(DonaRogAppPermissions.Donors.Edit)]
        public async Task<DonorAttachmentDto> CreateAsync(CreateDonorAttachmentDto input, IRemoteStreamContent file)
        {
            // Validate donor exists
            var donor = await _donorRepository.GetAsync(input.DonorId);

            // Validate file
            if (file == null || file.ContentLength == 0)
            {
                throw new UserFriendlyException("File is required");
            }

            if (file.ContentLength > MaxFileSizeBytes)
            {
                throw new UserFriendlyException($"File size exceeds maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)}MB");
            }

            // Extract file info
            var fileName = Path.GetFileNameWithoutExtension(file.FileName ?? input.FileName);
            var fileExtension = Path.GetExtension(file.FileName ?? input.FileName);
            var mimeType = file.ContentType ?? "application/octet-stream";

            // Generate unique blob name
            var blobName = $"{input.DonorId}/{Guid.NewGuid()}{fileExtension}";

            // Save to blob storage
            await using var stream = file.GetStream();
            await _blobContainer.SaveAsync(blobName, stream, overrideExisting: false);

            // Create entity
            var attachment = new DonorAttachment(
                id: GuidGenerator.Create(),
                donorId: input.DonorId,
                fileName: fileName + fileExtension,
                fileExtension: fileExtension,
                mimeType: mimeType,
                fileSizeBytes: file.ContentLength ?? 0,
                blobName: blobName,
                tenantId: CurrentTenant.Id,
                attachmentType: input.AttachmentType,
                description: input.Description,
                displayOrder: input.DisplayOrder
            );

            await _attachmentRepository.InsertAsync(attachment, autoSave: true);

            return MapToDto(attachment);
        }

        [Authorize(DonaRogAppPermissions.Donors.Edit)]
        public async Task<DonorAttachmentDto> UpdateAsync(Guid id, UpdateDonorAttachmentDto input)
        {
            var attachment = await _attachmentRepository.GetAsync(id);

            attachment.UpdateAttachmentType(input.AttachmentType);
            attachment.UpdateDescription(input.Description);
            attachment.UpdateDisplayOrder(input.DisplayOrder);

            await _attachmentRepository.UpdateAsync(attachment, autoSave: true);

            return MapToDto(attachment);
        }

        [Authorize(DonaRogAppPermissions.Donors.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            var attachment = await _attachmentRepository.GetAsync(id);

            // Delete from blob storage
            await _blobContainer.DeleteAsync(attachment.BlobName);

            // Delete entity
            await _attachmentRepository.DeleteAsync(attachment, autoSave: true);
        }

        public async Task<IRemoteStreamContent> DownloadAsync(Guid id)
        {
            var attachment = await _attachmentRepository.GetAsync(id);

            var stream = await _blobContainer.GetAsync(attachment.BlobName);

            return new RemoteStreamContent(stream, attachment.FileName, attachment.MimeType);
        }

        [Authorize(DonaRogAppPermissions.Donors.Edit)]
        public async Task ReorderAsync(Guid donorId, List<Guid> attachmentIds)
        {
            var attachments = await _attachmentRepository.GetListAsync(x => x.DonorId == donorId);

            for (int i = 0; i < attachmentIds.Count; i++)
            {
                var attachment = attachments.FirstOrDefault(x => x.Id == attachmentIds[i]);
                if (attachment != null)
                {
                    attachment.UpdateDisplayOrder(i);
                }
            }

            await _attachmentRepository.UpdateManyAsync(attachments, autoSave: true);
        }

        private DonorAttachmentDto MapToDto(DonorAttachment attachment)
        {
            return new DonorAttachmentDto
            {
                Id = attachment.Id,
                DonorId = attachment.DonorId,
                FileName = attachment.FileName,
                FileExtension = attachment.FileExtension,
                MimeType = attachment.MimeType,
                FileSizeBytes = attachment.FileSizeBytes,
                AttachmentType = attachment.AttachmentType,
                Description = attachment.Description,
                DisplayOrder = attachment.DisplayOrder,
                CreationTime = attachment.CreationTime
            };
        }
    }
}
