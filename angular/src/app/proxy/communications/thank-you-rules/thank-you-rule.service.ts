import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { PagedResultDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type {
  CreateUpdateThankYouRuleDto,
  EvaluateThankYouRulesDto,
  RuleOrderDto,
  ThankYouRuleDto,
  ThankYouRuleEvaluationResultDto,
} from './models';

@Injectable({
  providedIn: 'root',
})
export class ThankYouRuleService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  create = (input: CreateUpdateThankYouRuleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ThankYouRuleDto>(
      {
        method: 'POST',
        url: '/api/thank-you-rules',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/thank-you-rules/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  evaluateRules = (input: EvaluateThankYouRulesDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ThankYouRuleEvaluationResultDto>(
      {
        method: 'POST',
        url: '/api/thank-you-rules/evaluate',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ThankYouRuleDto>(
      {
        method: 'GET',
        url: `/api/thank-you-rules/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ThankYouRuleDto>>(
      {
        method: 'GET',
        url: '/api/thank-you-rules',
        params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
      },
      { apiName: this.apiName, ...config }
    );

  reorderRules = (order: RuleOrderDto[], config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'POST',
        url: '/api/thank-you-rules/reorder',
        body: order,
      },
      { apiName: this.apiName, ...config }
    );

  toggleActive = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ThankYouRuleDto>(
      {
        method: 'POST',
        url: `/api/thank-you-rules/${id}/toggle-active`,
      },
      { apiName: this.apiName, ...config }
    );

  update = (id: string, input: CreateUpdateThankYouRuleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ThankYouRuleDto>(
      {
        method: 'PUT',
        url: `/api/thank-you-rules/${id}`,
        body: input,
      },
      { apiName: this.apiName, ...config }
    );
}
