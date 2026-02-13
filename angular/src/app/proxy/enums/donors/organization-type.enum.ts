import { mapEnumToOptions } from '@abp/ng.core';

export enum OrganizationType {
  Association = 1,
  Foundation = 2,
  NGO = 3,
  Charity = 4,
  Religious = 5,
  Educational = 6,
  Healthcare = 7,
  Government = 8,
  PrivateCompany = 9,
  Cooperative = 10,
  SocialEnterprise = 11,
  Other = 99,
}

export const organizationTypeOptions = mapEnumToOptions(OrganizationType);
