using DonaRogApp.Application.Contracts.Communications.Dto;
using DonaRogApp.Enums.Communications;
using DonaRogApp.LetterTemplates.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.LetterTemplates
{
    /// <summary>
    /// Application service interface for Letter Templates
    /// </summary>
    public interface ILetterTemplateAppService :
        ICrudAppService<LetterTemplateDto, Guid, GetLetterTemplatesInput, CreateUpdateLetterTemplateDto>
    {
        // ======================================================================
        // RENDERING AND PREVIEW
        // ======================================================================
        
        /// <summary>
        /// Render a template with provided tag values
        /// </summary>
        Task<string> RenderTemplateAsync(Guid templateId, Dictionary<string, string> tagValues);
        
        /// <summary>
        /// Render a template with real donor data
        /// </summary>
        Task<string> RenderTemplateWithDonorDataAsync(RenderTemplateWithDonorInput input);
        
        // ======================================================================
        // SELECTION AND SUGGESTIONS
        // ======================================================================
        
        /// <summary>
        /// Get suggested templates based on criteria
        /// </summary>
        Task<List<LetterTemplateDto>> GetSuggestedTemplatesAsync(SelectTemplateInput input);
        
        /// <summary>
        /// Get default/fallback template for a category and language
        /// </summary>
        Task<LetterTemplateDto?> GetDefaultTemplateAsync(TemplateCategory category, string language);
        
        // ======================================================================
        // TEMPLATE MANAGEMENT
        // ======================================================================
        
        /// <summary>
        /// Duplicate a template
        /// </summary>
        Task<LetterTemplateDto> DuplicateAsync(Guid templateId);
        
        /// <summary>
        /// Get lightweight list view (without Content for performance)
        /// </summary>
        Task<PagedResultDto<LetterTemplateListDto>> GetListViewAsync(GetLetterTemplatesInput input);
        
        // ======================================================================
        // TEST AND SEND
        // ======================================================================
        
        /// <summary>
        /// Send a test email with template rendering
        /// </summary>
        Task SendTestEmailAsync(SendTestEmailInput input);
        
        // ======================================================================
        // FILE-BASED TEMPLATES (DOCX UPLOAD)
        // ======================================================================
        
        /// <summary>
        /// Upload DOCX template file
        /// </summary>
        Task<TemplateFileDto> UploadTemplateFileAsync(UploadTemplateDto input);
        
        /// <summary>
        /// Download template file
        /// </summary>
        Task<byte[]> DownloadTemplateFileAsync(Guid templateId);
        
        /// <summary>
        /// Get template file information
        /// </summary>
        Task<TemplateFileDto?> GetTemplateFileInfoAsync(Guid templateId);
        
        /// <summary>
        /// Delete template file (keeps template, removes file)
        /// </summary>
        Task DeleteTemplateFileAsync(Guid templateId);
        
        // ======================================================================
        // TEMPLATE CONVERSION
        // ======================================================================
        
        /// <summary>
        /// Convert DOCX template to HTML (with merge fields conversion)
        /// </summary>
        Task<TemplateConversionResultDto> ConvertDocxToHtmlAsync(ConvertTemplateDto input);
        
        // ======================================================================
        // PLACEHOLDER MANAGEMENT
        // ======================================================================
        
        /// <summary>
        /// Get all available placeholders
        /// </summary>
        Task<PlaceholderListDto> GetAvailablePlaceholdersAsync();
        
        /// <summary>
        /// Validate placeholders in template content
        /// </summary>
        Task<List<string>> ValidatePlaceholdersAsync(Guid templateId);
        
        // ======================================================================
        // TEMPLATE PREVIEW
        // ======================================================================
        
        /// <summary>
        /// Preview template with sample data
        /// </summary>
        Task<TemplatePreviewResultDto> PreviewTemplateAsync(PreviewTemplateDto input);
    }
}





