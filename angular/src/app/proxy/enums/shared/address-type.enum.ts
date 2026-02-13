import { mapEnumToOptions } from '@abp/ng.core';

export enum AddressType {
  Home = 1,
  Work = 2,
  Billing = 3,
  Shipping = 4,
  Temporary = 5,
  Other = 6,
}

export const addressTypeOptions = mapEnumToOptions(AddressType);
