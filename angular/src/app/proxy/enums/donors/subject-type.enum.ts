import { mapEnumToOptions } from '@abp/ng.core';

export enum SubjectType {
  Individual = 1,
  Organization = 2,
}

export const subjectTypeOptions = mapEnumToOptions(SubjectType);
