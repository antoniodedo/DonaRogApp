import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'print-batches',
    loadComponent: () => import('./print-batches/print-batch-list.component').then(m => m.PrintBatchListComponent)
  },
  {
    path: 'thank-you-rules',
    loadComponent: () => import('./thank-you-rules/thank-you-rules.component').then(m => m.ThankYouRulesComponent)
  },
  {
    path: '',
    redirectTo: 'print-batches',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CommunicationsRoutingModule { }
