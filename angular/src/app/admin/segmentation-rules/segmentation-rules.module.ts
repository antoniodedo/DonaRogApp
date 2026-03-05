import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SegmentationRulesRoutingModule } from './segmentation-rules-routing.module';
import { SegmentationRuleListComponent } from './segmentation-rule-list/segmentation-rule-list.component';
import { SegmentationRuleFormComponent } from './segmentation-rule-form/segmentation-rule-form.component';
import { SegmentationRulePreviewComponent } from './segmentation-rule-preview/segmentation-rule-preview.component';
import { SegmentationBatchComponent } from './segmentation-batch/segmentation-batch.component';

// Ng-Zorro imports
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';

@NgModule({
  declarations: [
    SegmentationRuleListComponent,
    SegmentationRuleFormComponent,
    SegmentationRulePreviewComponent,
    SegmentationBatchComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    CoreModule,
    ThemeSharedModule,
    NgbModule,
    SegmentationRulesRoutingModule,
    // Ng-Zorro modules
    NzCardModule,
    NzTableModule,
    NzButtonModule,
    NzIconModule,
    NzTagModule,
    NzSwitchModule,
    NzSpaceModule,
    NzToolTipModule,
    NzFormModule,
    NzInputModule,
    NzSelectModule,
    NzDatePickerModule,
    NzInputNumberModule,
    NzAlertModule,
    NzModalModule,
    NzDividerModule,
    NzStatisticModule,
    NzCheckboxModule
  ]
})
export class SegmentationRulesModule { }
