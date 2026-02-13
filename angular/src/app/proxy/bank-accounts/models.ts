import type { EntityDto, FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface BankAccountDto extends FullAuditedEntityDto<string> {
  accountName: string;
  iban: string;
  bankName?: string;
  swift?: string;
  isActive: boolean;
  isDefault: boolean;
  notes?: string;
  formattedIban: string;
  maskedIban: string;
}

export interface BankAccountListDto extends EntityDto<string> {
  accountName: string;
  maskedIban: string;
  bankName?: string;
  isActive: boolean;
  isDefault: boolean;
}

export interface CreateUpdateBankAccountDto {
  accountName: string;
  iban: string;
  bankName?: string;
  swift?: string;
  notes?: string;
}

export interface GetBankAccountsInput extends PagedAndSortedResultRequestDto {
  isActive?: boolean;
  isDefault?: boolean;
  search?: string;
}
