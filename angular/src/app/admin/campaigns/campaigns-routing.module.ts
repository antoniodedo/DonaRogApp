import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CampaignsListComponent } from './campaigns-list/campaigns-list.component';
import { CampaignDetailComponent } from './campaign-detail/campaign-detail.component';

const routes: Routes = [
  {
    path: '',
    component: CampaignsListComponent,
    data: { title: 'Campagne' }
  },
  {
    path: ':id',
    component: CampaignDetailComponent,
    data: { title: 'Dettaglio Campagna' }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CampaignsRoutingModule { }
