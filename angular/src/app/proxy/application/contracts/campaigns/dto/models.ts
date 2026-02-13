import type { EntityDto, FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { ResponseType } from '../../../../enums/campaigns/response-type.enum';
import type { CampaignType } from '../../../../enums/campaigns/campaign-type.enum';
import type { CampaignChannel } from '../../../../enums/campaigns/campaign-channel.enum';
import type { CampaignStatus } from '../../../../enums/campaigns/campaign-status.enum';
import type { TagFilterMode } from './tag-filter-mode.enum';
import type { DonorStatus } from '../../../../enums/donors/donor-status.enum';
import type { DonorCategory } from '../../../../enums/donors/donor-category.enum';
import type { LogicalOperator } from './logical-operator.enum';

export interface CampaignDonorDto extends EntityDto {
  campaignId?: string;
  donorId?: string;
  donorName?: string;
  donorEmail?: string;
  donorPhone?: string;
  donorCity?: string;
  donorRegion?: string;
  trackingCode?: string;
  extractedAt?: string;
  dispatchedAt?: string;
  openedAt?: string;
  clickedAt?: string;
  responseType?: ResponseType;
  donationAmount?: number;
  donationDate?: string;
  notes?: string;
}

export interface CampaignDto extends FullAuditedEntityDto<string> {
  name?: string;
  year: number;
  code?: string;
  description?: string;
  campaignType?: CampaignType;
  channel?: CampaignChannel;
  status?: CampaignStatus;
  recurrenceId?: string;
  recurrenceName?: string;
  recurrenceDate?: string;
  createdDate?: string;
  extractionScheduledDate?: string;
  extractionDate?: string;
  dispatchScheduledDate?: string;
  dispatchDate?: string;
  yearlySequenceNumber?: number;
  postalCode674?: string;
  mailchimpCampaignId?: string;
  mailchimpListId?: string;
  smsProviderId?: string;
  totalCost: number;
  totalRaised: number;
  targetDonorCount: number;
  extractedDonorCount: number;
  dispatchedCount: number;
  responseCount: number;
  responseRate: number;
  donationCount: number;
  averageDonation: number;
  conversionRate: number;
  roi: number;
  netAmount: number;
  costPerAcquisition: number;
}

export interface CampaignListDto extends EntityDto<string> {
  name?: string;
  year: number;
  code?: string;
  campaignType?: CampaignType;
  channel?: CampaignChannel;
  status?: CampaignStatus;
  recurrenceId?: string;
  recurrenceName?: string;
  dispatchDate?: string;
  extractedDonorCount: number;
  responseCount: number;
  responseRate: number;
  totalRaised: number;
  roi: number;
}

export interface CampaignStatisticsDto {
  campaignId?: string;
  campaignName?: string;
  year: number;
  extractedDonorCount: number;
  dispatchedCount: number;
  responseCount: number;
  openedCount: number;
  clickedCount: number;
  donationCount: number;
  unsubscribedCount: number;
  bouncedCount: number;
  responseRate: number;
  openRate: number;
  clickRate: number;
  conversionRate: number;
  totalCost: number;
  totalRaised: number;
  netAmount: number;
  roi: number;
  averageDonation: number;
  costPerAcquisition: number;
  costPerClick: number;
  responseTypeBreakdown: Record<string, number>;
  regionBreakdown: Record<string, number>;
  segmentBreakdown: Record<string, number>;
  responseTimeline: DailyResponseDto[];
  donationTimeline: DailyDonationDto[];
}

export interface CreateCampaignDto {
  name: string;
  year: number;
  code: string;
  description?: string;
  campaignType: CampaignType;
  channel: CampaignChannel;
  recurrenceId?: string;
  extractionScheduledDate?: string;
  dispatchScheduledDate?: string;
  recurrenceDate?: string;
  totalCost: number;
  targetDonorCount: number;
}

export interface DailyDonationDto {
  date?: string;
  count: number;
  amount: number;
}

export interface DailyResponseDto {
  date?: string;
  openedCount: number;
  clickedCount: number;
  donationCount: number;
}

export interface DateRangeFilter {
  from?: string;
  to?: string;
}

export interface DecimalRangeFilter {
  min?: number;
  max?: number;
}

export interface DonorExtractionPreviewDto {
  totalCount: number;
  donors: DonorPreviewItemDto[];
  statistics: ExtractionStatisticsDto;
  filterBreakdown: FilterBreakdownDto[];
}

export interface DonorPreviewItemDto {
  id?: string;
  name?: string;
  email?: string;
  phone?: string;
  city?: string;
  region?: string;
  lastDonationAmount: number;
  lastDonationDate?: string;
  totalDonations: number;
  tags: string[];
}

export interface ExtractDonorsInput {
  donationDateRange: DateRangeFilter;
  donationAmountRange: DecimalRangeFilter;
  includedTagIds: string[];
  excludedTagIds: string[];
  tagFilterMode?: TagFilterMode;
  includedSegmentIds: string[];
  excludedSegmentIds: string[];
  includedDonorNames: string[];
  excludedDonorNames: string[];
  includedCampaignIds: string[];
  excludedCampaignIds: string[];
  includedRegions: string[];
  excludedRegions: string[];
  includedProvinces: string[];
  excludedProvinces: string[];
  donorStatus?: DonorStatus;
  donorCategory?: DonorCategory;
  requireNewsletterConsent?: boolean;
  requireMailConsent?: boolean;
  logicalOperator?: LogicalOperator;
  maxResults?: number;
  randomSample: boolean;
}

export interface ExtractionStatisticsDto {
  totalDonors: number;
  totalPotentialRevenue: number;
  averageDonationAmount: number;
  activeDonors: number;
  lapsedDonors: number;
  regionDistribution: Record<string, number>;
  segmentDistribution: Record<string, number>;
}

export interface FilterBreakdownDto {
  filterName?: string;
  matchCount: number;
  percentage: number;
}

export interface GetCampaignsInput extends PagedAndSortedResultRequestDto {
  filter?: string;
  year?: number;
  status?: CampaignStatus;
  campaignType?: CampaignType;
  channel?: CampaignChannel;
  recurrenceId?: string;
  dateFrom?: string;
  dateTo?: string;
}

export interface RecordDonationInput {
  donorId: string;
  amount: number;
  donationDate?: string;
  notes?: string;
}

export interface UpdateCampaignDto {
  name: string;
  description?: string;
  recurrenceId?: string;
  extractionScheduledDate?: string;
  dispatchScheduledDate?: string;
  recurrenceDate?: string;
  totalCost: number;
  targetDonorCount: number;
}
