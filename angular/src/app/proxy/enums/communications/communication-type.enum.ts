import { mapEnumToOptions } from '@abp/ng.core';

export enum CommunicationType {
  Email = 1,
  Letter = 2,
  SMS = 3,
  WhatsApp = 4,
  PhoneCall = 5,
  PushNotification = 6,
  InApp = 7,
  SocialMedia = 8,
}

export const communicationTypeOptions = mapEnumToOptions(CommunicationType);
