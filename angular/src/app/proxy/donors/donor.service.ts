import type { GetDonorsInput } from './dto/models';
import type { AssignTagDto, CreateDonorAddressDto, CreateDonorDto, CreateDonorEmailDto, DonorAddressDto, DonorDto, DonorEmailDto, DonorStatusHistoryDto, DonorTagDto, UpdateDonorDto } from './dtos/models';
import type { CreateDonorContactDto, DonorContactDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DonorService {
  apiName = 'Default';
  

  addAddress = (donorId: string, input: CreateDonorAddressDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/address/${donorId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  addContact = (donorId: string, input: CreateDonorContactDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/contact/${donorId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  addEmail = (donorId: string, input: CreateDonorEmailDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/email/${donorId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  addTag = (donorId: string, input: AssignTagDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/tag/${donorId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  anonymize = (donorId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/anonymize/${donorId}`,
    },
    { apiName: this.apiName,...config });
  

  changeStatus = (donorId: string, status: number, note?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/change-status/${donorId}`,
      params: { status, note },
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateDonorDto, config?: Partial<Rest.Config>) =>
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
  

  deleteTag = (donorId: string, tagId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/donor/tag',
      params: { donorId, tagId },
    },
    { apiName: this.apiName,...config });
  

  endAddress = (donorId: string, addressId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/donor/end-address',
      params: { donorId, addressId },
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonorDto>({
      method: 'GET',
      url: `/api/app/donor/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getAddresses = (donorId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonorAddressDto[]>({
      method: 'GET',
      url: `/api/app/donor/addresses/${donorId}`,
    },
    { apiName: this.apiName,...config });
  

  getContacts = (donorId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonorContactDto[]>({
      method: 'GET',
      url: `/api/app/donor/contacts/${donorId}`,
    },
    { apiName: this.apiName,...config });
  

  getEmails = (donorId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonorEmailDto[]>({
      method: 'GET',
      url: `/api/app/donor/emails/${donorId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetDonorsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DonorDto>>({
      method: 'GET',
      url: '/api/app/donor',
      params: { filter: input.filter, subjectType: input.subjectType, status: input.status, category: input.category, titleId: input.titleId, donorCode: input.donorCode, email: input.email, phoneNumber: input.phoneNumber, city: input.city, postalCode: input.postalCode, province: input.province, country: input.country, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getStatusHistory = (donorId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonorStatusHistoryDto[]>({
      method: 'GET',
      url: `/api/app/donor/status-history/${donorId}`,
    },
    { apiName: this.apiName,...config });
  

  getTags = (donorId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonorTagDto[]>({
      method: 'GET',
      url: `/api/app/donor/tags/${donorId}`,
    },
    { apiName: this.apiName,...config });
  

  grantMailConsent = (donorId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/grant-mail-consent/${donorId}`,
    },
    { apiName: this.apiName,...config });
  

  grantNewsletterConsent = (donorId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/grant-newsletter-consent/${donorId}`,
    },
    { apiName: this.apiName,...config });
  

  grantPrivacyConsent = (donorId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/grant-privacy-consent/${donorId}`,
    },
    { apiName: this.apiName,...config });
  

  recordEmailBounce = (donorId: string, emailAddress: string, reason?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/record-email-bounce/${donorId}`,
      params: { emailAddress, reason },
    },
    { apiName: this.apiName,...config });
  

  removeContact = (donorId: string, phoneNumber: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/donor/contact/${donorId}`,
      params: { phoneNumber },
    },
    { apiName: this.apiName,...config });
  

  removeEmail = (donorId: string, emailAddress: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/donor/email/${donorId}`,
      params: { emailAddress },
    },
    { apiName: this.apiName,...config });
  

  revokeMailConsent = (donorId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/revoke-mail-consent/${donorId}`,
    },
    { apiName: this.apiName,...config });
  

  revokeNewsletterConsent = (donorId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/revoke-newsletter-consent/${donorId}`,
    },
    { apiName: this.apiName,...config });
  

  revokePrivacyConsent = (donorId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/revoke-privacy-consent/${donorId}`,
    },
    { apiName: this.apiName,...config });
  

  setDefaultAddress = (donorId: string, addressId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/donor/set-default-address',
      params: { donorId, addressId },
    },
    { apiName: this.apiName,...config });
  

  setDefaultContact = (donorId: string, phoneNumber: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/set-default-contact/${donorId}`,
      params: { phoneNumber },
    },
    { apiName: this.apiName,...config });
  

  setDefaultEmail = (donorId: string, emailAddress: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/set-default-email/${donorId}`,
      params: { emailAddress },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateDonorDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DonorDto>({
      method: 'PUT',
      url: `/api/app/donor/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  verifyEmail = (donorId: string, emailAddress: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/donor/verify-email/${donorId}`,
      params: { emailAddress },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
