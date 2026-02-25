using DonaRogApp.Application.Contracts.Donations;
using DonaRogApp.Application.Contracts.Donations.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace DonaRogApp.Controllers
{
    [Route("api/donations/{donationId}/documents")]
    [ApiController]
    public class DonationDocumentController : AbpControllerBase
    {
        private readonly IDonationAppService _donationAppService;

        public DonationDocumentController(IDonationAppService donationAppService)
        {
            _donationAppService = donationAppService;
        }

        [HttpGet]
        public async Task<List<DonationDocumentDto>> GetDocuments(Guid donationId)
        {
            return await _donationAppService.GetDocumentsAsync(donationId);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<DonationDocumentDto> UploadDocument(
            Guid donationId,
            [FromForm] IFormFile file,
            [FromForm] int documentType,
            [FromForm] string? notes = null)
        {
            if (file == null || file.Length == 0)
            {
                throw new BusinessException("DonaRog:EmptyFile");
            }

            // Prepare input DTO
            var input = new UploadDonationDocumentDto
            {
                DocumentType = (DonaRogApp.Enums.Donations.DonationDocumentType)documentType,
                Notes = notes
            };

            // Call application service with file stream
            using (var stream = file.OpenReadStream())
            {
                return await _donationAppService.SaveDocumentAsync(
                    donationId,
                    stream,
                    file.FileName,
                    file.ContentType,
                    file.Length,
                    input
                );
            }
        }

        [HttpPost("text")]
        public async Task<DonationDocumentDto> CreateTextDocument(
            Guid donationId,
            [FromBody] CreateTextDocumentDto input)
        {
            return await _donationAppService.SaveTextDocumentAsync(donationId, input);
        }

        [HttpGet("{documentId}")]
        public async Task<IActionResult> DownloadDocument(Guid donationId, Guid documentId)
        {
            var (stream, fileName, mimeType) = await _donationAppService.GetDocumentFileAsync(documentId);
            
            return File(stream, mimeType, fileName);
        }

        [HttpDelete("{documentId}")]
        public async Task DeleteDocument(Guid donationId, Guid documentId)
        {
            await _donationAppService.DeleteDocumentAsync(documentId);
        }
    }
}
