import type { PagedResultDto } from '@abp/ng.core';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import type {
  CreateDonationDto,
  CreateTextDocumentDto,
  DonationDto,
  DonationDocumentDto,
  DonationListDto,
  DonationStatisticsDto,
  ExternalDonationDto,
  GetDonationsInput,
  RejectDonationDto,
  UpdateDonationDto,
  UploadDonationDocumentDto,
  VerifyDonationDto,
} from './models';

@Injectable({
  providedIn: 'root',
})
export class DonationService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonationDto>(
      {
        method: 'GET',
        url: `/api/app/donation/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  getList = (input: GetDonationsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DonationListDto>>(
      {
        method: 'GET',
        url: '/api/app/donation',
        params: {
          status: input.status,
          channel: input.channel,
          donorId: input.donorId,
          campaignId: input.campaignId,
          projectId: input.projectId,
          bankAccountId: input.bankAccountId,
          fromDate: input.fromDate,
          toDate: input.toDate,
          minAmount: input.minAmount,
          maxAmount: input.maxAmount,
          search: input.search,
          sorting: input.sorting,
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
        },
      },
      { apiName: this.apiName, ...config }
    );

  create = (input: CreateDonationDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonationDto>(
      {
        method: 'POST',
        url: '/api/app/donation',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  update = (id: string, input: UpdateDonationDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonationDto>(
      {
        method: 'PUT',
        url: `/api/app/donation/${id}`,
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/app/donation/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  verify = (id: string, input: VerifyDonationDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonationDto>(
      {
        method: 'POST',
        url: `/api/app/donation/${id}/verify`,
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  reject = (id: string, input: RejectDonationDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonationDto>(
      {
        method: 'POST',
        url: `/api/app/donation/${id}/reject`,
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  allocateToProject = (id: string, projectId: string, amount: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'POST',
        url: `/api/app/donation/${id}/allocate-to-project`,
        params: { projectId, amount },
      },
      { apiName: this.apiName, ...config }
    );

  removeProjectAllocation = (id: string, projectId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/app/donation/${id}/project-allocation/${projectId}`,
      },
      { apiName: this.apiName, ...config }
    );

  addExternal = (input: ExternalDonationDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonationDto>(
      {
        method: 'POST',
        url: '/api/app/donation/add-external',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  getStatistics = (filter: GetDonationsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonationStatisticsDto>(
      {
        method: 'GET',
        url: '/api/app/donation/statistics',
        params: {
          status: filter.status,
          channel: filter.channel,
          donorId: filter.donorId,
          campaignId: filter.campaignId,
          fromDate: filter.fromDate,
          toDate: filter.toDate,
        },
      },
      { apiName: this.apiName, ...config }
    );

  getDocuments = (donationId: string, config?: Partial<Rest.Config>): Observable<DonationDocumentDto[]> =>
    this.restService.request<any, DonationDocumentDto[]>(
      {
        method: 'GET',
        url: `/api/donations/${donationId}/documents`,
      },
      { apiName: this.apiName, ...config }
    );

  uploadDocument = (donationId: string, file: File, documentType: number, notes?: string, config?: Partial<Rest.Config>): Observable<DonationDocumentDto> => {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('documentType', documentType.toString());
    if (notes) {
      formData.append('notes', notes);
    }
    
    return this.restService.request<any, DonationDocumentDto>(
      {
        method: 'POST',
        url: `/api/donations/${donationId}/documents`,
        body: formData,
      },
      { apiName: this.apiName, ...config }
    );
  };

  downloadDocument = (donationId: string, documentId: string, config?: Partial<Rest.Config>): Observable<Blob> =>
    this.restService.request<any, Blob>(
      {
        method: 'GET',
        url: `/api/donations/${donationId}/documents/${documentId}`,
        responseType: 'blob' as any,
      },
      { apiName: this.apiName, ...config }
    );

  deleteDocument = (donationId: string, documentId: string, config?: Partial<Rest.Config>): Observable<void> =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/donations/${donationId}/documents/${documentId}`,
      },
      { apiName: this.apiName, ...config }
    );

  createTextDocument = (donationId: string, input: CreateTextDocumentDto, config?: Partial<Rest.Config>): Observable<DonationDocumentDto> =>
    this.restService.request<any, DonationDocumentDto>(
      {
        method: 'POST',
        url: `/api/donations/${donationId}/documents/text`,
        body: input,
      },
      { apiName: this.apiName, ...config }
    );
}
