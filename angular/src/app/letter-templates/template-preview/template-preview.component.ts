import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { LetterTemplateService } from '../../proxy/letter-templates/letter-template.service';
import { DonorService } from '../../proxy/donors/donor.service';
import { LetterTemplateDto, RenderTemplateWithDonorInput } from '../../proxy/letter-templates/dto/models';
import { DonorDto } from '../../proxy/donors/dtos/models';

@Component({
  selector: 'app-template-preview',
  standalone: false,
  templateUrl: './template-preview.component.html',
  styleUrls: ['./template-preview.component.scss'],
})
export class TemplatePreviewComponent implements OnInit {
  @Input() template: LetterTemplateDto | null = null;
  @Output() closed = new EventEmitter<void>();

  loading = false;
  renderedContent = '';
  donors: DonorDto[] = [];
  selectedDonorId?: string;
  testEmail = '';
  sendingTest = false;

  // Preview tabs
  selectedTab = 0; // 0: Raw Template, 1: Preview with Data

  constructor(
    private templateService: LetterTemplateService,
    private donorService: DonorService,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    if (this.template) {
      this.renderedContent = this.template.content;
      this.loadDonors();
    }
  }

  loadDonors(): void {
    // Load first 50 donors for preview selection
    this.donorService.getList({ maxResultCount: 50, sorting: 'creationTime desc' }).subscribe({
      next: result => {
        this.donors = result.items || [];
        if (this.donors.length > 0) {
          // Auto-select first donor
          this.selectedDonorId = this.donors[0].id;
          this.renderWithDonorData();
        }
      },
      error: err => {
        console.error('Errore nel caricamento donatori', err);
      },
    });
  }

  onDonorChange(): void {
    if (this.selectedDonorId) {
      this.renderWithDonorData();
    }
  }

  renderWithDonorData(): void {
    if (!this.template || !this.selectedDonorId) return;

    this.loading = true;
    const input: RenderTemplateWithDonorInput = {
      templateId: this.template.id,
      donorId: this.selectedDonorId,
      additionalTags: {},
    };

    this.templateService.renderTemplateWithDonorData(input).subscribe({
      next: rendered => {
        this.renderedContent = rendered;
        this.loading = false;
      },
      error: err => {
        this.message.error('Errore nel rendering del template');
        this.loading = false;
        console.error(err);
      },
    });
  }

  sendTestEmail(): void {
    if (!this.template || !this.testEmail) {
      this.message.warning('Inserisci un indirizzo email per il test');
      return;
    }

    this.sendingTest = true;
    const input = {
      templateId: this.template.id,
      testEmail: this.testEmail,
      donorId: this.selectedDonorId,
      tagValues: {},
    };

    this.templateService.sendTestEmail(input).subscribe({
      next: () => {
        this.message.success(`Email di test inviata a ${this.testEmail}`);
        this.sendingTest = false;
      },
      error: err => {
        this.message.error("Errore nell'invio dell'email di test");
        this.sendingTest = false;
        console.error(err);
      },
    });
  }

  onClose(): void {
    this.closed.emit();
  }

  getDonorDisplayName(donor: DonorDto): string {
    // Use fullName if available, otherwise construct from firstName/lastName or companyName
    if (donor.fullName) {
      return donor.fullName;
    }
    if (donor.firstName || donor.lastName) {
      return `${donor.firstName || ''} ${donor.lastName || ''}`.trim();
    }
    return donor.companyName || 'N/A';
  }
}
