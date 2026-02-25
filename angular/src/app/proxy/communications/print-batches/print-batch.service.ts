import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { PagedResultDto } from '@abp/ng.core';
import type {
  BatchPdfGenerationResultDto,
  CancelBatchDto,
  CreatePrintBatchDto,
  GenerateBatchPdfDto,
  GetPrintBatchesInput,
  MarkBatchAsPrintedDto,
  PrintBatchDto,
  PrintBatchFilterDto,
  PrintBatchPreviewDto,
  PrintBatchStatisticsDto,
  UpdatePrintBatchDto,
} from './models';

@Injectable({
  providedIn: 'root',
})
export class PrintBatchService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  cancel = (input: CancelBatchDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PrintBatchDto>(
      {
        method: 'POST',
        url: '/api/print-batches/cancel',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  create = (input: CreatePrintBatchDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PrintBatchDto>(
      {
        method: 'POST',
        url: '/api/print-batches',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/print-batches/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  downloadPdf = (batchId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>(
      {
        method: 'GET',
        responseType: 'blob',
        url: `/api/print-batches/${batchId}/download-pdf`,
      },
      { apiName: this.apiName, ...config }
    );

  generatePdf = (input: GenerateBatchPdfDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BatchPdfGenerationResultDto>(
      {
        method: 'POST',
        url: '/api/print-batches/generate-pdf',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PrintBatchDto>(
      {
        method: 'GET',
        url: `/api/print-batches/${id}`,
      },
      { apiName: this.apiName, ...config }
    );

  getGenerationStatus = (batchId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BatchPdfGenerationResultDto>(
      {
        method: 'GET',
        url: `/api/print-batches/${batchId}/generation-status`,
      },
      { apiName: this.apiName, ...config }
    );

  getList = (input: GetPrintBatchesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PrintBatchDto>>(
      {
        method: 'GET',
        url: '/api/print-batches',
        params: {
          filter: input.filter,
          status: input.status,
          generatedFrom: input.generatedFrom,
          generatedTo: input.generatedTo,
          generatedBy: input.generatedBy,
          isPrinted: input.isPrinted,
          includeCancelled: input.includeCancelled,
          sorting: input.sorting,
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
        },
      },
      { apiName: this.apiName, ...config }
    );

  getStatistics = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, PrintBatchStatisticsDto>(
      {
        method: 'GET',
        url: '/api/print-batches/statistics',
      },
      { apiName: this.apiName, ...config }
    );

  markAsDownloaded = (batchId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PrintBatchDto>(
      {
        method: 'POST',
        url: `/api/print-batches/${batchId}/mark-downloaded`,
      },
      { apiName: this.apiName, ...config }
    );

  markAsPrinted = (input: MarkBatchAsPrintedDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PrintBatchDto>(
      {
        method: 'POST',
        url: '/api/print-batches/mark-printed',
        body: input,
      },
      { apiName: this.apiName, ...config }
    );

  previewBatch = (filters: PrintBatchFilterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PrintBatchPreviewDto>(
      {
        method: 'POST',
        url: '/api/print-batches/preview',
        body: filters,
      },
      { apiName: this.apiName, ...config }
    );

  update = (id: string, input: UpdatePrintBatchDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PrintBatchDto>(
      {
        method: 'PUT',
        url: `/api/print-batches/${id}`,
        body: input,
      },
      { apiName: this.apiName, ...config }
    );
}
