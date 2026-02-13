import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { DonorService } from '../../proxy/donors/donor.service';
import type { DonorStatusHistoryDto } from '../../proxy/donors/dtos/models';
import { DonorDto } from '../../proxy/donors/dtos/models';
import { SubjectType } from '../../proxy/enums/donors/subject-type.enum';
import { DonorStatus } from '../../proxy/enums/donors/donor-status.enum';
import { Gender } from '../../proxy/enums/donors/gender.enum';
import { OrganizationType } from '../../proxy/enums/donors/organization-type.enum';
import { LegalForm } from '../../proxy/enums/donors/legal-form.enum';
import { DonorOrigin } from '../../proxy/enums/donors/donor-origin.enum';
import { NoteService } from '../../proxy/notes/note.service';
import type { NoteDto } from '../../proxy/notes/dto/models';
import { DonationService } from '../../proxy/donations/donation.service';
import type { DonationListDto } from '../../proxy/donations/models';
import { DonationChannel, DonationStatus as DonationStatusEnum } from '../../proxy/donations/models';

@Component({
  selector: 'app-donor-detail',
  standalone: false,
  templateUrl: './donor-detail.component.html',
  styleUrls: ['./donor-detail.component.scss']
})
export class DonorDetailComponent implements OnInit {
  donor: DonorDto | null = null;
  loading = false;
  donorId!: string;

  isEditModalVisible = false;
  isStatusModalVisible = false;
  selectedStatus: number | null = null;
  statusNote: string = '';
  statusHistory: DonorStatusHistoryDto[] = [];
  recentNotes: NoteDto[] = [];

  // Donations
  donations: DonationListDto[] = [];
  donationsLoading = false;
  donationsTotal = 0;
  donationsPageSize = 10;
  donationsPageIndex = 1;

  SubjectType = SubjectType;
  DonorStatus = DonorStatus;
  DonationChannel = DonationChannel;
  DonationStatusEnum = DonationStatusEnum;

  // Lista degli stati disponibili
  statusOptions = [
    { value: DonorStatus.New, label: 'Nuovo', color: 'blue', icon: 'plus-circle' },
    { value: DonorStatus.Active, label: 'Attivo', color: 'green', icon: 'check-circle' },
    { value: DonorStatus.Lapsed, label: 'Decaduto', color: 'orange', icon: 'clock-circle' },
    { value: DonorStatus.Inactive, label: 'Inattivo', color: 'default', icon: 'pause-circle' },
    { value: DonorStatus.Suspended, label: 'Sospeso', color: 'red', icon: 'stop' },
    { value: DonorStatus.Disabled, label: 'Disabilitato', color: 'default', icon: 'close-circle' },
    { value: DonorStatus.Deceased, label: 'Deceduto', color: 'default', icon: 'frown' },
    { value: DonorStatus.DoNotContact, label: 'Non contattare', color: 'volcano', icon: 'stop' }
  ];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private donorService: DonorService,
    private noteService: NoteService,
    private donationService: DonationService,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.donorId = this.route.snapshot.paramMap.get('id')!;
    this.loadDonor();
    this.loadDonations();
  }

  loadDonor(): void {
    this.loading = true;
    this.donorService.get(this.donorId).subscribe({
      next: (donor) => {
        this.donor = donor;
        this.loading = false;
        this.loadStatusHistory();
        this.loadRecentNotes();
      },
      error: (err) => {
        this.message.error('Errore nel caricamento del donatore');
        this.loading = false;
        console.error(err);
      }
    });
  }

  loadRecentNotes(): void {
    this.noteService.getListByDonor(this.donorId, {
      skipCount: 0,
      maxResultCount: 5,
      sorting: 'creationTime DESC'
    }).subscribe({
      next: (result) => {
        this.recentNotes = result.items || [];
      },
      error: (err) => {
        console.error('Errore nel caricamento delle note recenti', err);
        this.recentNotes = [];
      }
    });
  }

  loadDonations(): void {
    this.donationsLoading = true;
    this.donationService.getList({
      donorId: this.donorId,
      skipCount: (this.donationsPageIndex - 1) * this.donationsPageSize,
      maxResultCount: this.donationsPageSize,
      sorting: 'donationDate DESC'
    }).subscribe({
      next: (result) => {
        this.donations = result.items || [];
        this.donationsTotal = result.totalCount || 0;
        this.donationsLoading = false;
      },
      error: (err) => {
        console.error('Errore nel caricamento delle donazioni', err);
        this.donations = [];
        this.donationsLoading = false;
        this.message.error('Errore nel caricamento delle donazioni');
      }
    });
  }

  onDonationsPageChange(pageIndex: number): void {
    this.donationsPageIndex = pageIndex;
    this.loadDonations();
  }

  onDonationsPageSizeChange(pageSize: number): void {
    this.donationsPageSize = pageSize;
    this.donationsPageIndex = 1;
    this.loadDonations();
  }

  goBack(): void {
    this.router.navigate(['/donors']);
  }

  openEditModal(): void {
    this.isEditModalVisible = true;
  }

  closeEditModal(): void {
    this.isEditModalVisible = false;
  }

  onDonorUpdated(): void {
    this.closeEditModal();
    this.loadDonor();
  }

  openStatusModal(): void {
    this.selectedStatus = this.donor?.status ?? null;
    this.statusNote = '';
    this.isStatusModalVisible = true;
  }

  closeStatusModal(): void {
    this.isStatusModalVisible = false;
    this.selectedStatus = null;
    this.statusNote = '';
  }

  changeStatus(): void {
    if (this.selectedStatus === null || this.selectedStatus === this.donor?.status) {
      this.closeStatusModal();
      return;
    }

    this.donorService.changeStatus(this.donorId, this.selectedStatus, this.statusNote || undefined).subscribe({
      next: () => {
        this.message.success('Stato aggiornato');
        this.closeStatusModal();
        this.loadDonor();
        this.loadStatusHistory();
      },
      error: () => {
        this.message.error('Errore nel cambio stato');
      }
    });
  }

  loadStatusHistory(): void {
    this.donorService.getStatusHistory(this.donorId).subscribe({
      next: (history) => {
        this.statusHistory = history || [];
      },
      error: (err) => {
        console.error('Errore nel caricamento dello storico stati', err);
        this.statusHistory = [];
      }
    });
  }

  deleteDonor(): void {
    this.donorService.delete(this.donorId).subscribe({
      next: () => {
        this.message.success('Donatore eliminato');
        this.router.navigate(['/donors']);
      },
      error: () => {
        this.message.error('Errore nell\'eliminazione del donatore');
      }
    });
  }

  grantPrivacyConsent(): void {
    this.donorService.grantPrivacyConsent(this.donorId).subscribe({
      next: () => {
        this.message.success('Consenso privacy concesso');
        this.loadDonor();
      },
      error: (err) => {
        const errorMessage = err.error?.error?.message || 'Operazione non consentita';
        this.message.warning(errorMessage);
      }
    });
  }

  revokePrivacyConsent(): void {
    this.donorService.revokePrivacyConsent(this.donorId).subscribe({
      next: () => {
        this.message.success('Consenso privacy revocato');
        this.loadDonor();
      },
      error: () => this.message.error('Errore')
    });
  }

  grantNewsletterConsent(): void {
    this.donorService.grantNewsletterConsent(this.donorId).subscribe({
      next: () => {
        this.message.success('Consenso newsletter concesso');
        this.loadDonor();
      },
      error: () => this.message.error('Errore')
    });
  }

  revokeNewsletterConsent(): void {
    this.donorService.revokeNewsletterConsent(this.donorId).subscribe({
      next: () => {
        this.message.success('Consenso newsletter revocato');
        this.loadDonor();
      },
      error: () => this.message.error('Errore')
    });
  }

  grantMailConsent(): void {
    this.donorService.grantMailConsent(this.donorId).subscribe({
      next: () => {
        this.message.success('Consenso spedizioni cartacee concesso');
        this.loadDonor();
      },
      error: (err) => {
        if (err.error?.error?.code === 'Donor:043') {
          this.message.error('Impossibile abilitare: consenso privacy non attivo');
        } else {
          this.message.error('Errore');
        }
      }
    });
  }

  revokeMailConsent(): void {
    this.donorService.revokeMailConsent(this.donorId).subscribe({
      next: () => {
        this.message.success('Consenso spedizioni cartacee revocato');
        this.loadDonor();
      },
      error: () => this.message.error('Errore')
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
      default: return '-';
    }
  }

  getGenderLabel(gender: Gender | undefined): string {
    switch (gender) {
      case Gender.Male: return 'Maschio';
      case Gender.Female: return 'Femmina';
      case Gender.Other: return 'Altro';
      default: return 'Non specificato';
    }
  }

  getOrganizationTypeLabel(type: OrganizationType | undefined): string {
    switch (type) {
      case OrganizationType.Association: return 'Associazione';
      case OrganizationType.Foundation: return 'Fondazione';
      case OrganizationType.NGO: return 'ONG';
      case OrganizationType.Charity: return 'Ente Benefico';
      case OrganizationType.Religious: return 'Religioso';
      case OrganizationType.Educational: return 'Educativo';
      case OrganizationType.Healthcare: return 'Sanitario';
      case OrganizationType.Government: return 'Ente Pubblico';
      case OrganizationType.PrivateCompany: return 'Azienda Privata';
      case OrganizationType.Cooperative: return 'Cooperativa';
      case OrganizationType.SocialEnterprise: return 'Impresa Sociale';
      default: return 'Altro';
    }
  }

  getLegalFormLabel(form: LegalForm | undefined): string {
    switch (form) {
      case LegalForm.SoleProprietorship: return 'Ditta Individuale';
      case LegalForm.LLC: return 'S.R.L.';
      case LegalForm.SimplifiedLLC: return 'S.R.L.S.';
      case LegalForm.Corporation: return 'S.P.A.';
      case LegalForm.GeneralPartnership: return 'S.N.C.';
      case LegalForm.LimitedPartnership: return 'S.A.S.';
      case LegalForm.Cooperative: return 'Cooperativa';
      case LegalForm.Association: return 'Associazione';
      case LegalForm.Foundation: return 'Fondazione';
      case LegalForm.ONLUS: return 'ONLUS';
      case LegalForm.ETS: return 'ETS';
      default: return 'Altro';
    }
  }

  getOriginLabel(origin: DonorOrigin | undefined): string {
    switch (origin) {
      case DonorOrigin.Website: return 'Sito Web';
      case DonorOrigin.SocialMedia: return 'Social Media';
      case DonorOrigin.Event: return 'Evento';
      case DonorOrigin.Referral: return 'Passaparola';
      case DonorOrigin.DirectMail: return 'Posta';
      case DonorOrigin.PhoneCampaign: return 'Telefono';
      case DonorOrigin.EmailCampaign: return 'Campagna Email';
      case DonorOrigin.Media: return 'Media';
      case DonorOrigin.Other: return 'Altro';
      default: return 'Sconosciuto';
    }
  }

  formatDate(date: string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('it-IT');
  }

  formatDateTime(date: string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleString('it-IT', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatCurrency(amount: number | undefined): string {
    if (amount === undefined || amount === null) return '€ 0,00';
    return new Intl.NumberFormat('it-IT', { style: 'currency', currency: 'EUR' }).format(amount);
  }

  getCategoryLabel(category: string | undefined): string {
    if (!category) return 'Altro';
    const categories: { [key: string]: string } = {
      'phone': 'Telefonata',
      'meeting': 'Incontro',
      'email': 'Email',
      'event': 'Evento',
      'donation': 'Donazione',
      'other': 'Altro'
    };
    return categories[category] || category;
  }

  getDonationChannelLabel(channel: DonationChannel | undefined): string {
    switch (channel) {
      case DonationChannel.BankTransfer: return 'Bonifico';
      case DonationChannel.PostalOrder: return 'Bollettino';
      case DonationChannel.PostalOrderTelematic: return 'Bollettino Telematico';
      case DonationChannel.CreditCard: return 'Carta di Credito';
      case DonationChannel.DirectDebit: return 'Addebito Diretto';
      case DonationChannel.Cash: return 'Contanti';
      case DonationChannel.Check: return 'Assegno';
      case DonationChannel.PayPal: return 'PayPal';
      case DonationChannel.Stripe: return 'Stripe';
      case DonationChannel.Bequest: return 'Lascito';
      case DonationChannel.Unknown: return 'Sconosciuto';
      case DonationChannel.Other: return 'Altro';
      default: return '-';
    }
  }

  getDonationStatusLabel(status: DonationStatusEnum | undefined): string {
    switch (status) {
      case DonationStatusEnum.Pending: return 'Da Verificare';
      case DonationStatusEnum.Verified: return 'Verificata';
      case DonationStatusEnum.Rejected: return 'Rifiutata';
      case DonationStatusEnum.Suspended: return 'Sospesa';
      default: return '-';
    }
  }

  getDonationStatusColor(status: DonationStatusEnum | undefined): string {
    switch (status) {
      case DonationStatusEnum.Pending: return 'orange';
      case DonationStatusEnum.Verified: return 'green';
      case DonationStatusEnum.Rejected: return 'red';
      case DonationStatusEnum.Suspended: return 'default';
      default: return 'default';
    }
  }
}
