import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CampaignService } from 'src/app/proxy/application/campaigns';
import { RecurrenceService } from 'src/app/proxy/application/recurrences';
import { CreateCampaignDto, CampaignDto, UpdateCampaignDto } from 'src/app/proxy/application/contracts/campaigns/dto/models';
import { RecurrenceListDto } from 'src/app/proxy/application/contracts/recurrences/dto/models';
import { CampaignType, CampaignChannel, campaignTypeOptions, campaignChannelOptions } from 'src/app/proxy/enums/campaigns';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-campaign-form',
  standalone: false,
  templateUrl: './campaign-form.component.html',
  styleUrls: ['./campaign-form.component.scss']
})
export class CampaignFormComponent implements OnInit {
  @Input() campaignId?: string;
  @Output() onSave = new EventEmitter<void>();
  @Output() onCancel = new EventEmitter<void>();

  form: FormGroup;
  loading = false;
  campaignTypeOptions = campaignTypeOptions;
  campaignChannelOptions = campaignChannelOptions;
  recurrences: RecurrenceListDto[] = [];
  isEditMode = false;
  
  // Calcolo automatico costi
  showCalculationHint = false;
  calculationHint = '';
  private isCalculating = false;

  constructor(
    private fb: FormBuilder,
    private campaignService: CampaignService,
    private recurrenceService: RecurrenceService,
    private message: NzMessageService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      year: [new Date().getFullYear(), [Validators.required]],
      code: ['', [Validators.required, Validators.maxLength(50)]],
      description: ['', [Validators.maxLength(2000)]],
      campaignType: [CampaignType.Archive, [Validators.required]],
      channel: [CampaignChannel.Email, [Validators.required]],
      recurrenceId: [null],
      extractionScheduledDate: [null],
      dispatchScheduledDate: [null],
      recurrenceDate: [null],
      unitCost: [0, [Validators.min(0)]], // Campo helper per calcolo
      totalCost: [0, [Validators.required, Validators.min(0)]],
      targetDonorCount: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.loadRecurrences();
    if (this.campaignId) {
      this.isEditMode = true;
      this.loadCampaign();
    }
  }

  loadRecurrences(): void {
    this.recurrenceService.getActiveRecurrences().subscribe({
      next: (result) => {
        this.recurrences = result.items || [];
      },
      error: (err) => {
        console.warn('Impossibile caricare le ricorrenze:', err);
        this.recurrences = [];
        // Non mostrare warning se il backend non è attivo
      }
    });
  }

  loadCampaign(): void {
    if (!this.campaignId) return;

    this.loading = true;
    this.campaignService.get(this.campaignId).subscribe({
      next: (campaign: CampaignDto) => {
        // Calcola costo unitario se possibile
        const unitCost = campaign.targetDonorCount > 0 
          ? campaign.totalCost / campaign.targetDonorCount 
          : 0;

        this.form.patchValue({
          name: campaign.name,
          year: campaign.year,
          code: campaign.code,
          description: campaign.description,
          campaignType: campaign.campaignType,
          channel: campaign.channel,
          recurrenceId: campaign.recurrenceId,
          extractionScheduledDate: campaign.extractionScheduledDate ? new Date(campaign.extractionScheduledDate) : null,
          dispatchScheduledDate: campaign.dispatchScheduledDate ? new Date(campaign.dispatchScheduledDate) : null,
          recurrenceDate: campaign.recurrenceDate ? new Date(campaign.recurrenceDate) : null,
          unitCost: unitCost,
          totalCost: campaign.totalCost,
          targetDonorCount: campaign.targetDonorCount
        });
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.message.error('Errore nel caricamento della campagna');
      }
    });
  }

  save(): void {
    if (this.form.invalid) {
      Object.values(this.form.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      return;
    }

    this.loading = true;

    if (this.isEditMode && this.campaignId) {
      const updateDto: UpdateCampaignDto = {
        name: this.form.value.name,
        description: this.form.value.description,
        extractionScheduledDate: this.form.value.extractionScheduledDate?.toISOString(),
        dispatchScheduledDate: this.form.value.dispatchScheduledDate?.toISOString(),
        recurrenceDate: this.form.value.recurrenceDate?.toISOString(),
        recurrenceId: this.form.value.recurrenceId,
        totalCost: this.form.value.totalCost,
        targetDonorCount: this.form.value.targetDonorCount
      };

      this.campaignService.update(this.campaignId, updateDto).subscribe({
        next: () => {
          this.message.success('Campagna aggiornata con successo');
          this.onSave.emit();
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.message.error('Errore durante l\'aggiornamento della campagna');
        }
      });
    } else {
      const createDto: CreateCampaignDto = {
        ...this.form.value,
        extractionScheduledDate: this.form.value.extractionScheduledDate?.toISOString(),
        dispatchScheduledDate: this.form.value.dispatchScheduledDate?.toISOString(),
        recurrenceDate: this.form.value.recurrenceDate?.toISOString()
      };

      this.campaignService.create(createDto).subscribe({
        next: () => {
          this.message.success('Campagna creata con successo');
          this.onSave.emit();
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.message.error('Errore durante la creazione della campagna');
        }
      });
    }
  }

  cancel(): void {
    this.onCancel.emit();
  }

  // ============================================
  // CALCOLO AUTOMATICO COSTI
  // ============================================
  
  onUnitCostChange(): void {
    if (this.isCalculating) return;
    
    const unitCost = this.form.get('unitCost')?.value || 0;
    const targetDonors = this.form.get('targetDonorCount')?.value || 0;
    
    if (unitCost > 0 && targetDonors > 0) {
      this.isCalculating = true;
      const totalCost = unitCost * targetDonors;
      this.form.patchValue({ totalCost }, { emitEvent: false });
      this.showCalculationHint = true;
      this.calculationHint = `Costo totale calcolato: ${this.formatCurrency(unitCost)} × ${targetDonors} = ${this.formatCurrency(totalCost)}`;
      this.isCalculating = false;
    }
  }

  onTargetChange(): void {
    if (this.isCalculating) return;
    
    const unitCost = this.form.get('unitCost')?.value || 0;
    const targetDonors = this.form.get('targetDonorCount')?.value || 0;
    
    if (unitCost > 0 && targetDonors > 0) {
      this.isCalculating = true;
      const totalCost = unitCost * targetDonors;
      this.form.patchValue({ totalCost }, { emitEvent: false });
      this.showCalculationHint = true;
      this.calculationHint = `Costo totale calcolato: ${this.formatCurrency(unitCost)} × ${targetDonors} = ${this.formatCurrency(totalCost)}`;
      this.isCalculating = false;
    } else {
      this.showCalculationHint = false;
    }
  }

  onTotalCostChange(): void {
    if (this.isCalculating) return;
    
    const totalCost = this.form.get('totalCost')?.value || 0;
    const targetDonors = this.form.get('targetDonorCount')?.value || 0;
    
    if (totalCost > 0 && targetDonors > 0) {
      this.isCalculating = true;
      const unitCost = totalCost / targetDonors;
      this.form.patchValue({ unitCost }, { emitEvent: false });
      this.showCalculationHint = true;
      this.calculationHint = `Costo unitario calcolato: ${this.formatCurrency(totalCost)} ÷ ${targetDonors} = ${this.formatCurrency(unitCost)}`;
      this.isCalculating = false;
    } else {
      this.showCalculationHint = false;
    }
  }

  private formatCurrency(value: number): string {
    return new Intl.NumberFormat('it-IT', { 
      style: 'currency', 
      currency: 'EUR' 
    }).format(value);
  }

  // ============================================
  // FORMATTERS
  // ============================================
  
  // Formatter per Euro
  formatterEuro = (value: number): string => `€ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
  parserEuro = (value: string): string => value.replace(/€\s?|(\.*)/g, '');

  // Formatter per numeri
  formatterNumber = (value: number): string => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
  parserNumber = (value: string): string => value.replace(/\./g, '');
}
