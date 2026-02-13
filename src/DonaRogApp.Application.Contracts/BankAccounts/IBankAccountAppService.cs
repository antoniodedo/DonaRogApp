using DonaRogApp.Application.Contracts.BankAccounts.Dto;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Application.Contracts.BankAccounts
{
    public interface IBankAccountAppService : IApplicationService
    {
        Task<BankAccountDto> GetAsync(Guid id);
        
        Task<PagedResultDto<BankAccountListDto>> GetListAsync(GetBankAccountsInput input);
        
        Task<BankAccountDto> CreateAsync(CreateUpdateBankAccountDto input);
        
        Task<BankAccountDto> UpdateAsync(Guid id, CreateUpdateBankAccountDto input);
        
        Task DeleteAsync(Guid id);
        
        Task<BankAccountDto> ActivateAsync(Guid id);
        
        Task<BankAccountDto> DeactivateAsync(Guid id);
        
        Task<BankAccountDto> SetAsDefaultAsync(Guid id);
    }
}
