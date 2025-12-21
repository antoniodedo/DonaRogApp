import type { CreateUpdateLetterTemplateDto, LetterTemplateDto } from './dto/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LetterTemplateService {
  apiName = 'Default';
  

  create = (input: CreateUpdateLetterTemplateDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LetterTemplateDto>({
      method: 'POST',
      url: '/api/app/letter-template',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/letter-template/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LetterTemplateDto>({
      method: 'GET',
      url: `/api/app/letter-template/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<LetterTemplateDto>>({
      method: 'GET',
      url: '/api/app/letter-template',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateLetterTemplateDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LetterTemplateDto>({
      method: 'PUT',
      url: `/api/app/letter-template/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
