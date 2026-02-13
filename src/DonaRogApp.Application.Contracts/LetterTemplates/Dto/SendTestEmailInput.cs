using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.LetterTemplates.Dto
{
    /// <summary>
    /// Input DTO for sending test email
    /// </summary>
    public class SendTestEmailInput
    {
        [Required]
        public Guid TemplateId { get; set; }
        
        [Required]
        [EmailAddress]
        public string TestEmail { get; set; } = null!;
        
        /// <summary>
        /// Optional donor ID to use real donor data
        /// </summary>
        public Guid? DonorId { get; set; }
        
        /// <summary>
        /// Manual tag values (used if DonorId is not provided)
        /// </summary>
        public Dictionary<string, string> TagValues { get; set; } = new();
    }
}
