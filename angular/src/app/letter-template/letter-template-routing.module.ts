import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LetterTemplateComponent } from './letter-template.component';

const routes: Routes = [{ path: '', component: LetterTemplateComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LetterTemplateRoutingModule { }
