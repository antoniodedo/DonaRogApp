using DonaRogApp.Enums.Communications;
using System;

namespace DonaRogApp.LetterTemplates.Dto
{
    /// <summary>
    /// Input DTO for automatic template selection
    /// </summary>
    public class SelectTemplateInput
    {
        public TemplateCategory Category { get; set; }
        public string Language { get; set; } = "it";
        public decimal DonationAmount { get; set; }
        public bool IsNewDonor { get; set; }
        public bool IsPlural { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? RecurrenceId { get; set; }
        public CommunicationType? PreferredCommunicationType { get; set; }
    }
}
