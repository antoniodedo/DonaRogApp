import { Injectable } from '@angular/core';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import type {
  SegmentationRuleDto,
  CreateUpdateSegmentationRuleDto,
  RuleOrderDto,
  SegmentEvaluationPreviewDto,
  SegmentationBatchResultDto,
  GetSegmentationRulesInput,
  SegmentDto
} from './models';

@Injectable({
  providedIn: 'root',
})
export class SegmentationRuleService {
  apiName = 'default';

  constructor(private restService: RestService) {}

  create(input: CreateUpdateSegmentationRuleDto) {
    return this.restService.request<any, SegmentationRuleDto>({
      method: 'POST',
      url: '/api/app/segmentation-rule',
      body: input,
    },
    { apiName: this.apiName });
  }

  delete(id: string) {
    return this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/segmentation-rule/${id}`,
    },
    { apiName: this.apiName });
  }

  get(id: string) {
    return this.restService.request<any, SegmentationRuleDto>({
      method: 'GET',
      url: `/api/app/segmentation-rule/${id}`,
    },
    { apiName: this.apiName });
  }

  getList(input: GetSegmentationRulesInput) {
    return this.restService.request<any, PagedResultDto<SegmentationRuleDto>>({
      method: 'GET',
      url: '/api/app/segmentation-rule',
      params: { 
        filter: input.filter, 
        sorting: input.sorting, 
        skipCount: input.skipCount, 
        maxResultCount: input.maxResultCount 
      },
    },
    { apiName: this.apiName });
  }

  update(id: string, input: CreateUpdateSegmentationRuleDto) {
    return this.restService.request<any, SegmentationRuleDto>({
      method: 'PUT',
      url: `/api/app/segmentation-rule/${id}`,
      body: input,
    },
    { apiName: this.apiName });
  }

  toggleActive(id: string) {
    return this.restService.request<any, SegmentationRuleDto>({
      method: 'POST',
      url: `/api/app/segmentation-rule/${id}/toggle-active`,
    },
    { apiName: this.apiName });
  }

  reorderRules(order: RuleOrderDto[]) {
    return this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/segmentation-rule/reorder',
      body: order,
    },
    { apiName: this.apiName });
  }

  previewRule(ruleId: string, maxResults: number = 100) {
    return this.restService.request<any, SegmentEvaluationPreviewDto>({
      method: 'GET',
      url: `/api/app/segmentation-rule/${ruleId}/preview`,
      params: { maxResults },
    },
    { apiName: this.apiName });
  }

  applyRuleManually(ruleId: string) {
    return this.restService.request<any, number>({
      method: 'POST',
      url: `/api/app/segmentation-rule/${ruleId}/apply-manually`,
    },
    { apiName: this.apiName });
  }

  runSegmentationBatch() {
    return this.restService.request<any, SegmentationBatchResultDto>({
      method: 'POST',
      url: '/api/app/segmentation-rule/batch',
    },
    { apiName: this.apiName });
  }

  getLastBatchResult() {
    return this.restService.request<any, SegmentationBatchResultDto | null>({
      method: 'GET',
      url: '/api/app/segmentation-rule/last-batch-result',
    },
    { apiName: this.apiName });
  }

  getAvailableSegments() {
    return this.restService.request<any, SegmentDto[]>({
      method: 'GET',
      url: '/api/app/segmentation-rule/available-segments',
    },
    { apiName: this.apiName });
  }
}
