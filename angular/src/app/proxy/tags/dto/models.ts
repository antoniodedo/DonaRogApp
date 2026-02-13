import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateTagDto {
  name: string;
  description?: string;
  colorCode?: string;
  category?: string;
}

export interface TagDto extends EntityDto<string> {
  code?: string;
  name?: string;
  description?: string;
  colorCode?: string;
  category?: string;
  usageCount: number;
  isActive: boolean;
  isSystem: boolean;
}
