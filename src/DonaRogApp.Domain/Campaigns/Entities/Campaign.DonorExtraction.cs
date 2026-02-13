using DonaRogApp.Enums.Campaigns;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;

namespace DonaRogApp.Domain.Campaigns.Entities
{
    public partial class Campaign
    {
        /// <summary>
        /// Extract donors for the campaign
        /// </summary>
        public void ExtractDonors(List<Guid> donorIds)
        {
            if (Status != CampaignStatus.Draft && Status != CampaignStatus.InPreparation)
            {
                throw new BusinessException("DonaRog:CampaignCannotExtractDonors")
                    .WithData("campaignId", Id)
                    .WithData("status", Status);
            }

            foreach (var donorId in donorIds)
            {
                // Check if donor already exists
                if (CampaignDonors.Any(cd => cd.DonorId == donorId && cd.RemovedAt == null))
                    continue;

                var campaignDonor = new CampaignDonor(Id, donorId);
                CampaignDonors.Add(campaignDonor);
            }

            ExtractionDate = DateTime.UtcNow;
            Status = CampaignStatus.Extracted;
            
            UpdateStatistics();
        }

        /// <summary>
        /// Remove a donor from the campaign
        /// </summary>
        public void RemoveDonor(Guid donorId)
        {
            var campaignDonor = CampaignDonors.FirstOrDefault(cd => cd.DonorId == donorId && cd.RemovedAt == null);
            if (campaignDonor == null)
            {
                throw new BusinessException("DonaRog:CampaignDonorNotFound")
                    .WithData("campaignId", Id)
                    .WithData("donorId", donorId);
            }

            campaignDonor.Remove();
            UpdateStatistics();
        }

        /// <summary>
        /// Add a single donor to the campaign
        /// </summary>
        public void AddDonor(Guid donorId)
        {
            if (Status == CampaignStatus.Completed || Status == CampaignStatus.Cancelled)
            {
                throw new BusinessException("DonaRog:CampaignCannotAddDonor")
                    .WithData("campaignId", Id)
                    .WithData("status", Status);
            }

            // Check if donor already exists
            if (CampaignDonors.Any(cd => cd.DonorId == donorId && cd.RemovedAt == null))
            {
                throw new BusinessException("DonaRog:CampaignDonorAlreadyExists")
                    .WithData("campaignId", Id)
                    .WithData("donorId", donorId);
            }

            var campaignDonor = new CampaignDonor(Id, donorId);
            CampaignDonors.Add(campaignDonor);

            UpdateStatistics();
        }

        /// <summary>
        /// Clear all donors (before re-extraction)
        /// </summary>
        public void ClearDonors()
        {
            if (Status == CampaignStatus.Dispatched || Status == CampaignStatus.Completed)
            {
                throw new BusinessException("DonaRog:CampaignCannotClearDonors")
                    .WithData("campaignId", Id)
                    .WithData("status", Status);
            }

            foreach (var cd in CampaignDonors.Where(cd => cd.RemovedAt == null))
            {
                cd.Remove();
            }

            UpdateStatistics();
        }
    }
}
