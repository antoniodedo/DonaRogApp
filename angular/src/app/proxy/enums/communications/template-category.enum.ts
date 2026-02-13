import { mapEnumToOptions } from '@abp/ng.core';

export enum TemplateCategory {
  ThankYou = 1,
  Reminder = 2,
  Newsletter = 3,
  HolidayGreeting = 4,
  ProjectUpdate = 5,
  Solicitation = 6,
  Confirmation = 7,
  InformationRequest = 8,
  Birthday = 9,
  Anniversary = 10,
  Survey = 11,
  EventInvitation = 12,
  FiscalReceipt = 13,
  Other = 99,
}

export const templateCategoryOptions = mapEnumToOptions(TemplateCategory);
