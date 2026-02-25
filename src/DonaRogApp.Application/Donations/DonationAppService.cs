using DonaRogApp.Application.Communications;
using DonaRogApp.Application.Contracts.Communications;
using DonaRogApp.Application.Contracts.Communications.Dto;
using DonaRogApp.Application.Contracts.Communications.ThankYouRules;
using DonaRogApp.Application.Contracts.Communications.ThankYouRules.Dto;
using DonaRogApp.Application.Contracts.Donations;
using DonaRogApp.Application.Contracts.Donations.Dto;
using DonaRogApp.Domain.Donations.Entities;
using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Domain.Projects.Entities;
using DonaRogApp.Domain.Storage;
using DonaRogApp.Enums.Communications;
using DonaRogApp.Enums.Donations;
using DonaRogApp.Enums.Donors;
using DonaRogApp.LetterTemplates;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Validation;

namespace DonaRogApp.Application.Donations
{
    public class DonationAppService : ApplicationService, IDonationAppService
    {
        private readonly IRepository<Donation, Guid> _donationRepository;
        private readonly IRepository<Donor, Guid> _donorRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<DonationDocument, Guid> _documentRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IRepository<Communication, Guid> _communicationRepository;
        private readonly IRepository<LetterTemplate, Guid> _templateRepository;
        private readonly IThankYouRuleAppService _thankYouRuleService;
        private readonly ICommunicationAppService _communicationService;
        private readonly PlaceholderService _placeholderService;
        private readonly TemplateMergeService _templateMergeService;
        private readonly IGuidGenerator _guidGenerator;

        public DonationAppService(
            IRepository<Donation, Guid> donationRepository,
            IRepository<Donor, Guid> donorRepository,
            IRepository<Project, Guid> projectRepository,
            IRepository<DonationDocument, Guid> documentRepository,
            IFileStorageService fileStorageService,
            IRepository<Communication, Guid> communicationRepository,
            IRepository<LetterTemplate, Guid> templateRepository,
            IThankYouRuleAppService thankYouRuleService,
            ICommunicationAppService communicationService,
            PlaceholderService placeholderService,
            TemplateMergeService templateMergeService,
            IGuidGenerator guidGenerator)
        {
            _donationRepository = donationRepository;
            _donorRepository = donorRepository;
            _projectRepository = projectRepository;
            _documentRepository = documentRepository;
            _fileStorageService = fileStorageService;
            _communicationRepository = communicationRepository;
            _templateRepository = templateRepository;
            _thankYouRuleService = thankYouRuleService;
            _communicationService = communicationService;
            _placeholderService = placeholderService;
            _templateMergeService = templateMergeService;
            _guidGenerator = guidGenerator;
        }

        // ======================================================================
        // CRUD OPERATIONS
        // ======================================================================
        public async Task<DonationDto> GetAsync(Guid id)
        {
            var query = await _donationRepository.WithDetailsAsync(
                d => d.Donor,
                d => d.Campaign,
                d => d.BankAccount,
                d => d.ThankYouTemplate,
                d => d.Projects
            );

            var donation = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == id));
            
            if (donation == null)
            {
                throw new BusinessException("DonaRog:DonationNotFound")
                    .WithData("donationId", id);
            }

            return await MapToDtoAsync(donation);
        }

        public async Task<PagedResultDto<DonationListDto>> GetListAsync(GetDonationsInput input)
        {
            var query = await _donationRepository.WithDetailsAsync(
                d => d.Donor,
                d => d.Campaign,
                d => d.Projects
            );

            // Apply filters
            if (input.Status.HasValue)
            {
                query = query.Where(x => x.Status == input.Status.Value);
            }

            if (input.Channel.HasValue)
            {
                query = query.Where(x => x.Channel == input.Channel.Value);
            }

            if (input.DonorId.HasValue)
            {
                query = query.Where(x => x.DonorId == input.DonorId.Value);
            }

            if (input.CampaignId.HasValue)
            {
                query = query.Where(x => x.CampaignId == input.CampaignId.Value);
            }

            if (input.BankAccountId.HasValue)
            {
                query = query.Where(x => x.BankAccountId == input.BankAccountId.Value);
            }

            if (input.ProjectId.HasValue)
            {
                query = query.Where(x => x.Projects.Any(p => p.ProjectId == input.ProjectId.Value));
            }

            if (input.FromDate.HasValue)
            {
                query = query.Where(x => x.DonationDate >= input.FromDate.Value);
            }

            if (input.ToDate.HasValue)
            {
                query = query.Where(x => x.DonationDate <= input.ToDate.Value);
            }

            if (input.MinAmount.HasValue)
            {
                query = query.Where(x => x.TotalAmount >= input.MinAmount.Value);
            }

            if (input.MaxAmount.HasValue)
            {
                query = query.Where(x => x.TotalAmount <= input.MaxAmount.Value);
            }

            if (!string.IsNullOrWhiteSpace(input.Search))
            {
                var search = input.Search.ToLower();
                query = query.Where(x =>
                    x.Reference.ToLower().Contains(search) ||
                    (x.ExternalId != null && x.ExternalId.ToLower().Contains(search)) ||
                    (x.Notes != null && x.Notes.ToLower().Contains(search)));
            }

            // Get total count
            var totalCount = await AsyncExecuter.CountAsync(query);

            // Apply sorting
            if (string.IsNullOrEmpty(input.Sorting))
            {
                query = query.OrderByDescending(x => x.DonationDate);
            }
            else
            {
                // For now, default sorting
                query = query.OrderByDescending(x => x.DonationDate);
            }

            // Apply paging
            query = query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var items = await AsyncExecuter.ToListAsync(query);

            var listDtos = items.Select(MapToListDto).ToList();

            return new PagedResultDto<DonationListDto>(totalCount, listDtos);
        }

        public async Task<DonationDto> CreateAsync(CreateDonationDto input)
        {
            // Validate donor exists
            var donor = await _donorRepository.GetAsync(input.DonorId);

            // Generate reference
            var lastDonationCount = await _donationRepository.CountAsync();
            var reference = Domain.Donations.Entities.Donation.GenerateReference(lastDonationCount + 1);

            // Create donation - use CreatePending if Status is explicitly Pending, otherwise CreateVerified
            Domain.Donations.Entities.Donation donation;
            
            if (input.Status.HasValue && input.Status.Value == DonationStatus.Pending)
            {
                // Create as Pending (needs verification)
                donation = Domain.Donations.Entities.Donation.CreatePending(
                    GuidGenerator.Create(),
                    CurrentTenant.Id,
                    reference,
                    input.DonorId,
                    input.Channel,
                    input.TotalAmount,
                    input.DonationDate,
                    input.ExternalId ?? $"MAN-{reference}", // ExternalId is required for Pending
                    input.CreditDate,
                    input.Notes
                );
            }
            else
            {
                // Create as Verified (manual creation by operator)
                donation = Domain.Donations.Entities.Donation.CreateVerified(
                    GuidGenerator.Create(),
                    CurrentTenant.Id,
                    reference,
                    input.DonorId,
                    input.Channel,
                    input.TotalAmount,
                    input.DonationDate,
                    input.CreditDate,
                    input.CampaignId,
                    input.BankAccountId,
                    input.Notes,
                    input.InternalNotes
                );
                
                // Add project allocations only for verified donations
                if (input.ProjectAllocations.Any())
                {
                    foreach (var allocation in input.ProjectAllocations)
                    {
                        // Validate project exists
                        await _projectRepository.GetAsync(allocation.ProjectId);
                        donation.AllocateToProject(allocation.ProjectId, allocation.AllocatedAmount);
                    }
                }
            }

            await _donationRepository.InsertAsync(donation, autoSave: true);

            // Update statistics only for verified donations
            if (donation.Status == DonationStatus.Verified)
            {
                // Update donor statistics
                await UpdateDonorStatisticsAsync(input.DonorId, input.TotalAmount);

                // Update project statistics
                foreach (var allocation in input.ProjectAllocations)
                {
                    await UpdateProjectStatisticsAsync(allocation.ProjectId);
                }
            }

            // Map directly to DTO instead of querying again
            return await MapToDtoAsync(donation);
        }

        public async Task<DonationDto> UpdateAsync(Guid id, UpdateDonationDto input)
        {
            Logger.LogInformation($"UpdateAsync called for donation {id}");
            Logger.LogInformation($"Input: Channel={input.Channel}, Amount={input.TotalAmount}, ProjectAllocations={input.ProjectAllocations?.Count ?? 0}");
            
            var query = await _donationRepository.WithDetailsAsync(d => d.Projects);
            var donation = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == id));

            if (donation == null)
            {
                throw new BusinessException("DonaRog:DonationNotFound")
                    .WithData("donationId", id);
            }

            Logger.LogInformation($"Current donation: Amount={donation.TotalAmount}, Projects={donation.Projects.Count}, ExternalId={donation.ExternalId}");

            // Store old amount for statistics recalculation
            var oldAmount = donation.TotalAmount;
            var oldDonorId = donation.DonorId;

            // IMPORTANT: Update project allocations BEFORE updating core data
            // This prevents VerifyInvariants from failing with old allocations vs new total
            // Always update allocations (even if empty) to allow clearing all projects
            var allocations = input.ProjectAllocations
                .Select(a => (a.ProjectId, a.AllocatedAmount))
                .ToArray();
            
            // Validate all projects exist
            foreach (var allocation in input.ProjectAllocations)
            {
                await _projectRepository.GetAsync(allocation.ProjectId);
            }
            
            donation.SetProjectAllocations(allocations);

            // Update core data (channel, amount, dates) - only if provided and donation is manual
            if (input.Channel.HasValue || input.TotalAmount.HasValue || input.DonationDate.HasValue)
            {
                var channel = input.Channel ?? donation.Channel;
                var totalAmount = input.TotalAmount ?? donation.TotalAmount;
                var donationDate = input.DonationDate ?? donation.DonationDate;
                var creditDate = input.CreditDate.HasValue ? input.CreditDate : donation.CreditDate;

                // This will throw if donation has ExternalId
                donation.UpdateCoreData(channel, totalAmount, donationDate, creditDate);
            }

            // Update metadata (always allowed)
            donation.UpdateMetadata(
                input.CampaignId,
                input.BankAccountId,
                input.ThankYouTemplateId,
                input.Notes,
                input.InternalNotes
            );

            await _donationRepository.UpdateAsync(donation);

            // Recalculate statistics if amount or donor changed
            if (oldAmount != donation.TotalAmount || oldDonorId != donation.DonorId)
            {
                Logger.LogInformation($"Recalculating statistics: oldAmount={oldAmount}, newAmount={donation.TotalAmount}");
                
                // Recalculate donor statistics from scratch
                await RecalculateDonorStatisticsAsync(donation.DonorId);
                
                // If donor changed, recalculate old donor too
                if (oldDonorId != donation.DonorId)
                {
                    await RecalculateDonorStatisticsAsync(oldDonorId);
                }
            }

            // Update project statistics for all projects (old and new)
            var allProjectIds = donation.Projects.Select(p => p.ProjectId).Distinct();
            foreach (var projectId in allProjectIds)
            {
                await UpdateProjectStatisticsAsync(projectId);
            }

            return await GetAsync(id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var donation = await _donationRepository.GetAsync(id);

            if (donation.Status == DonationStatus.Verified)
            {
                throw new BusinessException("DonaRog:CannotDeleteVerifiedDonation")
                    .WithData("donationId", id);
            }

            await _donationRepository.DeleteAsync(id);
        }

        // ======================================================================
        // WORKFLOW OPERATIONS
        // ======================================================================
        public async Task<DonationDto> VerifyAsync(Guid id, VerifyDonationDto input)
        {
            var query = await _donationRepository.WithDetailsAsync(d => d.Projects);
            var donation = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == id));

            if (donation == null)
            {
                throw new BusinessException("DonaRog:DonationNotFound")
                    .WithData("donationId", id);
            }

            // Update donor if changed
            if (donation.DonorId != input.DonorId)
            {
                await _donorRepository.GetAsync(input.DonorId); // Validate exists
                donation.UpdateDonor(input.DonorId);
            }

            // Set project allocations (replaces existing)
            if (input.ProjectAllocations.Any())
            {
                var allocations = input.ProjectAllocations
                    .Select(a => (a.ProjectId, a.AllocatedAmount))
                    .ToArray();
                
                donation.SetProjectAllocations(allocations);
            }

            // Verify donation
            donation.Verify(
                CurrentUser.Id ?? Guid.Empty,
                input.CampaignId,
                input.BankAccountId,
                input.ThankYouTemplateId,
                input.Notes,
                input.InternalNotes
            );

            await _donationRepository.UpdateAsync(donation);
            await CurrentUnitOfWork!.SaveChangesAsync();

            // ======================================================================
            // THANK YOU WORKFLOW (New - Phase 5)
            // ======================================================================
            await ProcessThankYouAsync(donation, input);

            // Update donor statistics
            await UpdateDonorStatisticsAsync(donation.DonorId, donation.TotalAmount);

            // Update project statistics
            foreach (var allocation in input.ProjectAllocations)
            {
                await UpdateProjectStatisticsAsync(allocation.ProjectId);
            }

            return await GetAsync(id);
        }
        
        /// <summary>
        /// Process thank you creation based on rules and user input
        /// </summary>
        private async Task ProcessThankYouAsync(Domain.Donations.Entities.Donation donation, VerifyDonationDto input)
        {
            Logger.LogInformation("Processing thank you for donation {DonationId}", donation.Id);

            // Step 1: Check for duplicates and show alerts
            var duplicateCheck = await _communicationService.CheckDuplicateLettersAsync(new CheckDuplicateLettersDto
            {
                DonorId = donation.DonorId,
                ErrorThresholdDays = 7,
                WarningThresholdDays = 15,
                CheckLastDays = 30
            });

            if (duplicateCheck.HasCriticalAlert)
            {
                Logger.LogWarning("Critical duplicate alert for donor {DonorId}: {Message}", 
                    donation.DonorId, duplicateCheck.Message);
            }

            // Step 2: Determine if we should create thank you
            bool shouldCreateThankYou;
            PreferredThankYouChannel channel;
            Guid? templateId;
            string explanation;

            if (input.CreateThankYou.HasValue)
            {
                shouldCreateThankYou = input.CreateThankYou.Value;
                channel = input.ThankYouChannel ?? PreferredThankYouChannel.Letter;
                templateId = input.ThankYouTemplateId;
                explanation = input.CreateThankYou.Value ? 
                    "Ringraziamento forzato dall'operatore" : 
                    $"Ringraziamento escluso dall'operatore: {input.NoThankYouReason ?? "N/A"}";
                
                Logger.LogInformation("Thank you decision overridden by operator: {Decision}", shouldCreateThankYou);
            }
            else
            {
                // Evaluate rules
                var ruleEvaluation = await _thankYouRuleService.EvaluateRulesAsync(new EvaluateThankYouRulesDto
                {
                    DonorId = donation.DonorId,
                    DonationId = donation.Id
                });

                shouldCreateThankYou = ruleEvaluation.ShouldCreateThankYou;
                channel = input.ThankYouChannel ?? ruleEvaluation.SuggestedChannel;
                templateId = input.ThankYouTemplateId ?? ruleEvaluation.SuggestedTemplateId;
                explanation = ruleEvaluation.Explanation;

                Logger.LogInformation("Rules evaluated: Create={Create}, Channel={Channel}, Explanation={Explanation}",
                    shouldCreateThankYou, channel, explanation);
            }

            if (!shouldCreateThankYou)
            {
                Logger.LogInformation("Skipping thank you creation for donation {DonationId}: {Reason}", 
                    donation.Id, explanation);
                return;
            }

            // Step 3: Create communication(s) based on channel
            var donor = await _donorRepository.GetAsync(donation.DonorId, includeDetails: true);
            
            if (channel == PreferredThankYouChannel.Letter || channel == PreferredThankYouChannel.Both)
            {
                await CreateThankYouLetterAsync(donor, donation, templateId, input.PrintImmediately);
            }

            if (channel == PreferredThankYouChannel.Email || channel == PreferredThankYouChannel.Both)
            {
                await CreateThankYouEmailAsync(donor, donation, templateId);
            }

            Logger.LogInformation("Thank you processing completed for donation {DonationId}", donation.Id);
        }

        /// <summary>
        /// Create thank you letter communication
        /// </summary>
        private async Task CreateThankYouLetterAsync(
            Donor donor, 
            Domain.Donations.Entities.Donation donation, 
            Guid? templateId, 
            bool printImmediately)
        {
            var template = templateId.HasValue ? 
                await _templateRepository.GetAsync(templateId.Value) : 
                await _templateRepository.FirstOrDefaultAsync(t => t.Category == TemplateCategory.ThankYou && t.IsActive);

            if (template == null)
            {
                Logger.LogWarning("No template found for thank you letter");
                return;
            }

            // Build merge data
            var mergeData = _placeholderService.BuildMergeData(donor, donation);
            
            // Get template content (HTML or DOCX converted to HTML)
            string htmlContent;
            if (template.TemplateType == TemplateType.Html)
            {
                htmlContent = template.Content ?? "";
            }
            else if (template.TemplateType == TemplateType.Docx && !string.IsNullOrEmpty(template.TemplateFilePath))
            {
                htmlContent = await _templateMergeService.ConvertDocxToHtmlAsync(template.TemplateFilePath);
            }
            else
            {
                Logger.LogWarning("Template {TemplateId} has invalid type or no content", template.Id);
                return;
            }

            // Replace placeholders
            var body = _placeholderService.ReplacePlaceholders(htmlContent, mergeData);

            // Create communication
            var communication = Communication.Create(
                donor.Id,
                CommunicationType.Letter,
                $"Ringraziamento donazione {donation.Reference}",
                "Donor Address", // Recipient will be resolved from donor address
                CurrentTenant.Id,
                TemplateCategory.ThankYou,
                template.Id,
                donation.Id,
                donation.CampaignId,
                body,
                CurrentUser.Id,
                null,
                CommunicationStatus.PendingPrint);

            await _communicationRepository.InsertAsync(communication);
            
            Logger.LogInformation("Thank you letter created: {CommunicationId} for donation {DonationId}, PrintImmediately={Print}",
                communication.Id, donation.Id, printImmediately);

            // TODO: If printImmediately, generate PDF and return download link
        }

        /// <summary>
        /// Create thank you email communication
        /// </summary>
        private async Task CreateThankYouEmailAsync(
            Donor donor, 
            Domain.Donations.Entities.Donation donation, 
            Guid? templateId)
        {
            var template = templateId.HasValue ? 
                await _templateRepository.GetAsync(templateId.Value) : 
                await _templateRepository.FirstOrDefaultAsync(t => t.Category == TemplateCategory.ThankYou && t.IsActive);

            if (template == null)
            {
                Logger.LogWarning("No template found for thank you email");
                return;
            }

            // Get donor email
            var donorEmail = donor.Emails.FirstOrDefault(e => e.IsDefault)?.EmailAddress;
            if (string.IsNullOrEmpty(donorEmail))
            {
                Logger.LogWarning("Donor {DonorId} has no default email address", donor.Id);
                return;
            }

            // Build merge data
            var mergeData = _placeholderService.BuildMergeData(donor, donation);
            
            // Get template content
            string htmlContent;
            if (template.TemplateType == TemplateType.Html)
            {
                htmlContent = template.Content ?? "";
            }
            else if (template.TemplateType == TemplateType.Docx && !string.IsNullOrEmpty(template.TemplateFilePath))
            {
                htmlContent = await _templateMergeService.ConvertDocxToHtmlAsync(template.TemplateFilePath);
            }
            else
            {
                Logger.LogWarning("Template {TemplateId} has invalid type or no content", template.Id);
                return;
            }

            // Replace placeholders
            var body = _placeholderService.ReplacePlaceholders(htmlContent, mergeData);
            var subject = string.IsNullOrEmpty(template.EmailSubject) ? 
                $"Grazie per la tua donazione - {donation.Reference}" : 
                _placeholderService.ReplacePlaceholders(template.EmailSubject, mergeData);

            // Create communication
            var communication = Communication.Create(
                donor.Id,
                CommunicationType.Email,
                subject,
                donorEmail,
                CurrentTenant.Id,
                TemplateCategory.ThankYou,
                template.Id,
                donation.Id,
                donation.CampaignId,
                body,
                CurrentUser.Id,
                null,
                CommunicationStatus.Draft);
            
            await _communicationRepository.InsertAsync(communication);
            
            Logger.LogInformation("Thank you email created: {CommunicationId} for donation {DonationId}", 
                communication.Id, donation.Id);

            // TODO: Send email immediately (requires email service integration)
        }

        public async Task<DonationDto> RejectAsync(Guid id, RejectDonationDto input)
        {
            var donation = await _donationRepository.GetAsync(id);

            donation.Reject(
                CurrentUser.Id ?? Guid.Empty,
                input.Reason,
                input.Notes
            );

            await _donationRepository.UpdateAsync(donation);

            return await GetAsync(id);
        }

        // ======================================================================
        // PROJECT ALLOCATION OPERATIONS
        // ======================================================================
        public async Task AllocateToProjectAsync(Guid id, Guid projectId, decimal amount)
        {
            var query = await _donationRepository.WithDetailsAsync(d => d.Projects);
            var donation = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == id));

            if (donation == null)
            {
                throw new BusinessException("DonaRog:DonationNotFound")
                    .WithData("donationId", id);
            }

            // Validate project exists
            await _projectRepository.GetAsync(projectId);

            donation.AllocateToProject(projectId, amount);

            await _donationRepository.UpdateAsync(donation);

            // Update project statistics if donation is verified
            if (donation.IsVerified())
            {
                await UpdateProjectStatisticsAsync(projectId);
            }
        }

        public async Task RemoveProjectAllocationAsync(Guid id, Guid projectId)
        {
            var query = await _donationRepository.WithDetailsAsync(d => d.Projects);
            var donation = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == id));

            if (donation == null)
            {
                throw new BusinessException("DonaRog:DonationNotFound")
                    .WithData("donationId", id);
            }

            donation.RemoveProjectAllocation(projectId);

            await _donationRepository.UpdateAsync(donation);

            // Update project statistics if donation is verified
            if (donation.IsVerified())
            {
                await UpdateProjectStatisticsAsync(projectId);
            }
        }

        // ======================================================================
        // EXTERNAL IMPORT (for demo purposes)
        // ======================================================================
        public async Task<DonationDto> AddExternalAsync(ExternalDonationDto input)
        {
            // Check for duplicate external ID
            var existing = await _donationRepository.FirstOrDefaultAsync(d => d.ExternalId == input.ExternalId);
            if (existing != null)
            {
                throw new BusinessException("DonaRog:DonationExternalIdAlreadyExists")
                    .WithData("externalId", input.ExternalId)
                    .WithData("existingDonationId", existing.Id);
            }

            // Determine donor
            Guid donorId;
            if (input.DonorId.HasValue)
            {
                // Validate donor exists
                await _donorRepository.GetAsync(input.DonorId.Value);
                donorId = input.DonorId.Value;
            }
            else
            {
                // If no donor ID provided, create a placeholder or require manual verification
                // For demo purposes, we'll create with a default donor or fail
                throw new BusinessException("DonaRog:DonorIdRequiredForExternalDonation")
                    .WithData("externalId", input.ExternalId)
                    .WithData("donorReference", input.DonorReference);
            }

            // Generate reference
            var reference = $"EXT-{input.ExternalId}";

            // Create pending donation
            var donation = Domain.Donations.Entities.Donation.CreatePending(
                GuidGenerator.Create(),
                CurrentTenant.Id,
                reference,
                donorId,
                input.Channel,
                input.Amount,
                input.DonationDate,
                input.ExternalId,
                input.CreditDate,
                input.Notes
            );

            await _donationRepository.InsertAsync(donation);

            return await GetAsync(donation.Id);
        }

        // ======================================================================
        // STATISTICS
        // ======================================================================
        public async Task<DonationStatisticsDto> GetStatisticsAsync(GetDonationsInput filter)
        {
            var query = await _donationRepository.GetQueryableAsync();

            // Apply same filters as GetListAsync
            if (filter.Status.HasValue)
                query = query.Where(x => x.Status == filter.Status.Value);
            if (filter.Channel.HasValue)
                query = query.Where(x => x.Channel == filter.Channel.Value);
            if (filter.DonorId.HasValue)
                query = query.Where(x => x.DonorId == filter.DonorId.Value);
            if (filter.CampaignId.HasValue)
                query = query.Where(x => x.CampaignId == filter.CampaignId.Value);
            if (filter.FromDate.HasValue)
                query = query.Where(x => x.DonationDate >= filter.FromDate.Value);
            if (filter.ToDate.HasValue)
                query = query.Where(x => x.DonationDate <= filter.ToDate.Value);

            var donations = await AsyncExecuter.ToListAsync(query);

            return new DonationStatisticsDto
            {
                TotalCount = donations.Count,
                PendingCount = donations.Count(d => d.Status == DonationStatus.Pending),
                VerifiedCount = donations.Count(d => d.Status == DonationStatus.Verified),
                RejectedCount = donations.Count(d => d.Status == DonationStatus.Rejected),
                TotalAmount = donations.Sum(d => d.TotalAmount),
                TotalVerifiedAmount = donations.Where(d => d.Status == DonationStatus.Verified).Sum(d => d.TotalAmount),
                AverageAmount = donations.Any() ? donations.Average(d => d.TotalAmount) : 0,
                FirstDonationDate = donations.Any() ? donations.Min(d => d.DonationDate) : null,
                LastDonationDate = donations.Any() ? donations.Max(d => d.DonationDate) : null
            };
        }

        // ======================================================================
        // PRIVATE HELPER METHODS
        // ======================================================================
        
        private async Task UpdateDonorStatisticsAsync(Guid donorId, decimal donationAmount)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            donor.UpdateStatistics(donationAmount);
            await _donorRepository.UpdateAsync(donor);
        }
        
        private async Task RecalculateDonorStatisticsAsync(Guid donorId)
        {
            // Get all verified donations for this donor
            var donorDonations = await AsyncExecuter.ToListAsync(
                (await _donationRepository.GetQueryableAsync())
                .Where(d => d.DonorId == donorId && d.Status == DonationStatus.Verified)
            );
            
            var donor = await _donorRepository.GetAsync(donorId);
            
            // Calculate aggregates
            var totalDonated = donorDonations.Sum(d => d.TotalAmount);
            var donationCount = donorDonations.Count;
            
            var orderedDonations = donorDonations.OrderBy(d => d.DonationDate).ToList();
            DateTime? firstDonationDate = null;
            decimal firstDonationAmount = 0;
            DateTime? lastDonationDate = null;
            decimal lastDonationAmount = 0;
            
            if (orderedDonations.Any())
            {
                var firstDonation = orderedDonations.First();
                firstDonationDate = firstDonation.DonationDate;
                firstDonationAmount = firstDonation.TotalAmount;
                
                var lastDonation = orderedDonations.Last();
                lastDonationDate = lastDonation.DonationDate;
                lastDonationAmount = lastDonation.TotalAmount;
            }
            
            // Use domain method to recalculate
            donor.RecalculateStatisticsFromData(
                totalDonated,
                donationCount,
                firstDonationDate,
                firstDonationAmount,
                lastDonationDate,
                lastDonationAmount
            );
            
            await _donorRepository.UpdateAsync(donor);
            
            Logger.LogInformation($"Recalculated stats for donor {donorId}: Total={totalDonated}, Count={donationCount}");
        }

        private async Task UpdateProjectStatisticsAsync(Guid projectId)
        {
            var project = await _projectRepository.GetAsync(projectId);
            
            // Calculate project statistics from all verified donations
            var query = await _donationRepository.WithDetailsAsync(d => d.Projects);
            var projectDonations = await AsyncExecuter.ToListAsync(
                query.Where(d => 
                    d.Status == DonationStatus.Verified &&
                    d.Projects.Any(p => p.ProjectId == projectId))
            );

            var totalAmount = projectDonations
                .SelectMany(d => d.Projects)
                .Where(p => p.ProjectId == projectId)
                .Sum(p => p.AllocatedAmount);

            var donationsCount = projectDonations.Count;
            var averageDonation = donationsCount > 0 ? totalAmount / donationsCount : 0;
            var lastDonationDate = projectDonations.Any() 
                ? projectDonations.Max(d => d.DonationDate) 
                : (DateTime?)null;

            project.UpdateStatistics(totalAmount, donationsCount, averageDonation, lastDonationDate);
            await _projectRepository.UpdateAsync(project);
        }

        private async Task<DonationDto> MapToDtoAsync(Donation donation)
        {
            // Load related data if not already loaded
            if (donation.Projects == null || !donation.Projects.Any())
            {
                var query = await _donationRepository.WithDetailsAsync(d => d.Projects);
                donation = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donation.Id)) ?? donation;
            }

            var projectDtos = new System.Collections.Generic.List<DonationProjectDto>();
            if (donation.Projects.Any())
            {
                var projectIds = donation.Projects.Select(p => p.ProjectId).ToList();
                var projects = await _projectRepository.GetListAsync(p => projectIds.Contains(p.Id));

                projectDtos = donation.Projects.Select(dp =>
                {
                    var project = projects.FirstOrDefault(p => p.Id == dp.ProjectId);
                    return new DonationProjectDto
                    {
                        ProjectId = dp.ProjectId,
                        ProjectName = project?.Name ?? "Unknown",
                        AllocatedAmount = dp.AllocatedAmount
                    };
                }).ToList();
            }

            return new DonationDto
            {
                Id = donation.Id,
                Reference = donation.Reference,
                ExternalId = donation.ExternalId,
                DonorId = donation.DonorId,
                DonorFullName = donation.Donor != null 
                    ? $"{donation.Donor.FirstName} {donation.Donor.LastName}"
                    : "Unknown",
                CampaignId = donation.CampaignId,
                CampaignName = donation.Campaign?.Name,
                BankAccountId = donation.BankAccountId,
                BankAccountName = donation.BankAccount?.AccountName,
                ThankYouTemplateId = donation.ThankYouTemplateId,
                ThankYouTemplateName = donation.ThankYouTemplate?.Name,
                Channel = donation.Channel,
                Status = donation.Status,
                TotalAmount = donation.TotalAmount,
                Currency = donation.Currency,
                DonationDate = donation.DonationDate,
                CreditDate = donation.CreditDate,
                RejectionReason = donation.RejectionReason,
                RejectionNotes = donation.RejectionNotes,
                RejectedAt = donation.RejectedAt,
                RejectedBy = donation.RejectedBy,
                VerifiedAt = donation.VerifiedAt,
                VerifiedBy = donation.VerifiedBy,
                Notes = donation.Notes,
                InternalNotes = donation.InternalNotes,
                Projects = projectDtos,
                TotalAllocatedAmount = donation.GetTotalAllocatedAmount(),
                UnallocatedAmount = donation.GetUnallocatedAmount(),
                IsFullyAllocated = donation.IsFullyAllocated(),
                IsFromExternalFlow = donation.IsFromExternalFlow(),
                CanEditCoreData = donation.CanEditCoreData(),
                CreationTime = donation.CreationTime,
                CreatorId = donation.CreatorId,
                LastModificationTime = donation.LastModificationTime,
                LastModifierId = donation.LastModifierId
            };
        }

        private DonationListDto MapToListDto(Donation donation)
        {
            var projectNames = donation.Projects?
                .Select(p => "Project") // We don't have project name here, would need to load
                .ToList() ?? new System.Collections.Generic.List<string>();

            return new DonationListDto
            {
                Id = donation.Id,
                Reference = donation.Reference,
                DonorId = donation.DonorId,
                DonorFullName = donation.Donor != null
                    ? $"{donation.Donor.FirstName} {donation.Donor.LastName}"
                    : "Unknown",
                Channel = donation.Channel,
                Status = donation.Status,
                TotalAmount = donation.TotalAmount,
                Currency = donation.Currency,
                DonationDate = donation.DonationDate,
                CreditDate = donation.CreditDate,
                CampaignId = donation.CampaignId,
                CampaignName = donation.Campaign?.Name,
                ProjectNames = projectNames,
                IsFullyAllocated = donation.IsFullyAllocated()
            };
        }

        // ======================================================================
        // DOCUMENT MANAGEMENT
        // ======================================================================
        
        public async Task<List<DonationDocumentDto>> GetDocumentsAsync(Guid donationId)
        {
            var donation = await _donationRepository.GetAsync(donationId);
            
            var documents = await AsyncExecuter.ToListAsync(
                (await _documentRepository.GetQueryableAsync())
                .Where(d => d.DonationId == donationId)
                .OrderByDescending(d => d.CreationTime)
            );

            return documents.Select(d => new DonationDocumentDto
            {
                Id = d.Id,
                DonationId = d.DonationId,
                FileName = d.FileName,
                FileExtension = d.FileExtension,
                MimeType = d.MimeType,
                FileSizeBytes = d.FileSizeBytes,
                StoragePath = d.StoragePath,
                TextContent = d.TextContent,
                DocumentType = d.DocumentType,
                IsFromExternalFlow = d.IsFromExternalFlow,
                IsTextDocument = d.IsTextDocument,
                Notes = d.Notes,
                CreationTime = d.CreationTime
            }).ToList();
        }

        [DisableValidation]
        public async Task<DonationDocumentDto> SaveDocumentAsync(
            Guid donationId,
            Stream fileStream,
            string fileName,
            string mimeType,
            long fileSizeBytes,
            UploadDonationDocumentDto input)
        {
            var query = await _donationRepository.WithDetailsAsync(d => d.Documents);
            var donation = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donationId));
            
            if (donation == null)
            {
                throw new BusinessException("DonaRog:DonationNotFound")
                    .WithData("donationId", donationId);
            }
            
            // Get extension from filename
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            
            // Generate subfolder by date (e.g., "2024/01")
            var subfolder = $"{DateTime.UtcNow:yyyy}/{DateTime.UtcNow:MM}";
            
            // Save file to storage
            var storagePath = await _fileStorageService.SaveFileAsync(fileStream, fileName, subfolder);
            
            // Create document entity
            var document = donation.AddDocument(
                fileName,
                fileExtension,
                mimeType,
                fileSizeBytes,
                storagePath,
                input.DocumentType,
                isFromExternalFlow: false
            );
            
            if (!string.IsNullOrWhiteSpace(input.Notes))
            {
                document.UpdateNotes(input.Notes);
            }

            await _donationRepository.UpdateAsync(donation);

            return new DonationDocumentDto
            {
                Id = document.Id,
                DonationId = document.DonationId,
                FileName = document.FileName,
                FileExtension = document.FileExtension,
                MimeType = document.MimeType,
                FileSizeBytes = document.FileSizeBytes,
                StoragePath = document.StoragePath,
                TextContent = document.TextContent,
                DocumentType = document.DocumentType,
                IsFromExternalFlow = document.IsFromExternalFlow,
                IsTextDocument = document.IsTextDocument,
                Notes = document.Notes,
                CreationTime = document.CreationTime
            };
        }

        public async Task<DonationDocumentDto> SaveTextDocumentAsync(
            Guid donationId,
            CreateTextDocumentDto input)
        {
            var query = await _donationRepository.WithDetailsAsync(d => d.Documents);
            var donation = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donationId));

            if (donation == null)
            {
                throw new BusinessException("DonaRog:DonationNotFound")
                    .WithData("donationId", donationId);
            }

            // Create text document entity
            var document = donation.AddTextDocument(
                input.TextContent,
                input.DocumentType,
                input.IsFromExternalFlow
            );

            if (!string.IsNullOrWhiteSpace(input.Notes))
            {
                document.UpdateNotes(input.Notes);
            }

            await _donationRepository.UpdateAsync(donation);

            return new DonationDocumentDto
            {
                Id = document.Id,
                DonationId = document.DonationId,
                FileName = document.FileName,
                FileExtension = document.FileExtension,
                MimeType = document.MimeType,
                FileSizeBytes = document.FileSizeBytes,
                StoragePath = document.StoragePath,
                TextContent = document.TextContent,
                DocumentType = document.DocumentType,
                IsFromExternalFlow = document.IsFromExternalFlow,
                IsTextDocument = document.IsTextDocument,
                Notes = document.Notes,
                CreationTime = document.CreationTime
            };
        }

        public async Task<(Stream stream, string fileName, string mimeType)> GetDocumentFileAsync(Guid documentId)
        {
            var document = await _documentRepository.GetAsync(documentId);
            
            var stream = await _fileStorageService.GetFileAsync(document.StoragePath);
            
            return (stream, document.FileName, document.MimeType);
        }

        public async Task DeleteDocumentAsync(Guid documentId)
        {
            var document = await _documentRepository.GetAsync(documentId);
            
            var query = await _donationRepository.WithDetailsAsync(d => d.Documents);
            var donation = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == document.DonationId));
            
            if (donation == null)
            {
                throw new BusinessException("DonaRog:DonationNotFound")
                    .WithData("donationId", document.DonationId);
            }
            
            // Remove document from donation
            donation.RemoveDocument(documentId);
            
            // Delete physical file
            await _fileStorageService.DeleteFileAsync(document.StoragePath);
            
            await _donationRepository.UpdateAsync(donation);
        }
    }
}

