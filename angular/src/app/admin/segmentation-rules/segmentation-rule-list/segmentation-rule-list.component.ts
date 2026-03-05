import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { ToasterService } from '@abp/ng.theme.shared';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { SegmentationRuleService } from '@proxy/segmentation';
import type { SegmentationRuleDto, CreateUpdateSegmentationRuleDto } from '@proxy/segmentation';

@Component({
  selector: 'app-segmentation-rule-list',
  standalone: false,
  templateUrl: './segmentation-rule-list.component.html',
  styleUrls: ['./segmentation-rule-list.component.scss'],
  providers: [ListService]
})
export class SegmentationRuleListComponent implements OnInit {
  rules = { items: [], totalCount: 0 } as PagedResultDto<SegmentationRuleDto>;
  loading = false;

  // Modal state
  isModalVisible = false;
  isEditMode = false;
  currentRuleId?: string;
  ruleForm!: FormGroup;

  // Reference data
  segments: any[] = [];

  constructor(
    public readonly list: ListService,
    private ruleService: SegmentationRuleService,
    private confirmation: ConfirmationService,
    private toaster: ToasterService,
    private router: Router,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadSegments();
    
    const streamCreator = (query) => this.ruleService.getList(query);
    
    this.list.hookToQuery(streamCreator).subscribe((response) => {
      this.rules = response;
    });
  }

  initForm(): void {
    this.ruleForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(128)]],
      description: ['', Validators.maxLength(500)],
      priority: [100, [Validators.required, Validators.min(1)]],
      segmentId: [null, Validators.required],
      isActive: [true],
      
      // RFM Scores
      minRecencyScore: [null],
      maxRecencyScore: [null],
      minFrequencyScore: [null],
      maxFrequencyScore: [null],
      minMonetaryScore: [null],
      maxMonetaryScore: [null],
      
      // Raw values
      minTotalDonated: [null],
      maxTotalDonated: [null],
      minDonationCount: [null],
      maxDonationCount: [null],
      minDaysSinceLastDonation: [null],
      maxDaysSinceLastDonation: [null],
      
      // Dates
      firstDonationAfter: [null],
      firstDonationBefore: [null],
      lastDonationAfter: [null],
      lastDonationBefore: [null]
    });
  }

  loadSegments(): void {
    this.ruleService.getAvailableSegments().subscribe({
      next: (segments) => {
        this.segments = segments;
      },
      error: (error) => {
        console.error('Error loading segments:', error);
        this.toaster.error('Error loading segments');
        this.segments = [];
      }
    });
  }

  showCreateModal(): void {
    this.isEditMode = false;
    this.currentRuleId = undefined;
    this.ruleForm.reset({
      priority: 100,
      isActive: true
    });
    this.isModalVisible = true;
  }

  showEditModal(rule: SegmentationRuleDto): void {
    this.isEditMode = true;
    this.currentRuleId = rule.id;
    
    this.ruleForm.patchValue({
      name: rule.name,
      description: rule.description,
      priority: rule.priority,
      segmentId: rule.segmentId,
      isActive: rule.isActive,
      
      minRecencyScore: rule.minRecencyScore,
      maxRecencyScore: rule.maxRecencyScore,
      minFrequencyScore: rule.minFrequencyScore,
      maxFrequencyScore: rule.maxFrequencyScore,
      minMonetaryScore: rule.minMonetaryScore,
      maxMonetaryScore: rule.maxMonetaryScore,
      
      minTotalDonated: rule.minTotalDonated,
      maxTotalDonated: rule.maxTotalDonated,
      minDonationCount: rule.minDonationCount,
      maxDonationCount: rule.maxDonationCount,
      minDaysSinceLastDonation: rule.minDaysSinceLastDonation,
      maxDaysSinceLastDonation: rule.maxDaysSinceLastDonation,
      
      firstDonationAfter: rule.firstDonationAfter,
      firstDonationBefore: rule.firstDonationBefore,
      lastDonationAfter: rule.lastDonationAfter,
      lastDonationBefore: rule.lastDonationBefore
    });
    
    this.isModalVisible = true;
  }

  handleModalCancel(): void {
    this.isModalVisible = false;
    this.ruleForm.reset();
  }

  saveRule(): void {
    if (this.ruleForm.invalid) {
      Object.values(this.ruleForm.controls).forEach(control => {
        control.markAsDirty();
        control.updateValueAndValidity();
      });
      return;
    }

    this.loading = true;
    const formValue = this.ruleForm.value;
    
    const dto: CreateUpdateSegmentationRuleDto = {
      name: formValue.name,
      description: formValue.description,
      priority: formValue.priority,
      segmentId: formValue.segmentId,
      isActive: formValue.isActive,
      
      minRecencyScore: formValue.minRecencyScore,
      maxRecencyScore: formValue.maxRecencyScore,
      minFrequencyScore: formValue.minFrequencyScore,
      maxFrequencyScore: formValue.maxFrequencyScore,
      minMonetaryScore: formValue.minMonetaryScore,
      maxMonetaryScore: formValue.maxMonetaryScore,
      
      minTotalDonated: formValue.minTotalDonated,
      maxTotalDonated: formValue.maxTotalDonated,
      minDonationCount: formValue.minDonationCount,
      maxDonationCount: formValue.maxDonationCount,
      minDaysSinceLastDonation: formValue.minDaysSinceLastDonation,
      maxDaysSinceLastDonation: formValue.maxDaysSinceLastDonation,
      
      firstDonationAfter: formValue.firstDonationAfter,
      firstDonationBefore: formValue.firstDonationBefore,
      lastDonationAfter: formValue.lastDonationAfter,
      lastDonationBefore: formValue.lastDonationBefore
    };

    const request = this.isEditMode
      ? this.ruleService.update(this.currentRuleId!, dto)
      : this.ruleService.create(dto);

    request.subscribe({
      next: () => {
        this.toaster.success(this.isEditMode ? 'Rule updated successfully' : 'Rule created successfully');
        this.isModalVisible = false;
        this.ruleForm.reset();
        this.list.get();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error saving rule:', error);
        this.toaster.error('Error saving rule');
        this.loading = false;
      }
    });
  }

  createRule(): void {
    this.showCreateModal();
  }

  editRule(id: string): void {
    const rule = this.rules.items.find(r => r.id === id);
    if (rule) {
      this.showEditModal(rule);
    }
  }

  deleteRule(id: string): void {
    this.confirmation.warn('SegmentationRule::ConfirmDelete', 'AbpUi::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.ruleService.delete(id).subscribe(() => {
          this.toaster.success('AbpUi::SuccessfullyDeleted');
          this.list.get();
        });
      }
    });
  }

  toggleActive(rule: SegmentationRuleDto): void {
    this.ruleService.toggleActive(rule.id).subscribe(() => {
      this.toaster.success('AbpUi::SavedSuccessfully');
      this.list.get();
    });
  }

  previewRule(id: string): void {
    this.router.navigate(['/admin/segmentation-rules/preview', id]);
  }

  applyRuleManually(id: string, name: string): void {
    this.confirmation.info(
      'SegmentationRule::ApplyManually',
      `Are you sure you want to apply rule "${name}" to all donors? This may take some time.`
    ).subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.ruleService.applyRuleManually(id).subscribe((count) => {
          this.toaster.success(`Rule applied successfully! ${count} donors assigned.`);
          this.list.get();
        });
      }
    });
  }

  navigateToBatch(): void {
    this.router.navigate(['/admin/segmentation-rules/batch']);
  }

  getActiveRulesCount(): number {
    return this.rules.items.filter(r => r.isActive).length;
  }

  getInactiveRulesCount(): number {
    return this.rules.items.filter(r => !r.isActive).length;
  }
}
