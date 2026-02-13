import { RestService, Rest } from '@abp/ng.core';
import type { ListResultDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { CreateRecurrenceDto, GetRecurrencesInput, RecurrenceDto, RecurrenceListDto, UpdateRecurrenceDto } from '../contracts/recurrences/dto/models';

@Injectable({
  providedIn: 'root',
})
export class RecurrenceService {
  apiName = 'Default';
  

  create = (input: CreateRecurrenceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecurrenceDto>({
      method: 'POST',
      url: '/api/app/recurrence',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  deactivate = (id: string, reason: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/recurrence/${id}/deactivate`,
      params: { reason },
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/recurrence/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecurrenceDto>({
      method: 'GET',
      url: `/api/app/recurrence/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getActiveRecurrences = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<RecurrenceListDto>>({
      method: 'GET',
      url: '/api/app/recurrence/active-recurrences',
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetRecurrencesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<RecurrenceDto>>({
      method: 'GET',
      url: '/api/app/recurrence',
      params: { filter: input.filter, isActive: input.isActive, isCurrentlyInValidityPeriod: input.isCurrentlyInValidityPeriod, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getRecurrenceList = (input: GetRecurrencesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<RecurrenceListDto>>({
      method: 'GET',
      url: '/api/app/recurrence/recurrence-list',
      params: { filter: input.filter, isActive: input.isActive, isCurrentlyInValidityPeriod: input.isCurrentlyInValidityPeriod, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  reactivate = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/recurrence/${id}/reactivate`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateRecurrenceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecurrenceDto>({
      method: 'PUT',
      url: `/api/app/recurrence/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
