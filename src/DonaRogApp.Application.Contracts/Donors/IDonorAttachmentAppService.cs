using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DonaRogApp.Application.Contracts.Donors.Dto;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace DonaRogApp.Application.Contracts.Donors
{
    public interface IDonorAttachmentAppService : IApplicationService
    {
        /// <summary>
        /// Get all attachments for a specific donor
        /// </summary>
        Task<ListResultDto<DonorAttachmentDto>> GetListByDonorAsync(Guid donorId);

        /// <summary>
        /// Get a specific attachment by ID
        /// </summary>
        Task<DonorAttachmentDto> GetAsync(Guid id);

        /// <summary>
        /// Upload a new attachment for a donor
        /// </summary>
        Task<DonorAttachmentDto> CreateAsync(CreateDonorAttachmentDto input, IRemoteStreamContent file);

        /// <summary>
        /// Update attachment metadata (description, type, etc.)
        /// </summary>
        Task<DonorAttachmentDto> UpdateAsync(Guid id, UpdateDonorAttachmentDto input);

        /// <summary>
        /// Delete an attachment
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Download an attachment file
        /// </summary>
        Task<IRemoteStreamContent> DownloadAsync(Guid id);

        /// <summary>
        /// Reorder attachments
        /// </summary>
        Task ReorderAsync(Guid donorId, List<Guid> attachmentIds);
    }
}
