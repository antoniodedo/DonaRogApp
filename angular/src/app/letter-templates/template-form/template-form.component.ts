import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { LetterTemplateService } from '../../proxy/letter-templates/letter-template.service';
import { ProjectService } from '../../proxy/application/projects/project.service';
import { RecurrenceService } from '../../proxy/application/recurrences/recurrence.service';
import {
  LetterTemplateDto,
  CreateUpdateLetterTemplateDto,
} from '../../proxy/letter-templates/dto/models';
import { TemplateCategory } from '../../proxy/enums/communications/template-category.enum';
import { CommunicationType } from '../../proxy/enums/communications/communication-type.enum';
import { ProjectListDto } from '../../proxy/application/contracts/projects/dto/models';
import { RecurrenceDto } from '../../proxy/application/contracts/recurrences/dto/models';

@Component({
  selector: 'app-template-form',
  standalone: false,
  templateUrl: './template-form.component.html',
  styleUrls: ['./template-form.component.scss'],
})
export class TemplateFormComponent implements OnInit {
  template: LetterTemplateDto | null = null;
  templateId?: string;
  isEditMode = false;

  form!: FormGroup;
  loading = false;
  submitting = false;

  // Enum references
  TemplateCategory = TemplateCategory;
  CommunicationType = CommunicationType;

  // Dropdowns data
  projects: ProjectListDto[] = [];
  recurrences: RecurrenceDto[] = [];

  // TinyMCE Configuration - Self-hosted (GPL License)
  tinyMceConfig = {
    // Self-hosted configuration - no API key needed
    base_url: '/tinymce',
    suffix: '.min',
    promotion: false, // Disable upgrade prompts for self-hosted version
    height: 500,
    menubar: true,
    plugins: [
      'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview',
      'anchor', 'searchreplace', 'visualblocks', 'code', 'fullscreen',
      'insertdatetime', 'media', 'table', 'code', 'help', 'wordcount'
    ],
    toolbar: 'undo redo | formatselect | bold italic underline | ' +
      'alignleft aligncenter alignright alignjustify | ' +
      'bullist numlist outdent indent | removeformat | ' +
      'insertTag | code | help',
    setup: (editor: any) => {
      // Add custom button for inserting tags
      editor.ui.registry.addMenuButton('insertTag', {
        text: 'Inserisci Tag',
        icon: 'tag',
        fetch: (callback: any) => {
          const items = [
            {
              type: 'nestedmenuitem',
              text: 'Dati Donatore',
              getSubmenuItems: () => [
                { type: 'menuitem', text: 'Nome Completo', onAction: () => editor.insertContent('{{DonorName}}') },
                { type: 'menuitem', text: 'Titolo', onAction: () => editor.insertContent('{{DonorTitle}}') },
                { type: 'menuitem', text: 'Nome', onAction: () => editor.insertContent('{{DonorFirstName}}') },
                { type: 'menuitem', text: 'Cognome', onAction: () => editor.insertContent('{{DonorLastName}}') },
                { type: 'menuitem', text: 'Azienda', onAction: () => editor.insertContent('{{DonorCompanyName}}') },
                { type: 'menuitem', text: 'Email', onAction: () => editor.insertContent('{{DonorEmail}}') },
                { type: 'menuitem', text: 'Indirizzo Completo', onAction: () => editor.insertContent('{{DonorFullAddress}}') },
                { type: 'menuitem', text: 'Via', onAction: () => editor.insertContent('{{DonorStreet}}') },
                { type: 'menuitem', text: 'Città', onAction: () => editor.insertContent('{{DonorCity}}') },
                { type: 'menuitem', text: 'CAP', onAction: () => editor.insertContent('{{DonorPostalCode}}') },
              ]
            },
            {
              type: 'nestedmenuitem',
              text: 'Dati Donazione',
              getSubmenuItems: () => [
                { type: 'menuitem', text: 'Importo', onAction: () => editor.insertContent('{{DonationAmount}}') },
                { type: 'menuitem', text: 'Importo in Lettere', onAction: () => editor.insertContent('{{DonationAmountInWords}}') },
                { type: 'menuitem', text: 'Data', onAction: () => editor.insertContent('{{DonationDate}}') },
                { type: 'menuitem', text: 'Riferimento', onAction: () => editor.insertContent('{{DonationReference}}') },
              ]
            },
            {
              type: 'nestedmenuitem',
              text: 'Dati Progetto',
              getSubmenuItems: () => [
                { type: 'menuitem', text: 'Nome Progetto', onAction: () => editor.insertContent('{{ProjectName}}') },
                { type: 'menuitem', text: 'Codice', onAction: () => editor.insertContent('{{ProjectCode}}') },
                { type: 'menuitem', text: 'Descrizione', onAction: () => editor.insertContent('{{ProjectDescription}}') },
              ]
            },
            {
              type: 'nestedmenuitem',
              text: 'Dati Ricorrenza',
              getSubmenuItems: () => [
                { type: 'menuitem', text: 'Nome Ricorrenza', onAction: () => editor.insertContent('{{RecurrenceName}}') },
                { type: 'menuitem', text: 'Data', onAction: () => editor.insertContent('{{RecurrenceDate}}') },
                { type: 'menuitem', text: 'Anno', onAction: () => editor.insertContent('{{RecurrenceYear}}') },
              ]
            },
            {
              type: 'nestedmenuitem',
              text: 'Altri',
              getSubmenuItems: () => [
                { type: 'menuitem', text: 'Data Corrente', onAction: () => editor.insertContent('{{CurrentDate}}') },
                { type: 'menuitem', text: 'Anno Corrente', onAction: () => editor.insertContent('{{CurrentYear}}') },
                { type: 'menuitem', text: 'Nome Organizzazione', onAction: () => editor.insertContent('{{OrganizationName}}') },
                { type: 'menuitem', text: 'Carta Intestata', onAction: () => editor.insertContent('{{Letterhead}}') },
                { type: 'menuitem', text: 'Firma', onAction: () => editor.insertContent('{{Signature}}') },
              ]
            }
          ];
          callback(items);
        }
      });
    }
  };

  constructor(
    private fb: FormBuilder,
    private templateService: LetterTemplateService,
    private projectService: ProjectService,
    private recurrenceService: RecurrenceService,
    private message: NzMessageService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadProjects();
    this.loadRecurrences();

    // Check if we're in edit mode
    this.route.params.subscribe(params => {
      this.templateId = params['id'];
      this.isEditMode = !!this.templateId;

      if (this.isEditMode && this.templateId) {
        this.loadTemplate(this.templateId);
      }
    });
  }

  loadTemplate(id: string): void {
    this.loading = true;
    this.templateService.get(id).subscribe({
      next: template => {
        this.template = template;
        this.patchFormValues();
        this.loading = false;
      },
      error: err => {
        this.message.error('Errore nel caricamento del template');
        this.loading = false;
        console.error(err);
      },
    });
  }

  initForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(256)]],
      description: ['', Validators.maxLength(1000)],
      content: ['', Validators.required],
      category: [TemplateCategory.ThankYou, Validators.required],
      language: ['it', [Validators.required, Validators.maxLength(5)]],
      communicationType: [null],
      projectId: [null],
      recurrenceId: [null],
      minAmount: [null],
      maxAmount: [null],
      isForNewDonor: [false],
      isPlural: [false],
      isActive: [true],
      isDefault: [false],
    });
  }

  patchFormValues(): void {
    if (this.template) {
      this.form.patchValue({
        name: this.template.name,
        description: this.template.description,
        content: this.template.content,
        category: this.template.category,
        language: this.template.language,
        communicationType: this.template.communicationType,
        projectId: this.template.projectId,
        recurrenceId: this.template.recurrenceId,
        minAmount: this.template.minAmount,
        maxAmount: this.template.maxAmount,
        isForNewDonor: this.template.isForNewDonor,
        isPlural: this.template.isPlural,
        isActive: this.template.isActive,
        isDefault: this.template.isDefault,
      });
    }
  }

  loadProjects(): void {
    this.projectService.getProjectList({ maxResultCount: 1000 }).subscribe({
      next: result => {
        this.projects = result.items || [];
      },
      error: err => {
        console.error('Errore nel caricamento progetti', err);
      },
    });
  }

  loadRecurrences(): void {
    this.recurrenceService.getList({ maxResultCount: 1000 }).subscribe({
      next: result => {
        this.recurrences = result.items || [];
      },
      error: err => {
        console.error('Errore nel caricamento ricorrenze', err);
      },
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      Object.values(this.form.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      return;
    }

    this.submitting = true;
    const formValue: CreateUpdateLetterTemplateDto = this.form.value;

    const request$ = this.template
      ? this.templateService.update(this.template.id, formValue)
      : this.templateService.create(formValue);

    request$.subscribe({
      next: () => {
        this.message.success(
          this.template ? 'Template aggiornato con successo' : 'Template creato con successo'
        );
        this.router.navigate(['/admin/letter-templates']);
        this.submitting = false;
      },
      error: err => {
        this.message.error('Errore nel salvataggio del template');
        this.submitting = false;
        console.error(err);
      },
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/letter-templates']);
  }
}
