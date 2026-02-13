import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { DonorService } from '../../proxy/donors/donor.service';
import { DonorDto } from '../../proxy/donors/dtos/models';
import type { GetDonorsInput } from '../../proxy/donors/dto/models';
import { SubjectType } from '../../proxy/enums/donors/subject-type.enum';
import { DonorStatus } from '../../proxy/enums/donors/donor-status.enum';
import { DonorCategory } from '../../proxy/enums/donors/donor-category.enum';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-donor-list',
  standalone: false,
  templateUrl: './donor-list.component.html',
  styleUrls: ['./donor-list.component.scss']
})
export class DonorListComponent implements OnInit, OnDestroy {
  donors: DonorDto[] = [];
  loading = false;
  total = 0;
  pageIndex = 1;
  pageSize = 10;
  searchText = '';

  // Modal per nuovo/modifica donatore
  isFormModalVisible = false;
  editingDonor: DonorDto | null = null;

  // Ricerca avanzata
  isAdvancedSearchVisible = false;
  advancedFilters = {
    subjectType: undefined as SubjectType | undefined,
    status: undefined as DonorStatus | undefined,
    category: undefined as DonorCategory | undefined,
    titleId: undefined as string | undefined,
    donorCode: undefined as string | undefined,
    email: undefined as string | undefined,
    phoneNumber: undefined as string | undefined,
    city: undefined as string | undefined,
    postalCode: undefined as string | undefined,
    province: undefined as string | undefined,
    country: undefined as string | undefined
  };

  SubjectType = SubjectType;
  DonorStatus = DonorStatus;
  DonorCategory = DonorCategory;

  // Subject per debounce della ricerca
  private searchSubject = new Subject<string>();

  constructor(
    private donorService: DonorService,
    private router: Router,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.loadDonors();
    
    // Setup del debounce per la ricerca
    this.searchSubject.pipe(
      debounceTime(500), // Attende 500ms dopo l'ultimo carattere digitato
      distinctUntilChanged() // Evita ricerche duplicate
    ).subscribe(() => {
      this.pageIndex = 1;
      this.loadDonors();
    });
  }

  ngOnDestroy(): void {
    this.searchSubject.complete();
  }

  loadDonors(): void {
    this.loading = true;
    
    const input: GetDonorsInput = {
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize,
      sorting: 'creationTime desc',
      filter: this.searchText || undefined,
      subjectType: this.advancedFilters.subjectType,
      status: this.advancedFilters.status,
      category: this.advancedFilters.category,
      titleId: this.advancedFilters.titleId,
      donorCode: this.advancedFilters.donorCode || undefined,
      email: this.advancedFilters.email || undefined,
      phoneNumber: this.advancedFilters.phoneNumber || undefined,
      city: this.advancedFilters.city || undefined,
      postalCode: this.advancedFilters.postalCode || undefined,
      province: this.advancedFilters.province || undefined,
      country: this.advancedFilters.country || undefined
    };

    this.donorService.getList(input).subscribe({
      next: (result) => {
        this.donors = result.items || [];
        this.total = result.totalCount || 0;
        this.loading = false;
      },
      error: (err) => {
        this.message.error('Errore nel caricamento dei donatori');
        this.loading = false;
        console.error(err);
      }
    });
  }

  onPageIndexChange(pageIndex: number): void {
    this.pageIndex = pageIndex;
    this.loadDonors();
  }

  onPageSizeChange(pageSize: number): void {
    this.pageSize = pageSize;
    this.pageIndex = 1;
    this.loadDonors();
  }

  onSearch(): void {
    // Emette il valore nel Subject che gestisce il debounce
    this.searchSubject.next(this.searchText);
  }

  clearSearch(): void {
    this.searchText = '';
    this.pageIndex = 1;
    this.loadDonors(); // Chiamata immediata quando si cancella
  }

  openAdvancedSearch(): void {
    this.isAdvancedSearchVisible = true;
  }

  closeAdvancedSearch(): void {
    this.isAdvancedSearchVisible = false;
  }

  applyAdvancedFilters(): void {
    this.pageIndex = 1;
    this.loadDonors();
    this.closeAdvancedSearch();
  }

  clearAdvancedFilters(): void {
    this.advancedFilters = {
      subjectType: undefined,
      status: undefined,
      category: undefined,
      titleId: undefined,
      donorCode: undefined,
      email: undefined,
      phoneNumber: undefined,
      city: undefined,
      postalCode: undefined,
      province: undefined,
      country: undefined
    };
    this.pageIndex = 1;
    this.loadDonors();
  }

  hasActiveFilters(): boolean {
    return !!(
      this.advancedFilters.subjectType !== undefined ||
      this.advancedFilters.status !== undefined ||
      this.advancedFilters.category !== undefined ||
      this.advancedFilters.titleId !== undefined ||
      this.advancedFilters.donorCode ||
      this.advancedFilters.email ||
      this.advancedFilters.phoneNumber ||
      this.advancedFilters.city ||
      this.advancedFilters.postalCode ||
      this.advancedFilters.province ||
      this.advancedFilters.country
    );
  }

  getActiveFiltersCount(): number {
    let count = 0;
    if (this.advancedFilters.subjectType !== undefined) count++;
    if (this.advancedFilters.status !== undefined) count++;
    if (this.advancedFilters.category !== undefined) count++;
    if (this.advancedFilters.titleId !== undefined) count++;
    if (this.advancedFilters.donorCode) count++;
    if (this.advancedFilters.email) count++;
    if (this.advancedFilters.phoneNumber) count++;
    if (this.advancedFilters.city) count++;
    if (this.advancedFilters.postalCode) count++;
    if (this.advancedFilters.province) count++;
    if (this.advancedFilters.country) count++;
    return count;
  }

  openCreateModal(): void {
    this.editingDonor = null;
    this.isFormModalVisible = true;
  }

  openEditModal(donor: DonorDto): void {
    this.editingDonor = donor;
    this.isFormModalVisible = true;
  }

  closeFormModal(): void {
    this.isFormModalVisible = false;
    this.editingDonor = null;
  }

  onFormSaved(): void {
    this.closeFormModal();
    this.loadDonors();
  }

  viewDonor(donor: DonorDto): void {
    this.router.navigate(['/donors', donor.id]);
  }

  deleteDonor(donor: DonorDto): void {
    this.donorService.delete(donor.id!).subscribe({
      next: () => {
        this.message.success('Donatore eliminato');
        this.loadDonors();
      },
      error: () => {
        this.message.error('Errore nell\'eliminazione del donatore');
      }
    });
  }

  getStatusColor(status: DonorStatus | undefined, isAnonymized: boolean = false): string {
    if (isAnonymized) return 'purple';
    switch (status) {
      case DonorStatus.Active: return 'green';
      case DonorStatus.Inactive: return 'orange';
      case DonorStatus.Suspended: return 'red';
      case DonorStatus.Deceased: return 'default';
      case DonorStatus.Lapsed: return 'orange';
      case DonorStatus.Disabled: return 'red';
      default: return 'default';
    }
  }

  getStatusLabel(status: DonorStatus | undefined, isAnonymized: boolean = false): string {
    if (isAnonymized) return 'Anonimizzato';
    switch (status) {
      case DonorStatus.New: return 'Nuovo';
      case DonorStatus.Active: return 'Attivo';
      case DonorStatus.Inactive: return 'Inattivo';
      case DonorStatus.Suspended: return 'Sospeso';
      case DonorStatus.Deceased: return 'Deceduto';
      case DonorStatus.Lapsed: return 'Decaduto';
      case DonorStatus.Disabled: return 'Disabilitato';
      case DonorStatus.DoNotContact: return 'Non contattare';
      case DonorStatus.Duplicate: return 'Duplicato';
      default: return '-';
    }
  }

  getSubjectTypeLabel(type: SubjectType | undefined): string {
    return type === SubjectType.Individual ? 'Persona' : 'Organizzazione';
  }

  getSubjectTypeIcon(type: SubjectType | undefined): string {
    return type === SubjectType.Individual ? 'user' : 'bank';
  }

  // Statistics methods
  getActiveCount(): number {
    return this.donors.filter(d => d.status === DonorStatus.Active).length;
  }

  getIndividualsCount(): number {
    return this.donors.filter(d => d.subjectType === SubjectType.Individual).length;
  }

  getOrganizationsCount(): number {
    return this.donors.filter(d => d.subjectType === SubjectType.Organization).length;
  }
}
