import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { CampaignService } from 'src/app/proxy/application/campaigns';
import { CampaignListDto, GetCampaignsInput } from 'src/app/proxy/application/contracts/campaigns/dto/models';
import {
  CampaignStatus,
  CampaignType,
  CampaignChannel,
  campaignStatusOptions,
  campaignTypeOptions,
  campaignChannelOptions
} from 'src/app/proxy/enums/campaigns';
import { PagedResultDto } from '@abp/ng.core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-campaigns-list',
  standalone: false,
  templateUrl: './campaigns-list.component.html',
  styleUrls: ['./campaigns-list.component.scss']
})
export class CampaignsListComponent implements OnInit, OnDestroy {
  campaigns: CampaignListDto[] = [];
  total = 0;
  loading = false;
  pageSize = 10;
  pageIndex = 1;
  searchText = '';

  // Filters
  filterForm: FormGroup;
  campaignStatusOptions = campaignStatusOptions;
  campaignTypeOptions = campaignTypeOptions;
  campaignChannelOptions = campaignChannelOptions;
  isAdvancedSearchVisible = false;

  // Quick Filters
  quickFilters = {
    year: new Date().getFullYear() as number | null,
    campaignType: null as CampaignType | null,
    status: null as CampaignStatus | null
  };
  availableYears: number[] = [];

  // Modal
  isModalVisible = false;
  modalTitle = '';
  selectedCampaign: CampaignListDto | null = null;

  // Subject for search debounce
  private searchSubject = new Subject<string>();

  constructor(
    private campaignService: CampaignService,
    private fb: FormBuilder,
    private router: Router,
    private modal: NzModalService,
    private message: NzMessageService
  ) {
    this.filterForm = this.fb.group({
      filter: [''],
      year: [null], // Anno ora opzionale
      status: [null],
      campaignType: [null],
      channel: [null]
    });
  }

  ngOnInit(): void {
    this.initializeAvailableYears();
    this.loadCampaigns();
    
    // Setup debounce for search
    this.searchSubject.pipe(
      debounceTime(500),
      distinctUntilChanged()
    ).subscribe(() => {
      this.pageIndex = 1;
      this.loadCampaigns();
    });
  }

  initializeAvailableYears(): void {
    const currentYear = new Date().getFullYear();
    this.availableYears = [];
    // Anno successivo
    this.availableYears.push(currentYear + 1);
    // Anno corrente e ultimi 10 anni
    for (let i = currentYear; i >= currentYear - 10; i--) {
      this.availableYears.push(i);
    }
  }

  ngOnDestroy(): void {
    this.searchSubject.complete();
  }

  loadCampaigns(): void {
    this.loading = true;
    const input: GetCampaignsInput = {
      filter: this.searchText || this.filterForm.value.filter,
      year: this.quickFilters.year || this.filterForm.value.year || undefined,
      status: this.quickFilters.status !== null ? this.quickFilters.status : this.filterForm.value.status,
      campaignType: this.quickFilters.campaignType !== null ? this.quickFilters.campaignType : this.filterForm.value.campaignType,
      channel: this.filterForm.value.channel,
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize
    };

    this.campaignService.getCampaignList(input).subscribe({
      next: (result: PagedResultDto<CampaignListDto>) => {
        this.campaigns = result.items;
        this.total = result.totalCount;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.message.error('Errore nel caricamento delle campagne');
      }
    });
  }

  onSearch(): void {
    this.searchSubject.next(this.searchText);
  }

  clearSearch(): void {
    this.searchText = '';
    this.pageIndex = 1;
    this.loadCampaigns();
  }

  openAdvancedSearch(): void {
    this.isAdvancedSearchVisible = true;
  }

  closeAdvancedSearch(): void {
    this.isAdvancedSearchVisible = false;
  }

  applyQuickFilters(): void {
    this.pageIndex = 1;
    this.loadCampaigns();
  }

  clearQuickFilters(): void {
    this.quickFilters = {
      year: new Date().getFullYear(), // Ritorna all'anno corrente
      campaignType: null,
      status: null
    };
    this.applyQuickFilters();
  }

  hasQuickFilters(): boolean {
    return this.quickFilters.year !== null || 
           this.quickFilters.campaignType !== null || 
           this.quickFilters.status !== null;
  }

  hasActiveFilters(): boolean {
    return this.hasQuickFilters() ||
           !!this.filterForm.value.year || 
           !!this.filterForm.value.status || 
           !!this.filterForm.value.campaignType || 
           !!this.filterForm.value.channel;
  }

  getActiveFiltersCount(): number {
    let count = 0;
    // Quick filters
    if (this.quickFilters.year) count++;
    if (this.quickFilters.status !== null && this.quickFilters.status !== undefined) count++;
    if (this.quickFilters.campaignType !== null && this.quickFilters.campaignType !== undefined) count++;
    // Advanced filters
    if (this.filterForm.value.year) count++;
    if (this.filterForm.value.status !== null && this.filterForm.value.status !== undefined) count++;
    if (this.filterForm.value.campaignType !== null && this.filterForm.value.campaignType !== undefined) count++;
    if (this.filterForm.value.channel !== null && this.filterForm.value.channel !== undefined) count++;
    return count;
  }

  getActiveCount(): number {
    return this.campaigns.filter(c => c.status === CampaignStatus.Dispatched || c.status === CampaignStatus.InPreparation).length;
  }

  getTotalRaised(): number {
    return this.campaigns.reduce((sum, c) => sum + (c.totalRaised || 0), 0);
  }

  getAverageROI(): number {
    if (this.campaigns.length === 0) return 0;
    const totalROI = this.campaigns.reduce((sum, c) => sum + (c.roi || 0), 0);
    return totalROI / this.campaigns.length;
  }

  onPageChange(pageIndex: number): void {
    this.pageIndex = pageIndex;
    this.loadCampaigns();
  }

  onPageSizeChange(pageSize: number): void {
    this.pageSize = pageSize;
    this.pageIndex = 1;
    this.loadCampaigns();
  }

  applyFilters(): void {
    this.pageIndex = 1;
    this.closeAdvancedSearch();
    this.loadCampaigns();
  }

  resetFilters(): void {
    this.filterForm.reset({
      filter: '',
      year: null, // Anno ora opzionale
      status: null,
      campaignType: null,
      channel: null
    });
    this.applyFilters();
  }

  openCreateModal(): void {
    this.selectedCampaign = null;
    this.modalTitle = 'Nuova Campagna';
    this.isModalVisible = true;
  }

  openEditModal(campaign: CampaignListDto): void {
    this.selectedCampaign = campaign;
    this.modalTitle = 'Modifica Campagna';
    this.isModalVisible = true;
  }

  handleModalClose(saved: boolean): void {
    this.isModalVisible = false;
    if (saved) {
      this.loadCampaigns();
    }
  }

  viewCampaign(id: string): void {
    this.router.navigate(['/campaigns', id]);
  }

  deleteCampaign(id: string): void {
    this.campaignService.delete(id).subscribe({
      next: () => {
        this.message.success('Campagna eliminata con successo');
        this.loadCampaigns();
      },
      error: () => {
        this.message.error('Errore durante l\'eliminazione della campagna');
      }
    });
  }

  getStatusColor(status: CampaignStatus): string {
    switch (status) {
      case CampaignStatus.Draft: return 'default';
      case CampaignStatus.InPreparation: return 'processing';
      case CampaignStatus.Extracted: return 'warning';
      case CampaignStatus.Dispatched: return 'processing';
      case CampaignStatus.Completed: return 'success';
      case CampaignStatus.Cancelled: return 'error';
      default: return 'default';
    }
  }

  getStatusText(status: CampaignStatus): string {
    return campaignStatusOptions.find(o => o.value === status)?.key || '';
  }

  getTypeText(type: CampaignType): string {
    return campaignTypeOptions.find(o => o.value === type)?.key || '';
  }

  getChannelText(channel: CampaignChannel): string {
    return campaignChannelOptions.find(o => o.value === channel)?.key || '';
  }

  getTypeColor(type: CampaignType): string {
    switch (type) {
      case CampaignType.Prospect: return 'blue';
      case CampaignType.Archive: return 'green';
      default: return 'default';
    }
  }

  getChannelColor(channel: CampaignChannel): string {
    switch (channel) {
      case CampaignChannel.Postal: return 'orange';
      case CampaignChannel.Email: return 'blue';
      case CampaignChannel.SMS: return 'green';
      case CampaignChannel.Phone: return 'purple';
      case CampaignChannel.Mixed: return 'magenta';
      default: return 'default';
    }
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('it-IT', { style: 'currency', currency: 'EUR' }).format(value);
  }
}
