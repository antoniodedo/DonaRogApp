import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RecurrencesListComponent } from './recurrences-list/recurrences-list.component';
import { RecurrenceFormComponent } from './recurrence-form/recurrence-form.component';

const routes: Routes = [
  {
    path: '',
    component: RecurrencesListComponent,
  },
  {
    path: 'new',
    component: RecurrenceFormComponent,
  },
  {
    path: ':id/edit',
    component: RecurrenceFormComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class RecurrencesRoutingModule {}
