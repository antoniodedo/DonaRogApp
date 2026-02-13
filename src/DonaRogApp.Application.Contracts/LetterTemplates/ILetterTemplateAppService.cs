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
    }
}





