using DonaRogApp.Application.Contracts.Communications.ThankYouRules.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Application.Contracts.Communications.ThankYouRules
{
    /// <summary>
    /// Application service for Thank You Rules
    /// </summary>
    public interface IThankYouRuleAppService : ICrudAppService<
        ThankYouRuleDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateThankYouRuleDto,
        CreateUpdateThankYouRuleDto>
    {
        /// <summary>
        /// Evaluate rules for a donation and return suggested action
        /// </summary>
        Task<ThankYouRuleEvaluationResultDto> EvaluateRulesAsync(EvaluateThankYouRulesDto input);

        /// <summary>
        /// Toggle rule active status
        /// </summary>
        Task<ThankYouRuleDto> ToggleActiveAsync(Guid id);

        /// <summary>
        /// Reorder rule priorities
        /// </summary>
        Task ReorderRulesAsync(List<RuleOrderDto> order);

        /// <summary>
        /// Clone an existing rule with all its properties and template pool
        /// </summary>
        Task<ThankYouRuleDto> CloneRuleAsync(Guid id, string newName);

        /// <summary>
        /// Add a template to a rule's pool
        /// </summary>
        Task AddTemplateToPoolAsync(Guid ruleId, Guid templateId, int priority = 1, bool isActive = true);

        /// <summary>
        /// Remove a template from a rule's pool
        /// </summary>
        Task RemoveTemplateFromPoolAsync(Guid ruleId, Guid templateId);

        /// <summary>
        /// Update template priority in pool
        /// </summary>
        Task UpdateTemplatePoolPriorityAsync(Guid ruleId, Guid templateId, int newPriority);

        /// <summary>
        /// Toggle template active status in pool
        /// </summary>
        Task ToggleTemplateInPoolAsync(Guid ruleId, Guid templateId);
    }

    /// <summary>
    /// DTO for reordering rules
    /// </summary>
    public class RuleOrderDto
    {
        public Guid RuleId { get; set; }
        public int Priority { get; set; }
    }
}
