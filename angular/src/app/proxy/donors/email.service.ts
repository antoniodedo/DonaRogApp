import type { CreateUpdateEmailDto, EmailDto } from './dto/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class EmailService {
  apiName = 'Default';
  

  createByDonor = (donorId: string, input: CreateUpdateEmailDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EmailDto>({
      method: 'POST',
      url: `/api/app/email/by-donor/${donorId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/email/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EmailDto>({
      method: 'GET',
      url: `/api/app/email/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getListByDonor = (donorId: string, input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<EmailDto>>({
      method: 'GET',
      url: `/api/app/email/by-donor/${donorId}`,
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  updateByDonor = (donorId: string, id: string, input: CreateUpdateEmailDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EmailDto>({
      method: 'PUT',
      url: `/api/app/email/${id}/by-donor/${donorId}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
