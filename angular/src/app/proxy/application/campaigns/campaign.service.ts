import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { CampaignDonorDto, CampaignDto, CampaignListDto, CampaignStatisticsDto, CreateCampaignDto, DonorExtractionPreviewDto, ExtractDonorsInput, GetCampaignsInput, RecordDonationInput, UpdateCampaignDto } from '../contracts/campaigns/dto/models';

@Injectable({
  providedIn: 'root',
})
export class CampaignService {
  apiName = 'Default';
  

  addDonor = (campaignId: string, donorId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/campaign/donor',
      params: { campaignId, donorId },
    },
    { apiName: this.apiName,...config });
  

  cancel = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/campaign/${id}/cancel`,
    },
    { apiName: this.apiName,...config });
  

  complete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/campaign/${id}/complete`,
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateCampaignDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CampaignDto>({
      method: 'POST',
      url: '/api/app/campaign',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/campaign/${id}`,
    },
    { apiName: this.apiName,...config });
  

  extractDonors = (campaignId: string, input: ExtractDonorsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/campaign/extract-donors/${campaignId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  generatePostalCode = (campaignId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: `/api/app/campaign/generate-postal-code/${campaignId}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CampaignDto>({
      method: 'GET',
      url: `/api/app/campaign/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getCampaignDonors = (campaignId: string, input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CampaignDonorDto>>({
      method: 'GET',
      url: `/api/app/campaign/campaign-donors/${campaignId}`,
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getCampaignList = (input: GetCampaignsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CampaignListDto>>({
      method: 'GET',
      url: '/api/app/campaign/campaign-list',
      params: { filter: input.filter, year: input.year, status: input.status, campaignType: input.campaignType, channel: input.channel, recurrenceId: input.recurrenceId, dateFrom: input.dateFrom, dateTo: input.dateTo, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetCampaignsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CampaignDto>>({
      method: 'GET',
      url: '/api/app/campaign',
      params: { filter: input.filter, year: input.year, status: input.status, campaignType: input.campaignType, channel: input.channel, recurrenceId: input.recurrenceId, dateFrom: input.dateFrom, dateTo: input.dateTo, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getStatistics = (campaignId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CampaignStatisticsDto>({
      method: 'GET',
      url: `/api/app/campaign/statistics/${campaignId}`,
    },
    { apiName: this.apiName,...config });
  

  markAsDispatched = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/campaign/${id}/mark-as-dispatched`,
    },
    { apiName: this.apiName,...config });
  

  previewDonorExtraction = (input: ExtractDonorsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonorExtractionPreviewDto>({
      method: 'POST',
      url: '/api/app/campaign/preview-donor-extraction',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  recordDonation = (campaignId: string, input: RecordDonationInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/campaign/record-donation/${campaignId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  removeDonor = (campaignId: string, donorId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/campaign/donor',
      params: { campaignId, donorId },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateCampaignDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CampaignDto>({
      method: 'PUT',
      url: `/api/app/campaign/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateStatistics = (campaignId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: `/api/app/campaign/statistics/${campaignId}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
