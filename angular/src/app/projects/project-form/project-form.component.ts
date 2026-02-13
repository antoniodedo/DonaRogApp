import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ProjectService } from '../../proxy/application/projects/project.service';
import { ProjectDto, CreateProjectDto, UpdateProjectDto } from '../../proxy/application/contracts/projects/dto/models';
import { ProjectCategory } from '../../proxy/enums/projects/project-category.enum';
import { ProjectStatus } from '../../proxy/enums/projects/project-status.enum';

@Component({
  selector: 'app-project-form',
  standalone: false,
  templateUrl: './project-form.component.html',
  styleUrls: ['./project-form.component.scss']
})
export class ProjectFormComponent implements OnInit {
  @Input() visible = false;
  @Input() project: ProjectDto | null = null;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() saved = new EventEmitter<void>();

  form!: FormGroup;
  saving = false;
  
  ProjectCategory = ProjectCategory;
  ProjectStatus = ProjectStatus;

  get isEditMode(): boolean {
    return !!this.project;
  }

  constructor(
    private fb: FormBuilder,
    private projectService: ProjectService,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.initForm();
    if (this.project) {
      this.patchForm();
    }
  }

  private initForm(): void {
    this.form = this.fb.group({
      code: ['', Validators.maxLength(50)],
      name: ['', Validators.maxLength(200)],
      description: ['', Validators.maxLength(2000)],
      status: [ProjectStatus.Active],
      endDate: [null],
      targetAmount: [null, [Validators.min(0)]],
      currency: ['EUR'],
      responsiblePerson: ['', Validators.maxLength(100)],
      responsibleEmail: ['', Validators.maxLength(256)],
      responsiblePhone: ['', Validators.maxLength(50)],
      location: ['', Validators.maxLength(200)],
      latitude: [null, [Validators.min(-90), Validators.max(90)]],
      longitude: [null, [Validators.min(-180), Validators.max(180)]],
      mainImageUrl: ['', Validators.maxLength(1000)],
      thumbnailUrl: ['', Validators.maxLength(1000)]
    });
  }

  private patchForm(): void {
    if (!this.project) return;
    
    this.form.patchValue({
      code: this.project.code,
      name: this.project.name,
      description: this.project.description,
      status: this.project.status,
      endDate: this.project.endDate ? new Date(this.project.endDate) : null,
      targetAmount: this.project.targetAmount,
      currency: this.project.currency || 'EUR',
      responsiblePerson: this.project.responsiblePerson,
      responsibleEmail: this.project.responsibleEmail,
      responsiblePhone: this.project.responsiblePhone,
      location: this.project.location,
      latitude: this.project.latitude,
      longitude: this.project.longitude,
      mainImageUrl: this.project.mainImageUrl,
      thumbnailUrl: this.project.thumbnailUrl
    });
  }

  close(): void {
    this.visibleChange.emit(false);
  }

  private sanitizeValue(value: any): any {
    // Convert empty strings to null/undefined
    if (typeof value === 'string' && value.trim() === '') {
      return undefined;
    }
    return value;
  }

  save(): void {
    if (this.form.invalid) {
      Object.keys(this.form.controls).forEach(key => {
        const control = this.form.get(key);
        control?.markAsDirty();
        control?.updateValueAndValidity();
      });
      return;
    }

    this.saving = true;
    const formValue = this.form.value;

    if (this.isEditMode) {
      const dto: UpdateProjectDto = {
        code: this.sanitizeValue(formValue.code),
        name: this.sanitizeValue(formValue.name),
        description: this.sanitizeValue(formValue.description),
        category: this.project!.category, // Keep existing category
        status: formValue.status,
        startDate: this.project!.startDate, // Keep existing startDate
        endDate: formValue.endDate?.toISOString(),
        targetAmount: formValue.targetAmount,
        currency: formValue.currency,
        responsiblePerson: this.sanitizeValue(formValue.responsiblePerson),
        responsibleEmail: this.sanitizeValue(formValue.responsibleEmail),
        responsiblePhone: this.sanitizeValue(formValue.responsiblePhone),
        location: this.sanitizeValue(formValue.location),
        latitude: formValue.latitude,
        longitude: formValue.longitude,
        mainImageUrl: this.sanitizeValue(formValue.mainImageUrl),
        thumbnailUrl: this.sanitizeValue(formValue.thumbnailUrl)
      };

      this.projectService.update(this.project!.id, dto).subscribe({
        next: () => {
          this.message.success('Progetto aggiornato con successo');
          this.saving = false;
          this.saved.emit();
          this.close();
        },
        error: (err) => {
          this.message.error('Errore nell\'aggiornamento del progetto');
          this.saving = false;
        }
      });
    } else {
      const dto: CreateProjectDto = {
        code: this.sanitizeValue(formValue.code),
        name: this.sanitizeValue(formValue.name),
        description: this.sanitizeValue(formValue.description),
        category: ProjectCategory.Other, // Default category
        startDate: new Date().toISOString(), // Default to today
        endDate: formValue.endDate?.toISOString(),
        targetAmount: formValue.targetAmount,
        currency: formValue.currency,
        responsiblePerson: this.sanitizeValue(formValue.responsiblePerson),
        responsibleEmail: this.sanitizeValue(formValue.responsibleEmail),
        responsiblePhone: this.sanitizeValue(formValue.responsiblePhone),
        location: this.sanitizeValue(formValue.location),
        latitude: formValue.latitude,
        longitude: formValue.longitude,
        mainImageUrl: this.sanitizeValue(formValue.mainImageUrl),
        thumbnailUrl: this.sanitizeValue(formValue.thumbnailUrl)
      };

      this.projectService.create(dto).subscribe({
        next: () => {
          this.message.success('Progetto creato con successo');
          this.saving = false;
          this.saved.emit();
          this.close();
        },
        error: (err) => {
          this.message.error('Errore nella creazione del progetto');
          this.saving = false;
        }
      });
    }
  }
}
