import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateLetterTemplateDto extends EntityDto<string> {
  name?: string;
  content?: string;
  description?: string;
  isActive: boolean;
}

export interface LetterTemplateDto extends EntityDto<string> {
  name?: string;
  content?: string;
  description?: string;
  isActive: boolean;
}
