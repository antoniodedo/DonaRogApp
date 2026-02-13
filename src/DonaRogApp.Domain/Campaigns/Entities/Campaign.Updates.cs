using DonaRogApp.Enums.Campaigns;
using System;
using Volo.Abp;

namespace DonaRogApp.Domain.Campaigns.Entities
{
    public partial class Campaign
    {
        /// <summary>
        /// Update basic campaign details
        /// </summary>
        public void UpdateDetails(
            string name,
            string? description = null)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: 256);
            Description = description;

            VerifyInvariants();
        }

        /// <summary>
        /// Update campaign dates
        /// </summary>
        public void UpdateDates(
            DateTime? extractionScheduledDate = null,
            DateTime? dispatchScheduledDate = null,
            DateTime? recurrenceDate = null)
        {
            ExtractionScheduledDate = extractionScheduledDate;
            DispatchScheduledDate = dispatchScheduledDate;
            RecurrenceDate = recurrenceDate;

            VerifyInvariants();
        }

        /// <summary>
        /// Change campaign status
        /// </summary>
        public void ChangeStatus(CampaignStatus newStatus)
        {
            // Validate transition
            if (!CanTransitionTo(newStatus))
            {
                throw new BusinessException("DonaRog:CampaignInvalidStatusTransition")
                    .WithData("campaignId", Id)
                    .WithData("currentStatus", Status)
                    .WithData("newStatus", newStatus);
            }

            Status = newStatus;
        }

        /// <summary>
        /// Move campaign to In Preparation
        /// </summary>
        public void StartPreparation()
        {
            if (Status != CampaignStatus.Draft)
            {
                throw new BusinessException("DonaRog:CampaignCannotStartPreparation")
                    .WithData("campaignId", Id)
                    .WithData("status", Status);
            }

            Status = CampaignStatus.InPreparation;
        }

        /// <summary>
        /// Mark campaign as dispatched
        /// </summary>
        public void MarkAsDispatched()
        {
            if (Status != CampaignStatus.Extracted)
            {
                throw new BusinessException("DonaRog:CampaignCannotDispatch")
                    .WithData("campaignId", Id)
                    .WithData("status", Status);
            }

            Status = CampaignStatus.Dispatched;
            DispatchDate = DateTime.UtcNow;

            // Mark all extracted donors as dispatched
            foreach (var cd in CampaignDonors)
            {
                if (!cd.DispatchedAt.HasValue && cd.RemovedAt == null)
                {
                    cd.MarkAsDispatched();
                }
            }

            UpdateStatistics();
        }

        /// <summary>
        /// Complete the campaign
        /// </summary>
        public void Complete()
        {
            if (Status != CampaignStatus.Dispatched)
            {
                throw new BusinessException("DonaRog:CampaignCannotComplete")
                    .WithData("campaignId", Id)
                    .WithData("status", Status);
            }

            Status = CampaignStatus.Completed;
            UpdateStatistics(); // Final statistics calculation
        }

        /// <summary>
        /// Cancel the campaign
        /// </summary>
        public void Cancel()
        {
            if (Status == CampaignStatus.Completed)
            {
                throw new BusinessException("DonaRog:CampaignCannotCancelCompleted")
                    .WithData("campaignId", Id);
            }

            Status = CampaignStatus.Cancelled;
        }

        /// <summary>
        /// Check if can transition to new status
        /// </summary>
        private bool CanTransitionTo(CampaignStatus newStatus)
        {
            return (Status, newStatus) switch
            {
                (CampaignStatus.Draft, CampaignStatus.InPreparation) => true,
                (CampaignStatus.InPreparation, CampaignStatus.Extracted) => true,
                (CampaignStatus.InPreparation, CampaignStatus.Draft) => true,
                (CampaignStatus.Extracted, CampaignStatus.Dispatched) => true,
                (CampaignStatus.Extracted, CampaignStatus.InPreparation) => true,
                (CampaignStatus.Dispatched, CampaignStatus.Completed) => true,
                (_, CampaignStatus.Cancelled) => Status != CampaignStatus.Completed,
                _ => false
            };
        }

        /// <summary>
        /// Associate campaign with a recurrence
        /// </summary>
        public void AssociateWithRecurrence(Guid recurrenceId, DateTime? recurrenceDate = null)
        {
            RecurrenceId = recurrenceId;
            RecurrenceDate = recurrenceDate;
        }

        /// <summary>
        /// Remove recurrence association
        /// </summary>
        public void RemoveRecurrenceAssociation()
        {
            RecurrenceId = null;
            RecurrenceDate = null;
        }
    }
}
