import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateNoteDto {
  content: string;
  category?: string;
  interactionDate?: string;
  isImportant: boolean;
  isPrivate: boolean;
}

export interface NoteDto extends EntityDto<string> {
  donorId?: string;
  content?: string;
  category?: string;
  interactionDate?: string;
  isImportant: boolean;
  isPrivate: boolean;
  creationTime?: string;
  lastModificationTime?: string;
  creatorId?: string;
  lastModifierId?: string;
}
