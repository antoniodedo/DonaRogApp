import { mapEnumToOptions } from '@abp/ng.core';

export enum CampaignChannel {
  Postal = 0,
  Email = 1,
  SMS = 2,
  Phone = 3,
  Mixed = 4,
}

export const campaignChannelOptions = mapEnumToOptions(CampaignChannel);
