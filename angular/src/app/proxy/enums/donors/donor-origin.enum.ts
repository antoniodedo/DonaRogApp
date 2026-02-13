import { mapEnumToOptions } from '@abp/ng.core';

export enum DonorOrigin {
  Unknown = 0,
  Website = 1,
  SocialMedia = 2,
  Event = 3,
  PhoneCampaign = 4,
  DirectMail = 5,
  EmailCampaign = 6,
  Referral = 7,
  Partnership = 8,
  Media = 9,
  WalkIn = 10,
  Corporate = 11,
  Bequest = 12,
  Other = 99,
}

export const donorOriginOptions = mapEnumToOptions(DonorOrigin);
