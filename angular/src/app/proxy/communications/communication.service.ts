import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { PagedResultDto } from '@abp/ng.core';
import type {
  CheckDuplicateLettersDto,
  CommunicationHistoryDto,
  DuplicateCheckResultDto,
  GetCommunicationHistoryInput,
  RecentCommunicationDto,
  AlertLevel,
} from './models';

@Injectable({
  providedIn: 'root',
})
export class CommunicationService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  checkDuplicateLetters = (input: CheckDuplicateLettersDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DuplicateCheckResultDto>(
      {
        method: 'POST',
        url: '/api/communications/check-duplicates',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  getDuplicateAlertLevel = (
    donorId: string,
    errorThresholdDays: number = 7,
    warningThresholdDays: number = 15,
    config?: Partial<Rest.Config>
  ) =>
    this.restService.request<any, AlertLevel>(
      {
        method: 'GET',
        url: `/api/communications/alert-level/${donorId}`,
        params: { errorThresholdDays, warningThresholdDays },
      },
      { apiName: this.apiName, ...config }
    );

  getHistory = (input: GetCommunicationHistoryInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CommunicationHistoryDto>>(
      {
        method: 'GET',
        url: '/api/communications/history',
        params: {
          donorId: input.donorId,
          type: input.type,
          category: input.category,
          dateFrom: input.dateFrom,
          dateTo: input.dateTo,
          status: input.status,
          onlyPrinted: input.onlyPrinted,
          sorting: input.sorting,
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
        },
      },
      { apiName: this.apiName, ...config }
    );

  getDonorRecentCommunications = (donorId: string, lastDays: number = 30, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecentCommunicationDto[]>(
      {
        method: 'GET',
        url: `/api/communications/donor/${donorId}/recent`,
        params: { lastDays },
      },
      { apiName: this.apiName, ...config }
    );
}
