import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { PrintBatchService } from '../../proxy/communications/print-batches/print-batch.service';
import { 
  PrintBatchDto, 
  CreatePrintBatchDto, 
  PrintBatchFilterDto,
  PrintBatchPreviewDto
} from '../../proxy/communications/print-batches/models';
import { PrintBatchStatus } from '../../proxy/communications/models';
import { ProjectService } from '../../proxy/application/projects/project.service';
import { CampaignService } from '../../proxy/application/campaigns/campaign.service';

@Component({
  selector: 'app-print-batch-list',
  templateUrl: './print-batch-list.component.html',
  styleUrls: ['./print-batch-list.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    NzTableModule,
    NzButtonModule,
    NzIconModule,
    NzInputModule,
    NzSelectModule,
    NzDatePickerModule,
    NzCardModule,
    NzModalModule,
    NzTagModule,
    NzSpaceModule,
    NzDrawerModule,
    NzDividerModule,
    NzInputNumberModule,
    NzCheckboxModule,
    NzFormModule,
    NzDescriptionsModule,
    NzStatisticModule,
    NzSpinModule
  ]
})
export class PrintBatchListComponent implements OnInit {
  batches: PrintBatchDto[] = [];
  loading = false;
  totalCount = 0;
  pageSize = 20;
  pageIndex = 1;
  
  PrintBatchStatus = PrintBatchStatus;
  
  // Filters
  searchText = '';
  filterStatus: PrintBatchStatus | null = null;
  filterDateFrom: Date | null = null;
  filterDateTo: Date | null = null;
  
  // Create Batch Modal
  isCreateModalVisible = false;
  isLoadingPreview = false;
  batchPreview: PrintBatchPreviewDto | null = null;
  newBatchName = '';
  newBatchFilters: PrintBatchFilterDto = {
    onlyVerified: true,
    excludePrinted: true,
    excludeInOtherBatches: true
  };
  newBatchNotes = '';
  autoGeneratePdf = false;
  
  // Reference data
  projects: any[] = [];
  campaigns: any[] = [];
  
  constructor(
    private printBatchService: PrintBatchService,
    private projectService: ProjectService,
    private campaignService: CampaignService,
    private message: NzMessageService,
    private modal: NzModalService,
    private router: Router
  ) {}
  
  ngOnInit(): void {
    this.loadBatches();
    this.loadReferenceData();
  }
  
  loadReferenceData(): void {
    this.projectService.getList({ maxResultCount: 1000 }).subscribe({
      next: (result) => {
        this.projects = result.items || [];
      }
    });
    
    this.campaignService.getList({ maxResultCount: 1000 }).subscribe({
      next: (result) => {
        this.campaigns = result.items || [];
      }
    });
  }
  
  loadBatches(): void {
    this.loading = true;
    
    this.printBatchService.getList({
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize,
      filter: this.searchText || undefined,
      status: this.filterStatus !== null ? this.filterStatus : undefined,
      generatedFrom: this.filterDateFrom?.toISOString(),
      generatedTo: this.filterDateTo?.toISOString(),
      includeCancelled: true
    }).subscribe({
      next: (result) => {
        this.batches = result.items || [];
        this.totalCount = result.totalCount || 0;
        this.loading = false;
      },
      error: () => {
        this.message.error('Errore caricamento batch');
        this.loading = false;
      }
    });
  }
  
  onPageChange(pageIndex: number): void {
    this.pageIndex = pageIndex;
    this.loadBatches();
  }
  
  search(): void {
    this.pageIndex = 1;
    this.loadBatches();
  }
  
  resetFilters(): void {
    this.searchText = '';
    this.filterStatus = null;
    this.filterDateFrom = null;
    this.filterDateTo = null;
    this.search();
  }
  
  showCreateModal(): void {
    this.newBatchName = '';
    this.newBatchFilters = {
      onlyVerified: true,
      excludePrinted: true,
      excludeInOtherBatches: true
    };
    this.newBatchNotes = '';
    this.autoGeneratePdf = false;
    this.batchPreview = null;
    this.isCreateModalVisible = true;
  }
  
  previewBatch(): void {
    this.isLoadingPreview = true;
    
    this.printBatchService.previewBatch(this.newBatchFilters).subscribe({
      next: (preview) => {
        this.batchPreview = preview;
        this.isLoadingPreview = false;
      },
      error: () => {
        this.message.error('Errore caricamento anteprima');
        this.isLoadingPreview = false;
      }
    });
  }
  
  createBatch(): void {
    if (!this.batchPreview || this.batchPreview.totalLetters === 0) {
      this.message.warning('Nessuna lettera trovata con i filtri selezionati');
      return;
    }
    
    const dto: CreatePrintBatchDto = {
      name: this.newBatchName || undefined,
      filters: this.newBatchFilters,
      notes: this.newBatchNotes || undefined,
      autoGeneratePdf: this.autoGeneratePdf
    };
    
    this.loading = true;
    
    this.printBatchService.create(dto).subscribe({
      next: (created) => {
        this.message.success(`Batch creato con ${created.totalLetters} lettere`);
        this.isCreateModalVisible = false;
        this.loadBatches();
        
        if (this.autoGeneratePdf) {
          this.message.info('Generazione PDF avviata in background');
        }
      },
      error: () => {
        this.loading = false;
        this.message.error('Errore creazione batch');
      }
    });
  }
  
  handleCreateModalCancel(): void {
    this.isCreateModalVisible = false;
    this.batchPreview = null;
  }
  
  generatePdf(batch: PrintBatchDto): void {
    this.modal.confirm({
      nzTitle: 'Genera PDF',
      nzContent: `Vuoi generare il PDF per il batch "${batch.batchNumber}" con ${batch.totalLetters} lettere?`,
      nzOkText: 'Genera',
      nzCancelText: 'Annulla',
      nzOnOk: () => {
        this.loading = true;
        this.printBatchService.generatePdf({
          batchId: batch.id,
          runInBackground: batch.totalLetters > 50
        }).subscribe({
          next: (result) => {
            if (result.success) {
              this.message.success('PDF generato con successo');
            } else {
              this.message.info('Generazione PDF avviata in background');
            }
            this.loadBatches();
          },
          error: () => {
            this.loading = false;
            this.message.error('Errore generazione PDF');
          }
        });
      }
    });
  }
  
  downloadPdf(batch: PrintBatchDto): void {
    if (!batch.pdfFileSizeBytes) {
      this.message.warning('PDF non disponibile');
      return;
    }
    
    this.printBatchService.downloadPdf(batch.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `batch-${batch.batchNumber}.pdf`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        
        this.message.success('Download completato');
      },
      error: () => {
        this.message.error('Errore download PDF');
      }
    });
  }
  
  markAsPrinted(batch: PrintBatchDto): void {
    this.modal.confirm({
      nzTitle: 'Conferma Stampa',
      nzContent: `Confermi di aver stampato il batch "${batch.batchNumber}"? Questa operazione non può essere annullata.`,
      nzOkText: 'Conferma',
      nzOkDanger: true,
      nzCancelText: 'Annulla',
      nzOnOk: () => {
        this.printBatchService.markAsPrinted({
          batchId: batch.id,
          notes: undefined
        }).subscribe({
          next: () => {
            this.message.success('Batch marcato come stampato');
            this.loadBatches();
          },
          error: () => {
            this.message.error('Errore aggiornamento batch');
          }
        });
      }
    });
  }
  
  cancelBatch(batch: PrintBatchDto): void {
    this.modal.confirm({
      nzTitle: 'Annulla Batch',
      nzContent: `Vuoi annullare il batch "${batch.batchNumber}"? Le lettere torneranno disponibili per altri batch.`,
      nzOkText: 'Annulla Batch',
      nzOkDanger: true,
      nzCancelText: 'Chiudi',
      nzOnOk: () => {
        this.printBatchService.cancel({ batchId: batch.id, reason: 'Annullato da operatore' }).subscribe({
          next: () => {
            this.message.success('Batch annullato');
            this.loadBatches();
          },
          error: () => {
            this.message.error('Errore annullamento batch');
          }
        });
      }
    });
  }
  
  deleteBatch(batch: PrintBatchDto): void {
    this.modal.confirm({
      nzTitle: 'Elimina Batch',
      nzContent: `Vuoi eliminare definitivamente il batch "${batch.batchNumber}"?`,
      nzOkText: 'Elimina',
      nzOkDanger: true,
      nzCancelText: 'Annulla',
      nzOnOk: () => {
        this.printBatchService.delete(batch.id).subscribe({
          next: () => {
            this.message.success('Batch eliminato');
            this.loadBatches();
          },
          error: () => {
            this.message.error('Errore eliminazione batch');
          }
        });
      }
    });
  }
  
  getStatusTag(status: PrintBatchStatus): { color: string; text: string; icon: string } {
    switch (status) {
      case PrintBatchStatus.Draft:
        return { color: 'default', text: 'Bozza', icon: 'edit' };
      case PrintBatchStatus.Ready:
        return { color: 'processing', text: 'Pronto', icon: 'check-circle' };
      case PrintBatchStatus.Generating:
        return { color: 'warning', text: 'Generazione...', icon: 'sync' };
      case PrintBatchStatus.Downloaded:
        return { color: 'cyan', text: 'Scaricato', icon: 'download' };
      case PrintBatchStatus.Printed:
        return { color: 'success', text: 'Stampato', icon: 'printer' };
      case PrintBatchStatus.Cancelled:
        return { color: 'error', text: 'Annullato', icon: 'close-circle' };
      default:
        return { color: 'default', text: 'Sconosciuto', icon: 'question' };
    }
  }
  
  formatBytes(bytes: number | null | undefined): string {
    if (!bytes) return '-';
    
    const kb = bytes / 1024;
    if (kb < 1024) return `${kb.toFixed(1)} KB`;
    
    const mb = kb / 1024;
    return `${mb.toFixed(2)} MB`;
  }
  
  getRowClass(batch: PrintBatchDto): string {
    if (batch.status === PrintBatchStatus.Cancelled) return 'row-cancelled';
    if (batch.status === PrintBatchStatus.Printed) return 'row-printed';
    return '';
  }
}
