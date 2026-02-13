import { mapEnumToOptions } from '@abp/ng.core';

export enum LogicalOperator {
  And = 0,
  Or = 1,
}

export const logicalOperatorOptions = mapEnumToOptions(LogicalOperator);
