import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { DonorsRoutingModule } from './donors-routing.module';
import { DonorListComponent } from './donor-list/donor-list.component';
import { DonorDetailComponent } from './donor-detail/donor-detail.component';
import { DonorFormComponent } from './donor-form/donor-form.component';
import { DonorEmailsComponent } from './donor-emails/donor-emails.component';
import { DonorAddressesComponent } from './donor-addresses/donor-addresses.component';
import { DonorContactsComponent } from './donor-contacts/donor-contacts.component';
import { DonorNotesComponent } from './donor-notes/donor-notes.component';
import { DonorTagsComponent } from './donor-tags/donor-tags.component';
import { DonorAttachmentsComponent } from './donor-attachments/donor-attachments.component';

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
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzTimelineModule } from 'ng-zorro-antd/timeline';
import { NzAutocompleteModule } from 'ng-zorro-antd/auto-complete';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { NzUploadModule } from 'ng-zorro-antd/upload';

@NgModule({
  declarations: [
    DonorListComponent,
    DonorDetailComponent,
    DonorFormComponent,
    DonorEmailsComponent,
    DonorAddressesComponent,
    DonorContactsComponent,
    DonorNotesComponent,
    DonorTagsComponent,
    DonorAttachmentsComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    DonorsRoutingModule,
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
    NzRadioModule,
    NzSwitchModule,
    NzStatisticModule,
    NzAlertModule,
    NzToolTipModule,
    NzBadgeModule,
    NzGridModule,
    NzSpaceModule,
    NzEmptyModule,
    NzCheckboxModule,
    NzTimelineModule,
    NzAutocompleteModule,
    NzDrawerModule,
    NzUploadModule
  ]
})
export class DonorsModule { }
