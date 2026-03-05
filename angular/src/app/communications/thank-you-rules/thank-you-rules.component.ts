import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { ThankYouRuleService } from '../../proxy/communications/thank-you-rules/thank-you-rule.service';
import { 
  ThankYouRuleDto, 
  CreateUpdateThankYouRuleDto 
} from '../../proxy/communications/thank-you-rules/models';
import { PreferredThankYouChannel, TemplateType } from '../../proxy/communications/models';
import { LetterTemplateService } from '../../proxy/letter-templates/letter-template.service';
import { ProjectService } from '../../proxy/application/projects/project.service';
import { CampaignService } from '../../proxy/application/campaigns/campaign.service';
import { RecurrenceService } from '../../proxy/application/recurrences/recurrence.service';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzTransferModule } from 'ng-zorro-antd/transfer';

@Component({
  selector: 'app-thank-you-rules',
  templateUrl: './thank-you-rules.component.html',
  styleUrls: ['./thank-you-rules.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    NzTableModule,
    NzButtonModule,
    NzIconModule,
    NzInputModule,
    NzSelectModule,
    NzCardModule,
    NzModalModule,
    NzTagModule,
    NzSpaceModule,
    NzSwitchModule,
    NzFormModule,
    NzInputNumberModule,
    NzCheckboxModule,
    NzDividerModule,
    NzDescriptionsModule,
    NzCollapseModule,
    NzDatePickerModule,
    NzTransferModule
  ]
})
export class ThankYouRulesComponent implements OnInit {
  rules: ThankYouRuleDto[] = [];
  loading = false;
  
  get activeRulesCount(): number {
    return this.rules.filter(r => r.isActive).length;
  }
  
  get inactiveRulesCount(): number {
    return this.rules.filter(r => !r.isActive).length;
  }
  
  PreferredThankYouChannel = PreferredThankYouChannel;
  
  // Edit/Create Modal
  isModalVisible = false;
  isEditMode = false;
  currentRuleId?: string;
  ruleForm!: FormGroup;
  
  // Reference data
  letterTemplates: any[] = [];
  projects: any[] = [];
  campaigns: any[] = [];
  recurrences: any[] = [];
  
  // Template pool management
  selectedTemplatesForPool: string[] = [];
  
  constructor(
    private ruleService: ThankYouRuleService,
    private letterTemplateService: LetterTemplateService,
    private projectService: ProjectService,
    private campaignService: CampaignService,
    private recurrenceService: RecurrenceService,
    private fb: FormBuilder,
    private message: NzMessageService,
    private modal: NzModalService
  ) {}
  
  ngOnInit(): void {
    this.initForm();
    this.loadRules();
    this.loadReferenceData();
  }
  
  initForm(): void {
    this.ruleForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', Validators.maxLength(500)],
      priority: [100, [Validators.required, Validators.min(1), Validators.max(1000)]],
      isActive: [true],

      // Conditions
      minAmount: [null, Validators.min(0)],
      maxAmount: [null, Validators.min(0)],
      isFirstDonation: [null],
      donorCategories: [[]],
      subjectTypes: [[]],
      projectIds: [[]],
      campaignIds: [[]],
      recurrenceId: [null],

      // Validity Period (for temporary rules)
      validFrom: [null],
      validUntil: [null],

      // Actions
      createThankYou: [true, Validators.required],
      suggestedChannel: [null],
      suggestedTemplateId: [null],
      
      // Template Pool (LRU rotation)
      templatePoolItems: [[]]
    });
  }
  
  loadReferenceData(): void {
    this.letterTemplateService.getList({ maxResultCount: 1000 }).subscribe({
      next: (result) => {
        this.letterTemplates = result.items?.filter((t: any) => t.category === 1) || [];
      }
    });

    this.projectService.getList({ maxResultCount: 1000 }).subscribe({
      next: (result) => {
        this.projects = result.items || [];
      }
    });

    this.campaignService.getList({ maxResultCount: 1000 }).subscribe({
      next: (result) => {
        this.campaigns = result.items || [];
      }
    });

    this.recurrenceService.getList({ maxResultCount: 1000 }).subscribe({
      next: (result) => {
        this.recurrences = result.items || [];
      }
    });
  }
  
  loadRules(): void {
    this.loading = true;
    
    this.ruleService.getList({
      maxResultCount: 1000,
      skipCount: 0
    }).subscribe({
      next: (result) => {
        this.rules = result.items || [];
        this.rules.sort((a, b) => a.priority - b.priority);
        this.loading = false;
      },
      error: () => {
        this.message.error('Errore caricamento regole');
        this.loading = false;
      }
    });
  }
  
  showCreateModal(): void {
    this.isEditMode = false;
    this.currentRuleId = undefined;
    this.selectedTemplatesForPool = [];
    this.ruleForm.reset({
      priority: 100,
      isActive: true,
      createThankYou: true,
      donorCategories: [],
      subjectTypes: [],
      projectIds: [],
      campaignIds: [],
      recurrenceId: null,
      validFrom: null,
      validUntil: null,
      templatePoolItems: []
    });
    this.isModalVisible = true;
  }
  
  showEditModal(rule: ThankYouRuleDto): void {
    this.isEditMode = true;
    this.currentRuleId = rule.id;

    // Load template pool
    const templatePoolItems = (rule as any).templatePool?.map((tp: any) => ({
      templateId: tp.templateId,
      priority: tp.priority,
      isActive: tp.isActive
    })) || [];
    
    this.selectedTemplatesForPool = templatePoolItems.map((tp: any) => tp.templateId);

    this.ruleForm.patchValue({
      name: rule.name,
      description: rule.description,
      priority: rule.priority,
      isActive: rule.isActive,
      minAmount: rule.minAmount,
      maxAmount: rule.maxAmount,
      isFirstDonation: rule.isFirstDonation,
      donorCategories: rule.donorCategories || [],
      subjectTypes: rule.subjectTypes || [],
      projectIds: rule.projectIds || [],
      campaignIds: rule.campaignIds || [],
      recurrenceId: (rule as any).recurrenceId || null,
      validFrom: (rule as any).validFrom ? new Date((rule as any).validFrom) : null,
      validUntil: (rule as any).validUntil ? new Date((rule as any).validUntil) : null,
      createThankYou: rule.createThankYou,
      suggestedChannel: rule.suggestedChannel,
      suggestedTemplateId: rule.suggestedTemplateId,
      templatePoolItems: templatePoolItems
    });
    
    this.isModalVisible = true;
  }
  
  saveRule(): void {
    if (this.ruleForm.invalid) {
      Object.values(this.ruleForm.controls).forEach(control => {
        control.markAsDirty();
        control.updateValueAndValidity();
      });
      return;
    }

    const formValue = this.ruleForm.value;
    
    // Build template pool items from selected templates
    const templatePoolItems = this.selectedTemplatesForPool.map((templateId, index) => ({
      templateId: templateId,
      priority: index + 1,
      isActive: true
    }));

    const dto: CreateUpdateThankYouRuleDto = {
      ...formValue,
      donorCategories: formValue.donorCategories?.length > 0 ? formValue.donorCategories : undefined,
      subjectTypes: formValue.subjectTypes?.length > 0 ? formValue.subjectTypes : undefined,
      projectIds: formValue.projectIds?.length > 0 ? formValue.projectIds : undefined,
      campaignIds: formValue.campaignIds?.length > 0 ? formValue.campaignIds : undefined,
      recurrenceId: formValue.recurrenceId || null,
      validFrom: formValue.validFrom ? new Date(formValue.validFrom).toISOString() : null,
      validUntil: formValue.validUntil ? new Date(formValue.validUntil).toISOString() : null,
      templatePoolItems: templatePoolItems.length > 0 ? templatePoolItems : []
    };

    this.loading = true;

    const operation = this.isEditMode && this.currentRuleId
      ? this.ruleService.update(this.currentRuleId, dto)
      : this.ruleService.create(dto);

    operation.subscribe({
      next: () => {
        this.message.success(this.isEditMode ? 'Regola aggiornata' : 'Regola creata');
        this.isModalVisible = false;
        this.loadRules();
      },
      error: () => {
        this.loading = false;
        this.message.error('Errore salvataggio regola');
      }
    });
  }
  
  toggleActive(rule: ThankYouRuleDto): void {
    this.ruleService.toggleActive(rule.id).subscribe({
      next: () => {
        rule.isActive = !rule.isActive;
        this.message.success(rule.isActive ? 'Regola attivata' : 'Regola disattivata');
      },
      error: () => {
        this.message.error('Errore aggiornamento regola');
      }
    });
  }
  
  moveUp(index: number): void {
    if (index === 0) return;
    
    const rule = this.rules[index];
    const aboveRule = this.rules[index - 1];
    
    const newOrder = this.rules.map((r, i) => {
      if (i === index) return { ruleId: r.id, priority: aboveRule.priority - 1 };
      if (i === index - 1) return { ruleId: r.id, priority: rule.priority };
      return { ruleId: r.id, priority: r.priority };
    });
    
    this.ruleService.reorderRules(newOrder).subscribe({
      next: () => {
        this.loadRules();
      },
      error: () => {
        this.message.error('Errore riordinamento');
      }
    });
  }
  
  moveDown(index: number): void {
    if (index === this.rules.length - 1) return;
    
    const rule = this.rules[index];
    const belowRule = this.rules[index + 1];
    
    const newOrder = this.rules.map((r, i) => {
      if (i === index) return { ruleId: r.id, priority: belowRule.priority + 1 };
      if (i === index + 1) return { ruleId: r.id, priority: rule.priority };
      return { ruleId: r.id, priority: r.priority };
    });
    
    this.ruleService.reorderRules(newOrder).subscribe({
      next: () => {
        this.loadRules();
      },
      error: () => {
        this.message.error('Errore riordinamento');
      }
    });
  }
  
  deleteRule(rule: ThankYouRuleDto): void {
    this.modal.confirm({
      nzTitle: 'Elimina Regola',
      nzContent: `Vuoi eliminare la regola "${rule.name}"?`,
      nzOkText: 'Elimina',
      nzOkDanger: true,
      nzCancelText: 'Annulla',
      nzOnOk: () => {
        this.ruleService.delete(rule.id).subscribe({
          next: () => {
            this.message.success('Regola eliminata');
            this.loadRules();
          },
          error: () => {
            this.message.error('Errore eliminazione regola');
          }
        });
      }
    });
  }
  
  handleModalCancel(): void {
    this.isModalVisible = false;
  }
  
  getConditionsSummary(rule: ThankYouRuleDto): string {
    const conditions: string[] = [];
    
    if (rule.minAmount !== null && rule.minAmount !== undefined) {
      conditions.push(`≥ ${rule.minAmount}€`);
    }
    if (rule.maxAmount !== null && rule.maxAmount !== undefined) {
      conditions.push(`≤ ${rule.maxAmount}€`);
    }
    if (rule.isFirstDonation === true) {
      conditions.push('Prima donazione');
    }
    if (rule.isFirstDonation === false) {
      conditions.push('Donatore ricorrente');
    }
    if (rule.donorCategories && rule.donorCategories.length > 0) {
      conditions.push(`Cat: ${rule.donorCategories.length}`);
    }
    if (rule.projectIds && rule.projectIds.length > 0) {
      conditions.push(`Prog: ${rule.projectIds.length}`);
    }
    if (rule.campaignIds && rule.campaignIds.length > 0) {
      conditions.push(`Camp: ${rule.campaignIds.length}`);
    }
    
    return conditions.length > 0 ? conditions.join(' • ') : 'Tutte le donazioni';
  }
  
  getActionsSummary(rule: ThankYouRuleDto): string {
    if (!rule.createThankYou) {
      return '✗ Non creare ringraziamento';
    }
    
    const parts: string[] = ['✓ Crea ringraziamento'];
    
    if (rule.suggestedChannel === PreferredThankYouChannel.Letter) {
      parts.push('📬 Lettera');
    } else if (rule.suggestedChannel === PreferredThankYouChannel.Email) {
      parts.push('📧 Email');
    }
    
    if (rule.suggestedTemplateId) {
      const template = this.letterTemplates.find(t => t.id === rule.suggestedTemplateId);
      if (template) {
        parts.push(`📄 ${template.name}`);
      }
    }
    
    return parts.join(' • ');
  }
  
  getChannelLabel(channel: PreferredThankYouChannel | null | undefined): string {
    if (channel === PreferredThankYouChannel.Letter) return 'Lettera';
    if (channel === PreferredThankYouChannel.Email) return 'Email';
    return 'Auto';
  }

  // ======================================================================
  // TEMPLATE POOL MANAGEMENT
  // ======================================================================
  
  onTemplatePoolChange(selectedIds: string[]): void {
    this.selectedTemplatesForPool = selectedIds;
  }

  moveTemplateUp(index: number): void {
    if (index > 0) {
      const temp = this.selectedTemplatesForPool[index];
      this.selectedTemplatesForPool[index] = this.selectedTemplatesForPool[index - 1];
      this.selectedTemplatesForPool[index - 1] = temp;
    }
  }

  moveTemplateDown(index: number): void {
    if (index < this.selectedTemplatesForPool.length - 1) {
      const temp = this.selectedTemplatesForPool[index];
      this.selectedTemplatesForPool[index] = this.selectedTemplatesForPool[index + 1];
      this.selectedTemplatesForPool[index + 1] = temp;
    }
  }

  removeTemplateFromPool(templateId: string): void {
    this.selectedTemplatesForPool = this.selectedTemplatesForPool.filter(id => id !== templateId);
  }

  getTemplateName(templateId: string): string {
    return this.letterTemplates.find(t => t.id === templateId)?.name || 'Unknown';
  }

  // ======================================================================
  // RULE CLONING
  // ======================================================================

  cloneRule(rule: ThankYouRuleDto): void {
    this.modal.confirm({
      nzTitle: 'Clona Regola',
      nzContent: `Vuoi clonare la regola "${rule.name}"? Verrà creata una copia con lo stesso pool di template.`,
      nzOkText: 'Clona',
      nzCancelText: 'Annulla',
      nzOnOk: () => {
        const newName = `${rule.name} (Copia)`;
        this.loading = true;
        (this.ruleService as any).cloneRule(rule.id, newName).subscribe({
          next: () => {
            this.message.success('Regola clonata con successo');
            this.loadRules();
          },
          error: () => {
            this.loading = false;
            this.message.error('Errore durante la clonazione');
          }
        });
      }
    });
  }

  // ======================================================================
  // HELPERS
  // ======================================================================

  isTemporaryRule(rule: ThankYouRuleDto): boolean {
    return !!(rule as any).validFrom || !!(rule as any).validUntil;
  }

  getValidityLabel(rule: ThankYouRuleDto): string {
    const ruleAny = rule as any;
    if (!this.isTemporaryRule(rule)) return '';
    
    const from = ruleAny.validFrom ? new Date(ruleAny.validFrom).toLocaleDateString('it-IT') : '∞';
    const to = ruleAny.validUntil ? new Date(ruleAny.validUntil).toLocaleDateString('it-IT') : '∞';
    
    return `${from} - ${to}`;
  }
}
