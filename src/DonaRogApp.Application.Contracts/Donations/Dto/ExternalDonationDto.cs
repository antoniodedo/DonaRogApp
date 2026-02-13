using DonaRogApp.Enums.Donations;
using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Donations.Dto
{
    /// <summary>
    /// DTO for importing donations from external microservices (demo purposes)
    /// </summary>
    public class ExternalDonationDto
    {
        [Required]
        [StringLength(100)]
        public string ExternalId { get; set; } = string.Empty;

        [Required]
        public DonationChannel Channel { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DonationDate { get; set; }

        public DateTime? CreditDate { get; set; }

        /// <summary>
        /// Donor reference: could be donor code, email, or partial data
        /// Used to identify or create donor
        /// </summary>
        [StringLength(200)]
        public string? DonorReference { get; set; }

        /// <summary>
        /// If DonorReference is a code, try to find existing donor
        /// Otherwise, this will require manual verification
        /// </summary>
        public Guid? DonorId { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}
