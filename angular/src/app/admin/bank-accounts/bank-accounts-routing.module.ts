import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BankAccountsListComponent } from './bank-accounts-list/bank-accounts-list.component';

const routes: Routes = [
  {
    path: '',
    component: BankAccountsListComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class BankAccountsRoutingModule {}
