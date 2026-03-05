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
        private readonly IRepository<RuleTemplateAssociation> _associationRepository;
        private readonly TemplateSelectionService _templateSelectionService;

        public ThankYouRuleAppService(
            IRepository<ThankYouRule, Guid> repository,
            IRepository<Donor, Guid> donorRepository,
            IRepository<Donation, Guid> donationRepository,
            IRepository<DonaRogApp.LetterTemplates.LetterTemplate, Guid> templateRepository,
            IRepository<RuleTemplateAssociation> associationRepository,
            TemplateSelectionService templateSelectionService)
            : base(repository)
        {
            _donorRepository = donorRepository;
            _donationRepository = donationRepository;
            _templateRepository = templateRepository;
            _associationRepository = associationRepository;
            _templateSelectionService = templateSelectionService;
        }

        // ======================================================================
        // RULE EVALUATION
        // ======================================================================

        public virtual async Task<ThankYouRuleEvaluationResultDto> EvaluateRulesAsync(EvaluateThankYouRulesDto input)
        {
            // Load donor and donation
            var donor = await _donorRepository.GetAsync(input.DonorId, includeDetails: true);
            var donation = await _donationRepository.GetAsync(input.DonationId, includeDetails: true);

            var evaluationDate = DateTime.UtcNow;

            // Get active rules ordered by: temporary rules first (by priority), then permanent rules (by priority)
            var allRules = await Repository.GetListAsync(r => r.IsActive);
            
            // Filter rules by validity period
            var validRules = allRules.Where(r => r.IsValidOnDate(evaluationDate)).ToList();

            // Sort: temporary rules with higher priority, then permanent rules
            var rules = validRules
                .OrderByDescending(r => r.IsTemporaryRule()) // Temporary first
                .ThenBy(r => r.Priority) // Then by priority
                .ToList();

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

            // Template suggestion using LRU if rule has template pool
            if (bestRule != null)
            {
                // Try LRU selection from rule's template pool
                var selectedTemplateId = await _templateSelectionService.SelectTemplateForDonorAsync(
                    bestRule.Id,
                    donor.Id,
                    CurrentTenant.Id);

                if (selectedTemplateId.HasValue)
                {
                    result.SuggestedTemplateId = selectedTemplateId;
                    result.Explanation += $" | Template selezionato da pool LRU della regola '{bestRule.Name}'";
                }
                else if (bestRule.SuggestedTemplateId.HasValue)
                {
                    // Fallback to deprecated SuggestedTemplateId
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
            }
            else
            {
                // No matching rule: use default template
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
                input.SubjectType,
                input.RecurrenceId);

            rule.SetValidityPeriod(input.ValidFrom, input.ValidUntil);

            rule.UpdateActions(
                input.CreateThankYou,
                input.SuggestedChannel,
                input.SuggestedTemplateId);

            if (!input.IsActive)
            {
                rule.Deactivate();
            }

            await Repository.InsertAsync(rule);

            // Create template pool associations
            foreach (var poolItem in input.TemplatePoolItems ?? new List<CreateUpdateTemplatePoolItemDto>())
            {
                var association = new RuleTemplateAssociation(
                    rule.Id,
                    poolItem.TemplateId,
                    poolItem.Priority,
                    poolItem.IsActive);
                await _associationRepository.InsertAsync(association);
            }

            return await MapToGetOutputDtoAsync(rule);
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
                input.SubjectType,
                input.RecurrenceId);

            rule.SetValidityPeriod(input.ValidFrom, input.ValidUntil);

            rule.UpdateActions(
                input.CreateThankYou,
                input.SuggestedChannel,
                input.SuggestedTemplateId);

            if (input.IsActive)
                rule.Activate();
            else
                rule.Deactivate();

            await Repository.UpdateAsync(rule);

            // Update template pool associations
            var existingAssociations = await _associationRepository.GetListAsync(a => a.RuleId == id);
            var inputTemplateIds = (input.TemplatePoolItems ?? new List<CreateUpdateTemplatePoolItemDto>())
                .Select(p => p.TemplateId).ToHashSet();

            // Remove associations not in input
            var toRemove = existingAssociations.Where(a => !inputTemplateIds.Contains(a.TemplateId)).ToList();
            await _associationRepository.DeleteManyAsync(toRemove);

            // Add or update associations
            foreach (var poolItem in input.TemplatePoolItems ?? new List<CreateUpdateTemplatePoolItemDto>())
            {
                var existing = existingAssociations.FirstOrDefault(a => a.TemplateId == poolItem.TemplateId);
                if (existing != null)
                {
                    existing.Priority = poolItem.Priority;
                    existing.IsActive = poolItem.IsActive;
                    await _associationRepository.UpdateAsync(existing);
                }
                else
                {
                    var newAssociation = new RuleTemplateAssociation(
                        id,
                        poolItem.TemplateId,
                        poolItem.Priority,
                        poolItem.IsActive);
                    await _associationRepository.InsertAsync(newAssociation);
                }
            }

            return await MapToGetOutputDtoAsync(rule);
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
        // RULE CLONING
        // ======================================================================

        public virtual async Task<ThankYouRuleDto> CloneRuleAsync(Guid id, string newName)
        {
            var originalRule = await Repository.GetAsync(id, includeDetails: true);
            
            // Clone the rule entity
            var clonedRule = originalRule.Clone(newName);
            await Repository.InsertAsync(clonedRule);

            // Clone template associations
            var associations = originalRule.TemplateAssociations?.ToList() ?? new List<RuleTemplateAssociation>();
            foreach (var association in associations)
            {
                var newAssociation = new RuleTemplateAssociation(
                    clonedRule.Id,
                    association.TemplateId,
                    association.Priority,
                    association.IsActive);
                await _associationRepository.InsertAsync(newAssociation);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return await MapToGetOutputDtoAsync(clonedRule);
        }

        // ======================================================================
        // TEMPLATE POOL MANAGEMENT
        // ======================================================================

        public virtual async Task AddTemplateToPoolAsync(Guid ruleId, Guid templateId, int priority = 1, bool isActive = true)
        {
            // Check if association already exists
            var existing = await _associationRepository.FirstOrDefaultAsync(
                a => a.RuleId == ruleId && a.TemplateId == templateId);

            if (existing != null)
            {
                throw new Volo.Abp.UserFriendlyException("DonaRog:TemplateAlreadyInPool",
                    "This template is already in the rule's pool");
            }

            var association = new RuleTemplateAssociation(ruleId, templateId, priority, isActive);
            await _associationRepository.InsertAsync(association);
        }

        public virtual async Task RemoveTemplateFromPoolAsync(Guid ruleId, Guid templateId)
        {
            var association = await _associationRepository.FirstOrDefaultAsync(
                a => a.RuleId == ruleId && a.TemplateId == templateId);

            if (association != null)
            {
                await _associationRepository.DeleteAsync(association);
            }
        }

        public virtual async Task UpdateTemplatePoolPriorityAsync(Guid ruleId, Guid templateId, int newPriority)
        {
            var association = await _associationRepository.GetAsync(
                a => a.RuleId == ruleId && a.TemplateId == templateId);

            association.Priority = newPriority;
            await _associationRepository.UpdateAsync(association);
        }

        public virtual async Task ToggleTemplateInPoolAsync(Guid ruleId, Guid templateId)
        {
            var association = await _associationRepository.GetAsync(
                a => a.RuleId == ruleId && a.TemplateId == templateId);

            association.IsActive = !association.IsActive;
            await _associationRepository.UpdateAsync(association);
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
                RecurrenceId = entity.RecurrenceId,
                ValidFrom = entity.ValidFrom,
                ValidUntil = entity.ValidUntil,
                IsTemporary = entity.IsTemporaryRule(),
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

        protected override async Task<ThankYouRuleDto> MapToGetOutputDtoAsync(ThankYouRule entity)
        {
            var dto = MapToGetOutputDto(entity);

            // Load template pool
            var associations = await _associationRepository.GetListAsync(a => a.RuleId == entity.Id);
            var templateIds = associations.Select(a => a.TemplateId).ToList();
            var templates = templateIds.Any() 
                ? await _templateRepository.GetListAsync(t => templateIds.Contains(t.Id))
                : new List<LetterTemplate>();

            dto.TemplatePool = associations
                .OrderBy(a => a.Priority)
                .Select(a => new TemplatePoolItemDto
                {
                    TemplateId = a.TemplateId,
                    TemplateName = templates.FirstOrDefault(t => t.Id == a.TemplateId)?.Name ?? "Unknown",
                    Priority = a.Priority,
                    IsActive = a.IsActive
                }).ToList();

            return dto;
        }
    }
}
