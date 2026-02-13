import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { RecurrenceService } from 'src/app/proxy/application/recurrences';
import { RecurrenceListDto, GetRecurrencesInput } from 'src/app/proxy/application/contracts/recurrences/dto/models';
import { PagedResultDto } from '@abp/ng.core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-recurrences-list',
  standalone: false,
  templateUrl: './recurrences-list.component.html',
  styleUrls: ['./recurrences-list.component.scss']
})
export class RecurrencesListComponent implements OnInit, OnDestroy {
  recurrences: RecurrenceListDto[] = [];
  total = 0;
  loading = false;
  pageSize = 10;
  pageIndex = 1;
  searchText = '';

  // Filters
  filterForm: FormGroup;
  isAdvancedSearchVisible = false;

  // Modal
  isModalVisible = false;
  modalTitle = '';
  selectedRecurrence: RecurrenceListDto | null = null;

  // Subject for search debounce
  private searchSubject = new Subject<string>();

  constructor(
    private recurrenceService: RecurrenceService,
    private fb: FormBuilder,
    private modal: NzModalService,
    private message: NzMessageService
  ) {
    this.filterForm = this.fb.group({
      filter: [''],
      isActive: [null] // Filtro per ricorrenze attive/disattivate
    });
  }

  ngOnInit(): void {
    this.loadRecurrences();
    
    // Setup debounce for search
    this.searchSubject.pipe(
      debounceTime(500),
      distinctUntilChanged()
    ).subscribe(() => {
      this.pageIndex = 1;
      this.loadRecurrences();
    });
  }

  ngOnDestroy(): void {
    this.searchSubject.complete();
  }

  loadRecurrences(): void {
    this.loading = true;
    const input: GetRecurrencesInput = {
      filter: this.searchText || this.filterForm.value.filter,
      isActive: this.filterForm.value.isActive,
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize
    };

    this.recurrenceService.getRecurrenceList(input).subscribe({
      next: (result: PagedResultDto<RecurrenceListDto>) => {
        this.recurrences = result.items;
        this.total = result.totalCount;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.message.error('Errore nel caricamento delle ricorrenze');
      }
    });
  }

  onSearch(): void {
    this.searchSubject.next(this.searchText);
  }

  clearSearch(): void {
    this.searchText = '';
    this.pageIndex = 1;
    this.loadRecurrences();
  }

  openAdvancedSearch(): void {
    this.isAdvancedSearchVisible = true;
  }

  closeAdvancedSearch(): void {
    this.isAdvancedSearchVisible = false;
  }

  hasActiveFilters(): boolean {
    return this.filterForm.value.isActive !== null;
  }

  getActiveFiltersCount(): number {
    return this.filterForm.value.isActive !== null ? 1 : 0;
  }

  getActiveRecurrencesCount(): number {
    return this.recurrences.filter(r => r.isActive).length;
  }

  getInactiveRecurrencesCount(): number {
    return this.recurrences.filter(r => !r.isActive).length;
  }

  getRecurrencesWithDateCount(): number {
    return this.recurrences.filter(r => 
      r.recurrenceDay !== null && r.recurrenceDay !== undefined &&
      r.recurrenceMonth !== null && r.recurrenceMonth !== undefined
    ).length;
  }

  onPageChange(pageIndex: number): void {
    this.pageIndex = pageIndex;
    this.loadRecurrences();
  }

  onPageSizeChange(pageSize: number): void {
    this.pageSize = pageSize;
    this.pageIndex = 1;
    this.loadRecurrences();
  }

  applyFilters(): void {
    this.pageIndex = 1;
    this.closeAdvancedSearch();
    this.loadRecurrences();
  }

  resetFilters(): void {
    this.filterForm.reset({
      filter: '',
      isActive: null
    });
    this.applyFilters();
  }

  openCreateModal(): void {
    this.selectedRecurrence = null;
    this.modalTitle = 'Nuova Ricorrenza';
    this.isModalVisible = true;
  }

  openEditModal(recurrence: RecurrenceListDto): void {
    this.selectedRecurrence = recurrence;
    this.modalTitle = 'Modifica Ricorrenza';
    this.isModalVisible = true;
  }

  handleModalClose(saved: boolean): void {
    this.isModalVisible = false;
    if (saved) {
      this.loadRecurrences();
    }
  }

  deleteRecurrence(id: string): void {
    this.recurrenceService.delete(id).subscribe({
      next: () => {
        this.message.success('Ricorrenza eliminata con successo');
        this.loadRecurrences();
      },
      error: () => {
        this.message.error('Errore durante l\'eliminazione della ricorrenza');
      }
    });
  }

  deactivateRecurrence(id: string): void {
    this.modal.confirm({
      nzTitle: 'Disattiva Ricorrenza',
      nzContent: 'Vuoi disattivare questa ricorrenza? Inserisci il motivo:',
      nzOkText: 'Disattiva',
      nzCancelText: 'Annulla',
      nzOnOk: () => {
        // TODO: Add input field for reason in modal
        const reason = 'Disattivato dall\'utente';
        return this.recurrenceService.deactivate(id, reason).toPromise().then(() => {
          this.message.success('Ricorrenza disattivata con successo');
          this.loadRecurrences();
        }).catch(() => {
          this.message.error('Errore durante la disattivazione della ricorrenza');
        });
      }
    });
  }

  reactivateRecurrence(id: string): void {
    this.recurrenceService.reactivate(id).subscribe({
      next: () => {
        this.message.success('Ricorrenza riattivata con successo');
        this.loadRecurrences();
      },
      error: () => {
        this.message.error('Errore durante la riattivazione della ricorrenza');
      }
    });
  }
}
