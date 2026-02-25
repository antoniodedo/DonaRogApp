import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { PreferredThankYouChannel } from '../models';

export interface ThankYouRuleDto extends FullAuditedEntityDto<string> {
  name: string;
  description?: string;
  priority: number;
  isActive: boolean;
  minAmount?: number;
  maxAmount?: number;
  isFirstDonation?: boolean;
  projectIds?: string[];
  campaignIds?: string[];
  donorCategories?: number[];
  subjectTypes?: number[];
  createThankYou: boolean;
  suggestedChannel?: PreferredThankYouChannel;
  suggestedTemplateId?: string;
  suggestedTemplateName?: string;
}

export interface CreateUpdateThankYouRuleDto {
  name: string;
  description?: string;
  priority: number;
  isActive: boolean;
  minAmount?: number;
  maxAmount?: number;
  isFirstDonation?: boolean;
  projectIds?: string[];
  campaignIds?: string[];
  donorCategories?: number[];
  subjectTypes?: number[];
  createThankYou: boolean;
  suggestedChannel?: PreferredThankYouChannel;
  suggestedTemplateId?: string;
}

export interface EvaluateThankYouRulesDto {
  donorId: string;
  donationId: string;
}

export interface ThankYouRuleEvaluationResultDto {
  shouldCreateThankYou: boolean;
  suggestedChannel: PreferredThankYouChannel;
  suggestedTemplateId?: string;
  matchedRules: MatchedRuleDto[];
  donorPreference?: PreferredThankYouChannel;
  explanation: string;
}

export interface MatchedRuleDto {
  ruleId: string;
  ruleName: string;
  priority: number;
  matchScore: number;
  createThankYou: boolean;
  suggestedChannel?: PreferredThankYouChannel;
  suggestedTemplateId?: string;
  notes?: string;
}

export interface RuleOrderDto {
  ruleId: string;
  priority: number;
}
