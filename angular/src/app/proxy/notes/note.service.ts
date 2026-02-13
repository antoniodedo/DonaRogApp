import type { CreateUpdateNoteDto, NoteDto } from './dto/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class NoteService {
  apiName = 'Default';
  

  createForDonor = (donorId: string, input: CreateUpdateNoteDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, NoteDto>({
      method: 'POST',
      url: `/api/app/note/for-donor/${donorId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/note/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, NoteDto>({
      method: 'GET',
      url: `/api/app/note/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getListByDonor = (donorId: string, input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<NoteDto>>({
      method: 'GET',
      url: `/api/app/note/by-donor/${donorId}`,
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  toggleImportant = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/note/${id}/toggle-important`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateNoteDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, NoteDto>({
      method: 'PUT',
      url: `/api/app/note/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
