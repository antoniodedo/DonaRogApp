import { mapEnumToOptions } from '@abp/ng.core';

export enum Gender {
  Unspecified = 0,
  Male = 1,
  Female = 2,
}

export const genderOptions = mapEnumToOptions(Gender);
