import { mapEnumToOptions } from '@abp/ng.core';

export enum DonorCategory {
  Unclassified = 0,
  Standard = 1,
  Bronze = 2,
  Silver = 3,
  Gold = 4,
  Major = 5,
}

export const donorCategoryOptions = mapEnumToOptions(DonorCategory);
