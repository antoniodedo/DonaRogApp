using DonaRogApp.Application.Contracts.Campaigns;
using DonaRogApp.Application.Contracts.Campaigns.Dto;
using DonaRogApp.Domain.Campaigns.Entities;
using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Enums.Campaigns;
using DonaRogApp.Enums.Donors;
using DonaRogApp.Enums.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Application.Campaigns
{
    /// <summary>
    /// Application Service for managing Campaigns
    /// </summary>
    public class CampaignAppService : CrudAppService<
        Campaign,
        CampaignDto,
        Guid,
        GetCampaignsInput,
        CreateCampaignDto,
        UpdateCampaignDto>,
        ICampaignAppService
    {
        private readonly IRepository<Campaign, Guid> _campaignRepository;
        private readonly IRepository<CampaignDonor> _campaignDonorRepository;
        private readonly IRepository<Donor, Guid> _donorRepository;

        public CampaignAppService(
            IRepository<Campaign, Guid> campaignRepository,
            IRepository<CampaignDonor> campaignDonorRepository,
            IRepository<Donor, Guid> donorRepository)
            : base(campaignRepository)
        {
            _campaignRepository = campaignRepository;
            _campaignDonorRepository = campaignDonorRepository;
            _donorRepository = donorRepository;
        }

        // ======================================================================
        // OVERRIDE CRUD METHODS
        // ======================================================================

        protected override async Task<IQueryable<Campaign>> CreateFilteredQueryAsync(GetCampaignsInput input)
        {
            var query = await base.CreateFilteredQueryAsync(input);

            // Apply filters
            query = ApplyFilters(query, input);

            return query;
        }

        private IQueryable<Campaign> ApplyFilters(IQueryable<Campaign> query, GetCampaignsInput input)
        {
            // General search filter
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                query = query.Where(c =>
                    c.Name.Contains(input.Filter) ||
                    c.Code.Contains(input.Filter) ||
                    (c.Description != null && c.Description.Contains(input.Filter)));
            }

            // Year filter
            if (input.Year.HasValue)
            {
                query = query.Where(c => c.Year == input.Year.Value);
            }

            // Status filter
            if (input.Status.HasValue)
            {
                query = query.Where(c => c.Status == input.Status.Value);
            }

            // CampaignType filter
            if (input.CampaignType.HasValue)
            {
                query = query.Where(c => c.CampaignType == input.CampaignType.Value);
            }

            // Channel filter
            if (input.Channel.HasValue)
            {
                query = query.Where(c => c.Channel == input.Channel.Value);
            }

            // RecurrenceId filter
            if (input.RecurrenceId.HasValue)
            {
                query = query.Where(c => c.RecurrenceId == input.RecurrenceId.Value);
            }

            // Date range filters
            if (input.DateFrom.HasValue)
            {
                query = query.Where(c => c.DispatchDate >= input.DateFrom.Value);
            }

            if (input.DateTo.HasValue)
            {
                query = query.Where(c => c.DispatchDate <= input.DateTo.Value);
            }

            return query;
        }

        public override async Task<CampaignDto> CreateAsync(CreateCampaignDto input)
        {
            // Create campaign
            var campaign = Campaign.Create(
                id: GuidGenerator.Create(),
                tenantId: CurrentTenant.Id,
                name: input.Name,
                year: input.Year,
                code: input.Code,
                campaignType: input.CampaignType,
                channel: input.Channel,
                description: input.Description,
                recurrenceId: input.RecurrenceId);

            // Set dates
            campaign.UpdateDates(
                input.ExtractionScheduledDate,
                input.DispatchScheduledDate,
                input.RecurrenceDate);

            // Set cost
            campaign.UpdateTotalCost(input.TotalCost);
            campaign.SetTargetDonorCount(input.TargetDonorCount);

            await _campaignRepository.InsertAsync(campaign);
            if (CurrentUnitOfWork != null)
            {
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return ObjectMapper.Map<Campaign, CampaignDto>(campaign);
        }

        public override async Task<CampaignDto> UpdateAsync(Guid id, UpdateCampaignDto input)
        {
            var campaign = await _campaignRepository.GetAsync(id);

            // Update details
            campaign.UpdateDetails(input.Name, input.Description);

            // Update dates
            campaign.UpdateDates(
                input.ExtractionScheduledDate,
                input.DispatchScheduledDate,
                input.RecurrenceDate);

            // Update cost and target
            campaign.UpdateTotalCost(input.TotalCost);
            campaign.SetTargetDonorCount(input.TargetDonorCount);

            // Update recurrence association
            if (input.RecurrenceId.HasValue)
            {
                campaign.AssociateWithRecurrence(input.RecurrenceId.Value, input.RecurrenceDate);
            }
            else
            {
                campaign.RemoveRecurrenceAssociation();
            }

            await _campaignRepository.UpdateAsync(campaign);
            if (CurrentUnitOfWork != null)
            {
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return ObjectMapper.Map<Campaign, CampaignDto>(campaign);
        }

        // ======================================================================
        // CUSTOM LIST METHOD
        // ======================================================================

        public async Task<PagedResultDto<CampaignListDto>> GetCampaignListAsync(GetCampaignsInput input)
        {
            var query = await CreateFilteredQueryAsync(input);

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var campaigns = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<CampaignListDto>(
                totalCount,
                ObjectMapper.Map<List<Campaign>, List<CampaignListDto>>(campaigns));
        }

        // ======================================================================
        // DONOR EXTRACTION
        // ======================================================================

        public async Task<DonorExtractionPreviewDto> PreviewDonorExtractionAsync(ExtractDonorsInput input)
        {
            // Build query
            var query = await BuildDonorExtractionQueryAsync(input);

            // Get total count
            var totalCount = await AsyncExecuter.CountAsync(query);

            // Get sample donors (first 50 for preview)
            var donors = await AsyncExecuter.ToListAsync(query.Take(50));

            // Build statistics
            var statistics = await BuildExtractionStatisticsAsync(query);

            // Build filter breakdown
            var filterBreakdown = BuildFilterBreakdown(input, totalCount);

            return new DonorExtractionPreviewDto
            {
                TotalCount = totalCount,
                Donors = ObjectMapper.Map<List<Donor>, List<DonorPreviewItemDto>>(donors),
                Statistics = statistics,
                FilterBreakdown = filterBreakdown
            };
        }

        public async Task ExtractDonorsAsync(Guid campaignId, ExtractDonorsInput input)
        {
            var campaign = await _campaignRepository.GetAsync(campaignId);

            // Build query
            var query = await BuildDonorExtractionQueryAsync(input);

            // Apply max results if specified
            if (input.MaxResults.HasValue)
            {
                query = query.Take(input.MaxResults.Value);
            }

            var donorIds = await AsyncExecuter.ToListAsync(query.Select(d => d.Id));

            // Extract donors
            campaign.ExtractDonors(donorIds);

            await _campaignRepository.UpdateAsync(campaign);
            if (CurrentUnitOfWork != null)
            {
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        private async Task<IQueryable<Donor>> BuildDonorExtractionQueryAsync(ExtractDonorsInput input)
        {
            var query = await _donorRepository.GetQueryableAsync();

            // Apply filters based on logical operator
            if (input.LogicalOperator == LogicalOperator.And)
            {
                query = ApplyFiltersWithAnd(query, input);
            }
            else
            {
                query = ApplyFiltersWithOr(query, input);
            }

            // Apply random sampling if requested
            if (input.RandomSample)
            {
                query = query.OrderBy(d => Guid.NewGuid());
            }
            else
            {
                query = query.OrderBy(d => d.CreationTime);
            }

            return query;
        }

        private IQueryable<Donor> ApplyFiltersWithAnd(IQueryable<Donor> query, ExtractDonorsInput input)
        {
            // Donor Status Filter
            if (input.DonorStatus.HasValue)
            {
                query = query.Where(d => d.Status == input.DonorStatus.Value);
            }

            // Donor Category Filter
            if (input.DonorCategory.HasValue)
            {
                query = query.Where(d => d.Category == input.DonorCategory.Value);
            }

            // Tag Filters - Include
            if (input.IncludedTagIds.Any())
            {
                if (input.TagFilterMode == TagFilterMode.All)
                {
                    // Donor must have ALL included tags
                    foreach (var tagId in input.IncludedTagIds)
                    {
                        query = query.Where(d => d.Tags.Any(dt => dt.TagId == tagId));
                    }
                }
                else
                {
                    // Donor must have ANY of the included tags
                    query = query.Where(d => d.Tags.Any(dt => input.IncludedTagIds.Contains(dt.TagId)));
                }
            }

            // Tag Filters - Exclude
            if (input.ExcludedTagIds.Any())
            {
                query = query.Where(d => !d.Tags.Any(dt => input.ExcludedTagIds.Contains(dt.TagId)));
            }

            // Segment Filters - Include
            if (input.IncludedSegmentIds.Any())
            {
                query = query.Where(d => d.Segments.Any(ds => input.IncludedSegmentIds.Contains(ds.SegmentId)));
            }

            // Segment Filters - Exclude
            if (input.ExcludedSegmentIds.Any())
            {
                query = query.Where(d => !d.Segments.Any(ds => input.ExcludedSegmentIds.Contains(ds.SegmentId)));
            }

            // Name Filters - Include (partial match on FirstName, LastName, CompanyName)
            if (input.IncludedDonorNames.Any())
            {
                var namePredicates = input.IncludedDonorNames
                    .Select(name => (Expression<Func<Donor, bool>>)(d => 
                        (d.FirstName != null && d.FirstName.Contains(name)) ||
                        (d.LastName != null && d.LastName.Contains(name)) ||
                        (d.CompanyName != null && d.CompanyName.Contains(name))))
                    .ToList();
                
                var combinedPredicate = namePredicates.Aggregate((expr1, expr2) =>
                {
                    var parameter = Expression.Parameter(typeof(Donor), "d");
                    var body = Expression.OrElse(
                        Expression.Invoke(expr1, parameter),
                        Expression.Invoke(expr2, parameter)
                    );
                    return Expression.Lambda<Func<Donor, bool>>(body, parameter);
                });
                
                query = query.Where(combinedPredicate);
            }

            // Name Filters - Exclude
            if (input.ExcludedDonorNames.Any())
            {
                foreach (var name in input.ExcludedDonorNames)
                {
                    query = query.Where(d => 
                        (d.FirstName == null || !d.FirstName.Contains(name)) &&
                        (d.LastName == null || !d.LastName.Contains(name)) &&
                        (d.CompanyName == null || !d.CompanyName.Contains(name)));
                }
            }

            // Geographic Filters - Regions Include
            if (input.IncludedRegions.Any())
            {
                query = query.Where(d => d.Addresses.Any(a => 
                    a.Type == AddressType.Home && 
                    a.Region != null &&
                    input.IncludedRegions.Contains(a.Region)));
            }

            // Geographic Filters - Regions Exclude
            if (input.ExcludedRegions.Any())
            {
                query = query.Where(d => !d.Addresses.Any(a => 
                    a.Type == AddressType.Home && 
                    a.Region != null &&
                    input.ExcludedRegions.Contains(a.Region)));
            }

            // Geographic Filters - Provinces Include
            if (input.IncludedProvinces.Any())
            {
                query = query.Where(d => d.Addresses.Any(a => 
                    a.Type == AddressType.Home && 
                    a.Province != null &&
                    input.IncludedProvinces.Contains(a.Province)));
            }

            // Geographic Filters - Provinces Exclude
            if (input.ExcludedProvinces.Any())
            {
                query = query.Where(d => !d.Addresses.Any(a => 
                    a.Type == AddressType.Home && 
                    a.Province != null &&
                    input.ExcludedProvinces.Contains(a.Province)));
            }

            // Campaign Participation - Include
            // Note: This requires CampaignDonor entity to be queryable
            // For now, we'll implement when Donations module is ready
            // TODO: Implement when Campaign-Donor relationship is accessible via navigation

            // Consent Filters
            if (input.RequireNewsletterConsent.HasValue)
            {
                query = query.Where(d => d.NewsletterConsent == input.RequireNewsletterConsent.Value);
            }

            if (input.RequireMailConsent.HasValue)
            {
                query = query.Where(d => d.MailConsent == input.RequireMailConsent.Value);
            }

            // Donation Filters - Note: These require Donation entity
            // TODO: Implement when Donations module is ready
            // For now, commented out placeholders:
            /*
            if (input.DonationDateRange != null)
            {
                query = query.Where(d => d.Donations.Any(don =>
                    (!input.DonationDateRange.From.HasValue || don.Date >= input.DonationDateRange.From.Value) &&
                    (!input.DonationDateRange.To.HasValue || don.Date <= input.DonationDateRange.To.Value)));
            }

            if (input.DonationAmountRange != null)
            {
                query = query.Where(d => d.Donations
                    .Where(don => !input.DonationDateRange.From.HasValue || don.Date >= input.DonationDateRange.From.Value)
                    .Where(don => !input.DonationDateRange.To.HasValue || don.Date <= input.DonationDateRange.To.Value)
                    .Sum(don => don.Amount) >= input.DonationAmountRange.Min &&
                    d.Donations.Sum(don => don.Amount) <= input.DonationAmountRange.Max);
            }
            */

            return query;
        }

        private IQueryable<Donor> ApplyFiltersWithOr(IQueryable<Donor> query, ExtractDonorsInput input)
        {
            // For OR logic, we build a list of predicates and combine them
            var predicates = new List<Expression<Func<Donor, bool>>>();

            // Donor Status Filter
            if (input.DonorStatus.HasValue)
            {
                predicates.Add(d => d.Status == input.DonorStatus.Value);
            }

            // Donor Category Filter
            if (input.DonorCategory.HasValue)
            {
                predicates.Add(d => d.Category == input.DonorCategory.Value);
            }

            // Tag Filters - Include
            if (input.IncludedTagIds.Any())
            {
                predicates.Add(d => d.Tags.Any(dt => input.IncludedTagIds.Contains(dt.TagId)));
            }

            // Segment Filters - Include
            if (input.IncludedSegmentIds.Any())
            {
                predicates.Add(d => d.Segments.Any(ds => input.IncludedSegmentIds.Contains(ds.SegmentId)));
            }

            // Name Filters - Include
            if (input.IncludedDonorNames.Any())
            {
                foreach (var name in input.IncludedDonorNames)
                {
                    var localName = name; // Capture for closure
                    predicates.Add(d => 
                        (d.FirstName != null && d.FirstName.Contains(localName)) ||
                        (d.LastName != null && d.LastName.Contains(localName)) ||
                        (d.CompanyName != null && d.CompanyName.Contains(localName)));
                }
            }

            // Geographic Filters - Regions Include
            if (input.IncludedRegions.Any())
            {
                predicates.Add(d => d.Addresses.Any(a => 
                    a.Type == AddressType.Home && 
                    a.Region != null &&
                    input.IncludedRegions.Contains(a.Region)));
            }

            // Geographic Filters - Provinces Include
            if (input.IncludedProvinces.Any())
            {
                predicates.Add(d => d.Addresses.Any(a => 
                    a.Type == AddressType.Home && 
                    a.Province != null &&
                    input.IncludedProvinces.Contains(a.Province)));
            }

            // Consent Filters
            if (input.RequireNewsletterConsent.HasValue && input.RequireNewsletterConsent.Value)
            {
                predicates.Add(d => d.NewsletterConsent);
            }

            if (input.RequireMailConsent.HasValue && input.RequireMailConsent.Value)
            {
                predicates.Add(d => d.MailConsent);
            }

            // Combine all predicates with OR
            if (predicates.Any())
            {
                var combinedPredicate = predicates.Aggregate((expr1, expr2) =>
                {
                    var parameter = Expression.Parameter(typeof(Donor), "d");
                    var body = Expression.OrElse(
                        Expression.Invoke(expr1, parameter),
                        Expression.Invoke(expr2, parameter)
                    );
                    return Expression.Lambda<Func<Donor, bool>>(body, parameter);
                });

                query = query.Where(combinedPredicate);
            }

            // Always apply exclusion filters (they work with OR as well)
            if (input.ExcludedTagIds.Any())
            {
                query = query.Where(d => !d.Tags.Any(dt => input.ExcludedTagIds.Contains(dt.TagId)));
            }

            if (input.ExcludedSegmentIds.Any())
            {
                query = query.Where(d => !d.Segments.Any(ds => input.ExcludedSegmentIds.Contains(ds.SegmentId)));
            }

            if (input.ExcludedDonorNames.Any())
            {
                foreach (var name in input.ExcludedDonorNames)
                {
                    query = query.Where(d => 
                        (d.FirstName == null || !d.FirstName.Contains(name)) &&
                        (d.LastName == null || !d.LastName.Contains(name)) &&
                        (d.CompanyName == null || !d.CompanyName.Contains(name)));
                }
            }

            if (input.ExcludedRegions.Any())
            {
                query = query.Where(d => !d.Addresses.Any(a => 
                    a.Type == AddressType.Home && 
                    a.Region != null &&
                    input.ExcludedRegions.Contains(a.Region)));
            }

            if (input.ExcludedProvinces.Any())
            {
                query = query.Where(d => !d.Addresses.Any(a => 
                    a.Type == AddressType.Home && 
                    a.Province != null &&
                    input.ExcludedProvinces.Contains(a.Province)));
            }

            return query;
        }

        private async Task<ExtractionStatisticsDto> BuildExtractionStatisticsAsync(IQueryable<Donor> query)
        {
            // Calculate statistics
            var totalDonors = await AsyncExecuter.CountAsync(query);

            return new ExtractionStatisticsDto
            {
                TotalDonors = totalDonors,
                TotalPotentialRevenue = 0, // TODO: Calculate from donation history
                AverageDonationAmount = 0, // TODO: Calculate from donation history
                ActiveDonors = totalDonors,
                LapsedDonors = 0,
                RegionDistribution = new Dictionary<string, int>(),
                SegmentDistribution = new Dictionary<string, int>()
            };
        }

        private List<FilterBreakdownDto> BuildFilterBreakdown(ExtractDonorsInput input, int totalCount)
        {
            var breakdown = new List<FilterBreakdownDto>();

            // Add breakdown for each filter that was applied
            // This is simplified - you should calculate actual match counts

            return breakdown;
        }

        // ======================================================================
        // DONOR MANAGEMENT
        // ======================================================================

        public async Task AddDonorAsync(Guid campaignId, Guid donorId)
        {
            var campaign = await _campaignRepository.GetAsync(campaignId);
            campaign.AddDonor(donorId);
            await _campaignRepository.UpdateAsync(campaign);
        }

        public async Task RemoveDonorAsync(Guid campaignId, Guid donorId)
        {
            var campaign = await _campaignRepository.GetAsync(campaignId);
            campaign.RemoveDonor(donorId);
            await _campaignRepository.UpdateAsync(campaign);
        }

        public async Task<PagedResultDto<CampaignDonorDto>> GetCampaignDonorsAsync(
            Guid campaignId,
            PagedAndSortedResultRequestDto input)
        {
            var query = await _campaignDonorRepository.GetQueryableAsync();
            query = query.Where(cd => cd.CampaignId == campaignId && cd.RemovedAt == null);

            var totalCount = await AsyncExecuter.CountAsync(query);

            // Apply paging
            query = query
                .OrderByDescending(cd => cd.ExtractedAt)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var campaignDonors = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<CampaignDonorDto>(
                totalCount,
                ObjectMapper.Map<List<CampaignDonor>, List<CampaignDonorDto>>(campaignDonors));
        }

        // ======================================================================
        // WORKFLOW
        // ======================================================================

        public async Task MarkAsDispatchedAsync(Guid id)
        {
            var campaign = await _campaignRepository.GetAsync(id);
            campaign.MarkAsDispatched();
            await _campaignRepository.UpdateAsync(campaign);
        }

        public async Task CompleteAsync(Guid id)
        {
            var campaign = await _campaignRepository.GetAsync(id);
            campaign.Complete();
            await _campaignRepository.UpdateAsync(campaign);
        }

        public async Task CancelAsync(Guid id)
        {
            var campaign = await _campaignRepository.GetAsync(id);
            campaign.Cancel();
            await _campaignRepository.UpdateAsync(campaign);
        }

        // ======================================================================
        // TRACKING
        // ======================================================================

        public async Task<string> GeneratePostalCodeAsync(Guid campaignId)
        {
            var campaign = await _campaignRepository.GetAsync(campaignId);

            // Get next sequence number for the year
            var query = await _campaignRepository.GetQueryableAsync();
            var maxSequence = await AsyncExecuter.MaxAsync(
                query.Where(c => c.Year == campaign.Year && c.YearlySequenceNumber.HasValue)
                     .Select(c => (int?)c.YearlySequenceNumber)) ?? 0;

            var nextSequence = maxSequence + 1;

            // Generate postal code
            campaign.GeneratePostalCode(nextSequence);

            await _campaignRepository.UpdateAsync(campaign);
            if (CurrentUnitOfWork != null)
            {
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return campaign.PostalCode!.Value;
        }

        public async Task RecordDonationAsync(Guid campaignId, RecordDonationInput input)
        {
            var campaignDonor = await _campaignDonorRepository
                .FirstOrDefaultAsync(cd =>
                    cd.CampaignId == campaignId &&
                    cd.DonorId == input.DonorId &&
                    cd.RemovedAt == null);

            if (campaignDonor == null)
            {
                throw new BusinessException("DonaRog:CampaignDonorNotFound")
                    .WithData("campaignId", campaignId)
                    .WithData("donorId", input.DonorId);
            }

            campaignDonor.RecordDonation(input.Amount, input.DonationDate);

            if (!string.IsNullOrWhiteSpace(input.Notes))
            {
                campaignDonor.UpdateNotes(input.Notes);
            }

            await _campaignDonorRepository.UpdateAsync(campaignDonor);

            // Update campaign statistics
            var campaign = await _campaignRepository.GetAsync(campaignId);
            campaign.UpdateStatistics();
            await _campaignRepository.UpdateAsync(campaign);
        }

        // ======================================================================
        // STATISTICS
        // ======================================================================

        public async Task<CampaignStatisticsDto> GetStatisticsAsync(Guid campaignId)
        {
            var campaign = await _campaignRepository.GetAsync(campaignId);

            // Get campaign donors for detailed statistics
            var query = await _campaignDonorRepository.GetQueryableAsync();
            var campaignDonors = await AsyncExecuter.ToListAsync(
                query.Where(cd => cd.CampaignId == campaignId && cd.RemovedAt == null));

            var statistics = new CampaignStatisticsDto
            {
                CampaignId = campaignId,
                CampaignName = campaign.Name,
                Year = campaign.Year,

                // Counts
                ExtractedDonorCount = campaign.ExtractedDonorCount,
                DispatchedCount = campaign.DispatchedCount,
                ResponseCount = campaign.ResponseCount,
                OpenedCount = campaignDonors.Count(cd => cd.OpenedAt.HasValue),
                ClickedCount = campaignDonors.Count(cd => cd.ClickedAt.HasValue),
                DonationCount = campaign.DonationCount,
                UnsubscribedCount = campaignDonors.Count(cd => cd.ResponseType == ResponseType.Unsubscribed),
                BouncedCount = campaignDonors.Count(cd => cd.ResponseType == ResponseType.Bounced),

                // Rates
                ResponseRate = campaign.ResponseRate,
                OpenRate = campaign.DispatchedCount > 0 ?
                    (decimal)campaignDonors.Count(cd => cd.OpenedAt.HasValue) / campaign.DispatchedCount * 100 : 0,
                ClickRate = campaign.DispatchedCount > 0 ?
                    (decimal)campaignDonors.Count(cd => cd.ClickedAt.HasValue) / campaign.DispatchedCount * 100 : 0,
                ConversionRate = campaign.ConversionRate,

                // Financial
                TotalCost = campaign.TotalCost,
                TotalRaised = campaign.TotalRaised,
                NetAmount = campaign.GetNetAmount(),
                ROI = campaign.ROI,
                AverageDonation = campaign.AverageDonation,
                CostPerAcquisition = campaign.GetCostPerAcquisition(),
                CostPerClick = campaignDonors.Count(cd => cd.ClickedAt.HasValue) > 0 ?
                    campaign.TotalCost / campaignDonors.Count(cd => cd.ClickedAt.HasValue) : 0,

                // Breakdowns (TODO: implement grouping)
                ResponseTypeBreakdown = new Dictionary<string, int>(),
                RegionBreakdown = new Dictionary<string, decimal>(),
                SegmentBreakdown = new Dictionary<string, decimal>(),

                // Timelines (TODO: implement)
                ResponseTimeline = new List<DailyResponseDto>(),
                DonationTimeline = new List<DailyDonationDto>()
            };

            return statistics;
        }

        public async Task UpdateStatisticsAsync(Guid campaignId)
        {
            var campaign = await _campaignRepository.GetAsync(campaignId);
            campaign.UpdateStatistics();
            await _campaignRepository.UpdateAsync(campaign);
        }
    }
}
