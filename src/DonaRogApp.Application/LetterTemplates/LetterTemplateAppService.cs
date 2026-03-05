using DonaRogApp.Application.Communications;
using DonaRogApp.Application.Contracts.Communications.Dto;
using DonaRogApp.Application.LetterTemplates;
using DonaRogApp.Domain.Storage;
using DonaRogApp.Enums.Communications;
using DonaRogApp.LetterTemplates.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
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
        private readonly IRepository<Domain.Donations.Entities.Donation, Guid> _donationRepository;
        private readonly IRepository<Domain.Communications.Entities.ThankYouRule, Guid> _thankYouRuleRepository;
        private readonly PlaceholderService _placeholderService;
        private readonly TemplateMergeService _templateMergeService;
        private readonly IFileStorageService _fileStorageService;

        public LetterTemplateAppService(
            IRepository<DonaRogApp.LetterTemplates.LetterTemplate, Guid> repository,
            TemplateRenderer templateRenderer,
            IRepository<Domain.Donors.Entities.Donor, Guid> donorRepository,
            IRepository<Domain.Donations.Entities.Donation, Guid> donationRepository,
            IRepository<Domain.Communications.Entities.ThankYouRule, Guid> thankYouRuleRepository,
            PlaceholderService placeholderService,
            TemplateMergeService templateMergeService,
            IFileStorageService fileStorageService)
            : base(repository)
        {
            _templateRenderer = templateRenderer;
            _donorRepository = donorRepository;
            _donationRepository = donationRepository;
            _thankYouRuleRepository = thankYouRuleRepository;
            _placeholderService = placeholderService;
            _templateMergeService = templateMergeService;
            _fileStorageService = fileStorageService;
        }

        // ======================================================================
        // OVERRIDE CRUD METHODS
        // ======================================================================

        protected override async Task<IQueryable<DonaRogApp.LetterTemplates.LetterTemplate>> CreateFilteredQueryAsync(GetLetterTemplatesInput input)
        {
            var query = await Repository.WithDetailsAsync(x => x.Attachments);

            return query
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), x =>
                    x.Name.Contains(input.Filter!) ||
                    (x.Description != null && x.Description.Contains(input.Filter!)) ||
                    (x.Tags != null && x.Tags.Contains(input.Filter!)))
                .WhereIf(input.Category.HasValue, x => x.Category == input.Category!.Value)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Language), x => x.Language == input.Language!)
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive!.Value)
                .WhereIf(input.IsDefault.HasValue, x => x.IsDefault == input.IsDefault!.Value)
                .WhereIf(input.CommunicationType.HasValue, x => x.CommunicationType == input.CommunicationType!.Value || x.CommunicationType == null)
                .WhereIf(input.IsPlural.HasValue, x => x.IsPlural == input.IsPlural!.Value);
        }

        protected override async Task<LetterTemplateDto> MapToGetOutputDtoAsync(DonaRogApp.LetterTemplates.LetterTemplate entity)
        {
            var dto = await base.MapToGetOutputDtoAsync(entity);

            // Load template associations from rules
            var rulesQueryable = await _thankYouRuleRepository.WithDetailsAsync(r => r.TemplateAssociations);
            var rulesUsingTemplate = rulesQueryable
                .Where(r => r.TemplateAssociations.Any(ta => ta.TemplateId == entity.Id))
                .ToList();
            
            dto.AssociatedRules = rulesUsingTemplate
                .SelectMany(rule => rule.TemplateAssociations
                    .Where(ta => ta.TemplateId == entity.Id)
                    .Select(ta => new RuleTemplateAssociationDto
                    {
                        RuleId = rule.Id,
                        RuleName = rule.Name,
                        TemplateId = entity.Id,
                        TemplatePriorityInPool = ta.Priority,
                        IsTemplateActiveInPool = ta.IsActive,
                        RulePriority = rule.Priority,
                        IsRuleActive = rule.IsActive,
                        IsRuleTemporary = rule.IsTemporaryRule(),
                        RuleValidFrom = rule.ValidFrom,
                        RuleValidUntil = rule.ValidUntil
                    }))
                .OrderBy(r => r.RulePriority)
                .ThenBy(r => r.TemplatePriorityInPool)
                .ToList();

            return dto;
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
                IsPlural = entity.IsPlural,
                IsActive = entity.IsActive,
                IsDefault = entity.IsDefault,
                UsageCount = entity.UsageCount,
                LastUsedDate = entity.LastUsedDate,
                Version = entity.Version,
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

            // Build tag values from donor data
            var tagValues = _templateRenderer.BuildDonorTagValues(
                donor,
                project: null,
                recurrence: null
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
        
        // NOTE: Template selection is now handled by ThankYouRules
        // This method returns all active templates for manual selection

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

        // ======================================================================
        // FILE-BASED TEMPLATES (DOCX UPLOAD)
        // ======================================================================

        public virtual async Task<TemplateFileDto> UploadTemplateFileAsync(UploadTemplateDto input)
        {
            var template = await Repository.GetAsync(input.TemplateId);
            
            // Validate file extension
            if (!input.FileExtension.Equals(".docx", StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessException("DonaRog:InvalidTemplateFileExtension")
                    .WithData("allowedExtensions", ".docx")
                    .WithData("providedExtension", input.FileExtension);
            }
            
            // Decode base64 file content
            byte[] fileBytes;
            try
            {
                fileBytes = Convert.FromBase64String(input.FileContentBase64);
            }
            catch (FormatException)
            {
                throw new BusinessException("DonaRog:InvalidBase64FileContent");
            }
            
            // Validate file size
            if (fileBytes.Length != input.FileSizeBytes)
            {
                throw new BusinessException("DonaRog:FileSizeMismatch")
                    .WithData("expectedSize", input.FileSizeBytes)
                    .WithData("actualSize", fileBytes.Length);
            }
            
            // Save file to storage
            using var fileStream = new MemoryStream(fileBytes);
            var storagePath = await _fileStorageService.SaveFileAsync(fileStream, input.FileName, $"templates/{template.Id}");
            
            // Convert DOCX to HTML
            using var conversionStream = new MemoryStream(fileBytes);
            var htmlContent = await _templateMergeService.ConvertDocxToHtmlAsync(conversionStream);
            htmlContent = _templateMergeService.ConvertWordMergeFields(htmlContent);
            
            // Update template
            template.SetTemplateFile(storagePath, input.FileName, input.FileSizeBytes, htmlContent);
            template.UpdateTemplateType(TemplateType.Docx);
            
            await Repository.UpdateAsync(template);
            
            return new TemplateFileDto
            {
                TemplateId = template.Id,
                FileName = template.TemplateFileName,
                FileExtension = Path.GetExtension(template.TemplateFileName),
                FileSizeBytes = template.TemplateFileSizeBytes,
                UploadedAt = template.TemplateFileUploadedAt,
                StoragePath = template.TemplateFilePath
            };
        }

        public virtual async Task<byte[]> DownloadTemplateFileAsync(Guid templateId)
        {
            var template = await Repository.GetAsync(templateId);
            
            if (string.IsNullOrEmpty(template.TemplateFilePath))
            {
                throw new BusinessException("DonaRog:TemplateFileNotFound")
                    .WithData("templateId", templateId);
            }
            
            using var stream = await _fileStorageService.GetFileAsync(template.TemplateFilePath);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public virtual async Task<TemplateFileDto?> GetTemplateFileInfoAsync(Guid templateId)
        {
            var template = await Repository.GetAsync(templateId);
            
            if (string.IsNullOrEmpty(template.TemplateFilePath))
            {
                return null;
            }
            
            return new TemplateFileDto
            {
                TemplateId = template.Id,
                FileName = template.TemplateFileName,
                FileExtension = Path.GetExtension(template.TemplateFileName),
                FileSizeBytes = template.TemplateFileSizeBytes,
                UploadedAt = template.TemplateFileUploadedAt,
                StoragePath = template.TemplateFilePath
            };
        }

        public virtual async Task DeleteTemplateFileAsync(Guid templateId)
        {
            var template = await Repository.GetAsync(templateId);
            
            if (!string.IsNullOrEmpty(template.TemplateFilePath))
            {
                await _fileStorageService.DeleteFileAsync(template.TemplateFilePath);
                
                // Switch back to HTML type
                template.UpdateTemplateType(TemplateType.Html);
                await Repository.UpdateAsync(template);
            }
        }

        // ======================================================================
        // TEMPLATE CONVERSION
        // ======================================================================

        public virtual async Task<TemplateConversionResultDto> ConvertDocxToHtmlAsync(ConvertTemplateDto input)
        {
            var template = await Repository.GetAsync(input.TemplateId);
            
            if (string.IsNullOrEmpty(template.TemplateFilePath))
            {
                throw new BusinessException("DonaRog:NoTemplateFileToConvert")
                    .WithData("templateId", input.TemplateId);
            }
            
            // Read DOCX file
            using var docxStream = await _fileStorageService.GetFileAsync(template.TemplateFilePath);
            using var memoryStream = new MemoryStream();
            await docxStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            
            // Convert to HTML
            var htmlContent = await _templateMergeService.ConvertDocxToHtmlAsync(memoryStream);
            
            // Convert merge fields if requested
            if (input.ConvertMergeFields)
            {
                htmlContent = _templateMergeService.ConvertWordMergeFields(htmlContent);
            }
            
            // Extract and validate placeholders
            var foundPlaceholders = _placeholderService.ExtractPlaceholders(htmlContent);
            var unrecognizedPlaceholders = _placeholderService.ValidatePlaceholders(htmlContent);
            
            // Save to template if requested
            if (input.SaveToTemplate)
            {
                template.UpdateContent(htmlContent);
                await Repository.UpdateAsync(template);
            }
            
            return new TemplateConversionResultDto
            {
                HtmlContent = htmlContent,
                FoundPlaceholders = foundPlaceholders,
                UnrecognizedPlaceholders = unrecognizedPlaceholders,
                Warnings = new List<string>(),
                Success = true,
                EstimatedPdfSizeBytes = _templateMergeService.EstimatePdfSize(htmlContent)
            };
        }

        // ======================================================================
        // PLACEHOLDER MANAGEMENT
        // ======================================================================

        public virtual async Task<PlaceholderListDto> GetAvailablePlaceholdersAsync()
        {
            var placeholders = _placeholderService.GetAllPlaceholders();
            
            var result = new PlaceholderListDto();
            
            // Group by category
            var categories = new Dictionary<string, List<PlaceholderInfoDto>>
            {
                ["Donatore"] = new(),
                ["Indirizzo"] = new(),
                ["Donazione"] = new(),
                ["Progetto"] = new(),
                ["Sistema"] = new(),
                ["Statistiche"] = new(),
                ["Organizzazione"] = new()
            };
            
            foreach (var kvp in placeholders)
            {
                var category = DeterminePlaceholderCategory(kvp.Key);
                
                if (!categories.ContainsKey(category))
                    categories[category] = new List<PlaceholderInfoDto>();
                
                categories[category].Add(new PlaceholderInfoDto
                {
                    Name = kvp.Key,
                    Description = kvp.Value,
                    Category = category,
                    ExampleValue = GetExampleValue(kvp.Key)
                });
            }
            
            result.PlaceholdersByCategory = categories;
            
            return await Task.FromResult(result);
        }

        public virtual async Task<List<string>> ValidatePlaceholdersAsync(Guid templateId)
        {
            var template = await Repository.GetAsync(templateId);
            return _placeholderService.ValidatePlaceholders(template.Content);
        }

        // ======================================================================
        // TEMPLATE PREVIEW
        // ======================================================================

        public virtual async Task<TemplatePreviewResultDto> PreviewTemplateAsync(PreviewTemplateDto input)
        {
            var template = await Repository.GetAsync(input.TemplateId);
            
            // Get sample donor and donation
            Domain.Donors.Entities.Donor donor;
            Domain.Donations.Entities.Donation donation;
            
            if (input.SampleDonorId.HasValue && input.SampleDonationId.HasValue)
            {
                donor = await _donorRepository.GetAsync(input.SampleDonorId.Value, includeDetails: true);
                donation = await _donationRepository.GetAsync(input.SampleDonationId.Value, includeDetails: true);
            }
            else
            {
                // Use mock data
                donor = await GetSampleDonorAsync();
                donation = await GetSampleDonationAsync();
            }
            
            // Build merge data
            var mergeData = _placeholderService.BuildMergeData(donor, donation);
            
            // Replace placeholders
            var htmlContent = _placeholderService.ReplacePlaceholders(template.Content, mergeData);
            
            // Return based on format
            if (input.OutputFormat == PreviewOutputFormat.Html)
            {
                return new TemplatePreviewResultDto
                {
                    Content = htmlContent,
                    Format = PreviewOutputFormat.Html
                };
            }
            else
            {
                // Generate PDF
                var pdfBytes = _templateMergeService.ConvertHtmlToPdf(htmlContent, template.Name);
                var pdfBase64 = Convert.ToBase64String(pdfBytes);
                
                return new TemplatePreviewResultDto
                {
                    Content = pdfBase64,
                    Format = PreviewOutputFormat.Pdf,
                    FileSizeBytes = pdfBytes.Length
                };
            }
        }

        // ======================================================================
        // HELPER METHODS
        // ======================================================================

        private string DeterminePlaceholderCategory(string placeholderName)
        {
            if (placeholderName.StartsWith("Donor"))
            {
                if (placeholderName.Contains("Street") || placeholderName.Contains("City") || 
                    placeholderName.Contains("Address") || placeholderName.Contains("Postal"))
                    return "Indirizzo";
                if (placeholderName.Contains("Total") || placeholderName.Contains("Count") || 
                    placeholderName.Contains("First"))
                    return "Statistiche";
                return "Donatore";
            }
            if (placeholderName.StartsWith("Donation"))
                return "Donazione";
            if (placeholderName.StartsWith("Project"))
                return "Progetto";
            if (placeholderName.StartsWith("Current"))
                return "Sistema";
            if (placeholderName.StartsWith("Org"))
                return "Organizzazione";
            
            return "Altro";
        }

        private string? GetExampleValue(string placeholderName)
        {
            return placeholderName switch
            {
                "DonorFirstName" => "Mario",
                "DonorLastName" => "Rossi",
                "DonorFullName" => "Mario Rossi",
                "DonationAmount" => "150.00",
                "DonationAmountFormatted" => "€150,00",
                "DonationDate" => "2026-02-22",
                "CurrentDate" => DateTime.Now.ToString("yyyy-MM-dd"),
                _ => null
            };
        }

        private async Task<Domain.Donors.Entities.Donor> GetSampleDonorAsync()
        {
            // Return first active donor or create mock
            var donor = await _donorRepository.FirstOrDefaultAsync(d => d.Status == Enums.Donors.DonorStatus.Active);
            
            if (donor != null)
                return donor;
            
            // Create mock donor (not saved to database)
            throw new BusinessException("DonaRog:NoSampleDonorAvailable")
                .WithData("hint", "Create at least one donor or provide SampleDonorId");
        }

        private async Task<Domain.Donations.Entities.Donation> GetSampleDonationAsync()
        {
            // Return first verified donation or create mock
            var donation = await _donationRepository.FirstOrDefaultAsync(d => d.Status == Enums.Donations.DonationStatus.Verified);
            
            if (donation != null)
                return donation;
            
            throw new BusinessException("DonaRog:NoSampleDonationAvailable")
                .WithData("hint", "Create at least one donation or provide SampleDonationId");
        }
    }
}
