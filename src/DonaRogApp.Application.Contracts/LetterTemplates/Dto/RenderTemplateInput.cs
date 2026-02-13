using System;
using System.Collections.Generic;

namespace DonaRogApp.LetterTemplates.Dto
{
    /// <summary>
    /// Input DTO for rendering a template with tag values
    /// </summary>
    public class RenderTemplateInput
    {
        public Guid TemplateId { get; set; }
        public Dictionary<string, string> TagValues { get; set; } = new();
    }
    
    /// <summary>
    /// Input DTO for rendering a template with donor data
    /// </summary>
    public class RenderTemplateWithDonorInput
    {
        public Guid TemplateId { get; set; }
        public Guid DonorId { get; set; }
        public Guid? DonationId { get; set; }
        
        /// <summary>
        /// Additional tag overrides
        /// </summary>
        public Dictionary<string, string> AdditionalTags { get; set; } = new();
    }
}
