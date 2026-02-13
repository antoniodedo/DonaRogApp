using System;
using System.Linq;
using Volo.Abp;

namespace DonaRogApp.Domain.Donations.Entities
{
    public partial class Donation
    {
        /// <summary>
        /// Allocate amount to a project
        /// </summary>
        public void AllocateToProject(Guid projectId, decimal amount)
        {
            Check.NotNull(projectId, nameof(projectId));
            Check.Positive(amount, nameof(amount));

            // Check if already allocated to this project
            var existing = Projects.FirstOrDefault(p => p.ProjectId == projectId);
            if (existing != null)
            {
                throw new BusinessException("DonaRog:DonationAlreadyAllocatedToProject")
                    .WithData("donationId", Id)
                    .WithData("projectId", projectId);
            }

            // Check if allocation would exceed total amount
            var currentlyAllocated = GetTotalAllocatedAmount();
            if (currentlyAllocated + amount > TotalAmount)
            {
                throw new BusinessException("DonaRog:DonationAllocationWouldExceedTotalAmount")
                    .WithData("donationId", Id)
                    .WithData("totalAmount", TotalAmount)
                    .WithData("currentlyAllocated", currentlyAllocated)
                    .WithData("attemptedAllocation", amount)
                    .WithData("available", TotalAmount - currentlyAllocated);
            }

            var allocation = new DonationProject(Id, projectId, amount);
            Projects.Add(allocation);

            // Raise event (only for verified donations, to update project statistics)
            if (IsVerified())
            {
                AddLocalEvent(new Domain.Donations.Events.DonationProjectAllocatedEvent(
                    Id,
                    projectId,
                    amount,
                    Status
                ));
            }
        }

        /// <summary>
        /// Update allocation amount for a project
        /// </summary>
        public void UpdateProjectAllocation(Guid projectId, decimal newAmount)
        {
            Check.NotNull(projectId, nameof(projectId));
            Check.Positive(newAmount, nameof(newAmount));

            var allocation = Projects.FirstOrDefault(p => p.ProjectId == projectId);
            if (allocation == null)
            {
                throw new BusinessException("DonaRog:DonationProjectAllocationNotFound")
                    .WithData("donationId", Id)
                    .WithData("projectId", projectId);
            }

            // Check if new allocation would exceed total amount
            var otherAllocations = GetTotalAllocatedAmount() - allocation.AllocatedAmount;
            if (otherAllocations + newAmount > TotalAmount)
            {
                throw new BusinessException("DonaRog:DonationAllocationWouldExceedTotalAmount")
                    .WithData("donationId", Id)
                    .WithData("totalAmount", TotalAmount)
                    .WithData("otherAllocations", otherAllocations)
                    .WithData("newAmount", newAmount)
                    .WithData("available", TotalAmount - otherAllocations);
            }

            var oldAmount = allocation.AllocatedAmount;
            allocation.UpdateAmount(newAmount);

            // Raise event (only for verified donations, to update project statistics)
            if (IsVerified())
            {
                AddLocalEvent(new Domain.Donations.Events.DonationProjectAllocationUpdatedEvent(
                    Id,
                    projectId,
                    oldAmount,
                    newAmount,
                    Status
                ));
            }
        }

        /// <summary>
        /// Remove project allocation
        /// </summary>
        public void RemoveProjectAllocation(Guid projectId)
        {
            var allocation = Projects.FirstOrDefault(p => p.ProjectId == projectId);
            if (allocation == null)
            {
                throw new BusinessException("DonaRog:DonationProjectAllocationNotFound")
                    .WithData("donationId", Id)
                    .WithData("projectId", projectId);
            }

            var amount = allocation.AllocatedAmount;
            Projects.Remove(allocation);

            // Raise event (only for verified donations, to update project statistics)
            if (IsVerified())
            {
                AddLocalEvent(new Domain.Donations.Events.DonationProjectAllocationRemovedEvent(
                    Id,
                    projectId,
                    amount,
                    Status
                ));
            }
        }

        /// <summary>
        /// Clear all project allocations
        /// </summary>
        public void ClearProjectAllocations()
        {
            Projects.Clear();

            // TODO: Raise AllProjectAllocationsRemovedEvent
        }

        /// <summary>
        /// Set multiple project allocations at once
        /// Replaces existing allocations
        /// </summary>
        public void SetProjectAllocations(params (Guid ProjectId, decimal Amount)[] allocations)
        {
            // Validate total doesn't exceed donation amount
            var total = allocations.Sum(a => a.Amount);
            if (total > TotalAmount)
            {
                throw new BusinessException("DonaRog:DonationAllocationExceedsTotalAmount")
                    .WithData("donationId", Id)
                    .WithData("totalAmount", TotalAmount)
                    .WithData("totalAllocated", total);
            }

            // Clear existing and add new
            Projects.Clear();

            foreach (var (projectId, amount) in allocations)
            {
                Check.Positive(amount, nameof(amount));
                Projects.Add(new DonationProject(Id, projectId, amount));
            }
        }
    }
}
