import { mapEnumToOptions } from '@abp/ng.core';

export enum DonorStatus {
  New = 1,
  Active = 2,
  Lapsed = 3,
  Inactive = 4,
  Suspended = 5,
  Disabled = 6,
  Deceased = 7,
  DoNotContact = 8,
  Duplicate = 9,
}

export const donorStatusOptions = mapEnumToOptions(DonorStatus);
