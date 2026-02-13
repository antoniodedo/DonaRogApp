import { mapEnumToOptions } from '@abp/ng.core';

export enum LegalForm {
  SoleProprietorship = 1,
  SimplePartnership = 2,
  GeneralPartnership = 3,
  LimitedPartnership = 4,
  LLC = 5,
  SimplifiedLLC = 6,
  Corporation = 7,
  PartnershipLimitedByShares = 8,
  Cooperative = 9,
  SocialCooperative = 10,
  Association = 11,
  RecognizedAssociation = 12,
  Foundation = 13,
  Committee = 14,
  ONLUS = 15,
  ETS = 16,
  ODV = 17,
  APS = 18,
  Other = 99,
}

export const legalFormOptions = mapEnumToOptions(LegalForm);
