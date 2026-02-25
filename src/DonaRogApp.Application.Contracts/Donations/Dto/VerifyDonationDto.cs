using DonaRogApp.Enums.Communications;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Donations.Dto
{
    public class VerifyDonationDto
    {
        [Required]
        public Guid DonorId { get; set; }

        public Guid? CampaignId { get; set; }

        public Guid? BankAccountId { get; set; }

        // ======================================================================
        // THANK YOU OPTIONS (Enhanced workflow)
        // ======================================================================
        
        /// <summary>
        /// Should create thank you?
        /// null = evaluate using rules (default)
        /// true = force create
        /// false = do not create
        /// </summary>
        public bool? CreateThankYou { get; set; } = null;

        /// <summary>
        /// Preferred channel for this specific donation (overrides donor preference)
        /// </summary>
        public PreferredThankYouChannel? ThankYouChannel { get; set; }

        /// <summary>
        /// Template ID for thank you (overrides rule suggestion)
        /// </summary>
        public Guid? ThankYouTemplateId { get; set; }

        /// <summary>
        /// Reason for not creating thank you (required if CreateThankYou = false)
        /// </summary>
        [StringLength(500)]
        public string? NoThankYouReason { get; set; }

        /// <summary>
        /// Should print immediately? (default = add to batch)
        /// </summary>
        public bool PrintImmediately { get; set; } = false;

        // ======================================================================
        // PROJECT ALLOCATION & NOTES
        // ======================================================================
        
        public List<DonationProjectAllocationDto> ProjectAllocations { get; set; } = new List<DonationProjectAllocationDto>();

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(1000)]
        public string? InternalNotes { get; set; }
    }
}

