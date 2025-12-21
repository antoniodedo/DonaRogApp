import type { AuditedEntityDto, EntityDto } from '@abp/ng.core';
import type { Gender } from '../../enums/gender.enum';

export interface CreateUpdateDonorDto extends EntityDto<string> {
  firstName?: string;
  lastName: string;
  rawAddress?: string;
  rawCap?: string;
  rawComune?: string;
}

export interface CreateUpdateDonorTitleDto extends EntityDto<string> {
  title: string;
  gender: Gender;
  isGroup: boolean;
  isActive: boolean;
}

export interface CreateUpdateEmailDto extends AuditedEntityDto<string> {
  donorId?: string;
  address?: string;
  isPrimary: boolean;
}

export interface DonorDto extends EntityDto<string> {
  firstName?: string;
  lastName?: string;
  rawAddress?: string;
  rawCap?: string;
  rawComune?: string;
}

export interface DonorListDto extends EntityDto<string> {
  firstName?: string;
  lastName?: string;
  rawAddress?: string;
  rawCap?: string;
  rawComune?: string;
}

export interface DonorTitleDto extends EntityDto<string> {
  title?: string;
  gender?: Gender;
  isGroup: boolean;
  isActive: boolean;
}

export interface EmailDto extends AuditedEntityDto<string> {
  donorId?: string;
  address?: string;
  isPrimary: boolean;
}
