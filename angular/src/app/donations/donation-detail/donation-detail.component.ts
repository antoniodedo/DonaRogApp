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
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { EditorModule, TINYMCE_SCRIPT_SRC } from '@tinymce/tinymce-angular';
import { DonationService } from '../../proxy/donations/donation.service';
import { 
  DonationDto, 
  RejectionReason, 
  DonationChannel, 
  DonationStatus,
  DonationDocumentDto,
  DonationDocumentType,
  UploadDonationDocumentDto
} from '../../proxy/donations/models';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { UploadDocumentModalComponent } from './upload-document-modal.component';
import { DonorService } from '../../proxy/donors/donor.service';
import { CampaignService } from '../../proxy/application/campaigns/campaign.service';
import { ProjectService } from '../../proxy/application/projects/project.service';
import { LetterTemplateService } from '../../proxy/letter-templates/letter-template.service';
import { BankAccountService } from '../../proxy/bank-accounts/bank-account.service';
import { NoteService } from '../../proxy/notes/note.service';
import { CommunicationService } from '../../proxy/communications/communication.service';
import { 
  AlertLevel, 
  DuplicateCheckResultDto, 
  PreferredThankYouChannel 
} from '../../proxy/communications/models';
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
    EditorModule,
    NzRadioModule
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
  private _createTotalAmount: number = 50;
  private _isLoadingData: boolean = false; // Flag to prevent auto-recalc during data load
  createDonationDate: Date = new Date();
  createCreditDate?: Date;
  currentStatus: DonationStatus = DonationStatus.Pending;
  
  get createTotalAmount(): number {
    return this._createTotalAmount;
  }
  
  set createTotalAmount(value: number) {
    this._createTotalAmount = value;
  }
  
  onTotalAmountChange(newValue: number): void {
    const oldValue = Number(this._createTotalAmount);
    const numericNewValue = Number(newValue);
    
    console.log('=== onTotalAmountChange triggered ===');
    console.log('oldValue:', oldValue, 'type:', typeof oldValue);
    console.log('newValue:', numericNewValue, 'type:', typeof numericNewValue);
    console.log('_isLoadingData:', this._isLoadingData);
    console.log('projectAllocations.length:', this.projectAllocations.length);
    
    this.createTotalAmount = numericNewValue;
    
    // Only trigger recalc popup if user is manually changing (not during data load)
    const shouldShowPopup = !this._isLoadingData && 
                           this.projectAllocations.length > 0 && 
                           oldValue !== numericNewValue && 
                           oldValue > 0 && 
                           numericNewValue > 0;
    
    console.log('shouldShowPopup:', shouldShowPopup);
    
    if (shouldShowPopup) {
      console.log('Showing recalc modal...');
      this.modal.confirm({
        nzTitle: 'Ricalcolare allocazioni?',
        nzContent: `L'importo totale è cambiato da ${oldValue.toFixed(2)}€ a ${numericNewValue.toFixed(2)}€. Vuoi ricalcolare le allocazioni ai progetti in modo proporzionale?`,
        nzOkText: 'Sì, ricalcola',
        nzCancelText: 'No, le modifico manualmente',
        nzOnOk: () => {
          this.recalculateAllocationsProportionally();
        }
      });
    }
  }
  
  // Documents
  documents: DonationDocumentDto[] = [];
  isLoadingDocuments = false;
  selectedDocumentForPreview: DonationDocumentDto | null = null;
  currentDocumentIndex = 0;
  documentPreviewModalRef: any = null;

  // Slider marks
  sliderMarks: { [key: number]: string } = {
    0: '0 €',
    1000: 'Max'
  };
  
  // Project allocations
  projectAllocations: Array<{
    projectId: string; 
    projectName?: string; 
    allocatedAmount: number; 
    editing?: boolean;
    editMode?: 'amount' | 'percentage';
    editPercentage?: number;
  }> = [];
  availableProjects: any[] = [];
  newProjectId: string | null = null;
  newProjectAmount: number = 0;
  newProjectPercentage: number = 0;
  allocationMode: 'amount' | 'percentage' = 'amount';
  
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
  
  // Thank You & Alert System
  duplicateAlert: DuplicateCheckResultDto | null = null;
  isCheckingDuplicates = false;
  AlertLevel = AlertLevel;
  PreferredThankYouChannel = PreferredThankYouChannel;
  createThankYou: boolean | null = null; // null = auto (evaluate rules)
  thankYouChannel: PreferredThankYouChannel | null = null;
  printImmediately = false;
  noThankYouReason: string = '';

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
    private communicationService: CommunicationService,
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
        
        // Check for donorId in query params (coming from donor detail page)
        this.route.queryParams.subscribe(queryParams => {
          if (queryParams['donorId']) {
            this.verifyForm.patchValue({ donorId: queryParams['donorId'] });
            this.loadDonorDetails(queryParams['donorId']);
          }
        });
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
        
        // Check for duplicate letters
        if (this.mode === 'verify' || this.mode === 'create') {
          this.checkDuplicateLetters(donorId);
        }
        
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
    this._isLoadingData = true; // Prevent auto-recalc modal during data load
    
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

        // Load documents
        this.loadDocuments();

        this.loading = false;
        this._isLoadingData = false; // Re-enable auto-recalc modal
      },
      error: () => {
        this.loading = false;
        this._isLoadingData = false;
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
  
  updateProjectAllocation(projectId: string, newAmount: number): void {
    const allocation = this.projectAllocations.find(pa => pa.projectId === projectId);
    if (allocation) {
      allocation.allocatedAmount = newAmount;
      // Update percentage automatically
      if (this.createTotalAmount > 0) {
        allocation.editPercentage = (newAmount / this.createTotalAmount) * 100;
      }
      console.log(`Updated ${allocation.projectName} to ${newAmount}€`);
    }
  }
  
  updateProjectAllocationByPercentage(projectId: string, newPercentage: number): void {
    const allocation = this.projectAllocations.find(pa => pa.projectId === projectId);
    if (allocation) {
      allocation.editPercentage = newPercentage;
      allocation.allocatedAmount = (this.createTotalAmount * newPercentage) / 100;
      console.log(`Updated ${allocation.projectName} to ${newPercentage}% (${allocation.allocatedAmount}€)`);
    }
  }
  
  toggleProjectEditMode(projectId: string): void {
    const allocation = this.projectAllocations.find(pa => pa.projectId === projectId);
    if (allocation) {
      allocation.editMode = allocation.editMode === 'percentage' ? 'amount' : 'percentage';
      // Initialize percentage if not set
      if (!allocation.editPercentage && this.createTotalAmount > 0) {
        allocation.editPercentage = (allocation.allocatedAmount / this.createTotalAmount) * 100;
      }
    }
  }
  
  recalculateAllocationsProportionally(): void {
    console.log('=== RECALCULATE START ===');
    console.log('projectAllocations before:', JSON.parse(JSON.stringify(this.projectAllocations)));
    
    if (this.projectAllocations.length === 0) {
      console.log('No allocations to recalculate');
      return;
    }
    
    const currentTotal = this.getTotalAllocated();
    console.log('currentTotal:', currentTotal);
    
    if (currentTotal === 0) {
      console.log('Current total is 0, cannot recalculate');
      return;
    }
    
    const targetTotal = this.createTotalAmount;
    console.log('targetTotal:', targetTotal);
    
    // Calculate proportions and redistribute
    let remainingToAllocate = targetTotal;
    const proportionalAllocations: number[] = [];
    
    // Calculate proportional amounts (with rounding)
    this.projectAllocations.forEach((pa, index) => {
      const proportion = pa.allocatedAmount / currentTotal;
      console.log(`${pa.projectName}: proportion=${proportion.toFixed(4)}`);
      
      if (index === this.projectAllocations.length - 1) {
        // Last item gets the remainder to avoid rounding errors
        proportionalAllocations.push(remainingToAllocate);
      } else {
        const proportionalAmount = Math.round(targetTotal * proportion * 100) / 100;
        proportionalAllocations.push(proportionalAmount);
        remainingToAllocate -= proportionalAmount;
      }
    });
    
    console.log('proportionalAllocations:', proportionalAllocations);
    
    // Apply new amounts - create new array to trigger Angular change detection
    this.projectAllocations = this.projectAllocations.map((pa, index) => {
      const oldAmount = pa.allocatedAmount;
      const newAmount = proportionalAllocations[index];
      console.log(`${pa.projectName}: ${oldAmount}€ → ${newAmount}€`);
      return {
        ...pa,
        allocatedAmount: newAmount
      };
    });
    
    console.log('projectAllocations after:', JSON.parse(JSON.stringify(this.projectAllocations)));
    console.log('new total:', this.getTotalAllocated());
    console.log('=== RECALCULATE END ===');
    
    this.message.success(`Allocazioni ricalcolate: ${this.getTotalAllocated().toFixed(2)} €`);
  }
  
  trackByProjectId(index: number, item: any): string {
    return item.projectId;
  }

  getTotalAllocated(): number {
    return this.projectAllocations.reduce((sum, pa) => sum + pa.allocatedAmount, 0);
  }

  getRemainingAmount(): number {
    const totalAmount = (this.mode === 'create' || this.mode === 'edit') ? this.createTotalAmount : (this.donation?.totalAmount || 0);
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

    // Validate required fields for create mode
    if (this.mode === 'create') {
      if (!this.verifyForm.get('donorId')?.value) {
        this.message.warning('Seleziona un donatore');
        return;
      }
      
      if (!this.createChannel) {
        this.message.warning('Seleziona un canale di donazione');
        return;
      }
      
      if (!this.createTotalAmount || this.createTotalAmount <= 0) {
        this.message.warning('Inserisci un importo valido');
        return;
      }
      
      if (!this.createDonationDate) {
        this.message.warning('Inserisci la data della donazione');
        return;
      }
    }

    // Validate allocations only if there are some
    if (this.projectAllocations.length > 0) {
      const remaining = this.getRemainingAmount();
      
      if (remaining > 0) {
        this.message.warning(`Hai allocato ${this.getTotalAllocated().toFixed(2)} € ma il totale è ${this.createTotalAmount} €. Rimanente: ${remaining.toFixed(2)} €`);
        this.loading = false;
        return;
      }
      
      if (remaining < 0) {
        this.message.error(`Hai allocato troppo! Totale donazione: ${this.createTotalAmount} €, Allocato: ${this.getTotalAllocated().toFixed(2)} €. Clicca "Ricalcola" o riduci manualmente di ${Math.abs(remaining).toFixed(2)} €`);
        this.loading = false;
        return;
      }
    }

    this.loading = true;
    
    try {
      // DEBUG: Dump complete state before sending
      console.log('=== PRE-SUBMIT STATE ===');
      console.log('Mode:', this.mode);
      console.log('createTotalAmount:', this.createTotalAmount);
      console.log('projectAllocations RAW:', this.projectAllocations);
      
      // Force recalculate totals to ensure they're fresh
      const actualTotalAllocated = this.projectAllocations.reduce((sum, pa) => sum + (Number(pa.allocatedAmount) || 0), 0);
      console.log('Actual total allocated (fresh):', actualTotalAllocated);
      console.log('Remaining:', this.createTotalAmount - actualTotalAllocated);
      
      const dto = {
        ...this.verifyForm.value,
        projectAllocations: this.projectAllocations.map(pa => ({
          projectId: pa.projectId,
          allocatedAmount: Number(pa.allocatedAmount) // Ensure it's a number
        })),
        // Thank You Options
        createThankYou: this.createThankYou,
        thankYouChannel: this.thankYouChannel,
        noThankYouReason: this.noThankYouReason || undefined,
        printImmediately: this.printImmediately
      };

      console.log('DTO base:', dto);

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
      
      console.log('CreateDto:', createDto);
      
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
    } else if (this.mode === 'edit') {
      // EDIT mode - use update endpoint
      // Validate edit-specific fields
      if (!this.createChannel) {
        this.message.warning('Seleziona un canale di donazione');
        this.loading = false;
        return;
      }
      
      if (!this.createTotalAmount || this.createTotalAmount <= 0) {
        this.message.warning('Inserisci un importo valido');
        this.loading = false;
        return;
      }
      
      if (!this.createDonationDate) {
        this.message.warning('Inserisci la data della donazione');
        this.loading = false;
        return;
      }
      
      const updateDto = {
        ...dto,
        channel: this.createChannel,
        totalAmount: this.createTotalAmount,
        donationDate: this.createDonationDate instanceof Date ? this.createDonationDate.toISOString() : new Date(this.createDonationDate).toISOString(),
        creditDate: this.createCreditDate ? (this.createCreditDate instanceof Date ? this.createCreditDate.toISOString() : new Date(this.createCreditDate).toISOString()) : undefined,
      };
      
      console.log('=== UPDATE DEBUG ===');
      console.log('createTotalAmount:', this.createTotalAmount);
      console.log('projectAllocations:', JSON.parse(JSON.stringify(this.projectAllocations)));
      console.log('totalAllocated:', this.getTotalAllocated());
      console.log('UpdateDto:', JSON.parse(JSON.stringify(updateDto)));
      console.log('UpdateDto.totalAmount:', updateDto.totalAmount);
      console.log('UpdateDto.projectAllocations:', JSON.parse(JSON.stringify(updateDto.projectAllocations)));
      
      const dtoTotal = updateDto.projectAllocations?.reduce((sum: number, pa: any) => sum + (pa.allocatedAmount || 0), 0) || 0;
      console.log('DTO Total Allocated:', dtoTotal);
      console.log('Difference:', updateDto.totalAmount - dtoTotal);
      
      this.donationService.update(this.donationId!, updateDto).subscribe({
        next: () => {
          this.message.success('Donazione aggiornata con successo');
          this.router.navigate(['/donations', this.donationId]);
        },
        error: (err) => {
          this.loading = false;
          
          // Check if error is about external donation
          if (err.error?.error?.code === 'DonaRog:CannotUpdateExternalDonationCoreData') {
            this.message.error('Impossibile modificare importo/canale/date di una donazione da flusso esterno');
          } else {
            this.message.error('Errore aggiornamento donazione');
          }
        },
      });
    } else {
      // VERIFY mode - use verify endpoint
      this.donationService.verify(this.donationId!, dto).subscribe({
        next: () => {
          this.message.success('Donazione verificata con successo');
          this.router.navigate(['/donations', this.donationId]);
        },
        error: () => {
          this.loading = false;
          this.message.error('Errore verifica donazione');
        },
      });
    }
    } catch (error) {
      console.error('Error building DTO:', error);
      this.loading = false;
      this.message.error('Errore nella preparazione dei dati: ' + (error as Error).message);
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

  // ======================================================================
  // DOCUMENT MANAGEMENT
  // ======================================================================
  
  async loadDocuments(): Promise<void> {
    if (!this.donation?.id) return;
    
    this.isLoadingDocuments = true;
    try {
      this.documents = await this.donationService.getDocuments(this.donation.id).toPromise() || [];
    } catch (error) {
      console.error('Error loading documents:', error);
      this.message.error('Errore nel caricamento dei documenti');
    } finally {
      this.isLoadingDocuments = false;
    }
  }

  async uploadDocument(file: File, documentType: DonationDocumentType, notes?: string): Promise<void> {
    console.log('uploadDocument called with:', { file, documentType, notes, donationId: this.donation?.id });
    
    if (!this.donation?.id) {
      console.error('No donation ID available');
      return;
    }
    
    if (!file) {
      console.error('No file provided');
      this.message.error('Nessun file selezionato');
      return;
    }
    
    try {
      console.log('Calling donationService.uploadDocument...');
      const uploaded = await this.donationService.uploadDocument(
        this.donation.id, 
        file, 
        documentType, 
        notes
      ).toPromise();
      
      console.log('Upload response:', uploaded);
      
      if (uploaded) {
        this.documents.push(uploaded);
        this.message.success('Documento caricato con successo');
      }
    } catch (error) {
      console.error('Error uploading document:', error);
      this.message.error('Errore nel caricamento del documento');
      throw error;
    }
  }

  openDocumentPreview(doc: DonationDocumentDto, index: number = 0): void {
    this.selectedDocumentForPreview = doc;
    this.currentDocumentIndex = index;

    // Close existing modal if any
    if (this.documentPreviewModalRef) {
      this.documentPreviewModalRef.close();
    }

    // Handle text-only documents
    if (doc.isTextDocument) {
      this.documentPreviewModalRef = this.modal.create({
        nzTitle: this.getModalTitle(doc, index),
        nzContent: `
          <div style="padding: 20px;">
            <div style="background: linear-gradient(135deg, #f0f9ff, #e0f2fe); padding: 24px; border-radius: 12px; border: 2px solid #0ea5e9; box-shadow: inset 0 2px 8px rgba(14, 165, 233, 0.1);">
              <div style="display: flex; align-items: center; gap: 12px; margin-bottom: 16px; padding-bottom: 12px; border-bottom: 2px solid rgba(14, 165, 233, 0.2);">
                <span style="font-size: 32px;">💳</span>
                <strong style="color: #0c4a6e; font-size: 16px;">Descrizione bonifico</strong>
              </div>
              <div style="background: white; padding: 20px; border-radius: 8px; border: 1px solid rgba(14, 165, 233, 0.2); box-shadow: 0 2px 8px rgba(0,0,0,0.05);">
                <p style="white-space: pre-wrap; color: #0c4a6e; line-height: 2; margin: 0; font-size: 14px;">${doc.textContent || ''}</p>
              </div>
            </div>
            ${doc.notes ? `
              <div style="margin-top: 16px; padding: 16px 20px; background: linear-gradient(135deg, #fff7ed, #ffedd5); border-radius: 8px; border-left: 4px solid #f59e0b;">
                <div style="display: flex; align-items: center; gap: 8px; margin-bottom: 8px;">
                  <span style="font-size: 18px;">📝</span>
                  <strong style="color: #92400e; font-size: 13px; text-transform: uppercase; letter-spacing: 0.5px;">Note</strong>
                </div>
                <p style="margin: 0; color: #78350f; font-size: 13px; line-height: 1.6;">${doc.notes}</p>
              </div>
            ` : ''}
          </div>
        `,
        nzWidth: '700px',
        nzFooter: this.getModalFooter(doc, index),
        nzClosable: true,
        nzMaskClosable: false
      });
      return;
    }

    // Handle file-based documents (images, PDFs)
    const isPdf = doc.mimeType === 'application/pdf';
    const downloadUrl = `https://localhost:44318/api/donations/${doc.donationId}/documents/${doc.id}`;

    this.documentPreviewModalRef = this.modal.create({
      nzTitle: this.getModalTitle(doc, index),
      nzContent: isPdf
        ? `<div style="position: relative; height: 75vh; background: #525252; border-radius: 8px; overflow: hidden;">
             <iframe src="${downloadUrl}" style="width: 100%; height: 100%; border: none;"></iframe>
           </div>`
        : `<div style="display: flex; justify-content: center; align-items: center; background: #000; border-radius: 8px; padding: 20px; min-height: 60vh;">
             <img src="${downloadUrl}" alt="${doc.fileName}" style="max-width: 100%; max-height: 75vh; height: auto; border-radius: 4px; box-shadow: 0 8px 32px rgba(0,0,0,0.3);" />
           </div>`,
      nzWidth: '95%',
      nzFooter: this.getModalFooter(doc, index),
      nzClosable: true,
      nzMaskClosable: false,
      nzBodyStyle: { padding: '24px', background: '#f5f5f5' }
    });
  }

  getModalTitle(doc: DonationDocumentDto, index: number): string {
    const docType = doc.isTextDocument ? '📄 Testo' : (doc.mimeType?.startsWith('image/') ? '🖼️ Immagine' : '📑 PDF');
    const fileName = doc.isTextDocument ? 'Descrizione bonifico' : doc.fileName;
    const counter = `<span style="display: inline-block; background: linear-gradient(135deg, #14b8a6, #0d9488); color: white; padding: 4px 12px; border-radius: 6px; font-size: 12px; font-weight: 700; margin-left: 12px;">${index + 1} / ${this.documents.length}</span>`;
    return `${docType} ${fileName} ${counter}`;
  }

  getModalFooter(doc: DonationDocumentDto, index: number): any[] {
    const footer: any[] = [];

    // Navigation section (left side)
    const hasNavigation = this.documents.length > 1;
    if (hasNavigation) {
      // Previous button
      footer.push({
        label: '◀ Precedente',
        disabled: index === 0,
        onClick: () => {
          this.navigateToPreviousDocument();
          return false;
        },
        show: true,
        type: 'default'
      });

      // Next button
      footer.push({
        label: 'Successivo ▶',
        disabled: index >= this.documents.length - 1,
        onClick: () => {
          this.navigateToNextDocument();
          return false;
        },
        show: true,
        type: 'default'
      });
    }

    // Download button (only for file-based documents)
    if (!doc.isTextDocument) {
      footer.push({
        label: '⬇️ Scarica',
        type: 'primary',
        onClick: () => {
          this.downloadDocument(doc);
          return false;
        }
      });
    }

    // Delete button (only if not from external flow and not in view mode)
    if (this.mode !== 'view' && !doc.isFromExternalFlow) {
      footer.push({
        label: '🗑️ Elimina',
        danger: true,
        onClick: () => {
          this.deleteDocument(doc);
          return true; // Will close after confirmation
        }
      });
    }

    // Close button
    footer.push({
      label: 'Chiudi',
      onClick: () => {
        this.documentPreviewModalRef = null;
        return true;
      }
    });

    return footer;
  }

  navigateToPreviousDocument(): void {
    if (this.currentDocumentIndex > 0) {
      const prevIndex = this.currentDocumentIndex - 1;
      const prevDoc = this.documents[prevIndex];
      this.openDocumentPreview(prevDoc, prevIndex);
    }
  }

  navigateToNextDocument(): void {
    if (this.currentDocumentIndex < this.documents.length - 1) {
      const nextIndex = this.currentDocumentIndex + 1;
      const nextDoc = this.documents[nextIndex];
      this.openDocumentPreview(nextDoc, nextIndex);
    }
  }

  downloadDocument(doc: DonationDocumentDto): void {
    if (!this.donation?.id) return;

    // Text documents cannot be downloaded
    if (doc.isTextDocument) {
      this.message.info('I documenti testuali non possono essere scaricati');
      return;
    }

    this.donationService.downloadDocument(this.donation.id, doc.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = doc.fileName || 'document';
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Error downloading document:', error);
        this.message.error('Errore nel download del documento');
      }
    });
  }

  async deleteDocument(doc: DonationDocumentDto): Promise<void> {
    if (!this.donation?.id) return;

    const documentName = doc.isTextDocument 
      ? 'documento testuale' 
      : doc.fileName || 'documento';

    this.modal.confirm({
      nzTitle: 'Conferma eliminazione',
      nzContent: `Sei sicuro di voler eliminare ${doc.isTextDocument ? 'questo' : 'il documento'} "${documentName}"?`,
      nzOkText: 'Elimina',
      nzOkDanger: true,
      nzCancelText: 'Annulla',
      nzOnOk: async () => {
        try {
          await this.donationService.deleteDocument(this.donation!.id, doc.id).toPromise();
          this.documents = this.documents.filter(d => d.id !== doc.id);
          this.message.success('Documento eliminato con successo');
          
          // Close preview modal if it was the document being previewed
          if (this.selectedDocumentForPreview?.id === doc.id && this.documentPreviewModalRef) {
            this.documentPreviewModalRef.close();
            this.documentPreviewModalRef = null;
          }
        } catch (error) {
          console.error('Error deleting document:', error);
          this.message.error('Errore nell\'eliminazione del documento');
        }
      }
    });
  }

  getDocumentTooltip(doc: DonationDocumentDto): string {
    if (doc.isTextDocument) {
      return `Documento testuale${doc.isFromExternalFlow ? ' (flusso esterno)' : ''}\nClicca per visualizzare`;
    }
    
    const type = doc.mimeType?.startsWith('image/') ? 'Immagine' : 'PDF';
    const source = doc.isFromExternalFlow ? ' (flusso esterno)' : '';
    return `${type}: ${doc.fileName}${source}\nClicca per visualizzare`;
  }

  getDocumentTypeBadgeTooltip(doc: DonationDocumentDto): string {
    const typeLabels: { [key: number]: string } = {
      0: 'Ricevuta Bonifico',
      1: 'Ricevuta Postale',
      2: 'Ricevuta PayPal',
      3: 'Immagine Assegno',
      4: 'Ricevuta Contanti',
      99: 'Altro'
    };
    
    const typeLabel = typeLabels[doc.documentType] || 'Documento';
    const formatLabel = doc.isTextDocument ? 'Testo' : (doc.mimeType?.startsWith('image/') ? 'Immagine' : 'PDF');
    
    return `${typeLabel}\nFormato: ${formatLabel}`;
  }

  showUploadModal(): void {
    console.log('showUploadModal called');
    
    const modalRef = this.modal.create({
      nzTitle: 'Carica Documento',
      nzContent: UploadDocumentModalComponent,
      nzWidth: '500px',
      nzOkText: 'Carica',
      nzCancelText: 'Annulla',
      nzOnOk: async (component) => {
        console.log('Modal OK clicked');
        const result = component.getResult();
        
        console.log('Modal result:', result);
        
        if (!result) {
          console.log('No result from modal');
          return false;
        }

        try {
          await this.uploadDocument(result.file, result.documentType, result.notes);
          console.log('Upload completed successfully');
          return true;
        } catch (error) {
          console.error('Upload failed:', error);
          return false;
        }
      }
    });
  }

  // Open image zoom modal (deprecated - use openDocumentPreview)
  openImageZoom(): void {
    if (this.documents.length > 0) {
      this.openDocumentPreview(this.documents[0], 0);
    } else {
      this.message.info('Nessun documento disponibile');
    }
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
    this.newProjectPercentage = 0;
    this.allocationMode = 'amount';
  }
  
  onAllocationModeChange(): void {
    this.newProjectAmount = 0;
    this.newProjectPercentage = 0;
  }
  
  onPercentageChange(percentage: number): void {
    this.newProjectPercentage = percentage;
    if (percentage > 0 && percentage <= 100) {
      this.newProjectAmount = (this.createTotalAmount * percentage) / 100;
    }
  }
  
  onAmountChange(amount: number): void {
    this.newProjectAmount = amount;
    if (amount > 0 && this.createTotalAmount > 0) {
      this.newProjectPercentage = (amount / this.createTotalAmount) * 100;
    }
  }

  // Handle project modal cancel
  handleProjectCancel(): void {
    this.isProjectModalVisible = false;
    this.newProjectId = null;
    this.newProjectAmount = 0;
  }

  // Handle project modal OK
  handleProjectOk(): void {
    if (!this.newProjectId) {
      this.message.warning('Seleziona un progetto');
      return;
    }
    
    // Calculate amount based on mode
    let finalAmount: number;
    if (this.allocationMode === 'percentage') {
      if (this.newProjectPercentage <= 0 || this.newProjectPercentage > 100) {
        this.message.warning('Inserisci una percentuale valida (1-100%)');
        return;
      }
      finalAmount = (this.createTotalAmount * this.newProjectPercentage) / 100;
    } else {
      if (this.newProjectAmount <= 0) {
        this.message.warning('Inserisci un importo valido');
        return;
      }
      finalAmount = this.newProjectAmount;
    }

    const remaining = this.getRemainingAmount();
    if (finalAmount > remaining) {
      this.message.error(`Importo massimo disponibile: ${remaining.toFixed(2)} € (Totale: ${this.createTotalAmount} €, Già allocato: ${this.getTotalAllocated().toFixed(2)} €)`);
      return;
    }
    
    // Set the calculated amount
    this.newProjectAmount = finalAmount;

    this.addProjectAllocation();
    this.isProjectModalVisible = false;
    this.newProjectId = null;
    this.newProjectAmount = 0;
    this.newProjectPercentage = 0;
    this.allocationMode = 'amount';
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
  
  // ========================================================================
  // THANK YOU & DUPLICATE CHECK METHODS
  // ========================================================================
  
  checkDuplicateLetters(donorId: string): void {
    this.isCheckingDuplicates = true;
    this.duplicateAlert = null;
    
    this.communicationService.checkDuplicateLetters({
      donorId: donorId,
      checkLastDays: 30,
      errorThresholdDays: 7,
      warningThresholdDays: 15
    }).subscribe({
      next: (result) => {
        this.isCheckingDuplicates = false;
        if (result.alertLevel !== AlertLevel.None) {
          this.duplicateAlert = result;
        }
      },
      error: () => {
        this.isCheckingDuplicates = false;
        console.error('Error checking duplicates');
      }
    });
  }
  
  getAlertTypeClass(): string {
    if (!this.duplicateAlert) return '';
    switch (this.duplicateAlert.alertLevel) {
      case AlertLevel.Error: return 'error';
      case AlertLevel.Warning: return 'warning';
      case AlertLevel.Info: return 'info';
      default: return '';
    }
  }
  
  getAlertIcon(): string {
    if (!this.duplicateAlert) return '';
    switch (this.duplicateAlert.alertLevel) {
      case AlertLevel.Error: return 'exclamation-circle';
      case AlertLevel.Warning: return 'warning';
      case AlertLevel.Info: return 'info-circle';
      default: return 'info';
    }
  }
  
  showCommunicationHistory(): void {
    if (!this.selectedDonor) return;
    
    this.modal.info({
      nzTitle: 'Storico Comunicazioni',
      nzContent: this.buildHistoryContent(),
      nzWidth: 800,
      nzOkText: 'Chiudi'
    });
  }
  
  private buildHistoryContent(): string {
    if (!this.duplicateAlert?.recentCommunications.length) {
      return '<p>Nessuna comunicazione recente trovata.</p>';
    }
    
    let html = '<table style="width:100%; border-collapse: collapse;">';
    html += '<thead><tr style="background: #f5f5f5;"><th style="padding:8px;text-align:left;">Data</th><th style="padding:8px;text-align:left;">Tipo</th><th style="padding:8px;text-align:left;">Donazione</th><th style="padding:8px;text-align:right;">Importo</th><th style="padding:8px;text-align:center;">Stato</th></tr></thead>';
    html += '<tbody>';
    
    for (const comm of this.duplicateAlert.recentCommunications) {
      const date = new Date(comm.sentDate);
      html += `<tr style="border-bottom: 1px solid #f0f0f0;">`;
      html += `<td style="padding:8px;">${date.toLocaleDateString('it-IT')}</td>`;
      html += `<td style="padding:8px;">Lettera</td>`;
      html += `<td style="padding:8px;">${comm.donationReference || 'N/A'}</td>`;
      html += `<td style="padding:8px;text-align:right;">${comm.donationAmount?.toFixed(2) || '0.00'} €</td>`;
      html += `<td style="padding:8px;text-align:center;">${comm.isPrinted ? '✓ Stampata' : (comm.isInBatch ? '📦 In batch' : '⏳ In attesa')}</td>`;
      html += `</tr>`;
    }
    
    html += '</tbody></table>';
    return html;
  }
}
