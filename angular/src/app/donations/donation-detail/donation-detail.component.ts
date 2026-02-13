import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { DonationService } from '../../proxy/donations/donation.service';
import { DonationDto, DonationStatus, DonationChannel, DonationListDto, RejectionReason } from '../../proxy/donations/models';

@Component({
  selector: 'app-donation-detail',
  templateUrl: './donation-detail.component.html',
  styleUrls: ['./donation-detail.component.scss'],
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    NzCardModule, 
    NzButtonModule, 
    NzDescriptionsModule, 
    NzDividerModule, 
    NzTableModule, 
    NzAlertModule, 
    NzTagModule,
    NzGridModule,
    NzIconModule,
    NzSpinModule,
    NzPageHeaderModule,
    NzListModule,
    NzEmptyModule
  ],
})
export class DonationDetailComponent implements OnInit {
  donation?: DonationDto;
  donorDonations: DonationListDto[] = [];
  loading = false;
  DonationStatus = DonationStatus;
  DonationChannel = DonationChannel;

  constructor(
    private donationService: DonationService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.loadDonation(params['id']);
    });
  }

  loadDonation(id: string): void {
    this.loading = true;
    this.donationService.get(id).subscribe({
      next: (donation) => {
        this.donation = donation;
        this.loading = false;
        // Carica altre donazioni dello stesso donatore
        this.loadDonorDonations(donation.donorId, id);
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  loadDonorDonations(donorId: string, currentDonationId: string): void {
    this.donationService.getList({
      donorId: donorId,
      skipCount: 0,
      maxResultCount: 10,
      sorting: 'donationDate DESC',
    }).subscribe({
      next: (result) => {
        // Include anche la donazione corrente per mostrarla evidenziata nello storico
        this.donorDonations = (result.items || []).slice(0, 10);
      },
    });
  }

  getTotalAllocated(): number {
    if (!this.donation?.projects) return 0;
    return this.donation.projects.reduce((sum, p) => sum + p.allocatedAmount, 0);
  }

  getChannelLabel(channel: DonationChannel): string {
    const labels: Record<DonationChannel, string> = {
      [DonationChannel.BankTransfer]: 'Bonifico Bancario',
      [DonationChannel.PostalOrder]: 'Bollettino Postale',
      [DonationChannel.PostalOrderTelematic]: 'Bollettino Telematico',
      [DonationChannel.CreditCard]: 'Carta di Credito',
      [DonationChannel.DirectDebit]: 'RID/SDD',
      [DonationChannel.Cash]: 'Contanti',
      [DonationChannel.Check]: 'Assegno',
      [DonationChannel.PayPal]: 'PayPal',
      [DonationChannel.Stripe]: 'Stripe',
      [DonationChannel.Bequest]: 'Lasciti',
      [DonationChannel.Unknown]: 'Sconosciuto',
      [DonationChannel.Other]: 'Altro',
    };
    return labels[channel] || 'N/D';
  }

  getChannelIcon(channel: DonationChannel): string {
    const icons: Record<DonationChannel, string> = {
      [DonationChannel.BankTransfer]: 'bank',
      [DonationChannel.PostalOrder]: 'mail',
      [DonationChannel.PostalOrderTelematic]: 'mobile',
      [DonationChannel.CreditCard]: 'credit-card',
      [DonationChannel.DirectDebit]: 'transaction',
      [DonationChannel.Cash]: 'dollar',
      [DonationChannel.Check]: 'file-text',
      [DonationChannel.PayPal]: 'paypal-circle',
      [DonationChannel.Stripe]: 'credit-card',
      [DonationChannel.Bequest]: 'gift',
      [DonationChannel.Unknown]: 'question-circle',
      [DonationChannel.Other]: 'more',
    };
    return icons[channel] || 'question-circle';
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

  getRejectionReasonText(reason: RejectionReason): string {
    switch (reason) {
      case RejectionReason.Duplicate:
        return 'Duplicato';
      case RejectionReason.InvalidData:
        return 'Dati non validi';
      case RejectionReason.Fraudulent:
        return 'Fraudolento';
      case RejectionReason.Cancelled:
        return 'Annullato';
      case RejectionReason.Other:
        return 'Altro';
      default:
        return 'Sconosciuto';
    }
  }

  viewDonation(id: string): void {
    this.router.navigate(['/donations', id]);
  }

  verifyDonation(): void {
    if (this.donation?.id) {
      this.router.navigate(['/donations/verify', this.donation.id]);
    }
  }

  printDonation(): void {
    window.print();
  }

  back(): void {
    this.router.navigate(['/donations']);
  }
}
