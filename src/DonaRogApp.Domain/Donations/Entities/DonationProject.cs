using DonaRogApp.Domain.Projects.Entities;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace DonaRogApp.Domain.Donations.Entities
{
    /// <summary>
    /// DonationProject - Many-to-many relationship with allocation amount
    /// 
    /// Represents the allocation of a donation amount to a specific project.
    /// A donation can be split across multiple projects.
    /// 
    /// Example:
    /// Donation of 1000€ can be allocated as:
    /// - 600€ to Project A (Education)
    /// - 400€ to Project B (Health)
    /// </summary>
    public class DonationProject : Entity
    {
        /// <summary>
        /// Donation ID (composite key)
        /// </summary>
        public Guid DonationId { get; private set; }

        /// <summary>
        /// Project ID (composite key)
        /// </summary>
        public Guid ProjectId { get; private set; }

        /// <summary>
        /// Amount allocated to this project
        /// Must be positive and sum of all allocations must not exceed donation total
        /// </summary>
        public decimal AllocatedAmount { get; private set; }

        // ======================================================================
        // NAVIGATION PROPERTIES
        // ======================================================================
        /// <summary>
        /// Navigation property: Donation
        /// </summary>
        public virtual Donation Donation { get; private set; }

        /// <summary>
        /// Navigation property: Project
        /// </summary>
        public virtual Project Project { get; private set; }

        // ======================================================================
        // CONSTRUCTOR
        // ======================================================================
        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private DonationProject()
        {
        }

        /// <summary>
        /// Constructor for creating new allocation
        /// </summary>
        public DonationProject(Guid donationId, Guid projectId, decimal allocatedAmount)
        {
            DonationId = Check.NotNull(donationId, nameof(donationId));
            ProjectId = Check.NotNull(projectId, nameof(projectId));
            AllocatedAmount = Check.Positive(allocatedAmount, nameof(allocatedAmount));
        }

        // ======================================================================
        // COMPOSITE KEY
        // ======================================================================
        /// <summary>
        /// Override GetKeys for composite key (DonationId + ProjectId)
        /// </summary>
        public override object[] GetKeys()
        {
            return new object[] { DonationId, ProjectId };
        }

        // ======================================================================
        // METHODS
        // ======================================================================
        /// <summary>
        /// Update allocated amount
        /// </summary>
        public void UpdateAmount(decimal newAmount)
        {
            AllocatedAmount = Check.Positive(newAmount, nameof(newAmount));
        }
    }
}
