import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { PrintBatchStatus } from '../models';

export interface PrintBatchDto extends FullAuditedEntityDto<string> {
  batchNumber: string;
  name?: string;
  status: PrintBatchStatus;
  totalLetters: number;
  totalDonationAmount: number;
  pdfFileSizeBytes?: number;
  generatedAt?: string;
  generatedBy?: string;
  printedAt?: string;
  printedBy?: string;
  filterSummary?: BatchFilterSummaryDto;
  notes?: string;
  canEdit: boolean;
  canGeneratePdf: boolean;
  canCancel: boolean;
}

export interface BatchFilterSummaryDto {
  minAmount?: number;
  maxAmount?: number;
  region?: string;
  projectNames?: string;
  campaignNames?: string;
  filterCount: number;
}

export interface CreatePrintBatchDto {
  name?: string;
  filters: PrintBatchFilterDto;
  notes?: string;
  autoGeneratePdf: boolean;
}

export interface PrintBatchFilterDto {
  minAmount?: number;
  maxAmount?: number;
  region?: string;
  projectIds?: string[];
  campaignIds?: string[];
  donationDateFrom?: string;
  donationDateTo?: string;
  donorCategory?: number;
  onlyVerified: boolean;
  excludePrinted: boolean;
  excludeInOtherBatches: boolean;
}

export interface PrintBatchPreviewDto {
  totalLetters: number;
  totalDonationAmount: number;
  estimatedPdfSizeMB: number;
  sampleLetters: LetterPreviewItemDto[];
  byProject: Record<string, number>;
  byRegion: Record<string, number>;
  appliedFilters: PrintBatchFilterDto;
}

export interface LetterPreviewItemDto {
  communicationId: string;
  donorId: string;
  donorName: string;
  donationId: string;
  donationReference: string;
  donationAmount: number;
  donationDate: string;
  region?: string;
  projectName?: string;
  createdAt: string;
}

export interface GenerateBatchPdfDto {
  batchId: string;
  runInBackground: boolean;
}

export interface BatchPdfGenerationResultDto {
  success: boolean;
  isBackgroundJob: boolean;
  jobId?: string;
  pdfFileSizeBytes?: number;
  durationMs?: number;
  errorMessage?: string;
  batch?: PrintBatchDto;
}

export interface MarkBatchAsPrintedDto {
  batchId: string;
  printedAt?: string;
  notes?: string;
}

export interface CancelBatchDto {
  batchId: string;
  reason: string;
}

export interface GetPrintBatchesInput extends PagedAndSortedResultRequestDto {
  filter?: string;
  status?: PrintBatchStatus;
  generatedFrom?: string;
  generatedTo?: string;
  generatedBy?: string;
  isPrinted?: boolean;
  includeCancelled: boolean;
}

export interface UpdatePrintBatchDto {
  name?: string;
  notes?: string;
}

export interface PrintBatchStatisticsDto {
  totalBatches: number;
  pendingBatches: number;
  generatedBatches: number;
  printedBatches: number;
  totalLettersPrinted: number;
  lettersPendingPrint: number;
}
