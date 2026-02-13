import { mapEnumToOptions } from '@abp/ng.core';

export enum CampaignStatus {
  Draft = 0,
  InPreparation = 1,
  Extracted = 2,
  Dispatched = 3,
  Completed = 4,
  Cancelled = 5,
}

export const campaignStatusOptions = mapEnumToOptions(CampaignStatus);
