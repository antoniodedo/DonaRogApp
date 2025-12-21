import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';


import { DonorRoutingModule } from './donor-routing.module';
import { DonorComponent } from './donor.component';
import { DonorTitleTableComponent } from './donor-title-table/donor-title-table.component';
import { DonorDetailsComponent } from './donor-details/donor-details.component';
import { DonorCardComponent } from './donor-card/donor-card.component';
import { DonorTabsComponent } from './donor-tabs/donor-tabs.component';
import { NoteModule } from '../note/note.module';


@NgModule({
  declarations: [
    DonorComponent,
    DonorTitleTableComponent,
    DonorDetailsComponent,
    DonorCardComponent,
    DonorTabsComponent
  ],
  imports: [
    SharedModule,
    DonorRoutingModule,
    NgbNavModule,
    NoteModule
  ]
})
export class DonorModule { }
