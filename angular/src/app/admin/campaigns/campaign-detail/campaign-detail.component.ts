import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CampaignService } from 'src/app/proxy/application/campaigns';
import { CampaignDto } from 'src/app/proxy/application/contracts/campaigns/dto/models';
import { CampaignChannel, CampaignStatus, CampaignType } from 'src/app/proxy/enums/campaigns';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-campaign-detail',
  standalone: false,
  templateUrl: './campaign-detail.component.html',
  styleUrls: ['./campaign-detail.component.scss']
})
export class CampaignDetailComponent implements OnInit {
  campaign: CampaignDto | null = null;
  loading = false;
  campaignId: string;
  wizardVisible = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private campaignService: CampaignService,
    private message: NzMessageService
  ) {
    this.campaignId = this.route.snapshot.paramMap.get('id') || '';
  }

  ngOnInit(): void {
    this.loadCampaign();
  }

  loadCampaign(): void {
    this.loading = true;
    this.campaignService.get(this.campaignId).subscribe({
      next: (campaign) => {
        this.campaign = campaign;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.message.error('Errore nel caricamento della campagna');
        this.router.navigate(['/campaigns']);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/campaigns']);
  }

  openEditModal(): void {
    // TODO: Implement edit modal
    this.message.info('Modifica campagna - funzionalità in sviluppo');
  }

  openExtractionWizard(): void {
    this.wizardVisible = true;
  }

  closeExtractionWizard(): void {
    this.wizardVisible = false;
  }

  handleExtractionComplete(): void {
    this.wizardVisible = false;
    this.loadCampaign(); // Reload campaign to update stats
    this.message.success('Estrazione completata con successo!');
  }

  getStatusColor(status: CampaignStatus): string {
    switch (status) {
      case CampaignStatus.Draft:
        return 'default';
      case CampaignStatus.InPreparation:
        return 'processing';
      case CampaignStatus.Extracted:
        return 'blue';
      case CampaignStatus.Dispatched:
        return 'cyan';
      case CampaignStatus.Completed:
        return 'success';
      case CampaignStatus.Cancelled:
        return 'error';
      default:
        return 'default';
    }
  }

  getStatusLabel(status: CampaignStatus): string {
    switch (status) {
      case CampaignStatus.Draft:
        return 'Bozza';
      case CampaignStatus.InPreparation:
        return 'In preparazione';
      case CampaignStatus.Extracted:
        return 'Estratta';
      case CampaignStatus.Dispatched:
        return 'Spedita';
      case CampaignStatus.Completed:
        return 'Completata';
      case CampaignStatus.Cancelled:
        return 'Annullata';
      default:
        return 'Sconosciuto';
    }
  }

  getCampaignTypeLabel(type: CampaignType): string {
    switch (type) {
      case CampaignType.Prospect:
        return 'Prospect';
      case CampaignType.Archive:
        return 'Archivio';
      default:
        return 'Non specificato';
    }
  }

  getCampaignChannelLabel(channel: CampaignChannel): string {
    switch (channel) {
      case CampaignChannel.Postal:
        return 'Postale';
      case CampaignChannel.Email:
        return 'Email';
      case CampaignChannel.SMS:
        return 'SMS';
      case CampaignChannel.Phone:
        return 'Telefono';
      case CampaignChannel.Mixed:
        return 'Misto';
      default:
        return 'Non specificato';
    }
  }
}
