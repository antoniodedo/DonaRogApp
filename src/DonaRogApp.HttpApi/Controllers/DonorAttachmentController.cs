using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DonaRogApp.Application.Contracts.Donors;
using DonaRogApp.Application.Contracts.Donors.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace DonaRogApp.Controllers
{
    [Area("app")]
    [RemoteService(Name = "DonaRogApp")]
    [Route("api/app/donor-attachments")]
    public class DonorAttachmentController : AbpControllerBase
    {
        private readonly IDonorAttachmentAppService _attachmentAppService;

        public DonorAttachmentController(IDonorAttachmentAppService attachmentAppService)
        {
            _attachmentAppService = attachmentAppService;
        }

        [HttpGet]
        [Route("by-donor/{donorId}")]
        public async Task<ListResultDto<DonorAttachmentDto>> GetListByDonorAsync(Guid donorId)
        {
            return await _attachmentAppService.GetListByDonorAsync(donorId);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<DonorAttachmentDto> GetAsync(Guid id)
        {
            return await _attachmentAppService.GetAsync(id);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<DonorAttachmentDto> CreateAsync([FromForm] CreateDonorAttachmentDto input, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new UserFriendlyException("File is required");
            }

            await using var stream = file.OpenReadStream();
            var remoteStreamContent = new RemoteStreamContent(stream, file.FileName, file.ContentType, file.Length);

            return await _attachmentAppService.CreateAsync(input, remoteStreamContent);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<DonorAttachmentDto> UpdateAsync(Guid id, [FromBody] UpdateDonorAttachmentDto input)
        {
            return await _attachmentAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _attachmentAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("{id}/download")]
        public async Task<IActionResult> DownloadAsync(Guid id)
        {
            var result = await _attachmentAppService.DownloadAsync(id);
            return File(result.GetStream(), result.ContentType ?? "application/octet-stream", result.FileName);
        }

        [HttpPost]
        [Route("reorder")]
        public async Task ReorderAsync([FromQuery] Guid donorId, [FromBody] List<Guid> attachmentIds)
        {
            await _attachmentAppService.ReorderAsync(donorId, attachmentIds);
        }
    }
}
