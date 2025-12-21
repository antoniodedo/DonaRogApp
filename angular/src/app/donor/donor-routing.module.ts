import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DonorComponent } from './donor.component';
import { DonorTitleTableComponent } from './donor-title-table/donor-title-table.component';
import { DonorDetailsComponent } from './donor-details/donor-details.component';

const routes: Routes = [{ path: '', component: DonorComponent }, 
                        { path: 'donor-title', component: DonorTitleTableComponent },
                        { path: 'donor-details/:id', component: DonorDetailsComponent }
                      ];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DonorRoutingModule { }
