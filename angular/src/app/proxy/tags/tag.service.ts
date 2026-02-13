import type { CreateUpdateTagDto, TagDto } from './dto/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TagService {
  apiName = 'Default';
  

  activate = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/tag/${id}/activate`,
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateUpdateTagDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TagDto>({
      method: 'POST',
      url: '/api/app/tag',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  deactivate = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/tag/${id}/deactivate`,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/tag/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TagDto>({
      method: 'GET',
      url: `/api/app/tag/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getActiveList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, TagDto[]>({
      method: 'GET',
      url: '/api/app/tag/active-list',
    },
    { apiName: this.apiName,...config });
  

  getCategories = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, string[]>({
      method: 'GET',
      url: '/api/app/tag/categories',
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<TagDto>>({
      method: 'GET',
      url: '/api/app/tag',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateTagDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TagDto>({
      method: 'PUT',
      url: `/api/app/tag/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
