import { mapEnumToOptions } from '@abp/ng.core';

export enum CampaignType {
  Prospect = 0,
  Archive = 1,
}

export const campaignTypeOptions = mapEnumToOptions(CampaignType);
