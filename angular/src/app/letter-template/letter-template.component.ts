import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { LetterTemplateService} from '@proxy/letter-templates/letter-template.service';
import { LetterTemplateDto } from '@proxy/letter-templates/dto';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-letter-template',
  standalone: false,
  templateUrl: './letter-template.component.html',
  styleUrl: './letter-template.component.scss',
  providers:[ListService]
})
export class LetterTemplateComponent implements OnInit {
templates = { items: [], totalCount: 0 } as PagedResultDto<LetterTemplateDto>;
  form: FormGroup;

  selectedTemplate = {} as LetterTemplateDto; 
  isModalOpen = false; 

  constructor(
    public readonly list: ListService, 
    private templateService: LetterTemplateService,  
    private fb: FormBuilder) {}

  ngOnInit() {
    const titleStreamCreator = (query) => this.templateService.getList(query);

    this.list.hookToQuery(titleStreamCreator).subscribe((response) => {
      this.templates = response;
    });
  }

  createTemplate() {
    this.buildForm(); 
    this.isModalOpen = true;
  }

   editTemplate(id: string) {
    this.templateService.get(id).subscribe((title) => {
      this.selectedTemplate = title;
      this.buildForm();
      this.isModalOpen = true;
    });
  }

  buildForm() {
    this.form = this.fb.group({
      name: [this.selectedTemplate.name || '', Validators.required],
      description: [this.selectedTemplate.description || '', Validators.required],
      content: [this.selectedTemplate.content || '', Validators.required],
      isActive: [this.selectedTemplate.isActive || true, Validators.required]
    });
  }

  save() {
    if (this.form.invalid) {
      return;
    }

    const request = this.selectedTemplate.id
      ? this.templateService.update(this.selectedTemplate.id, this.form.value)
      : this.templateService.create(this.form.value);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.form.reset();
      this.list.get();
    });
  }
}
