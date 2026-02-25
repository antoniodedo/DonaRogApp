import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export enum CommunicationStatus {
  Draft = 1,
  PendingPrint = 2,
  InBatch = 3,
  Printed = 4,
  Sent = 5,
  Delivered = 6,
  Failed = 7,
  Cancelled = 99,
}

export enum PrintBatchStatus {
  Draft = 1,
  Ready = 2,
  Generating = 3,
  Generated = 4,
  Downloaded = 5,
  Printed = 6,
  Cancelled = 99,
}

export enum AlertLevel {
  None = 0,
  Info = 1,
  Warning = 2,
  Error = 3,
}

export enum PreferredThankYouChannel {
  Email = 1,
  Letter = 2,
  Both = 3,
  None = 99,
}

export enum TemplateType {
  Html = 1,
  Docx = 2,
}

export interface RecentCommunicationDto {
  id: string;
  type: number;
  category?: number;
  subject: string;
  sentDate: string;
  isPrinted: boolean;
  isInBatch: boolean;
  printBatchNumber?: string;
  donationId?: string;
  donationReference?: string;
  donationAmount?: number;
  daysAgo: number;
  alertLevel: AlertLevel;
}

export interface DuplicateCheckResultDto {
  alertLevel: AlertLevel;
  recentCommunications: RecentCommunicationDto[];
  hasCriticalAlert: boolean;
  message?: string;
}

export interface CheckDuplicateLettersDto {
  donorId: string;
  category?: number;
  checkLastDays: number;
  errorThresholdDays: number;
  warningThresholdDays: number;
}

export interface CommunicationHistoryDto {
  id: string;
  donorId: string;
  donorName: string;
  type: number;
  category?: number;
  subject: string;
  sentDate: string;
  status: CommunicationStatus;
  isPrinted: boolean;
  printedAt?: string;
  printBatchId?: string;
  printBatchNumber?: string;
  donationId?: string;
  donationReference?: string;
  creationTime: string;
}

export interface GetCommunicationHistoryInput extends PagedAndSortedResultRequestDto {
  donorId?: string;
  type?: number;
  category?: number;
  dateFrom?: string;
  dateTo?: string;
  status?: CommunicationStatus;
  onlyPrinted?: boolean;
}
