import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { DonorNotesComponent } from './donor-notes/donor-notes.component';



@NgModule({
  declarations: [
    DonorNotesComponent
  ],
  imports: [
    SharedModule
  ],
  exports: [
    DonorNotesComponent
  ]
})
export class NoteModule { }
