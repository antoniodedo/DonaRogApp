import { mapEnumToOptions } from '@abp/ng.core';

export enum ProjectCategory {
  Education = 0,
  Health = 1,
  Environment = 2,
  SocialWelfare = 3,
  Emergency = 4,
  Infrastructure = 5,
  Culture = 6,
  Research = 7,
  Sports = 8,
  Other = 99,
}

export const projectCategoryOptions = mapEnumToOptions(ProjectCategory);
