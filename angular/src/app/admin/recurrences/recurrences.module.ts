import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

// Ng-Zorro Ant Design modules
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzMessageModule } from 'ng-zorro-antd/message';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzTypographyModule } from 'ng-zorro-antd/typography';

// Components
import { RecurrencesListComponent } from './recurrences-list/recurrences-list.component';
import { RecurrenceFormComponent } from './recurrence-form/recurrence-form.component';

// Routing
import { RecurrencesRoutingModule } from './recurrences-routing.module';

@NgModule({
  declarations: [RecurrencesListComponent, RecurrenceFormComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RecurrencesRoutingModule,
    // Ng-Zorro modules
    NzTableModule,
    NzButtonModule,
    NzIconModule,
    NzCardModule,
    NzFormModule,
    NzInputModule,
    NzSelectModule,
    NzDatePickerModule,
    NzInputNumberModule,
    NzSpaceModule,
    NzDividerModule,
    NzModalModule,
    NzMessageModule,
    NzTagModule,
    NzToolTipModule,
    NzDropDownModule,
    NzDrawerModule,
    NzStatisticModule,
    NzRadioModule,
    NzBadgeModule,
    NzEmptyModule,
    NzSpinModule,
    NzAlertModule,
    NzDescriptionsModule,
    NzTypographyModule,
  ],
})
export class RecurrencesModule {}
