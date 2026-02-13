import type { PagedResultDto } from '@abp/ng.core';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type {
  BankAccountDto,
  BankAccountListDto,
  CreateUpdateBankAccountDto,
  GetBankAccountsInput,
} from './models';

@Injectable({
  providedIn: 'root',
})
export class BankAccountService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BankAccountDto>(
      {
        method: 'GET',
        url: `/api/app/bank-account/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  getList = (input: GetBankAccountsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<BankAccountListDto>>(
      {
        method: 'GET',
        url: '/api/app/bank-account',
        params: {
          isActive: input.isActive,
          isDefault: input.isDefault,
          search: input.search,
          sorting: input.sorting,
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
        },
      },
      { apiName: this.apiName, ...config }
    );

  create = (input: CreateUpdateBankAccountDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BankAccountDto>(
      {
        method: 'POST',
        url: '/api/app/bank-account',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  update = (id: string, input: CreateUpdateBankAccountDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BankAccountDto>(
      {
        method: 'PUT',
        url: `/api/app/bank-account/${id}`,
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/app/bank-account/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  activate = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BankAccountDto>(
      {
        method: 'POST',
        url: `/api/app/bank-account/${id}/activate`,
      },
      { apiName: this.apiName, ...config }
    );

  deactivate = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BankAccountDto>(
      {
        method: 'POST',
        url: `/api/app/bank-account/${id}/deactivate`,
      },
      { apiName: this.apiName, ...config }
    );

  setAsDefault = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BankAccountDto>(
      {
        method: 'POST',
        url: `/api/app/bank-account/${id}/set-as-default`,
      },
      { apiName: this.apiName, ...config }
    );
}
