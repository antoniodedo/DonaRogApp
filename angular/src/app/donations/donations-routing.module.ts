import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DonationsListComponent } from './donations-list/donations-list.component';
import { DonationFormComponent } from './donation-form/donation-form.component';
import { DonationVerifyComponent } from './donation-verify/donation-verify.component';
import { DonationDetailComponent } from './donation-detail/donation-detail.component';
import { ExternalDonationsDemoComponent } from './external-donations-demo/external-donations-demo.component';

const routes: Routes = [
  {
    path: '',
    component: DonationsListComponent,
  },
  {
    path: 'new',
    component: DonationVerifyComponent, // Use unified form for creation
  },
  {
    path: 'verify/:id',
    component: DonationVerifyComponent, // Use unified form for verification
  },
  {
    path: 'demo',
    component: ExternalDonationsDemoComponent,
  },
  {
    path: ':id',
    component: DonationDetailComponent,
  },
  {
    path: ':id/edit',
    component: DonationVerifyComponent, // Use unified form for editing
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DonationsRoutingModule {}
