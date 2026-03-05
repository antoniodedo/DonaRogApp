using DonaRogApp.Application.Contracts.Segmentation;
using DonaRogApp.Application.Contracts.Segmentation.Dto;
using DonaRogApp.Domain.Segmentation;
using DonaRogApp.Domain.Segmentation.Entities;
using DonaRogApp.Domain.Shared.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Application.Segmentation
{
    /// <summary>
    /// Application service for managing segmentation rules
    /// </summary>
    public class SegmentationRuleAppService : CrudAppService<
        SegmentationRule,
        SegmentationRuleDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateSegmentationRuleDto,
        CreateUpdateSegmentationRuleDto>, ISegmentationRuleAppService
    {
        private readonly IRepository<Segment, Guid> _segmentRepository;
        private readonly DonorSegmentationService _segmentationService;
        private readonly IDistributedCache<SegmentationBatchResultDto> _batchResultCache;

        private const string LastBatchResultCacheKey = "SegmentationBatchResult:Last";

        public SegmentationRuleAppService(
            IRepository<SegmentationRule, Guid> repository,
            IRepository<Segment, Guid> segmentRepository,
            DonorSegmentationService segmentationService,
            IDistributedCache<SegmentationBatchResultDto> batchResultCache)
            : base(repository)
        {
            _segmentRepository = segmentRepository;
            _segmentationService = segmentationService;
            _batchResultCache = batchResultCache;
        }

        // ======================================================================
        // CRUD OVERRIDES
        // ======================================================================

        public override async Task<SegmentationRuleDto> CreateAsync(CreateUpdateSegmentationRuleDto input)
        {
            // Validate segment exists
            var segment = await _segmentRepository.GetAsync(input.SegmentId);

            var rule = SegmentationRule.Create(
                GuidGenerator.Create(),
                CurrentTenant.Id,
                input.Name,
                input.SegmentId,
                input.Priority,
                input.Description);

            // Update conditions
            rule.UpdateRfmConditions(
                input.MinRecencyScore,
                input.MaxRecencyScore,
                input.MinFrequencyScore,
                input.MaxFrequencyScore,
                input.MinMonetaryScore,
                input.MaxMonetaryScore);

            rule.UpdateRawValueConditions(
                input.MinTotalDonated,
                input.MaxTotalDonated,
                input.MinDonationCount,
                input.MaxDonationCount,
                input.MinDaysSinceLastDonation,
                input.MaxDaysSinceLastDonation);

            rule.UpdateDateConditions(
                input.FirstDonationAfter,
                input.FirstDonationBefore,
                input.LastDonationAfter,
                input.LastDonationBefore);

            if (!input.IsActive)
            {
                rule.Deactivate();
            }

            await Repository.InsertAsync(rule);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await MapToGetOutputDtoAsync(rule);
        }

        public override async Task<SegmentationRuleDto> UpdateAsync(Guid id, CreateUpdateSegmentationRuleDto input)
        {
            // Validate segment exists
            var segment = await _segmentRepository.GetAsync(input.SegmentId);

            var rule = await Repository.GetAsync(id);

            rule.UpdateDetails(input.Name, input.Description, input.Priority);
            rule.UpdateTargetSegment(input.SegmentId);

            rule.UpdateRfmConditions(
                input.MinRecencyScore,
                input.MaxRecencyScore,
                input.MinFrequencyScore,
                input.MaxFrequencyScore,
                input.MinMonetaryScore,
                input.MaxMonetaryScore);

            rule.UpdateRawValueConditions(
                input.MinTotalDonated,
                input.MaxTotalDonated,
                input.MinDonationCount,
                input.MaxDonationCount,
                input.MinDaysSinceLastDonation,
                input.MaxDaysSinceLastDonation);

            rule.UpdateDateConditions(
                input.FirstDonationAfter,
                input.FirstDonationBefore,
                input.LastDonationAfter,
                input.LastDonationBefore);

            if (input.IsActive)
                rule.Activate();
            else
                rule.Deactivate();

            await Repository.UpdateAsync(rule);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await MapToGetOutputDtoAsync(rule);
        }

        public override async Task<PagedResultDto<SegmentationRuleDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var query = await Repository.GetQueryableAsync();
            var totalCount = await AsyncExecuter.CountAsync(query);

            // Default sorting by priority if not specified
            if (string.IsNullOrWhiteSpace(input.Sorting))
            {
                query = query.OrderBy(r => r.Priority);
            }
            else
            {
                // ABP handles sorting automatically based on input.Sorting
                query = ApplySorting(query, input);
            }
            
            query = ApplyPaging(query, input);

            var rules = await AsyncExecuter.ToListAsync(query);
            var dtos = await MapToGetListOutputDtosAsync(rules);

            return new PagedResultDto<SegmentationRuleDto>(totalCount, dtos);
        }

        // ======================================================================
        // CUSTOM METHODS
        // ======================================================================

        public virtual async Task<SegmentationRuleDto> ToggleActiveAsync(Guid id)
        {
            var rule = await Repository.GetAsync(id);

            if (rule.IsActive)
                rule.Deactivate();
            else
                rule.Activate();

            await Repository.UpdateAsync(rule);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await MapToGetOutputDtoAsync(rule);
        }

        public virtual async Task ReorderRulesAsync(List<RuleOrderDto> order)
        {
            var ruleIds = order.Select(o => o.RuleId).ToList();
            var rules = await Repository.GetListAsync(r => ruleIds.Contains(r.Id));

            foreach (var rule in rules)
            {
                var orderItem = order.FirstOrDefault(o => o.RuleId == rule.Id);
                if (orderItem != null)
                {
                    rule.UpdatePriority(orderItem.Priority);
                }
            }

            await Repository.UpdateManyAsync(rules);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public virtual async Task<SegmentEvaluationPreviewDto> PreviewRuleAsync(Guid ruleId, int maxResults = 100)
        {
            Logger.LogInformation("Generating preview for rule {RuleId}", ruleId);

            var rule = await Repository.GetAsync(ruleId);

            // Count total matching donors
            var totalCount = await _segmentationService.CountMatchingDonorsAsync(rule);

            // Get sample of matching donors
            var matchingDonors = await _segmentationService.GetMatchingDonorsAsync(rule, maxResults);

            var preview = new SegmentEvaluationPreviewDto
            {
                TotalMatchingDonors = totalCount,
                PreviewDonors = matchingDonors.Select(d => new DonorPreviewDto
                {
                    Id = d.Id,
                    DonorCode = d.DonorCode,
                    FullName = d.GetFullName(),
                    RecencyScore = d.RecencyScore,
                    FrequencyScore = d.FrequencyScore,
                    MonetaryScore = d.MonetaryScore,
                    TotalDonated = d.TotalDonated,
                    DonationCount = d.DonationCount,
                    LastDonationDate = d.LastDonationDate,
                    DaysSinceLastDonation = d.LastDonationDate.HasValue 
                        ? (int)(DateTime.UtcNow - d.LastDonationDate.Value).TotalDays 
                        : null
                }).ToList()
            };

            Logger.LogInformation("Preview generated: {TotalCount} matching donors", totalCount);

            return preview;
        }

        public virtual async Task<int> ApplyRuleManuallyAsync(Guid ruleId)
        {
            Logger.LogInformation("Manually applying rule {RuleId}", ruleId);

            var assignedCount = await _segmentationService.ApplyRuleToAllDonorsAsync(ruleId);
            await CurrentUnitOfWork.SaveChangesAsync();

            Logger.LogInformation("Rule {RuleId} applied: {AssignedCount} donors assigned", ruleId, assignedCount);

            return assignedCount;
        }

        public virtual async Task<SegmentationBatchResultDto> RunSegmentationBatchAsync()
        {
            Logger.LogInformation("Starting manual segmentation batch execution");

            var batchResult = await _segmentationService.EvaluateAllDonorsAsync();
            await CurrentUnitOfWork.SaveChangesAsync();

            var resultDto = new SegmentationBatchResultDto
            {
                StartTime = batchResult.StartTime,
                EndTime = batchResult.EndTime,
                DonorsProcessed = batchResult.DonorsProcessed,
                AssignmentsCreated = batchResult.AssignmentsCreated,
                AssignmentsRemoved = batchResult.AssignmentsRemoved,
                Errors = batchResult.Errors,
                DurationSeconds = batchResult.DurationSeconds,
                Success = batchResult.Errors == 0,
                Message = $"Processed {batchResult.DonorsProcessed} donors: {batchResult.AssignmentsCreated} assignments created, {batchResult.AssignmentsRemoved} removed"
            };

            // Cache the result
            await _batchResultCache.SetAsync(
                LastBatchResultCacheKey,
                resultDto,
                new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                });

            Logger.LogInformation(
                "Batch execution completed: {DonorsProcessed} donors processed, {AssignmentsCreated} created, {AssignmentsRemoved} removed in {Duration}s",
                resultDto.DonorsProcessed, resultDto.AssignmentsCreated, resultDto.AssignmentsRemoved, resultDto.DurationSeconds);

            return resultDto;
        }

        public virtual async Task<SegmentationBatchResultDto?> GetLastBatchResultAsync()
        {
            return await _batchResultCache.GetAsync(LastBatchResultCacheKey);
        }

        public virtual async Task<List<SegmentDto>> GetAvailableSegmentsAsync()
        {
            var segments = await _segmentRepository.GetListAsync(s => s.IsActive);
            
            return segments.Select(s => new SegmentDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                Description = s.Description,
                ColorCode = s.ColorCode,
                Icon = s.Icon,
                DisplayOrder = s.DisplayOrder,
                IsActive = s.IsActive
            }).OrderBy(s => s.DisplayOrder).ToList();
        }

        // ======================================================================
        // MAPPING
        // ======================================================================

        protected override SegmentationRuleDto MapToGetOutputDto(SegmentationRule entity)
        {
            var dto = new SegmentationRuleDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive,
                Priority = entity.Priority,
                SegmentId = entity.SegmentId,
                
                // RFM Scores
                MinRecencyScore = entity.MinRecencyScore,
                MaxRecencyScore = entity.MaxRecencyScore,
                MinFrequencyScore = entity.MinFrequencyScore,
                MaxFrequencyScore = entity.MaxFrequencyScore,
                MinMonetaryScore = entity.MinMonetaryScore,
                MaxMonetaryScore = entity.MaxMonetaryScore,
                
                // Raw Values
                MinTotalDonated = entity.MinTotalDonated,
                MaxTotalDonated = entity.MaxTotalDonated,
                MinDonationCount = entity.MinDonationCount,
                MaxDonationCount = entity.MaxDonationCount,
                MinDaysSinceLastDonation = entity.MinDaysSinceLastDonation,
                MaxDaysSinceLastDonation = entity.MaxDaysSinceLastDonation,
                
                // Dates
                FirstDonationAfter = entity.FirstDonationAfter,
                FirstDonationBefore = entity.FirstDonationBefore,
                LastDonationAfter = entity.LastDonationAfter,
                LastDonationBefore = entity.LastDonationBefore,
                
                // Audit
                CreationTime = entity.CreationTime,
                CreatorId = entity.CreatorId,
                LastModificationTime = entity.LastModificationTime,
                LastModifierId = entity.LastModifierId,
                
                // Summary
                ConditionsSummary = entity.GetConditionsSummary()
            };

            return dto;
        }

        protected override async Task<SegmentationRuleDto> MapToGetOutputDtoAsync(SegmentationRule entity)
        {
            var dto = MapToGetOutputDto(entity);

            // Load segment details
            var segment = await _segmentRepository.FirstOrDefaultAsync(s => s.Id == entity.SegmentId);
            if (segment != null)
            {
                dto.SegmentName = segment.Name;
                dto.SegmentCode = segment.Code;
            }

            return dto;
        }

        protected override async Task<List<SegmentationRuleDto>> MapToGetListOutputDtosAsync(List<SegmentationRule> entities)
        {
            var dtos = new List<SegmentationRuleDto>();

            // Load all segments at once for efficiency
            var segmentIds = entities.Select(e => e.SegmentId).Distinct().ToList();
            var segments = await _segmentRepository.GetListAsync(s => segmentIds.Contains(s.Id));
            var segmentDict = segments.ToDictionary(s => s.Id);

            foreach (var entity in entities)
            {
                var dto = MapToGetOutputDto(entity);
                
                // Add segment details
                if (segmentDict.TryGetValue(entity.SegmentId, out var segment))
                {
                    dto.SegmentName = segment.Name;
                    dto.SegmentCode = segment.Code;
                }

                dtos.Add(dto);
            }

            return dtos;
        }
    }
}
