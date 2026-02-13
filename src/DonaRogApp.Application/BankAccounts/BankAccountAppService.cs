using DonaRogApp.Application.Contracts.BankAccounts;
using DonaRogApp.Application.Contracts.BankAccounts.Dto;
using DonaRogApp.Domain.BankAccounts.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Application.BankAccounts
{
    public class BankAccountAppService : ApplicationService, IBankAccountAppService
    {
        private readonly IRepository<BankAccount, Guid> _bankAccountRepository;

        public BankAccountAppService(IRepository<BankAccount, Guid> bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<BankAccountDto> GetAsync(Guid id)
        {
            var bankAccount = await _bankAccountRepository.GetAsync(id);
            return MapToDto(bankAccount);
        }

        public async Task<PagedResultDto<BankAccountListDto>> GetListAsync(GetBankAccountsInput input)
        {
            var query = await _bankAccountRepository.GetQueryableAsync();

            // Apply filters
            if (input.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == input.IsActive.Value);
            }

            if (input.IsDefault.HasValue)
            {
                query = query.Where(x => x.IsDefault == input.IsDefault.Value);
            }

            if (!string.IsNullOrWhiteSpace(input.Search))
            {
                var search = input.Search.ToLower();
                query = query.Where(x => 
                    x.AccountName.ToLower().Contains(search) ||
                    (x.BankName != null && x.BankName.ToLower().Contains(search)));
            }

            // Get total count
            var totalCount = await AsyncExecuter.CountAsync(query);

            // Apply sorting
            if (string.IsNullOrEmpty(input.Sorting))
            {
                query = query.OrderByDescending(x => x.IsDefault).ThenBy(x => x.AccountName);
            }
            else
            {
                // For now, default sorting if custom sorting provided
                query = query.OrderByDescending(x => x.IsDefault).ThenBy(x => x.AccountName);
            }

            // Apply paging
            query = query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var items = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<BankAccountListDto>(
                totalCount,
                items.Select(MapToListDto).ToList()
            );
        }

        public async Task<BankAccountDto> CreateAsync(CreateUpdateBankAccountDto input)
        {
            var bankAccount = BankAccount.Create(
                GuidGenerator.Create(),
                CurrentTenant.Id,
                input.AccountName,
                input.Iban,
                input.BankName,
                input.Swift,
                input.Notes
            );

            await _bankAccountRepository.InsertAsync(bankAccount);
            
            return MapToDto(bankAccount);
        }

        public async Task<BankAccountDto> UpdateAsync(Guid id, CreateUpdateBankAccountDto input)
        {
            var bankAccount = await _bankAccountRepository.GetAsync(id);

            bankAccount.Update(
                input.AccountName,
                input.Iban,
                input.BankName,
                input.Swift,
                input.Notes
            );

            await _bankAccountRepository.UpdateAsync(bankAccount);
            
            return MapToDto(bankAccount);
        }

        public async Task DeleteAsync(Guid id)
        {
            var bankAccount = await _bankAccountRepository.GetAsync(id);

            if (bankAccount.IsDefault)
            {
                throw new BusinessException("DonaRog:CannotDeleteDefaultBankAccount")
                    .WithData("accountId", id)
                    .WithData("accountName", bankAccount.AccountName);
            }

            // TODO: Check if there are donations associated with this account
            // If yes, prevent deletion or handle accordingly

            await _bankAccountRepository.DeleteAsync(id);
        }

        public async Task<BankAccountDto> ActivateAsync(Guid id)
        {
            var bankAccount = await _bankAccountRepository.GetAsync(id);
            
            bankAccount.Activate();
            
            await _bankAccountRepository.UpdateAsync(bankAccount);
            
            return MapToDto(bankAccount);
        }

        public async Task<BankAccountDto> DeactivateAsync(Guid id)
        {
            var bankAccount = await _bankAccountRepository.GetAsync(id);
            
            bankAccount.Deactivate();
            
            await _bankAccountRepository.UpdateAsync(bankAccount);
            
            return MapToDto(bankAccount);
        }

        public async Task<BankAccountDto> SetAsDefaultAsync(Guid id)
        {
            var bankAccount = await _bankAccountRepository.GetAsync(id);

            // Remove default from all other accounts
            var allAccounts = await _bankAccountRepository.GetListAsync();
            foreach (var account in allAccounts.Where(a => a.Id != id && a.IsDefault))
            {
                account.RemoveDefaultStatus();
                await _bankAccountRepository.UpdateAsync(account);
            }

            // Set this account as default
            bankAccount.SetAsDefault();
            await _bankAccountRepository.UpdateAsync(bankAccount);
            
            return MapToDto(bankAccount);
        }

        // ======================================================================
        // PRIVATE MAPPING METHODS
        // ======================================================================

        private BankAccountDto MapToDto(BankAccount bankAccount)
        {
            return new BankAccountDto
            {
                Id = bankAccount.Id,
                AccountName = bankAccount.AccountName,
                Iban = bankAccount.Iban.Value,
                BankName = bankAccount.BankName,
                Swift = bankAccount.Swift,
                IsActive = bankAccount.IsActive,
                IsDefault = bankAccount.IsDefault,
                Notes = bankAccount.Notes,
                FormattedIban = bankAccount.GetFormattedIban(),
                MaskedIban = bankAccount.GetMaskedIban(),
                CreationTime = bankAccount.CreationTime,
                CreatorId = bankAccount.CreatorId,
                LastModificationTime = bankAccount.LastModificationTime,
                LastModifierId = bankAccount.LastModifierId
            };
        }

        private BankAccountListDto MapToListDto(BankAccount bankAccount)
        {
            return new BankAccountListDto
            {
                Id = bankAccount.Id,
                AccountName = bankAccount.AccountName,
                MaskedIban = bankAccount.GetMaskedIban(),
                BankName = bankAccount.BankName,
                IsActive = bankAccount.IsActive,
                IsDefault = bankAccount.IsDefault
            };
        }
    }
}
