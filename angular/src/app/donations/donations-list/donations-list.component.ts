import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { DonationService } from '../../proxy/donations/donation.service';
import {
  DonationListDto,
  GetDonationsInput,
  DonationStatus,
  DonationChannel,
  DonationStatisticsDto,
} from '../../proxy/donations/models';
import { PagedResultDto } from '@abp/ng.core';

@Component({
  selector: 'app-donations-list',
  templateUrl: './donations-list.component.html',
  styleUrls: ['./donations-list.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    NzTableModule,
    NzButtonModule,
    NzSelectModule,
    NzDatePickerModule,
    NzTagModule,
    NzBadgeModule,
    NzCardModule,
    NzTabsModule,
    NzStatisticModule,
    NzAlertModule,
    NzDividerModule,
    NzInputNumberModule,
    NzPopconfirmModule,
    NzInputModule,
    NzGridModule,
    NzIconModule,
  ],
})
export class DonationsListComponent implements OnInit {
  donations: DonationListDto[] = [];
  loading = false;
  total = 0;
  pageSize = 10;
  pageIndex = 1;
  
  // Active tab
  activeTab = 0; // 0: All, 1: Pending, 2: Verified, 3: Rejected
  
  // Filters
  searchText = '';
  selectedChannel?: DonationChannel;
  selectedStatus?: DonationStatus;
  dateRange: Date[] = [];
  minAmount?: number;
  maxAmount?: number;
  
  // Statistics
  statistics?: DonationStatisticsDto;
  
  // Enums for template
  DonationStatus = DonationStatus;
  DonationChannel = DonationChannel;
  
  // Channel options
  channelOptions = [
    { value: DonationChannel.BankTransfer, label: 'Bonifico Bancario' },
    { value: DonationChannel.PostalOrder, label: 'Bollettino Postale' },
    { value: DonationChannel.PostalOrderTelematic, label: 'Bollettino Telematico' },
    { value: DonationChannel.PayPal, label: 'PayPal' },
    { value: DonationChannel.Cash, label: 'Contanti' },
    { value: DonationChannel.Check, label: 'Assegno' },
    { value: DonationChannel.Bequest, label: 'Lasciti' },
    { value: DonationChannel.Other, label: 'Altro' },
  ];

  constructor(
    private donationService: DonationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadDonations();
    this.loadStatistics();
  }

  loadDonations(): void {
    this.loading = true;
    
    const input: GetDonationsInput = {
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize,
      sorting: 'donationDate DESC',
    };
    
    // Apply tab filter
    if (this.activeTab === 1) {
      input.status = DonationStatus.Pending;
    } else if (this.activeTab === 2) {
      input.status = DonationStatus.Verified;
    } else if (this.activeTab === 3) {
      input.status = DonationStatus.Rejected;
    }
    
    // Apply search and filters
    if (this.searchText) {
      input.search = this.searchText;
    }
    if (this.selectedChannel !== undefined) {
      input.channel = this.selectedChannel;
    }
    if (this.selectedStatus !== undefined) {
      input.status = this.selectedStatus;
    }
    if (this.dateRange && this.dateRange.length === 2) {
      input.fromDate = this.dateRange[0].toISOString();
      input.toDate = this.dateRange[1].toISOString();
    }
    if (this.minAmount) {
      input.minAmount = this.minAmount;
    }
    if (this.maxAmount) {
      input.maxAmount = this.maxAmount;
    }
    
    this.donationService.getList(input).subscribe({
      next: (result: PagedResultDto<DonationListDto>) => {
        this.donations = result.items || [];
        this.total = result.totalCount || 0;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  loadStatistics(): void {
    const input: GetDonationsInput = {
      skipCount: 0,
      maxResultCount: 1,
    };
    
    this.donationService.getStatistics(input).subscribe({
      next: (stats) => {
        this.statistics = stats;
      },
    });
  }

  onPageChange(pageIndex: number): void {
    this.pageIndex = pageIndex;
    this.loadDonations();
  }

  onTabChange(index: number): void {
    this.activeTab = index;
    this.pageIndex = 1;
    this.loadDonations();
  }

  onSearch(): void {
    this.pageIndex = 1;
    this.loadDonations();
  }

  resetFilters(): void {
    this.searchText = '';
    this.selectedChannel = undefined;
    this.selectedStatus = undefined;
    this.dateRange = [];
    this.minAmount = undefined;
    this.maxAmount = undefined;
    this.pageIndex = 1;
    this.loadDonations();
  }

  viewDonation(id: string): void {
    this.router.navigate(['/donations', id]);
  }

  verifyDonation(id: string): void {
    this.router.navigate(['/donations/verify', id]);
  }

  createDonation(): void {
    this.router.navigate(['/donations/new']);
  }

  deleteDonation(id: string): void {
    this.donationService.delete(id).subscribe({
      next: () => {
        this.loadDonations();
        this.loadStatistics();
      },
    });
  }

  getStatusColor(status: DonationStatus): string {
    switch (status) {
      case DonationStatus.Pending:
        return 'orange';
      case DonationStatus.Verified:
        return 'green';
      case DonationStatus.Rejected:
        return 'red';
      default:
        return 'default';
    }
  }

  getStatusText(status: DonationStatus): string {
    switch (status) {
      case DonationStatus.Pending:
        return 'Da Verificare';
      case DonationStatus.Verified:
        return 'Verificata';
      case DonationStatus.Rejected:
        return 'Rifiutata';
      default:
        return 'Sconosciuto';
    }
  }

  getChannelIcon(channel: DonationChannel): string {
    switch (channel) {
      case DonationChannel.BankTransfer:
        return 'bank';
      case DonationChannel.PostalOrder:
      case DonationChannel.PostalOrderTelematic:
        return 'mail';
      case DonationChannel.PayPal:
        return 'paypal';
      case DonationChannel.Cash:
        return 'dollar';
      case DonationChannel.Check:
        return 'file-text';
      default:
        return 'question-circle';
    }
  }
}
