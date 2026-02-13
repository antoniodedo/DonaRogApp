import { mapEnumToOptions } from '@abp/ng.core';

export enum TagFilterMode {
  Any = 0,
  All = 1,
}

export const tagFilterModeOptions = mapEnumToOptions(TagFilterMode);
