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
    NzCollapseModule
  ]
})
export class ThankYouRulesComponent implements OnInit {
  rules: ThankYouRuleDto[] = [];
  loading = false;
  
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
  
  constructor(
    private ruleService: ThankYouRuleService,
    private letterTemplateService: LetterTemplateService,
    private projectService: ProjectService,
    private campaignService: CampaignService,
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
      
      // Actions
      createThankYou: [true, Validators.required],
      suggestedChannel: [null],
      suggestedTemplateId: [null]
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
    this.ruleForm.reset({
      priority: 100,
      isActive: true,
      createThankYou: true,
      donorCategories: [],
      subjectTypes: [],
      projectIds: [],
      campaignIds: []
    });
    this.isModalVisible = true;
  }
  
  showEditModal(rule: ThankYouRuleDto): void {
    this.isEditMode = true;
    this.currentRuleId = rule.id;
    
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
      createThankYou: rule.createThankYou,
      suggestedChannel: rule.suggestedChannel,
      suggestedTemplateId: rule.suggestedTemplateId
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
    
    const dto: CreateUpdateThankYouRuleDto = {
      ...this.ruleForm.value,
      donorCategories: this.ruleForm.value.donorCategories?.length > 0 ? this.ruleForm.value.donorCategories : undefined,
      subjectTypes: this.ruleForm.value.subjectTypes?.length > 0 ? this.ruleForm.value.subjectTypes : undefined,
      projectIds: this.ruleForm.value.projectIds?.length > 0 ? this.ruleForm.value.projectIds : undefined,
      campaignIds: this.ruleForm.value.campaignIds?.length > 0 ? this.ruleForm.value.campaignIds : undefined,
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
}
