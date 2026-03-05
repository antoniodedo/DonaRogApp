using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DonaRogApp.Domain.Communications.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.DependencyInjection;

namespace DonaRogApp.Application.Communications
{
    /// <summary>
    /// Service for selecting templates using LRU (Least Recently Used) rotation
    /// </summary>
    public class TemplateSelectionService : ITransientDependency
    {
        private readonly IRepository<RuleTemplateAssociation> _associationRepository;
        private readonly IRepository<DonorTemplateUsage> _usageRepository;

        public TemplateSelectionService(
            IRepository<RuleTemplateAssociation> associationRepository,
            IRepository<DonorTemplateUsage> usageRepository)
        {
            _associationRepository = associationRepository;
            _usageRepository = usageRepository;
        }

        /// <summary>
        /// Select the best template from a rule's pool using LRU logic for the given donor
        /// </summary>
        /// <param name="ruleId">Rule ID</param>
        /// <param name="donorId">Donor ID</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>Selected template ID, or null if no templates are available</returns>
        public async Task<Guid?> SelectTemplateForDonorAsync(Guid ruleId, Guid donorId, Guid? tenantId)
        {
            // Get all active templates in the rule's pool
            var associations = await _associationRepository.GetListAsync(
                a => a.RuleId == ruleId && a.IsActive,
                includeDetails: false);

            if (!associations.Any())
            {
                return null;
            }

            // Get usage history for this donor for templates in the pool
            var templateIds = associations.Select(a => a.TemplateId).ToList();
            var usageHistory = await _usageRepository.GetListAsync(
                u => u.DonorId == donorId && templateIds.Contains(u.TemplateId));

            // Find LRU template (least recently used)
            var templateIdsWithUsage = usageHistory.Select(u => u.TemplateId).ToHashSet();
            
            // Prioritize never-used templates first
            var neverUsedTemplates = associations
                .Where(a => !templateIdsWithUsage.Contains(a.TemplateId))
                .OrderBy(a => a.Priority)
                .ToList();

            if (neverUsedTemplates.Any())
            {
                return neverUsedTemplates.First().TemplateId;
            }

            // If all templates have been used, select the LRU one
            var lruUsage = usageHistory
                .OrderBy(u => u.LastUsedDate)
                .First();

            return lruUsage.TemplateId;
        }

        /// <summary>
        /// Record the usage of a template for a donor
        /// This updates the LRU tracking
        /// </summary>
        /// <param name="donorId">Donor ID</param>
        /// <param name="templateId">Template ID</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="usedDate">Usage date (defaults to now)</param>
        public async Task RecordTemplateUsageAsync(Guid donorId, Guid templateId, Guid? tenantId, DateTime? usedDate = null)
        {
            var date = usedDate ?? DateTime.UtcNow;

            // Find existing usage record
            var existingUsage = await _usageRepository.FirstOrDefaultAsync(
                u => u.DonorId == donorId && u.TemplateId == templateId);

            if (existingUsage != null)
            {
                // Update existing record
                existingUsage.RecordUsage(date);
                await _usageRepository.UpdateAsync(existingUsage);
            }
            else
            {
                // Create new usage record
                var newUsage = new DonorTemplateUsage(tenantId, donorId, templateId, date);
                await _usageRepository.InsertAsync(newUsage);
            }
        }

        /// <summary>
        /// Get usage statistics for a donor
        /// </summary>
        public async Task<List<DonorTemplateUsage>> GetDonorTemplateUsageHistoryAsync(Guid donorId)
        {
            return await _usageRepository.GetListAsync(u => u.DonorId == donorId);
        }

        /// <summary>
        /// Get usage statistics for a template across all donors
        /// </summary>
        public async Task<List<DonorTemplateUsage>> GetTemplateUsageHistoryAsync(Guid templateId)
        {
            return await _usageRepository.GetListAsync(u => u.TemplateId == templateId);
        }
    }
}
