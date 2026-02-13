import type {
  CreateUpdateLetterTemplateDto,
  GetLetterTemplatesInput,
  LetterTemplateDto,
  LetterTemplateListDto,
  RenderTemplateWithDonorInput,
  SelectTemplateInput,
  SendTestEmailInput,
} from './dto/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { TemplateCategory } from '../enums/communications/template-category.enum';

@Injectable({
  providedIn: 'root',
})
export class LetterTemplateService {
  apiName = 'Default';

  create = (input: CreateUpdateLetterTemplateDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LetterTemplateDto>(
      {
        method: 'POST',
        url: '/api/app/letter-template',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/app/letter-template/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  duplicate = (templateId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LetterTemplateDto>(
      {
        method: 'POST',
        url: `/api/app/letter-template/${templateId}/duplicate`,
      },
      { apiName: this.apiName, ...config }
    );

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LetterTemplateDto>(
      {
        method: 'GET',
        url: `/api/app/letter-template/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  getDefaultTemplate = (
    category: TemplateCategory,
    language: string,
    config?: Partial<Rest.Config>
  ) =>
    this.restService.request<any, LetterTemplateDto>(
      {
        method: 'GET',
        url: '/api/app/letter-template/default-template',
        params: { category, language },
      },
      { apiName: this.apiName, ...config }
    );

  getList = (input: GetLetterTemplatesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<LetterTemplateDto>>(
      {
        method: 'GET',
        url: '/api/app/letter-template',
        params: {
          filter: input.filter,
          category: input.category,
          language: input.language,
          isActive: input.isActive,
          isDefault: input.isDefault,
          projectId: input.projectId,
          recurrenceId: input.recurrenceId,
          communicationType: input.communicationType,
          isForNewDonor: input.isForNewDonor,
          isPlural: input.isPlural,
          sorting: input.sorting,
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
        },
      },
      { apiName: this.apiName, ...config }
    );

  getListView = (input: GetLetterTemplatesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<LetterTemplateListDto>>(
      {
        method: 'GET',
        url: '/api/app/letter-template/list-view',
        params: {
          filter: input.filter,
          category: input.category,
          language: input.language,
          isActive: input.isActive,
          isDefault: input.isDefault,
          projectId: input.projectId,
          recurrenceId: input.recurrenceId,
          communicationType: input.communicationType,
          isForNewDonor: input.isForNewDonor,
          isPlural: input.isPlural,
          sorting: input.sorting,
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
        },
      },
      { apiName: this.apiName, ...config }
    );

  getSuggestedTemplates = (input: SelectTemplateInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LetterTemplateDto[]>(
      {
        method: 'POST',
        url: '/api/app/letter-template/suggested-templates',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  renderTemplate = (
    templateId: string,
    tagValues: Record<string, string>,
    config?: Partial<Rest.Config>
  ) =>
    this.restService.request<any, string>(
      {
        method: 'POST',
        url: `/api/app/letter-template/${templateId}/render-template`,
        body: tagValues,
      },
      { apiName: this.apiName, ...config }
    );

  renderTemplateWithDonorData = (
    input: RenderTemplateWithDonorInput,
    config?: Partial<Rest.Config>
  ) =>
    this.restService.request<any, string>(
      {
        method: 'POST',
        url: '/api/app/letter-template/render-template-with-donor-data',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  sendTestEmail = (input: SendTestEmailInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'POST',
        url: '/api/app/letter-template/send-test-email',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  update = (id: string, input: CreateUpdateLetterTemplateDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LetterTemplateDto>(
      {
        method: 'PUT',
        url: `/api/app/letter-template/${id}`,
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  constructor(private restService: RestService) {}
}
