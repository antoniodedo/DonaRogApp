import type { CreateUpdateTitleDto, DeleteTitlePreviewDto, DeleteTitleResultDto, TitleDto, TitleListWithStatsDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { ListResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TitleService {
  apiName = 'Default';
  

  create = (input: CreateUpdateTitleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TitleDto>({
      method: 'POST',
      url: '/api/app/title',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, replacementTitleId?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DeleteTitleResultDto>({
      method: 'DELETE',
      url: `/api/app/title/${id}`,
      params: { replacementTitleId },
    },
    { apiName: this.apiName,...config });
  

  getAll = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<TitleDto>>({
      method: 'GET',
      url: '/api/app/title',
    },
    { apiName: this.apiName,...config });
  

  getAllWithStats = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, TitleListWithStatsDto>({
      method: 'GET',
      url: '/api/app/title/with-stats',
    },
    { apiName: this.apiName,...config });
  

  getById = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TitleDto>({
      method: 'GET',
      url: `/api/app/title/${id}/by-id`,
    },
    { apiName: this.apiName,...config });
  

  getDeletePreview = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DeleteTitlePreviewDto>({
      method: 'GET',
      url: `/api/app/title/${id}/delete-preview`,
    },
    { apiName: this.apiName,...config });
  

  reactivate = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/title/${id}/reactivate`,
    },
    { apiName: this.apiName,...config });
  

  removeDefault = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/title/${id}/default`,
    },
    { apiName: this.apiName,...config });
  

  setAsDefault = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/title/${id}/set-as-default`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateTitleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TitleDto>({
      method: 'PUT',
      url: `/api/app/title/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
