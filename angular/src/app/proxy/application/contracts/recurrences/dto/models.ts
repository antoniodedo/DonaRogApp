import type { EntityDto, FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateRecurrenceDto {
  name: string;
  code?: string;
  description?: string;
  recurrenceDay?: number;
  recurrenceMonth?: number;
  daysBeforeRecurrence: number;
  daysAfterRecurrence: number;
  notes?: string;
}

export interface GetRecurrencesInput extends PagedAndSortedResultRequestDto {
  filter?: string;
  isActive?: boolean;
  isCurrentlyInValidityPeriod?: boolean;
}

export interface RecurrenceDto extends FullAuditedEntityDto<string> {
  name?: string;
  code?: string;
  description?: string;
  recurrenceDay?: number;
  recurrenceMonth?: number;
  daysBeforeRecurrence: number;
  daysAfterRecurrence: number;
  notes?: string;
  isActive: boolean;
  deactivatedDate?: string;
  deactivationReason?: string;
  totalValidityDays: number;
  fullDisplayName?: string;
}

export interface RecurrenceListDto extends EntityDto<string> {
  name?: string;
  code?: string;
  recurrenceDay?: number;
  recurrenceMonth?: number;
  daysBeforeRecurrence: number;
  daysAfterRecurrence: number;
  isActive: boolean;
}

export interface UpdateRecurrenceDto {
  name: string;
  description?: string;
  recurrenceDay?: number;
  recurrenceMonth?: number;
  daysBeforeRecurrence: number;
  daysAfterRecurrence: number;
  notes?: string;
}
