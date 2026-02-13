import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { CampaignsRoutingModule } from './campaigns-routing.module';
import { CampaignsListComponent } from './campaigns-list/campaigns-list.component';
import { CampaignFormComponent } from './campaign-form/campaign-form.component';
import { CampaignDetailComponent } from './campaign-detail/campaign-detail.component';
import { DonorExtractionWizardComponent } from './donor-extraction-wizard/donor-extraction-wizard.component';
import { CampaignDonorsListComponent } from './campaign-donors-list/campaign-donors-list.component';
import { CampaignStatisticsComponent } from './campaign-statistics/campaign-statistics.component';
import { SharedModule } from 'src/app/shared/shared.module';

// NG-ZORRO imports
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzTransferModule } from 'ng-zorro-antd/transfer';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzProgressModule } from 'ng-zorro-antd/progress';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzListModule } from 'ng-zorro-antd/list';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    CampaignsListComponent,
    CampaignFormComponent,
    CampaignDetailComponent,
    DonorExtractionWizardComponent,
    CampaignDonorsListComponent,
    CampaignStatisticsComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    CampaignsRoutingModule,
    SharedModule,
    NzTableModule,
    NzButtonModule,
    NzFormModule,
    NzInputModule,
    NzSelectModule,
    NzDatePickerModule,
    NzCardModule,
    NzModalModule,
    NzIconModule,
    NzTagModule,
    NzBadgeModule,
    NzToolTipModule,
    NzDividerModule,
    NzSpaceModule,
    NzPopconfirmModule,
    NzTabsModule,
    NzStepsModule,
    NzStatisticModule,
    NzInputNumberModule,
    NzTransferModule,
    NzAlertModule,
    NzDescriptionsModule,
    NzProgressModule,
    NzRadioModule,
    NzDrawerModule,
    NzSpinModule,
    NzCheckboxModule,
    NzListModule
  ]
})
export class CampaignsModule { }
