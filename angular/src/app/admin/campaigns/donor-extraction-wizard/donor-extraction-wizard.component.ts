import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CampaignService } from 'src/app/proxy/application/campaigns';
import { TagService } from 'src/app/proxy/tags';
import { ExtractDonorsInput, DonorExtractionPreviewDto } from 'src/app/proxy/application/contracts/campaigns/dto/models';
import { DonorStatus } from 'src/app/proxy/enums/donors/donor-status.enum';
import { DonorCategory } from 'src/app/proxy/enums/donors/donor-category.enum';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-donor-extraction-wizard',
  standalone: false,
  templateUrl: './donor-extraction-wizard.component.html',
  styleUrls: ['./donor-extraction-wizard.component.scss']
})
export class DonorExtractionWizardComponent implements OnInit {
  @Input() campaignId!: string;
  @Output() onComplete = new EventEmitter<void>();
  @Output() onCancel = new EventEmitter<void>();

  currentStep = 0;
  loading = false;

  // Forms for each step
  baseFiltersForm: FormGroup;
  tagsForm: FormGroup;
  geographicForm: FormGroup;
  campaignsForm: FormGroup;
  namesForm: FormGroup;

  // Data
  availableTags: any[] = [];
  availableSegments: any[] = [];
  availableCampaigns: any[] = [];
  availableRegions: string[] = [];
  availableProvinces: string[] = [];
  
  // Preview
  preview: DonorExtractionPreviewDto | null = null;
  previewLoading = false;

  // Enums
  DonorStatus = DonorStatus;
  DonorCategory = DonorCategory;
  donorStatusOptions = [
    { key: 'Attivo', value: DonorStatus.Active },
    { key: 'Inattivo', value: DonorStatus.Inactive },
    { key: 'Sospeso', value: DonorStatus.Suspended },
    { key: 'Deceduto', value: DonorStatus.Deceased }
  ];
  donorCategoryOptions = [
    { key: 'Non Classificato', value: DonorCategory.Unclassified },
    { key: 'Standard', value: DonorCategory.Standard },
    { key: 'Bronze', value: DonorCategory.Bronze },
    { key: 'Silver', value: DonorCategory.Silver },
    { key: 'Gold', value: DonorCategory.Gold },
    { key: 'Major', value: DonorCategory.Major }
  ];

  constructor(
    private fb: FormBuilder,
    private campaignService: CampaignService,
    private tagService: TagService,
    private message: NzMessageService
  ) {
    // Initialize forms
    this.baseFiltersForm = this.fb.group({
      donorStatus: [null],
      donorCategory: [null],
      requireNewsletterConsent: [false],
      requireMailConsent: [false],
      logicalOperator: ['And']
    });

    this.tagsForm = this.fb.group({
      includedTagIds: [[]],
      excludedTagIds: [[]],
      tagFilterMode: ['Any'],
      includedSegmentIds: [[]],
      excludedSegmentIds: [[]]
    });

    this.geographicForm = this.fb.group({
      includedRegions: [[]],
      excludedRegions: [[]],
      includedProvinces: [[]],
      excludedProvinces: [[]]
    });

    this.campaignsForm = this.fb.group({
      includedCampaignIds: [[]],
      excludedCampaignIds: [[]]
    });

    this.namesForm = this.fb.group({
      includedDonorNames: [[]],
      excludedDonorNames: [[]],
      maxResults: [null],
      randomSample: [false]
    });
  }

  ngOnInit(): void {
    this.loadFiltersData();
  }

  loadFiltersData(): void {
    // Load tags
    this.tagService.getList({ skipCount: 0, maxResultCount: 100 }).subscribe({
      next: (result) => {
        this.availableTags = result.items || [];
      },
      error: () => console.warn('Unable to load tags')
    });

    // Load regions and provinces (Italian regions)
    this.availableRegions = [
      'Abruzzo', 'Basilicata', 'Calabria', 'Campania', 'Emilia-Romagna',
      'Friuli-Venezia Giulia', 'Lazio', 'Liguria', 'Lombardia', 'Marche',
      'Molise', 'Piemonte', 'Puglia', 'Sardegna', 'Sicilia',
      'Toscana', 'Trentino-Alto Adige', 'Umbria', 'Valle d\'Aosta', 'Veneto'
    ];

    // Load campaigns for filters
    this.campaignService.getCampaignList({ skipCount: 0, maxResultCount: 100 }).subscribe({
      next: (result) => {
        this.availableCampaigns = result.items.filter(c => c.id !== this.campaignId) || [];
      },
      error: () => console.warn('Unable to load campaigns')
    });
  }

  // Step navigation
  nextStep(): void {
    if (this.currentStep < 5) {
      this.currentStep++;
      if (this.currentStep === 5) {
        // On preview step, load preview
        this.loadPreview();
      }
    }
  }

  prevStep(): void {
    if (this.currentStep > 0) {
      this.currentStep--;
    }
  }

  goToStep(step: number): void {
    this.currentStep = step;
    if (step === 5) {
      this.loadPreview();
    }
  }

  // Preview
  loadPreview(): void {
    this.previewLoading = true;
    const input = this.buildExtractionInput();

    this.campaignService.previewDonorExtraction(input).subscribe({
      next: (result) => {
        this.preview = result;
        this.previewLoading = false;
      },
      error: () => {
        this.message.error('Errore nel caricamento dell\'anteprima');
        this.previewLoading = false;
      }
    });
  }

  // Execute extraction
  executeExtraction(): void {
    this.loading = true;
    const input = this.buildExtractionInput();

    this.campaignService.extractDonors(this.campaignId, input).subscribe({
      next: () => {
        this.message.success('Donatori estratti con successo!');
        this.loading = false;
        this.onComplete.emit();
      },
      error: () => {
        this.message.error('Errore durante l\'estrazione dei donatori');
        this.loading = false;
      }
    });
  }

  cancel(): void {
    this.onCancel.emit();
  }

  private buildExtractionInput(): ExtractDonorsInput {
    return {
      ...this.baseFiltersForm.value,
      ...this.tagsForm.value,
      ...this.geographicForm.value,
      ...this.campaignsForm.value,
      ...this.namesForm.value,
      // Convert string arrays to proper format
      includedDonorNames: this.namesForm.value.includedDonorNames.filter((n: string) => n && n.trim()),
      excludedDonorNames: this.namesForm.value.excludedDonorNames.filter((n: string) => n && n.trim())
    };
  }

  // Helper to check if any filter is active
  hasActiveFilters(): boolean {
    return this.baseFiltersForm.value.donorStatus !== null ||
           this.baseFiltersForm.value.donorCategory !== null ||
           this.tagsForm.value.includedTagIds.length > 0 ||
           this.tagsForm.value.excludedTagIds.length > 0 ||
           this.geographicForm.value.includedRegions.length > 0 ||
           this.geographicForm.value.includedProvinces.length > 0 ||
           this.namesForm.value.includedDonorNames.length > 0;
  }
}
