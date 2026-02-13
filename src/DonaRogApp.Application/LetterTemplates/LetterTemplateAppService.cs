using DonaRogApp.Application.LetterTemplates;
using DonaRogApp.Enums.Communications;
using DonaRogApp.LetterTemplates.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Application.LetterTemplates
{
    /// <summary>
    /// Application Service for Letter Templates
    /// </summary>
    public class LetterTemplateAppService : CrudAppService<
            DonaRogApp.LetterTemplates.LetterTemplate,
            LetterTemplateDto,
            Guid,
            GetLetterTemplatesInput,
            CreateUpdateLetterTemplateDto>,
        DonaRogApp.LetterTemplates.ILetterTemplateAppService
    {
        private readonly TemplateRenderer _templateRenderer;
        private readonly IRepository<Domain.Donors.Entities.Donor, Guid> _donorRepository;
        private readonly IRepository<Domain.Projects.Entities.Project, Guid> _projectRepository;
        private readonly IRepository<Domain.Recurrences.Entities.Recurrence, Guid> _recurrenceRepository;

        public LetterTemplateAppService(
            IRepository<DonaRogApp.LetterTemplates.LetterTemplate, Guid> repository,
            TemplateRenderer templateRenderer,
            IRepository<Domain.Donors.Entities.Donor, Guid> donorRepository,
            IRepository<Domain.Projects.Entities.Project, Guid> projectRepository,
            IRepository<Domain.Recurrences.Entities.Recurrence, Guid> recurrenceRepository)
            : base(repository)
        {
            _templateRenderer = templateRenderer;
            _donorRepository = donorRepository;
            _projectRepository = projectRepository;
            _recurrenceRepository = recurrenceRepository;
        }

        // ======================================================================
        // OVERRIDE CRUD METHODS
        // ======================================================================

        protected override async Task<IQueryable<DonaRogApp.LetterTemplates.LetterTemplate>> CreateFilteredQueryAsync(GetLetterTemplatesInput input)
        {
            var query = await Repository.WithDetailsAsync(x => x.Project, x => x.Recurrence, x => x.Attachments);

            return query
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), x =>
                    x.Name.Contains(input.Filter!) ||
                    (x.Description != null && x.Description.Contains(input.Filter!)) ||
                    (x.Tags != null && x.Tags.Contains(input.Filter!)))
                .WhereIf(input.Category.HasValue, x => x.Category == input.Category!.Value)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Language), x => x.Language == input.Language!)
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive!.Value)
                .WhereIf(input.IsDefault.HasValue, x => x.IsDefault == input.IsDefault!.Value)
                .WhereIf(input.ProjectId.HasValue, x => x.ProjectId == input.ProjectId!.Value)
                .WhereIf(input.RecurrenceId.HasValue, x => x.RecurrenceId == input.RecurrenceId!.Value)
                .WhereIf(input.CommunicationType.HasValue, x => x.CommunicationType == input.CommunicationType!.Value || x.CommunicationType == null)
                .WhereIf(input.IsForNewDonor.HasValue, x => x.IsForNewDonor == input.IsForNewDonor!.Value)
                .WhereIf(input.IsPlural.HasValue, x => x.IsPlural == input.IsPlural!.Value);
        }

        protected override async Task<LetterTemplateDto> MapToGetOutputDtoAsync(DonaRogApp.LetterTemplates.LetterTemplate entity)
        {
            var dto = await base.MapToGetOutputDtoAsync(entity);
            
            // Map navigation properties
            dto.ProjectName = entity.Project?.Name;
            dto.RecurrenceName = entity.Recurrence?.Name;
            
            return dto;
        }

        public override async Task<LetterTemplateDto> CreateAsync(CreateUpdateLetterTemplateDto input)
        {
            // Verify invariants
            if (input.MinAmount.HasValue && input.MaxAmount.HasValue && input.MinAmount.Value > input.MaxAmount.Value)
            {
                throw new Volo.Abp.UserFriendlyException("MinAmount cannot be greater than MaxAmount");
            }

            return await base.CreateAsync(input);
        }

        public override async Task<LetterTemplateDto> UpdateAsync(Guid id, CreateUpdateLetterTemplateDto input)
        {
            // Verify invariants
            if (input.MinAmount.HasValue && input.MaxAmount.HasValue && input.MinAmount.Value > input.MaxAmount.Value)
            {
                throw new Volo.Abp.UserFriendlyException("MinAmount cannot be greater than MaxAmount");
            }

            return await base.UpdateAsync(id, input);
        }

        // ======================================================================
        // LIGHTWEIGHT LIST VIEW
        // ======================================================================

        public virtual async Task<PagedResultDto<LetterTemplateListDto>> GetListViewAsync(GetLetterTemplatesInput input)
        {
            var query = await CreateFilteredQueryAsync(input);

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncExecuter.ToListAsync(query);

            var dtos = entities.Select(entity => new LetterTemplateListDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Category = entity.Category,
                Language = entity.Language,
                CommunicationType = entity.CommunicationType,
                ProjectId = entity.ProjectId,
                RecurrenceId = entity.RecurrenceId,
                MinAmount = entity.MinAmount,
                MaxAmount = entity.MaxAmount,
                IsForNewDonor = entity.IsForNewDonor,
                IsPlural = entity.IsPlural,
                IsActive = entity.IsActive,
                IsDefault = entity.IsDefault,
                UsageCount = entity.UsageCount,
                LastUsedDate = entity.LastUsedDate,
                Version = entity.Version,
                ProjectName = entity.Project?.Name,
                RecurrenceName = entity.Recurrence?.Name,
                Tags = entity.Tags,
                CreationTime = entity.CreationTime,
                LastModificationTime = entity.LastModificationTime
            }).ToList();

            return new PagedResultDto<LetterTemplateListDto>(totalCount, dtos);
        }

        // ======================================================================
        // RENDERING
        // ======================================================================

        public virtual async Task<string> RenderTemplateAsync(Guid templateId, Dictionary<string, string> tagValues)
        {
            var template = await Repository.GetAsync(templateId);
            return _templateRenderer.Render(template.Content, tagValues);
        }

        public virtual async Task<string> RenderTemplateWithDonorDataAsync(RenderTemplateWithDonorInput input)
        {
            var template = await Repository.GetAsync(input.TemplateId);
            
            // Load donor with navigation properties
            var donorQuery = await _donorRepository.WithDetailsAsync(d => d.Addresses, d => d.Emails);
            var donor = donorQuery.FirstOrDefault(d => d.Id == input.DonorId);
            
            if (donor == null)
            {
                throw new Volo.Abp.UserFriendlyException("Donor not found");
            }

            // Load project if template has one
            Domain.Projects.Entities.Project? project = null;
            if (template.ProjectId.HasValue)
            {
                project = await _projectRepository.FirstOrDefaultAsync(p => p.Id == template.ProjectId.Value);
            }

            // Load recurrence if template has one
            Domain.Recurrences.Entities.Recurrence? recurrence = null;
            if (template.RecurrenceId.HasValue)
            {
                recurrence = await _recurrenceRepository.FirstOrDefaultAsync(r => r.Id == template.RecurrenceId.Value);
            }

            // Build tag values from donor data
            var tagValues = _templateRenderer.BuildDonorTagValues(
                donor,
                project: project,
                recurrence: recurrence
            );

            // Merge with additional tags
            foreach (var kvp in input.AdditionalTags)
            {
                tagValues[kvp.Key] = kvp.Value;
            }

            // Render
            return _templateRenderer.Render(template.Content, tagValues);
        }

        // ======================================================================
        // SELECTION AND SUGGESTIONS
        // ======================================================================

        public virtual async Task<List<LetterTemplateDto>> GetSuggestedTemplatesAsync(SelectTemplateInput input)
        {
            var query = await Repository.WithDetailsAsync(x => x.Project, x => x.Recurrence);
            
            var templates = query
                .Where(x => x.IsActive)
                .Where(x => x.Category == input.Category)
                .Where(x => x.Language == input.Language)
                .Where(x => x.CommunicationType == input.PreferredCommunicationType || x.CommunicationType == null)
                .ToList();

            // Filter and score templates
            var scoredTemplates = templates
                .Select(t => new
                {
                    Template = t,
                    Score = t.GetMatchScore(
                        input.DonationAmount,
                        input.IsNewDonor,
                        input.IsPlural,
                        input.ProjectId,
                        input.RecurrenceId)
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Select(x => x.Template)
                .ToList();

            return ObjectMapper.Map<List<DonaRogApp.LetterTemplates.LetterTemplate>, List<LetterTemplateDto>>(scoredTemplates);
        }

        public virtual async Task<LetterTemplateDto?> GetDefaultTemplateAsync(TemplateCategory category, string language)
        {
            var template = await Repository.FirstOrDefaultAsync(x =>
                x.Category == category &&
                x.Language == language &&
                x.IsDefault &&
                x.IsActive);

            return template != null ? await MapToGetOutputDtoAsync(template) : null;
        }

        // ======================================================================
        // TEMPLATE MANAGEMENT
        // ======================================================================

        public virtual async Task<LetterTemplateDto> DuplicateAsync(Guid templateId)
        {
            var originalTemplate = await Repository.GetAsync(templateId);

            var newTemplate = new DonaRogApp.LetterTemplates.LetterTemplate
            {
                TenantId = originalTemplate.TenantId,
                Name = $"{originalTemplate.Name} (Copy)",
                Description = originalTemplate.Description,
                Content = originalTemplate.Content,
                Category = originalTemplate.Category,
                Language = originalTemplate.Language,
                CommunicationType = originalTemplate.CommunicationType,
                ProjectId = originalTemplate.ProjectId,
                RecurrenceId = originalTemplate.RecurrenceId,
                MinAmount = originalTemplate.MinAmount,
                MaxAmount = originalTemplate.MaxAmount,
                IsForNewDonor = originalTemplate.IsForNewDonor,
                IsPlural = originalTemplate.IsPlural,
                IsActive = false, // Duplicated templates start as inactive
                IsDefault = false, // Cannot duplicate as default
                CcEmails = originalTemplate.CcEmails,
                BccEmails = originalTemplate.BccEmails,
                Tags = originalTemplate.Tags
            };

            newTemplate = await Repository.InsertAsync(newTemplate);
            await CurrentUnitOfWork!.SaveChangesAsync();

            return await MapToGetOutputDtoAsync(newTemplate);
        }

        // ======================================================================
        // TEST EMAIL
        // ======================================================================

        public virtual async Task SendTestEmailAsync(SendTestEmailInput input)
        {
            // TODO: Implement email sending
            // For MVP, this can throw NotImplementedException or log a message
            // Will be implemented when email service is integrated
            
            await Task.CompletedTask;
            throw new NotImplementedException("Email sending will be implemented in a future phase");
        }
    }
}
