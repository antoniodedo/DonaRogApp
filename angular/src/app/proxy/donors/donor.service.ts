import type { CreateUpdateDonorDto, DonorDto, DonorListDto } from './dto/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DonorService {
  apiName = 'Default';
  

  create = (input: CreateUpdateDonorDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonorDto>({
      method: 'POST',
      url: '/api/app/donor',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/donor/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonorDto>({
      method: 'GET',
      url: `/api/app/donor/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getLightList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DonorListDto>>({
      method: 'GET',
      url: '/api/app/donor/light-list',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DonorDto>>({
      method: 'GET',
      url: '/api/app/donor',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateDonorDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonorDto>({
      method: 'PUT',
      url: `/api/app/donor/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
