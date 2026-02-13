import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DonorListComponent } from './donor-list/donor-list.component';
import { DonorDetailComponent } from './donor-detail/donor-detail.component';

const routes: Routes = [
  {
    path: '',
    component: DonorListComponent
  },
  {
    path: ':id',
    component: DonorDetailComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DonorsRoutingModule { }
