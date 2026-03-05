import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToasterService } from '@abp/ng.theme.shared';
import { SegmentationRuleService } from '@proxy/segmentation';
import type { CreateUpdateSegmentationRuleDto, SegmentationRuleDto } from '@proxy/segmentation';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-segmentation-rule-form',
  standalone: false,
  templateUrl: './segmentation-rule-form.component.html',
  styleUrls: ['./segmentation-rule-form.component.scss']
})
export class SegmentationRuleFormComponent implements OnInit {
  form!: FormGroup;
  ruleId?: string;
  isEditMode = false;
  loading = false;
  saving = false;

  // Available segments (TODO: load from API)
  segments = [
    { id: '1', name: 'VIP Donors' },
    { id: '2', name: 'Regular Donors' },
    { id: '3', name: 'Lapsed Donors' },
    { id: '4', name: 'New Donors' }
  ];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private segmentationService: SegmentationRuleService,
    private toaster: ToasterService
  ) {}

  ngOnInit(): void {
    this.buildForm();

    this.ruleId = this.route.snapshot.params['id'];
    this.isEditMode = !!this.ruleId;

    if (this.isEditMode && this.ruleId) {
      this.loadRule(this.ruleId);
    }
  }

  buildForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(128)]],
      description: ['', Validators.maxLength(500)],
      isActive: [true],
      priority: [10, [Validators.required, Validators.min(1)]],
      segmentId: ['', Validators.required],
      
      // RFM Scores
      minRecencyScore: [null, [Validators.min(1), Validators.max(5)]],
      maxRecencyScore: [null, [Validators.min(1), Validators.max(5)]],
      minFrequencyScore: [null, [Validators.min(1), Validators.max(5)]],
      maxFrequencyScore: [null, [Validators.min(1), Validators.max(5)]],
      minMonetaryScore: [null, [Validators.min(1), Validators.max(5)]],
      maxMonetaryScore: [null, [Validators.min(1), Validators.max(5)]],
      
      // Donation metrics
      minTotalDonated: [null, Validators.min(0)],
      maxTotalDonated: [null, Validators.min(0)],
      minDonationCount: [null, Validators.min(0)],
      maxDonationCount: [null, Validators.min(0)],
      minDaysSinceLastDonation: [null, Validators.min(0)],
      maxDaysSinceLastDonation: [null, Validators.min(0)],
      
      // Date ranges
      firstDonationAfter: [null],
      firstDonationBefore: [null],
      lastDonationAfter: [null],
      lastDonationBefore: [null]
    });
  }

  loadRule(id: string): void {
    this.loading = true;
    this.segmentationService
      .get(id)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (rule) => {
          this.form.patchValue(rule);
        },
        error: () => {
          this.toaster.error('SegmentationRule::FailedToLoadRule');
          this.router.navigate(['/admin/segmentation-rules']);
        }
      });
  }

  save(): void {
    if (this.form.invalid) {
      Object.keys(this.form.controls).forEach(key => {
        this.form.controls[key].markAsTouched();
      });
      return;
    }

    this.saving = true;
    const dto = this.form.value as CreateUpdateSegmentationRuleDto;

    const request = this.isEditMode && this.ruleId
      ? this.segmentationService.update(this.ruleId, dto)
      : this.segmentationService.create(dto);

    request
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: () => {
          this.toaster.success('AbpUi::SavedSuccessfully');
          this.router.navigate(['/admin/segmentation-rules']);
        },
        error: (error) => {
          this.toaster.error('AbpUi::Error');
        }
      });
  }

  cancel(): void {
    this.router.navigate(['/admin/segmentation-rules']);
  }
}
