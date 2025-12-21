import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateNoteDto extends EntityDto<string> {
  content?: string;
  donorId?: string;
  isImportant: boolean;
}

export interface NoteDto extends EntityDto<string> {
  content?: string;
  donorId?: string;
  isImportant: boolean;
}
