import { mapEnumToOptions } from '@abp/ng.core';

export enum ContactType {
  Mobile = 1,
  HomeLandline = 2,
  WorkLandline = 3,
  Fax = 4,
  WhatsApp = 5,
  Other = 6,
}

export const contactTypeOptions = mapEnumToOptions(ContactType);
