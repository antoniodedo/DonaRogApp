import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { EditorModule } from '@tinymce/tinymce-angular';

import { LetterTemplateRoutingModule } from './letter-template-routing.module';
import { LetterTemplateComponent } from './letter-template.component';


@NgModule({
  declarations: [
    LetterTemplateComponent
  ],
  imports: [
    SharedModule,
    LetterTemplateRoutingModule,
    EditorModule
  ]
})
export class LetterTemplateModule { }
