import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToasterService } from '@abp/ng.theme.shared';
import { SegmentationRuleService } from '@proxy/segmentation';
import type { SegmentEvaluationPreviewDto, DonorPreviewDto } from '@proxy/segmentation';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-segmentation-rule-preview',
  standalone: false,
  templateUrl: './segmentation-rule-preview.component.html',
  styleUrls: ['./segmentation-rule-preview.component.scss']
})
export class SegmentationRulePreviewComponent implements OnInit {
  ruleId?: string;
  preview?: SegmentEvaluationPreviewDto;
  loading = false;
  maxResults = 100;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private segmentationService: SegmentationRuleService,
    private toaster: ToasterService
  ) {}

  ngOnInit(): void {
    this.ruleId = this.route.snapshot.params['id'];
    if (this.ruleId) {
      this.loadPreview();
    }
  }

  loadPreview(): void {
    if (!this.ruleId) return;

    this.loading = true;
    this.segmentationService
      .previewRule(this.ruleId, this.maxResults)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (preview) => {
          this.preview = preview;
        },
        error: () => {
          this.toaster.error('SegmentationRule::FailedToLoadPreview');
          this.router.navigate(['/admin/segmentation-rules']);
        }
      });
  }

  refresh(): void {
    this.loadPreview();
  }

  back(): void {
    this.router.navigate(['/admin/segmentation-rules']);
  }

  editRule(): void {
    if (this.ruleId) {
      this.router.navigate(['/admin/segmentation-rules/edit', this.ruleId]);
    }
  }
}
