import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzAutocompleteModule } from 'ng-zorro-antd/auto-complete';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzSliderModule } from 'ng-zorro-antd/slider';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { EditorModule, TINYMCE_SCRIPT_SRC } from '@tinymce/tinymce-angular';
import { DonationService } from '../../proxy/donations/donation.service';
import { DonationDto, RejectionReason, DonationChannel, DonationStatus } from '../../proxy/donations/models';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { DonorService } from '../../proxy/donors/donor.service';
import { CampaignService } from '../../proxy/application/campaigns/campaign.service';
import { ProjectService } from '../../proxy/application/projects/project.service';
import { LetterTemplateService } from '../../proxy/letter-templates/letter-template.service';
import { BankAccountService } from '../../proxy/bank-accounts/bank-account.service';
import { NoteService } from '../../proxy/notes/note.service';
import { debounceTime } from 'rxjs';

@Component({
  selector: 'app-donation-detail',
  templateUrl: './donation-detail.component.html',
  styleUrls: ['./donation-detail.component.scss'],
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule,
    FormsModule,
    RouterModule, 
    NzFormModule, 
    NzInputModule, 
    NzButtonModule, 
    NzSelectModule, 
    NzCardModule, 
    NzModalModule, 
    NzDescriptionsModule, 
    NzDividerModule, 
    NzSpaceModule, 
    NzGridModule, 
    NzIconModule,
    NzTableModule,
    NzInputNumberModule,
    NzAutocompleteModule,
    NzPageHeaderModule,
    NzStepsModule,
    NzTagModule,
    NzAlertModule,
    NzSliderModule,
    NzCollapseModule,
    NzCheckboxModule,
    NzDatePickerModule,
    NzEmptyModule,
    NzDrawerModule,
    EditorModule
  ],
  providers: [
    { provide: TINYMCE_SCRIPT_SRC, useValue: 'tinymce/tinymce.min.js' }
  ]
})
export class DonationDetailComponent implements OnInit {
  // Unified component for viewing, creating, editing, and verifying donations
  donation?: DonationDto;
  verifyForm!: FormGroup;
  loading = false;
  donationId?: string;
  mode: 'create' | 'verify' | 'edit' | 'view' = 'create';
  DonationChannel = DonationChannel;
  
  // Donor autocomplete
  donorSearchControl = new FormControl('');
  donorOptions: any[] = [];
  donorSearchLoading = false;
  selectedDonor: any = null;
  
  // Campaigns, Projects, Templates, BankAccounts
  campaigns: any[] = [];
  selectedCampaign: any = null;
  projects: any[] = [];
  letterTemplates: any[] = [];
  bankAccounts: any[] = [];
  
  // Donor donations history
  donorDonations: any[] = [];
  
  // Donor notes
  donorNotes: any[] = [];
  
  // UI State
  // selectedHeaderStyle: string = ''; // TODO: Gestire da cruscotto impostazioni
  printEnabled: boolean = false;
  
  // CREATE mode fields
  createChannel: DonationChannel = DonationChannel.BankTransfer;
  createTotalAmount: number = 50;
  createDonationDate: Date = new Date();
  createCreditDate?: Date;
  currentStatus: DonationStatus = DonationStatus.Pending;
  
  // Computed property for scanned document
  get scannedDocumentUrl(): string | null {
    // In futuro, questo sarà caricato dal backend
    // Per ora ritorna null, mostrando il placeholder
    return null;
  }

  // Example document URL
  exampleDocumentUrl: string = 'assets/bollettino-esempio.png';

  // Slider marks
  sliderMarks: { [key: number]: string } = {
    0: '0 €',
    1000: 'Max'
  };
  
  // Project allocations
  projectAllocations: Array<{projectId: string; projectName?: string; allocatedAmount: number; editing?: boolean}> = [];
  availableProjects: any[] = [];
  newProjectId: string | null = null;
  newProjectAmount: number = 0;
  
  // ngModel standalone option
  ngModelOptions = { standalone: true };
  
  // TinyMCE Editor
  letterContent: string = '';
  selectedTemplateId: string | null = null;
  
  // TinyMCE Configuration - Self-hosted (GPL License)
  get editorConfig() {
    return {
      base_url: '/tinymce',
      suffix: '.min',
      promotion: false, // Disable upgrade prompts for self-hosted version
      height: 500,
      menubar: false,
      plugins: [
        'advlist', 'autolink', 'lists', 'link', 'image', 'charmap',
        'searchreplace', 'visualblocks', 'code', 'fullscreen',
        'insertdatetime', 'table', 'help', 'wordcount', 'print'
      ],
      toolbar: this.mode === 'view' ? false : 'undo redo | blocks fontsize | bold italic underline forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist | table link | print | removeformat fullscreen',
      content_style: 'body { font-family:Georgia,serif; font-size:14px; line-height:1.8; padding: 20px; }',
      language: 'it',
      toolbar_mode: 'sliding',
      branding: false,
      readonly: this.mode === 'view'
    };
  }
  
  // Rejection
  isRejectModalVisible = false;
  rejectForm!: FormGroup;
  
  rejectionReasonOptions = [
    { value: RejectionReason.Duplicate, label: 'Duplicato' },
    { value: RejectionReason.InvalidData, label: 'Dati Errati' },
    { value: RejectionReason.Fraudulent, label: 'Fraudolenta' },
    { value: RejectionReason.Cancelled, label: 'Annullata' },
    { value: RejectionReason.Other, label: 'Altro' },
  ];

  // Modals visibility
  isDonorSearchVisible = false;
  isCampaignModalVisible = false;
  isProjectModalVisible = false;
  isFilterDrawerVisible = false;

  // Navigation between filtered donations
  filteredDonationIds: string[] = [];
  currentIndex: number = 0;
  totalFiltered: number = 0;
  
  // Quick filters for navigation
  quickFilters = {
    status: null as DonationStatus | null,
    channel: null as DonationChannel | null,
    minAmount: null as number | null,
    maxAmount: null as number | null,
    dateFrom: null as Date | null,
    dateTo: null as Date | null
  };

  constructor(
    private fb: FormBuilder,
    private donationService: DonationService,
    private donorService: DonorService,
    private campaignService: CampaignService,
    private projectService: ProjectService,
    private letterTemplateService: LetterTemplateService,
    private bankAccountService: BankAccountService,
    private noteService: NoteService,
    private router: Router,
    private route: ActivatedRoute,
    private message: NzMessageService,
    private modal: NzModalService
  ) {}

  ngOnInit(): void {
    this.initForms();
    this.loadReferenceData();
    
    // Donor autocomplete
    this.donorSearchControl.valueChanges.pipe(
      debounceTime(300)
    ).subscribe(value => {
      if (value && value.length >= 2) {
        this.searchDonors(value);
      } else {
        this.donorOptions = [];
      }
    });
    
    this.route.params.subscribe(params => {
      this.donationId = params['id'];
      
      // Determine mode based on route path
      const urlSegments = this.route.snapshot.url;
      const isEditPath = urlSegments.some(segment => segment.path === 'edit');
      
      // Determine mode
      if (!this.donationId) {
        // No ID = CREATE mode
        this.mode = 'create';
        this.resetFormData();
      } else {
        // Reset data before loading new donation
        this.resetFormData();
        
        // Determine mode: edit or auto-detect based on status
        if (isEditPath) {
          this.mode = 'edit';
        } else {
          // Will be determined by donation status in loadDonation()
          this.mode = 'view'; // Default, will be updated
        }
        
        // Load donation and auto-determine mode based on status
        this.loadDonation();
        // Load filtered donations for navigation
        this.loadFilteredDonations();
      }
    });
  }

  initForms(): void {
    this.verifyForm = this.fb.group({
      donorId: ['', [Validators.required]],
      campaignId: [null],
      bankAccountId: [null],
      thankYouTemplateId: [null],
      notes: [''],
      internalNotes: [''],
    });

    this.rejectForm = this.fb.group({
      reason: [null, [Validators.required]],
      notes: [''],
    });
  }

  resetFormData(): void {
    // Reset donation data
    this.donation = undefined;
    
    // Reset form
    this.verifyForm.reset();
    
    // Reset donor
    this.donorSearchControl.setValue('', { emitEvent: false });
    this.donorOptions = [];
    this.selectedDonor = null;
    
    // Reset campaign
    this.selectedCampaign = null;
    
    // Reset project allocations
    this.projectAllocations = [];
    this.newProjectId = null;
    this.newProjectAmount = 0;
    
    // Reset donor donations history
    this.donorDonations = [];
    
    // Reset donor notes
    this.donorNotes = [];
    
    // Reset letter content
    this.letterContent = '';
    this.selectedTemplateId = null;
  }

  loadReferenceData(): void {
    // Load campaigns
    this.campaignService.getList({ skipCount: 0, maxResultCount: 100 }).subscribe({
      next: (result) => {
        this.campaigns = result.items || [];
      }
    });

    // Load projects
    this.projectService.getList({ skipCount: 0, maxResultCount: 100 }).subscribe({
      next: (result) => {
        this.projects = result.items || [];
        this.availableProjects = [...this.projects];
      }
    });

    // Load letter templates
    this.letterTemplateService.getList({ skipCount: 0, maxResultCount: 100 }).subscribe({
      next: (result) => {
        this.letterTemplates = result.items || [];
      }
    });

    // Load bank accounts
    this.bankAccountService.getList({ skipCount: 0, maxResultCount: 100 }).subscribe({
      next: (result) => {
        this.bankAccounts = result.items || [];
      }
    });
  }

  searchDonors(searchTerm: string): void {
    this.donorSearchLoading = true;
    this.donorService.getList({
      filter: searchTerm,
      skipCount: 0,
      maxResultCount: 10
    }).subscribe({
      next: (result) => {
        this.donorOptions = result.items || [];
        this.donorSearchLoading = false;
      },
      error: () => {
        this.donorSearchLoading = false;
      }
    });
  }

  onDonorSelect(donorId: string): void {
    this.verifyForm.patchValue({ donorId });
    this.loadDonorDetails(donorId);
    this.isDonorSearchVisible = false;
  }

  clearDonor(): void {
    this.verifyForm.patchValue({ donorId: null });
    this.donorSearchControl.setValue('');
    this.donorOptions = [];
    this.selectedDonor = null;
  }

  loadDonorDetails(donorId: string): void {
    if (!donorId) return;
    
    this.donorService.get(donorId).subscribe({
      next: (donor) => {
        this.selectedDonor = donor;
        this.donorSearchControl.setValue(donor.fullName || '', { emitEvent: false });
        
        // Load donor's donations and notes
        this.loadDonorDonations(donorId);
        this.loadDonorNotes(donorId);
        
        // Re-render template if one is selected to update with full donor data
        if (this.selectedTemplateId && this.donation) {
          this.letterTemplateService.get(this.selectedTemplateId).subscribe({
            next: (template) => {
              this.letterContent = this.renderTemplate(template.content || '', this.donation);
            }
          });
        }
      },
      error: () => {
        this.message.error('Errore caricamento dati donatore');
      }
    });
  }

  loadDonation(): void {
    if (!this.donationId) return;
    
    this.loading = true;
    this.donationService.get(this.donationId).subscribe({
      next: (donation) => {
        this.donation = donation;
        
        // Auto-determine mode based on donation status (only if not in edit mode)
        if (this.mode !== 'edit') {
          if (donation.status === DonationStatus.Pending) {
            this.mode = 'verify';
          } else {
            this.mode = 'view';
          }
        }
        
        // Update create fields with donation values for display
        this.createTotalAmount = donation.totalAmount;
        this.createDonationDate = new Date(donation.donationDate);
        if (donation.creditDate) {
          this.createCreditDate = new Date(donation.creditDate);
        }
        this.createChannel = donation.channel;
        this.currentStatus = donation.status;
        
        this.verifyForm.patchValue({
          donorId: donation.donorId,
          campaignId: donation.campaignId,
          bankAccountId: donation.bankAccountId,
          thankYouTemplateId: donation.thankYouTemplateId,
          notes: donation.notes,
          internalNotes: donation.internalNotes,
        });

        // Load existing project allocations
        if (donation.projects && donation.projects.length > 0) {
          this.projectAllocations = donation.projects.map(p => ({
            projectId: p.projectId,
            projectName: p.projectName,
            allocatedAmount: p.allocatedAmount
          }));
        }

        // Load selected campaign if present
        if (donation.campaignId) {
          const campaign = this.campaigns.find(c => c.id === donation.campaignId);
          if (campaign) {
            this.selectedCampaign = campaign;
          }
        }

        // Load donor details if present
        if (donation.donorId) {
          this.loadDonorDetails(donation.donorId);
        }

        // Load letter content if template is present (for both verify and view modes)
        if (donation.thankYouTemplateId) {
          this.selectedTemplateId = donation.thankYouTemplateId;
          // Load template and render with donation data
          this.letterTemplateService.get(donation.thankYouTemplateId).subscribe({
            next: (template) => {
              this.letterContent = this.renderTemplate(template.content || '', donation);
            },
            error: () => {
              console.error('Error loading letter template');
            }
          });
        }

        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.message.error('Errore caricamento donazione');
      },
    });
  }

  // Project Allocation Management
  addProjectAllocation(): void {
    if (!this.newProjectId || this.newProjectAmount <= 0) {
      this.message.warning('Seleziona un progetto e inserisci un importo valido');
      return;
    }

    const project = this.projects.find(p => p.id === this.newProjectId);
    if (!project) return;

    // Check if already allocated
    if (this.projectAllocations.some(pa => pa.projectId === this.newProjectId)) {
      this.message.warning('Progetto già allocato');
      return;
    }

    this.projectAllocations = [
      ...this.projectAllocations,
      {
        projectId: this.newProjectId,
        projectName: project.name,
        allocatedAmount: this.newProjectAmount
      }
    ];

    // Reset
    this.newProjectId = null;
    this.newProjectAmount = 0;
  }

  removeProjectAllocation(projectId: string): void {
    this.projectAllocations = this.projectAllocations.filter(pa => pa.projectId !== projectId);
  }

  getTotalAllocated(): number {
    return this.projectAllocations.reduce((sum, pa) => sum + pa.allocatedAmount, 0);
  }

  getRemainingAmount(): number {
    const totalAmount = this.mode === 'create' ? this.createTotalAmount : (this.donation?.totalAmount || 0);
    return totalAmount - this.getTotalAllocated();
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

  getChannelShort(channel: DonationChannel): string {
    const shorts: Record<DonationChannel, string> = {
      [DonationChannel.BankTransfer]: 'Bonif',
      [DonationChannel.PostalOrder]: 'Boll',
      [DonationChannel.PostalOrderTelematic]: 'Boll-T',
      [DonationChannel.CreditCard]: 'CC',
      [DonationChannel.DirectDebit]: 'RID',
      [DonationChannel.Cash]: 'Cash',
      [DonationChannel.Check]: 'Asseg',
      [DonationChannel.PayPal]: 'PayPal',
      [DonationChannel.Stripe]: 'Stripe',
      [DonationChannel.Bequest]: 'Lascit',
      [DonationChannel.Unknown]: 'N/D',
      [DonationChannel.Other]: 'Altro',
    };
    return shorts[channel] || 'N/D';
  }

  loadDonorDonations(donorId: string): void {
    this.donationService.getList({
      donorId: donorId,
      skipCount: 0,
      maxResultCount: 10,
      sorting: 'donationDate DESC'
    }).subscribe({
      next: (result) => {
        this.donorDonations = result.items || [];
      }
    });
  }

  loadDonorNotes(donorId: string): void {
    this.noteService.getListByDonor(donorId, {
      skipCount: 0,
      maxResultCount: 10,
      sorting: 'isImportant DESC, creationTime DESC' // Important notes first
    }).subscribe({
      next: (result) => {
        this.donorNotes = result.items || [];
      },
      error: () => {
        this.donorNotes = [];
      }
    });
  }

  getDocumentText(): string {
    if (!this.donation) return '';

    // Generate ASCII-style document text similar to the screenshot
    let text = '';
    
    // Header
    text += '┌────────────────────────────────────────────────────────┐\n';
    text += '│ ' + this.getChannelLabel(this.donation.channel).padEnd(54) + ' │\n';
    text += '├────────────────────────────────────────────────────────┤\n';
    
    // Reference and Date
    text += '│ Riferimento: ' + (this.donation.reference || '').padEnd(41) + ' │\n';
    text += '│ Data: ' + new Date(this.donation.donationDate).toLocaleDateString('it-IT').padEnd(49) + ' │\n';
    
    if (this.donation.creditDate) {
      text += '│ Data Accredito: ' + new Date(this.donation.creditDate).toLocaleDateString('it-IT').padEnd(39) + ' │\n';
    }
    
    text += '├────────────────────────────────────────────────────────┤\n';
    
    // Amount
    const amount = this.donation.totalAmount.toFixed(2) + ' EUR';
    text += '│ IMPORTO: ' + amount.padStart(46) + ' │\n';
    text += '├────────────────────────────────────────────────────────┤\n';
    
    // Donor
    text += '│ ESEGUITO DA:                                           │\n';
    text += '│ ' + (this.donation.donorFullName || '').padEnd(54) + ' │\n';
    
    if (this.donation.externalId) {
      text += '│ ID Esterno: ' + this.donation.externalId.substring(0, 41).padEnd(41) + ' │\n';
    }
    
    text += '├────────────────────────────────────────────────────────┤\n';
    
    // Campaign
    text += '│ CAUSALE/CAMPAGNA:                                      │\n';
    text += '│ ' + (this.donation.campaignName || 'Donazione libera').padEnd(54) + ' │\n';
    
    if (this.donation.notes) {
      const notes = this.donation.notes.substring(0, 54);
      text += '│ ' + notes.padEnd(54) + ' │\n';
    }
    
    text += '├────────────────────────────────────────────────────────┤\n';
    
    // Projects
    if (this.projectAllocations.length > 0) {
      text += '│ ALLOCAZIONE PROGETTI:                                  │\n';
      this.projectAllocations.forEach(pa => {
        const projName = (pa.projectName || '').substring(0, 35);
        const projAmount = pa.allocatedAmount.toFixed(2) + ' EUR';
        text += '│ • ' + projName.padEnd(37) + ' ' + projAmount.padStart(14) + ' │\n';
      });
      text += '├────────────────────────────────────────────────────────┤\n';
      const total = this.getTotalAllocated().toFixed(2) + ' EUR';
      text += '│ TOTALE ALLOCATO: ' + total.padStart(39) + ' │\n';
    }
    
    text += '└────────────────────────────────────────────────────────┘\n';
    
    return text;
  }

  verify(): void {
    if (this.verifyForm.invalid) {
      Object.values(this.verifyForm.controls).forEach(control => {
        control.markAsDirty();
        control.updateValueAndValidity();
      });
      return;
    }

    if (this.projectAllocations.length === 0) {
      this.message.warning('Alloca la donazione ad almeno un progetto');
      return;
    }

    const remaining = this.getRemainingAmount();
    if (remaining !== 0) {
      this.message.warning(`Importo rimanente da allocare: ${remaining.toFixed(2)} EUR`);
      return;
    }

    this.loading = true;
    const dto = {
      ...this.verifyForm.value,
      projectAllocations: this.projectAllocations.map(pa => ({
        projectId: pa.projectId,
        allocatedAmount: pa.allocatedAmount
      }))
    };

    if (this.mode === 'create') {
      // CREATE mode - create new donation
      const createDto = {
        ...dto,
        channel: this.createChannel,
        totalAmount: this.createTotalAmount,
        donationDate: this.createDonationDate.toISOString(),
        creditDate: this.createCreditDate ? this.createCreditDate.toISOString() : undefined,
        status: DonationStatus.Verified, // Create as verified
      };
      
      this.donationService.create(createDto).subscribe({
        next: (created) => {
          this.message.success('Donazione creata con successo');
          this.router.navigate(['/donations', created.id]);
        },
        error: () => {
          this.loading = false;
          this.message.error('Errore creazione donazione');
        },
      });
    } else {
      // VERIFY or EDIT mode - use verify endpoint
      this.donationService.verify(this.donationId!, dto).subscribe({
        next: () => {
          const successMsg = this.mode === 'verify' ? 'Donazione verificata con successo' : 'Donazione aggiornata con successo';
          this.message.success(successMsg);
          this.router.navigate(['/donations', this.donationId]);
        },
        error: () => {
          this.loading = false;
          const errorMsg = this.mode === 'verify' ? 'Errore verifica donazione' : 'Errore aggiornamento donazione';
          this.message.error(errorMsg);
        },
      });
    }
  }

  showRejectModal(): void {
    this.isRejectModalVisible = true;
  }

  handleRejectOk(): void {
    if (this.rejectForm.invalid) {
      Object.values(this.rejectForm.controls).forEach(control => {
        control.markAsDirty();
        control.updateValueAndValidity();
      });
      return;
    }

    this.loading = true;
    this.donationService.reject(this.donationId, this.rejectForm.value).subscribe({
      next: () => {
        this.message.success('Donazione rifiutata');
        this.isRejectModalVisible = false;
        this.router.navigate(['/donations']);
      },
      error: () => {
        this.loading = false;
        this.message.error('Errore rifiuto donazione');
      },
    });
  }

  handleRejectCancel(): void {
    this.isRejectModalVisible = false;
    this.rejectForm.reset();
  }

  cancel(): void {
    this.router.navigate(['/donations']);
  }

  // Formatter/Parser for nz-input-number
  formatterEuro = (value: number) => `€ ${value}`;
  parserEuro = (value: string) => value.replace('€ ', '');
  
  // TinyMCE: Load template content when template is selected
  onTemplateChange(templateId: string | null): void {
    if (!templateId || !this.donation) {
      this.letterContent = '';
      this.selectedTemplateId = null;
      return;
    }

    this.selectedTemplateId = templateId;
    
    // Ensure donor details are loaded before rendering
    if (this.donation.donorId && !this.selectedDonor) {
      this.loadDonorDetails(this.donation.donorId);
    }
    
    // Load template and render with donation data
    this.letterTemplateService.get(templateId).subscribe({
      next: (template) => {
        // Generate letter content from template with all dynamic tags
        this.letterContent = this.renderTemplate(template.content || '', this.donation);
      },
      error: () => {
        this.message.error('Errore caricamento template');
      }
    });
  }

  // Complete template rendering with all dynamic tags
  private renderTemplate(templateContent: string, donation: DonationDto): string {
    if (!donation) return templateContent;

    let content = templateContent;
    
    // ==== DATI DONATORE ====
    const donor = this.selectedDonor;
    content = content.replace(/\{\{DonorName\}\}/g, donation.donorFullName || donor?.fullName || '');
    content = content.replace(/\{\{DonorTitle\}\}/g, donor?.titleName || '');
    content = content.replace(/\{\{DonorFirstName\}\}/g, donor?.firstName || '');
    content = content.replace(/\{\{DonorLastName\}\}/g, donor?.lastName || '');
    content = content.replace(/\{\{DonorCompanyName\}\}/g, donor?.companyName || '');
    content = content.replace(/\{\{DonorEmail\}\}/g, donor?.email || '');
    
    // Indirizzo (usa l'indirizzo di default o il primo disponibile)
    const defaultAddress = donor?.addresses?.find((a: any) => a.isDefault) || donor?.addresses?.[0];
    content = content.replace(/\{\{DonorFullAddress\}\}/g, this.formatFullAddress(defaultAddress));
    content = content.replace(/\{\{DonorStreet\}\}/g, defaultAddress?.street || '');
    content = content.replace(/\{\{DonorCity\}\}/g, defaultAddress?.city || '');
    content = content.replace(/\{\{DonorPostalCode\}\}/g, defaultAddress?.postalCode || '');
    
    // ==== DATI DONAZIONE ====
    content = content.replace(/\{\{DonationAmount\}\}/g, donation.totalAmount.toFixed(2) + ' €');
    content = content.replace(/\{\{DonationAmountInWords\}\}/g, this.numberToWords(donation.totalAmount));
    content = content.replace(/\{\{DonationDate\}\}/g, new Date(donation.donationDate).toLocaleDateString('it-IT'));
    content = content.replace(/\{\{DonationReference\}\}/g, donation.reference || '');
    
    // ==== DATI PROGETTO ====
    // Se ci sono progetti allocati, usa il primo (o potremmo fare una lista)
    const firstProject = this.projectAllocations?.[0];
    const project = firstProject ? this.projects.find(p => p.id === firstProject.projectId) : null;
    content = content.replace(/\{\{ProjectName\}\}/g, firstProject?.projectName || project?.name || '');
    content = content.replace(/\{\{ProjectCode\}\}/g, project?.code || '');
    content = content.replace(/\{\{ProjectDescription\}\}/g, project?.description || '');
    
    // ==== DATI RICORRENZA ====
    // Per ora lasciamo vuoto - andrà gestito quando sarà associata una ricorrenza
    content = content.replace(/\{\{RecurrenceName\}\}/g, '');
    content = content.replace(/\{\{RecurrenceDate\}\}/g, '');
    content = content.replace(/\{\{RecurrenceYear\}\}/g, '');
    
    // ==== ALTRI TAG ====
    content = content.replace(/\{\{CurrentDate\}\}/g, new Date().toLocaleDateString('it-IT'));
    content = content.replace(/\{\{CurrentYear\}\}/g, new Date().getFullYear().toString());
    content = content.replace(/\{\{OrganizationName\}\}/g, ''); // TODO: Gestire da impostazioni/cruscotto
    
    // Carta intestata e firma (da gestire in futuro da impostazioni)
    content = content.replace(/\{\{Letterhead\}\}/g, '');
    content = content.replace(/\{\{CarataIntestata\}\}/g, '');
    content = content.replace(/\{\{Signature\}\}/g, '');
    content = content.replace(/\{\{Firma\}\}/g, '');
    
    // Legacy support - single braces (backward compatibility)
    content = content.replace(/\{DonorName\}/g, donation.donorFullName || '');
    content = content.replace(/\{Amount\}/g, donation.totalAmount.toFixed(2) + ' EUR');
    content = content.replace(/\{Date\}/g, new Date(donation.donationDate).toLocaleDateString('it-IT'));
    content = content.replace(/\{Reference\}/g, donation.reference || '');
    
    // Projects list
    if (donation.projects && donation.projects.length > 0) {
      const projectsList = donation.projects
        .map(p => `<li><strong>${p.projectName}</strong> (${p.allocatedAmount.toFixed(2)} €)</li>`)
        .join('');
      content = content.replace(/\{ProjectsList\}/g, `<ul>${projectsList}</ul>`);
    } else {
      content = content.replace(/\{ProjectsList\}/g, '');
    }

    return content;
  }

  // Helper: Format full address
  private formatFullAddress(address: any): string {
    if (!address) return '';
    const parts = [];
    if (address.street) parts.push(address.street);
    if (address.postalCode || address.city) {
      parts.push(`${address.postalCode || ''} ${address.city || ''}`.trim());
    }
    if (address.province) parts.push(`(${address.province})`);
    return parts.join(', ');
  }

  // Helper: Convert number to words (Italian)
  private numberToWords(num: number): string {
    // Simplified implementation - in production use a proper library
    const units = ['', 'uno', 'due', 'tre', 'quattro', 'cinque', 'sei', 'sette', 'otto', 'nove'];
    const teens = ['dieci', 'undici', 'dodici', 'tredici', 'quattordici', 'quindici', 'sedici', 'diciassette', 'diciotto', 'diciannove'];
    const tens = ['', '', 'venti', 'trenta', 'quaranta', 'cinquanta', 'sessanta', 'settanta', 'ottanta', 'novanta'];
    
    if (num === 0) return 'zero';
    if (num < 10) return units[num];
    if (num >= 10 && num < 20) return teens[num - 10];
    if (num >= 20 && num < 100) {
      const ten = Math.floor(num / 10);
      const unit = num % 10;
      return tens[ten] + (unit > 0 ? units[unit] : '');
    }
    
    // For larger numbers, return formatted number
    return num.toFixed(2);
  }

  // Open image zoom modal
  openImageZoom(): void {
    const imageUrl = this.scannedDocumentUrl || this.exampleDocumentUrl;
    const isExample = !this.scannedDocumentUrl;
    
    this.modal.create({
      nzTitle: isExample ? 'Documento di Esempio' : 'Documento Scansionato',
      nzContent: `
        <div style="
          display: flex; 
          align-items: center; 
          justify-content: center; 
          background: #f5f5f5; 
          width: 100%;
          height: calc(90vh - 110px);
          overflow: hidden;">
          <img src="${imageUrl}" 
               alt="Documento" 
               style="
                 width: 100%; 
                 height: 100%; 
                 object-fit: contain;
                 border: 1px solid #d9d9d9;
                 box-shadow: 0 4px 12px rgba(0,0,0,0.15);" />
        </div>
      `,
      nzWidth: '95vw',
      nzStyle: { 
        top: '20px'
      },
      nzBodyStyle: {
        padding: '0',
        overflow: 'hidden'
      },
      nzFooter: null,
      nzClosable: true,
      nzMaskClosable: true
    });
  }

  // Show donor search modal
  showDonorSearch(): void {
    this.isDonorSearchVisible = true;
    this.donorSearchControl.setValue('');
    this.donorOptions = [];
  }

  // Show campaign selection modal
  showCampaignModal(): void {
    this.isCampaignModalVisible = true;
  }

  // Handle campaign modal OK
  handleCampaignOk(): void {
    const campaignId = this.verifyForm.get('campaignId')?.value;
    if (!campaignId) {
      this.message.warning('Seleziona una campagna');
      return;
    }

    const campaign = this.campaigns.find(c => c.id === campaignId);
    if (campaign) {
      this.selectedCampaign = campaign;
    }

    this.isCampaignModalVisible = false;
  }

  // Handle campaign modal cancel
  handleCampaignCancel(): void {
    this.isCampaignModalVisible = false;
  }

  // Remove campaign
  removeCampaign(): void {
    this.selectedCampaign = null;
    this.verifyForm.patchValue({ campaignId: null });
  }

  // Show project allocation modal
  showProjectModal(): void {
    this.isProjectModalVisible = true;
    this.newProjectId = null;
    this.newProjectAmount = 0;
  }

  // Handle project modal cancel
  handleProjectCancel(): void {
    this.isProjectModalVisible = false;
    this.newProjectId = null;
    this.newProjectAmount = 0;
  }

  // Handle project modal OK
  handleProjectOk(): void {
    if (!this.newProjectId || this.newProjectAmount <= 0) {
      this.message.warning('Seleziona un progetto e inserisci un importo valido');
      return;
    }

    const remaining = this.getRemainingAmount();
    if (this.newProjectAmount > remaining) {
      this.message.warning(`L'importo massimo disponibile è ${remaining.toFixed(2)} €`);
      return;
    }

    this.addProjectAllocation();
    this.isProjectModalVisible = false;
    this.newProjectId = null;
    this.newProjectAmount = 0;
  }

  // Navigation between filtered donations
  loadFilteredDonations(): void {
    // Load list of donation IDs based on filters from the list page or quick filters
    const filters: any = {
      status: this.quickFilters.status,
      channel: this.quickFilters.channel,
      minAmount: this.quickFilters.minAmount,
      maxAmount: this.quickFilters.maxAmount,
      fromDate: this.quickFilters.dateFrom?.toISOString(),
      toDate: this.quickFilters.dateTo?.toISOString(),
      maxResultCount: 1000, // Load all filtered IDs for navigation
      skipCount: 0
    };

    this.donationService.getList(filters).subscribe({
      next: (result) => {
        this.filteredDonationIds = result.items?.map((d: any) => d.id) || [];
        this.totalFiltered = result.totalCount || 0;
        
        // Find current position
        if (this.donationId) {
          this.currentIndex = this.filteredDonationIds.indexOf(this.donationId);
          if (this.currentIndex === -1) {
            this.currentIndex = 0;
          }
        }
      },
      error: (err) => {
        console.error('Error loading filtered donations:', err);
        this.message.error('Errore caricamento lista filtrata');
      }
    });
  }

  goToFirst(): void {
    if (this.filteredDonationIds.length > 0) {
      const firstId = this.filteredDonationIds[0];
      
      if (this.mode === 'create') {
        return; // Cannot navigate in create mode
      } else if (this.mode === 'edit') {
        this.router.navigate(['/donations', firstId, 'edit'], {
          queryParamsHandling: 'preserve'
        });
      } else {
        // Navigate to detail page (will auto-detect verify/view based on status)
        this.router.navigate(['/donations', firstId], {
          queryParamsHandling: 'preserve'
        });
      }
    }
  }

  goToPrevious(): void {
    if (this.currentIndex > 0) {
      const prevId = this.filteredDonationIds[this.currentIndex - 1];
      
      if (this.mode === 'create') {
        return; // Cannot navigate in create mode
      } else if (this.mode === 'edit') {
        this.router.navigate(['/donations', prevId, 'edit'], {
          queryParamsHandling: 'preserve'
        });
      } else {
        // Navigate to detail page (will auto-detect verify/view based on status)
        this.router.navigate(['/donations', prevId], {
          queryParamsHandling: 'preserve'
        });
      }
    }
  }

  goToNext(): void {
    if (this.currentIndex < this.filteredDonationIds.length - 1) {
      const nextId = this.filteredDonationIds[this.currentIndex + 1];
      
      if (this.mode === 'create') {
        return; // Cannot navigate in create mode
      } else if (this.mode === 'edit') {
        this.router.navigate(['/donations', nextId, 'edit'], {
          queryParamsHandling: 'preserve'
        });
      } else {
        // Navigate to detail page (will auto-detect verify/view based on status)
        this.router.navigate(['/donations', nextId], {
          queryParamsHandling: 'preserve'
        });
      }
    }
  }

  goToLast(): void {
    if (this.filteredDonationIds.length > 0) {
      const lastId = this.filteredDonationIds[this.filteredDonationIds.length - 1];
      
      if (this.mode === 'create') {
        return; // Cannot navigate in create mode
      } else if (this.mode === 'edit') {
        this.router.navigate(['/donations', lastId, 'edit'], {
          queryParamsHandling: 'preserve'
        });
      } else {
        // Navigate to detail page (will auto-detect verify/view based on status)
        this.router.navigate(['/donations', lastId], {
          queryParamsHandling: 'preserve'
        });
      }
    }
  }

  get canGoPrevious(): boolean {
    return this.currentIndex > 0;
  }

  get canGoNext(): boolean {
    return this.currentIndex < this.filteredDonationIds.length - 1;
  }

  get currentPosition(): string {
    if (this.totalFiltered === 0) return '0 di 0';
    return `${this.currentIndex + 1} di ${this.totalFiltered}`;
  }

  applyQuickFilters(): void {
    this.loadFilteredDonations();
    this.isFilterDrawerVisible = false;
    this.message.success('Filtri applicati');
  }

  clearQuickFilters(): void {
    this.quickFilters = {
      status: null,
      channel: null,
      minAmount: null,
      maxAmount: null,
      dateFrom: null,
      dateTo: null
    };
    this.loadFilteredDonations();
    this.message.success('Filtri rimossi');
  }

  suspend(): void {
    if (this.mode === 'create') {
      this.message.warning('Non puoi sospendere una donazione non ancora creata');
      return;
    }

    if (!this.donationId) {
      this.message.warning('ID donazione non valido');
      return;
    }

    // Set status to Suspended
    this.currentStatus = DonationStatus.Suspended;

    // Call verify method with Suspended status
    this.verify();
  }

  confirm(): void {
    // Set status to Verified
    this.currentStatus = DonationStatus.Verified;

    // Call verify method which will handle the business logic
    // In the next sprint, additional business logic will be integrated here
    this.verify();
  }

  goBack(): void {
    this.router.navigate(['/donations']);
  }

  editDonation(): void {
    if (this.donationId) {
      this.router.navigate(['/donations', this.donationId, 'edit']);
    }
  }

  printDonation(): void {
    window.print();
  }
}
