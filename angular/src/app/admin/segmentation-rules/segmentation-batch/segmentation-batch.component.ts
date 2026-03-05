import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { ToasterService } from '@abp/ng.theme.shared';
import { SegmentationRuleService } from '@proxy/segmentation';
import type { SegmentationBatchResultDto } from '@proxy/segmentation';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-segmentation-batch',
  standalone: false,
  templateUrl: './segmentation-batch.component.html',
  styleUrls: ['./segmentation-batch.component.scss']
})
export class SegmentationBatchComponent implements OnInit {
  lastBatchResult?: SegmentationBatchResultDto | null;
  currentBatchResult?: SegmentationBatchResultDto;
  loading = false;
  running = false;

  constructor(
    private router: Router,
    private confirmation: ConfirmationService,
    private toaster: ToasterService,
    private segmentationService: SegmentationRuleService
  ) {}

  ngOnInit(): void {
    this.loadLastBatchResult();
  }

  loadLastBatchResult(): void {
    this.loading = true;
    this.segmentationService
      .getLastBatchResult()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (result) => {
          this.lastBatchResult = result;
        },
        error: () => {
          this.toaster.error('SegmentationRule::FailedToLoadBatchResult');
        }
      });
  }

  runBatch(): void {
    this.confirmation
      .warn(
        'SegmentationRule::ConfirmBatch',
        'Are you sure you want to run segmentation for all donors? This may take several minutes.'
      )
      .subscribe((status) => {
        if (status === Confirmation.Status.confirm) {
          this.executeBatch();
        }
      });
  }

  executeBatch(): void {
    this.running = true;
    this.currentBatchResult = undefined;

    this.segmentationService
      .runSegmentationBatch()
      .pipe(finalize(() => (this.running = false)))
      .subscribe({
        next: (result) => {
          this.currentBatchResult = result;
          this.lastBatchResult = result;
          this.toaster.success('SegmentationRule::BatchCompleted');
        },
        error: (error) => {
          this.toaster.error('SegmentationRule::BatchFailed');
        }
      });
  }

  back(): void {
    this.router.navigate(['/admin/segmentation-rules']);
  }

  formatDuration(seconds: number): string {
    if (seconds < 60) {
      return `${seconds.toFixed(1)}s`;
    }
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = Math.floor(seconds % 60);
    return `${minutes}m ${remainingSeconds}s`;
  }

  formatDateTime(dateTime: string): string {
    return new Date(dateTime).toLocaleString();
  }
}
