import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { WelcomeRoutingModule } from './welcome-routing.module';
import { WelcomeComponent } from './welcome.component';

import { NzCardModule } from 'ng-zorro-antd/card';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzAlertModule } from 'ng-zorro-antd/alert';

import { NgChartsModule } from 'ng2-charts';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    WelcomeRoutingModule,
    NzCardModule,
    NzStatisticModule,
    NzGridModule,
    NzIconModule,
    NzSpinModule,
    NzEmptyModule,
    NzSelectModule,
    NzDatePickerModule,
    NzBadgeModule,
    NzAlertModule,
    NgChartsModule
  ],
  declarations: [WelcomeComponent],
  exports: [WelcomeComponent]
})
export class WelcomeModule { }
