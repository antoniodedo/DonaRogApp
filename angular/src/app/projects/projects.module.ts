import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ProjectsRoutingModule } from './projects-routing.module';
import { ProjectListComponent } from './project-list/project-list.component';
import { ProjectDetailComponent } from './project-detail/project-detail.component';
import { ProjectFormComponent } from './project-form/project-form.component';
import { ProjectStatisticsComponent } from './project-statistics/project-statistics.component';
import { ProjectDocumentsComponent } from './project-documents/project-documents.component';

// NG-ZORRO Modules
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzMessageModule } from 'ng-zorro-antd/message';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzProgressModule } from 'ng-zorro-antd/progress';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzStepsModule } from 'ng-zorro-antd/steps';

@NgModule({
  declarations: [
    ProjectListComponent,
    ProjectDetailComponent,
    ProjectFormComponent,
    ProjectStatisticsComponent,
    ProjectDocumentsComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ProjectsRoutingModule,
    // NG-ZORRO
    NzTableModule,
    NzButtonModule,
    NzInputModule,
    NzSelectModule,
    NzModalModule,
    NzFormModule,
    NzIconModule,
    NzTagModule,
    NzCardModule,
    NzDescriptionsModule,
    NzTabsModule,
    NzSpinModule,
    NzMessageModule,
    NzPopconfirmModule,
    NzDividerModule,
    NzDatePickerModule,
    NzStatisticModule,
    NzAlertModule,
    NzToolTipModule,
    NzBadgeModule,
    NzGridModule,
    NzSpaceModule,
    NzEmptyModule,
    NzProgressModule,
    NzDrawerModule,
    NzUploadModule,
    NzListModule,
    NzInputNumberModule,
    NzStepsModule
  ]
})
export class ProjectsModule { }
