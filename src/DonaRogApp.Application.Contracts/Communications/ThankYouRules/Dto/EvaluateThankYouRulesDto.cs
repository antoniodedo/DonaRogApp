using DonaRogApp.Enums.Communications;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Communications.ThankYouRules.Dto
{
    /// <summary>
    /// Input for evaluating thank you rules
    /// </summary>
    public class EvaluateThankYouRulesDto
    {
        [Required]
        public Guid DonorId { get; set; }

        [Required]
        public Guid DonationId { get; set; }
    }

    /// <summary>
    /// Result of rule evaluation
    /// </summary>
    public class ThankYouRuleEvaluationResultDto
    {
        /// <summary>
        /// Should create thank you?
        /// </summary>
        public bool ShouldCreateThankYou { get; set; }

        /// <summary>
        /// Suggested channel (based on rules + donor preference)
        /// </summary>
        public PreferredThankYouChannel SuggestedChannel { get; set; }

        /// <summary>
        /// Suggested template ID
        /// </summary>
        public Guid? SuggestedTemplateId { get; set; }

        /// <summary>
        /// Matched rules (ordered by priority)
        /// </summary>
        public List<MatchedRuleDto> MatchedRules { get; set; } = new();

        /// <summary>
        /// Donor's preference (if set)
        /// </summary>
        public PreferredThankYouChannel? DonorPreference { get; set; }

        /// <summary>
        /// Explanation of decision
        /// </summary>
        public string Explanation { get; set; } = null!;
    }

    /// <summary>
    /// Matched rule info
    /// </summary>
    public class MatchedRuleDto
    {
        public Guid RuleId { get; set; }
        public string RuleName { get; set; } = null!;
        public int Priority { get; set; }
        public int MatchScore { get; set; }
        public bool CreateThankYou { get; set; }
        public PreferredThankYouChannel? SuggestedChannel { get; set; }
        public Guid? SuggestedTemplateId { get; set; }
        public string? Notes { get; set; }
    }
}
