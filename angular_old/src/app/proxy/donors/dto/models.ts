import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateDonorDto extends EntityDto<string> {
  firstName?: string;
  lastName: string;
  rawAddress?: string;
  rawCap?: string;
  rawComune?: string;
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
