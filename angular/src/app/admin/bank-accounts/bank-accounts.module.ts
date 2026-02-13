import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';

import { BankAccountsRoutingModule } from './bank-accounts-routing.module';
import { BankAccountsListComponent } from './bank-accounts-list/bank-accounts-list.component';
import { BankAccountFormComponent } from './bank-account-form/bank-account-form.component';

@NgModule({
  declarations: [],
  imports: [
    BankAccountsListComponent,
    BankAccountFormComponent,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    BankAccountsRoutingModule,
    ThemeSharedModule,
    NzTableModule,
    NzButtonModule,
    NzFormModule,
    NzInputModule,
    NzCardModule,
    NzModalModule,
    NzTagModule,
    NzPopconfirmModule,
  ],
})
export class BankAccountsModule {}
