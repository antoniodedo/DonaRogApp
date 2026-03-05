import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SegmentationRuleListComponent } from './segmentation-rule-list/segmentation-rule-list.component';
import { SegmentationRuleFormComponent } from './segmentation-rule-form/segmentation-rule-form.component';
import { SegmentationRulePreviewComponent } from './segmentation-rule-preview/segmentation-rule-preview.component';
import { SegmentationBatchComponent } from './segmentation-batch/segmentation-batch.component';

const routes: Routes = [
  {
    path: '',
    component: SegmentationRuleListComponent
  },
  {
    path: 'new',
    component: SegmentationRuleFormComponent
  },
  {
    path: 'edit/:id',
    component: SegmentationRuleFormComponent
  },
  {
    path: 'preview/:id',
    component: SegmentationRulePreviewComponent
  },
  {
    path: 'batch',
    component: SegmentationBatchComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SegmentationRulesRoutingModule { }
