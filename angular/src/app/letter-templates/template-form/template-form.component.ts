import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { LetterTemplateService } from '../../proxy/letter-templates/letter-template.service';
import {
  LetterTemplateDto,
  CreateUpdateLetterTemplateDto,
} from '../../proxy/letter-templates/dto/models';
import { TemplateCategory } from '../../proxy/enums/communications/template-category.enum';
import { CommunicationType } from '../../proxy/enums/communications/communication-type.enum';
import { ThankYouRuleService } from '../../proxy/communications/thank-you-rules/thank-you-rule.service';

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

  // Associated rules
  associatedRules: any[] = [];
  
  // Available rules for pool management
  availableRules: any[] = [];
  isAddToPoolModalVisible = false;
  selectedRuleForPool?: string;
  selectedPriorityForPool = 1;

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
    private ruleService: ThankYouRuleService,
    private message: NzMessageService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initForm();

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
        this.associatedRules = (template as any).associatedRules || [];
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
        isPlural: (this.template as any).isPlural,
        isActive: this.template.isActive,
        isDefault: (this.template as any).isDefault,
      });
    }
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
        this.router.navigate(['/letter-templates']);
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
    this.router.navigate(['/letter-templates']);
  }

  // ======================================================================
  // POOL TEMPLATE MANAGEMENT
  // ======================================================================

  loadAvailableRules(): void {
    this.ruleService.getList({ maxResultCount: 1000 }).subscribe({
      next: (result) => {
        this.availableRules = result.items || [];
      },
      error: (err) => {
        console.error('Errore caricamento regole', err);
      }
    });
  }

  showAddToPoolModal(): void {
    if (!this.template?.id) {
      this.message.warning('Salva prima il template per poterlo associare a una regola');
      return;
    }
    
    this.loadAvailableRules();
    this.selectedRuleForPool = undefined;
    this.selectedPriorityForPool = 1;
    this.isAddToPoolModalVisible = true;
  }

  addTemplateToPool(): void {
    if (!this.selectedRuleForPool || !this.template?.id) {
      return;
    }

    this.loading = true;
    (this.ruleService as any).addTemplateToPool(
      this.selectedRuleForPool,
      this.template.id,
      this.selectedPriorityForPool,
      true
    ).subscribe({
      next: () => {
        this.message.success('Template aggiunto al pool della regola');
        this.isAddToPoolModalVisible = false;
        this.loadTemplate(this.template!.id);
      },
      error: (err: any) => {
        this.loading = false;
        this.message.error('Errore nell\'aggiunta al pool: ' + (err.error?.error?.message || 'Errore sconosciuto'));
      }
    });
  }

  removeFromPool(ruleId: string): void {
    if (!this.template?.id) return;

    this.loading = true;
    (this.ruleService as any).removeTemplateFromPool(ruleId, this.template.id).subscribe({
      next: () => {
        this.message.success('Template rimosso dal pool');
        this.loadTemplate(this.template!.id);
      },
      error: () => {
        this.loading = false;
        this.message.error('Errore nella rimozione dal pool');
      }
    });
  }

  cancelAddToPool(): void {
    this.isAddToPoolModalVisible = false;
  }
}
