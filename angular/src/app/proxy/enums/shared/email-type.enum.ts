import { mapEnumToOptions } from '@abp/ng.core';

export enum EmailType {
  Personal = 1,
  Work = 2,
  Other = 3,
}

export const emailTypeOptions = mapEnumToOptions(EmailType);
