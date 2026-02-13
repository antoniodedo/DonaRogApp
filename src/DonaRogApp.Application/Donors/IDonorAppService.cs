using DonaRogApp.Donors.Dtos;
using DonaRogApp.Donors.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Donors
{
    /// <summary>
    /// Interface per DonorAppService
    /// Definisce i contratti per CRUD e operazioni su Email, Indirizzi, Privacy
    /// </summary>
    public interface IDonorAppService :
        ICrudAppService<
            Dtos.DonorDto,
            Guid,
            Dto.GetDonorsInput,
            Dtos.CreateDonorDto,
            Dtos.UpdateDonorDto>
    {
        // ======================================================================
        // EMAIL MANAGEMENT
        // ======================================================================

        Task AddEmailAsync(Guid donorId, CreateDonorEmailDto input);
        Task RemoveEmailAsync(Guid donorId, string emailAddress);
        Task SetDefaultEmailAsync(Guid donorId, string emailAddress);
        Task VerifyEmailAsync(Guid donorId, string emailAddress);
        Task RecordEmailBounceAsync(Guid donorId, string emailAddress, string? reason = null);
        Task<List<DonorEmailDto>> GetEmailsAsync(Guid donorId);

        // ======================================================================
        // ADDRESS MANAGEMENT
        // ======================================================================

        Task AddAddressAsync(Guid donorId, CreateDonorAddressDto input);
        Task EndAddressAsync(Guid donorId, Guid addressId);
        Task SetDefaultAddressAsync(Guid donorId, Guid addressId);
        Task<List<DonorAddressDto>> GetAddressesAsync(Guid donorId);

        // ======================================================================
        // PRIVACY MANAGEMENT
        // ======================================================================

        Task GrantPrivacyConsentAsync(Guid donorId);
        Task RevokePrivacyConsentAsync(Guid donorId);
        Task GrantNewsletterConsentAsync(Guid donorId);
        Task RevokeNewsletterConsentAsync(Guid donorId);
        Task GrantMailConsentAsync(Guid donorId);
        Task RevokeMailConsentAsync(Guid donorId);
        Task AnonymizeAsync(Guid donorId);

        // ======================================================================
        // STATUS MANAGEMENT
        // ======================================================================

        Task ChangeStatusAsync(Guid donorId, int status, string? note = null);
        Task<List<DonorStatusHistoryDto>> GetStatusHistoryAsync(Guid donorId);

        // ======================================================================
        // TAG MANAGEMENT
        // ======================================================================

        Task<List<DonorTagDto>> GetTagsAsync(Guid donorId);
        Task AddTagAsync(Guid donorId, AssignTagDto input);
        Task DeleteTagAsync(Guid donorId, Guid tagId);
    }
}
