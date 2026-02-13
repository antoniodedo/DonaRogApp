import { mapEnumToOptions } from '@abp/ng.core';

export enum ProjectStatus {
  Active = 0,
  Inactive = 1,
  Archived = 2,
}

export const projectStatusOptions = mapEnumToOptions(ProjectStatus);
