import type { AuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface SegmentationRuleDto extends AuditedEntityDto<string> {
  name: string;
  description?: string;
  isActive: boolean;
  priority: number;
  segmentId: string;
  segmentName?: string;
  minRecencyScore?: number;
  maxRecencyScore?: number;
  minFrequencyScore?: number;
  maxFrequencyScore?: number;
  minMonetaryScore?: number;
  maxMonetaryScore?: number;
  minTotalDonated?: number;
  maxTotalDonated?: number;
  minDonationCount?: number;
  maxDonationCount?: number;
  minDaysSinceLastDonation?: number;
  maxDaysSinceLastDonation?: number;
  firstDonationAfter?: string;
  firstDonationBefore?: string;
  lastDonationAfter?: string;
  lastDonationBefore?: string;
  conditionsSummary?: string;
}

export interface CreateUpdateSegmentationRuleDto {
  name: string;
  description?: string;
  isActive: boolean;
  priority: number;
  segmentId: string;
  minRecencyScore?: number;
  maxRecencyScore?: number;
  minFrequencyScore?: number;
  maxFrequencyScore?: number;
  minMonetaryScore?: number;
  maxMonetaryScore?: number;
  minTotalDonated?: number;
  maxTotalDonated?: number;
  minDonationCount?: number;
  maxDonationCount?: number;
  minDaysSinceLastDonation?: number;
  maxDaysSinceLastDonation?: number;
  firstDonationAfter?: string;
  firstDonationBefore?: string;
  lastDonationAfter?: string;
  lastDonationBefore?: string;
}

export interface RuleOrderDto {
  ruleId: string;
  priority: number;
}

export interface SegmentEvaluationPreviewDto {
  ruleId: string;
  ruleName: string;
  totalMatchingDonors: number;
  previewDonors: DonorPreviewDto[];
}

export interface DonorPreviewDto {
  donorId: string;
  donorCode: string;
  fullName: string;
  totalDonated: number;
  donationCount: number;
  daysSinceLastDonation?: number;
  recencyScore?: number;
  frequencyScore?: number;
  monetaryScore?: number;
}

export interface SegmentationBatchResultDto {
  startTime: string;
  endTime: string;
  donorsProcessed: number;
  assignmentsCreated: number;
  assignmentsRemoved: number;
  errors: number;
  durationSeconds: number;
}

export interface GetSegmentationRulesInput extends PagedAndSortedResultRequestDto {
  filter?: string;
}

export interface SegmentDto {
  id: string;
  code: string;
  name: string;
  description?: string;
  colorCode?: string;
  icon?: string;
  displayOrder: number;
  isActive: boolean;
}
