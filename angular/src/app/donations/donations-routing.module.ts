import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DonationsListComponent } from './donations-list/donations-list.component';
import { DonationFormComponent } from './donation-form/donation-form.component';
import { DonationDetailComponent } from './donation-detail/donation-detail.component';
import { ExternalDonationsDemoComponent } from './external-donations-demo/external-donations-demo.component';

const routes: Routes = [
  {
    path: '',
    component: DonationsListComponent,
  },
  {
    path: 'new',
    component: DonationDetailComponent, // Create new donation
  },
  {
    path: 'demo',
    component: ExternalDonationsDemoComponent,
  },
  {
    path: ':id/edit',
    component: DonationDetailComponent, // Edit existing donation
  },
  {
    path: ':id',
    component: DonationDetailComponent, // View or verify donation (auto-detected based on status)
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DonationsRoutingModule {}
