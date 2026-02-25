using DonaRogApp.Application.Contracts.Communications.ThankYouRules;
using DonaRogApp.Application.Contracts.Communications.ThankYouRules.Dto;
using DonaRogApp.Domain.Communications.Entities;
using DonaRogApp.Domain.Donations.Entities;
using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Enums.Communications;
using DonaRogApp.LetterTemplates;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Application.Communications.ThankYouRules
{
    /// <summary>
    /// Application service for Thank You Rules
    /// </summary>
    public class ThankYouRuleAppService : CrudAppService<
        ThankYouRule,
        ThankYouRuleDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateThankYouRuleDto,
        CreateUpdateThankYouRuleDto>, IThankYouRuleAppService
    {
        private readonly IRepository<Donor, Guid> _donorRepository;
        private readonly IRepository<Donation, Guid> _donationRepository;
        private readonly IRepository<LetterTemplate, Guid> _templateRepository;

        public ThankYouRuleAppService(
            IRepository<ThankYouRule, Guid> repository,
            IRepository<Donor, Guid> donorRepository,
            IRepository<Donation, Guid> donationRepository,
            IRepository<DonaRogApp.LetterTemplates.LetterTemplate, Guid> templateRepository)
            : base(repository)
        {
            _donorRepository = donorRepository;
            _donationRepository = donationRepository;
            _templateRepository = templateRepository;
        }

        // ======================================================================
        // RULE EVALUATION
        // ======================================================================

        public virtual async Task<ThankYouRuleEvaluationResultDto> EvaluateRulesAsync(EvaluateThankYouRulesDto input)
        {
            // Load donor and donation
            var donor = await _donorRepository.GetAsync(input.DonorId, includeDetails: true);
            var donation = await _donationRepository.GetAsync(input.DonationId, includeDetails: true);

            // Get active rules ordered by priority
            var rules = await Repository.GetListAsync(r => r.IsActive);
            rules = rules.OrderBy(r => r.Priority).ToList();

            // Find matching rules
            var matchedRules = new List<MatchedRuleDto>();
            ThankYouRule? bestRule = null;
            int bestScore = 0;

            foreach (var rule in rules)
            {
                // Check if rule matches
                // Determine if this is the donor's first donation
                var donorDonationCount = await _donationRepository.CountAsync(d => d.DonorId == input.DonorId);
                var isFirstDonation = donorDonationCount == 1;
                
                var matches = rule.Matches(
                    donation.TotalAmount,
                    isFirstDonation,
                    donor.Category,
                    donor.SubjectType,
                    donation.Projects?.FirstOrDefault()?.ProjectId,
                    donation.CampaignId);
                
                if (matches)
                {
                    var score = rule.GetMatchScore();
                    matchedRules.Add(new MatchedRuleDto
                    {
                        RuleId = rule.Id,
                        RuleName = rule.Name,
                        Priority = rule.Priority,
                        MatchScore = score,
                        CreateThankYou = rule.CreateThankYou,
                        SuggestedChannel = rule.SuggestedChannel,
                        SuggestedTemplateId = rule.SuggestedTemplateId,
                        Notes = rule.Description
                    });

                    // Best rule: lowest priority (processed first), highest score
                    if (bestRule == null || rule.Priority < bestRule.Priority || 
                        (rule.Priority == bestRule.Priority && score > bestScore))
                    {
                        bestRule = rule;
                        bestScore = score;
                    }
                }
            }

            // Build result
            var result = new ThankYouRuleEvaluationResultDto
            {
                ShouldCreateThankYou = bestRule?.CreateThankYou ?? true, // Default: yes
                DonorPreference = donor.PreferredThankYouChannel,
                MatchedRules = matchedRules.OrderBy(r => r.Priority).ThenByDescending(r => r.MatchScore).ToList()
            };

            // Determine channel: Donor preference > Rule suggestion > Default (Letter)
            if (donor.PreferredThankYouChannel.HasValue && donor.PreferredThankYouChannel.Value != PreferredThankYouChannel.None)
            {
                result.SuggestedChannel = donor.PreferredThankYouChannel.Value;
                result.Explanation = $"Canale suggerito da preferenze donatore: {result.SuggestedChannel}";
            }
            else if (bestRule?.SuggestedChannel.HasValue == true)
            {
                result.SuggestedChannel = bestRule.SuggestedChannel.Value;
                result.Explanation = $"Canale suggerito da regola '{bestRule.Name}': {result.SuggestedChannel}";
            }
            else
            {
                result.SuggestedChannel = PreferredThankYouChannel.Letter; // Default
                result.Explanation = "Canale predefinito: Lettera postale";
            }

            // Template suggestion
            if (bestRule?.SuggestedTemplateId.HasValue == true)
            {
                result.SuggestedTemplateId = bestRule.SuggestedTemplateId;
                result.Explanation += $" | Template suggerito dalla regola '{bestRule.Name}'";
            }
            else
            {
                // Try to find default template for category
                var defaultTemplate = await _templateRepository.FirstOrDefaultAsync(
                    t => t.Category == TemplateCategory.ThankYou && t.IsActive);
                
                result.SuggestedTemplateId = defaultTemplate?.Id;
                result.Explanation += " | Template predefinito";
            }

            // Handle "None" preference
            if (donor.PreferredThankYouChannel == PreferredThankYouChannel.None)
            {
                result.ShouldCreateThankYou = false;
                result.Explanation = "Donatore ha richiesto di NON ricevere ringraziamenti";
            }

            if (!result.ShouldCreateThankYou && bestRule != null)
            {
                result.Explanation = $"Regola '{bestRule.Name}' suggerisce di NON creare ringraziamento";
            }

            return result;
        }

        // ======================================================================
        // CRUD OVERRIDES
        // ======================================================================

        public override async Task<ThankYouRuleDto> CreateAsync(CreateUpdateThankYouRuleDto input)
        {
            var rule = ThankYouRule.Create(
                GuidGenerator.Create(),
                CurrentTenant.Id,
                input.Name,
                input.Priority,
                input.CreateThankYou,
                input.Description);

            rule.UpdateConditions(
                input.MinAmount,
                input.MaxAmount,
                input.IsFirstDonation,
                input.ProjectId,
                input.CampaignId,
                input.DonorCategory,
                input.SubjectType);

            rule.UpdateActions(
                input.CreateThankYou,
                input.SuggestedChannel,
                input.SuggestedTemplateId);

            if (!input.IsActive)
            {
                rule.Deactivate();
            }

            await Repository.InsertAsync(rule);
            return MapToGetOutputDto(rule);
        }

        public override async Task<ThankYouRuleDto> UpdateAsync(Guid id, CreateUpdateThankYouRuleDto input)
        {
            var rule = await Repository.GetAsync(id);

            rule.UpdateDetails(input.Name, input.Description, input.Priority);

            rule.UpdateConditions(
                input.MinAmount,
                input.MaxAmount,
                input.IsFirstDonation,
                input.ProjectId,
                input.CampaignId,
                input.DonorCategory,
                input.SubjectType);

            rule.UpdateActions(
                input.CreateThankYou,
                input.SuggestedChannel,
                input.SuggestedTemplateId);

            if (input.IsActive)
                rule.Activate();
            else
                rule.Deactivate();

            await Repository.UpdateAsync(rule);
            return MapToGetOutputDto(rule);
        }

        // ======================================================================
        // CUSTOM METHODS
        // ======================================================================

        public virtual async Task<ThankYouRuleDto> ToggleActiveAsync(Guid id)
        {
            var rule = await Repository.GetAsync(id);

            if (rule.IsActive)
                rule.Deactivate();
            else
                rule.Activate();

            await Repository.UpdateAsync(rule);
            return MapToGetOutputDto(rule);
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
        }

        // ======================================================================
        // MAPPING
        // ======================================================================

        protected override ThankYouRuleDto MapToGetOutputDto(ThankYouRule entity)
        {
            return new ThankYouRuleDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Priority = entity.Priority,
                IsActive = entity.IsActive,
                MinAmount = entity.MinAmount,
                MaxAmount = entity.MaxAmount,
                IsFirstDonation = entity.IsFirstDonation,
                ProjectId = entity.ProjectId,
                CampaignId = entity.CampaignId,
                DonorCategory = entity.DonorCategory,
                SubjectType = entity.SubjectType,
                CreateThankYou = entity.CreateThankYou,
                SuggestedChannel = entity.SuggestedChannel,
                SuggestedTemplateId = entity.SuggestedTemplateId,
                Notes = entity.Description,
                CreationTime = entity.CreationTime,
                CreatorId = entity.CreatorId,
                LastModificationTime = entity.LastModificationTime,
                LastModifierId = entity.LastModifierId
            };
        }
    }
}
