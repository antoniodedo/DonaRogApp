using DonaRogApp.Application.Contracts.Segmentation.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Application.Contracts.Segmentation
{
    /// <summary>
    /// Application service for managing segmentation rules
    /// </summary>
    public interface ISegmentationRuleAppService : 
        ICrudAppService<
            SegmentationRuleDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateSegmentationRuleDto,
            CreateUpdateSegmentationRuleDto>
    {
        /// <summary>
        /// Toggle rule active/inactive status
        /// </summary>
        Task<SegmentationRuleDto> ToggleActiveAsync(Guid id);

        /// <summary>
        /// Reorder rules by priority
        /// </summary>
        Task ReorderRulesAsync(List<RuleOrderDto> order);

        /// <summary>
        /// Preview donors matching a rule
        /// </summary>
        Task<SegmentEvaluationPreviewDto> PreviewRuleAsync(Guid ruleId, int maxResults = 100);

        /// <summary>
        /// Apply a single rule manually to all donors
        /// Returns count of donors assigned
        /// </summary>
        Task<int> ApplyRuleManuallyAsync(Guid ruleId);

        /// <summary>
        /// Run full segmentation batch for all donors
        /// </summary>
        Task<SegmentationBatchResultDto> RunSegmentationBatchAsync();

        /// <summary>
        /// Get last batch execution result
        /// </summary>
        Task<SegmentationBatchResultDto?> GetLastBatchResultAsync();

        /// <summary>
        /// Get available segments for rule assignment
        /// </summary>
        Task<List<SegmentDto>> GetAvailableSegmentsAsync();
    }
}
