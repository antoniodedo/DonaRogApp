using DonaRogApp.Application.Contracts.Donations;
using DonaRogApp.Application.Contracts.Donations.Dto;
using DonaRogApp.Domain.Donations.Entities;
using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Domain.Projects.Entities;
using DonaRogApp.Enums.Donations;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Application.Donations
{
    public class DonationAppService : ApplicationService, IDonationAppService
    {
        private readonly IRepository<Donation, Guid> _donationRepository;
        private readonly IRepository<Donor, Guid> _donorRepository;
        private readonly IRepository<Project, Guid> _projectRepository;

        public DonationAppService(
            IRepository<Donation, Guid> donationRepository,
            IRepository<Donor, Guid> donorRepository,
            IRepository<Project, Guid> projectRepository)
        {
            _donationRepository = donationRepository;
            _donorRepository = donorRepository;
            _projectRepository = projectRepository;
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

            // Update donor statistics
            await UpdateDonorStatisticsAsync(donation.DonorId, donation.TotalAmount);

            // Update project statistics
            foreach (var allocation in input.ProjectAllocations)
            {
                await UpdateProjectStatisticsAsync(allocation.ProjectId);
            }

            return await GetAsync(id);
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
    }
}

