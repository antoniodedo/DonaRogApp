import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EditorModule, TINYMCE_SCRIPT_SRC } from '@tinymce/tinymce-angular';

import { LetterTemplatesRoutingModule } from './letter-templates-routing.module';
import { TemplateListComponent } from './template-list/template-list.component';
import { TemplateFormComponent } from './template-form/template-form.component';
import { TemplatePreviewComponent } from './template-preview/template-preview.component';

// NG-ZORRO Modules
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzMessageModule } from 'ng-zorro-antd/message';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzBreadCrumbModule } from 'ng-zorro-antd/breadcrumb';

@NgModule({
  declarations: [
    TemplateListComponent,
    TemplateFormComponent,
    TemplatePreviewComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LetterTemplatesRoutingModule,
    // TinyMCE
    EditorModule,
    // NG-ZORRO
    NzTableModule,
    NzButtonModule,
    NzInputModule,
    NzSelectModule,
    NzModalModule,
    NzFormModule,
    NzIconModule,
    NzTagModule,
    NzCardModule,
    NzSpinModule,
    NzMessageModule,
    NzPopconfirmModule,
    NzDividerModule,
    NzToolTipModule,
    NzBadgeModule,
    NzSpaceModule,
    NzEmptyModule,
    NzDrawerModule,
    NzUploadModule,
    NzInputNumberModule,
    NzSwitchModule,
    NzCheckboxModule,
    NzDropDownModule,
    NzAlertModule,
    NzBreadCrumbModule
  ],
  providers: [
    { provide: TINYMCE_SCRIPT_SRC, useValue: 'tinymce/tinymce.min.js' }
  ]
})
export class LetterTemplatesModule { }
