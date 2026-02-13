import type { AuditedEntityDto, EntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { TemplateCategory } from '../../enums/communications/template-category.enum';
import { CommunicationType } from '../../enums/communications/communication-type.enum';

export interface LetterTemplateDto extends AuditedEntityDto<string> {
  name: string;
  description?: string;
  content: string;
  category: TemplateCategory;
  language: string;
  communicationType?: CommunicationType;
  projectId?: string;
  recurrenceId?: string;
  minAmount?: number;
  maxAmount?: number;
  isForNewDonor: boolean;
  isPlural: boolean;
  isActive: boolean;
  isDefault: boolean;
  ccEmails?: string;
  bccEmails?: string;
  usageCount: number;
  lastUsedDate?: string;
  version: number;
  previousVersionId?: string;
  tags?: string;
  projectName?: string;
  recurrenceName?: string;
  attachments: TemplateAttachmentDto[];
}

export interface LetterTemplateListDto extends EntityDto<string> {
  name: string;
  description?: string;
  category: TemplateCategory;
  language: string;
  communicationType?: CommunicationType;
  projectId?: string;
  recurrenceId?: string;
  minAmount?: number;
  maxAmount?: number;
  isForNewDonor: boolean;
  isPlural: boolean;
  isActive: boolean;
  isDefault: boolean;
  usageCount: number;
  lastUsedDate?: string;
  version: number;
  projectName?: string;
  recurrenceName?: string;
  tags?: string;
  creationTime: string;
  lastModificationTime?: string;
}

export interface CreateUpdateLetterTemplateDto {
  name: string;
  description?: string;
  content: string;
  category: TemplateCategory;
  language: string;
  communicationType?: CommunicationType;
  projectId?: string;
  recurrenceId?: string;
  minAmount?: number;
  maxAmount?: number;
  isForNewDonor: boolean;
  isPlural: boolean;
  isActive: boolean;
  isDefault: boolean;
  ccEmails?: string;
  bccEmails?: string;
  tags?: string;
}

export interface TemplateAttachmentDto extends EntityDto<string> {
  templateId: string;
  fileName: string;
  filePath: string;
  fileSize: number;
  description?: string;
  creationTime: string;
}

export interface GetLetterTemplatesInput extends PagedAndSortedResultRequestDto {
  filter?: string;
  category?: TemplateCategory;
  language?: string;
  isActive?: boolean;
  isDefault?: boolean;
  projectId?: string;
  recurrenceId?: string;
  communicationType?: CommunicationType;
  isForNewDonor?: boolean;
  isPlural?: boolean;
}

export interface SelectTemplateInput {
  category: TemplateCategory;
  language: string;
  donationAmount: number;
  isNewDonor: boolean;
  isPlural: boolean;
  projectId?: string;
  recurrenceId?: string;
  preferredCommunicationType?: CommunicationType;
}

export interface RenderTemplateWithDonorInput {
  templateId: string;
  donorId: string;
  donationId?: string;
  additionalTags: Record<string, string>;
}

export interface SendTestEmailInput {
  templateId: string;
  testEmail: string;
  donorId?: string;
  tagValues: Record<string, string>;
}
