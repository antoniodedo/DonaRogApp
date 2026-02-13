using DonaRogApp.Application.Contracts.Campaigns.Dto;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Application.Contracts.Campaigns
{
    /// <summary>
    /// Campaign App Service Interface - manages marketing campaigns
    /// </summary>
    public interface ICampaignAppService :
        ICrudAppService<
            CampaignDto,
            Guid,
            GetCampaignsInput,
            CreateCampaignDto,
            UpdateCampaignDto>
    {
        /// <summary>
        /// Get list of campaigns (simplified with CampaignListDto)
        /// </summary>
        Task<PagedResultDto<CampaignListDto>> GetCampaignListAsync(GetCampaignsInput input);

        // ======================================================================
        // DONOR EXTRACTION
        // ======================================================================

        /// <summary>
        /// Preview donor extraction results (with filters)
        /// </summary>
        Task<DonorExtractionPreviewDto> PreviewDonorExtractionAsync(ExtractDonorsInput input);

        /// <summary>
        /// Extract donors for campaign
        /// </summary>
        Task ExtractDonorsAsync(Guid campaignId, ExtractDonorsInput input);

        /// <summary>
        /// Add single donor to campaign
        /// </summary>
        Task AddDonorAsync(Guid campaignId, Guid donorId);

        /// <summary>
        /// Remove donor from campaign
        /// </summary>
        Task RemoveDonorAsync(Guid campaignId, Guid donorId);

        /// <summary>
        /// Get campaign donors
        /// </summary>
        Task<PagedResultDto<CampaignDonorDto>> GetCampaignDonorsAsync(Guid campaignId, PagedAndSortedResultRequestDto input);

        // ======================================================================
        // WORKFLOW
        // ======================================================================

        /// <summary>
        /// Mark campaign as dispatched
        /// </summary>
        Task MarkAsDispatchedAsync(Guid id);

        /// <summary>
        /// Complete the campaign
        /// </summary>
        Task CompleteAsync(Guid id);

        /// <summary>
        /// Cancel the campaign
        /// </summary>
        Task CancelAsync(Guid id);

        // ======================================================================
        // TRACKING
        // ======================================================================

        /// <summary>
        /// Generate postal code 674
        /// </summary>
        Task<string> GeneratePostalCodeAsync(Guid campaignId);

        /// <summary>
        /// Record donation for campaign donor
        /// </summary>
        Task RecordDonationAsync(Guid campaignId, RecordDonationInput input);

        // ======================================================================
        // STATISTICS
        // ======================================================================

        /// <summary>
        /// Get campaign statistics
        /// </summary>
        Task<CampaignStatisticsDto> GetStatisticsAsync(Guid campaignId);

        /// <summary>
        /// Update campaign statistics (recalculate)
        /// </summary>
        Task UpdateStatisticsAsync(Guid campaignId);
    }
}
