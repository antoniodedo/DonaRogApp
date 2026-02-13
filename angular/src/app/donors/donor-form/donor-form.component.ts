import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { DonorService } from '../../proxy/donors/donor.service';
import { DonorDto, CreateDonorDto, UpdateDonorDto } from '../../proxy/donors/dtos/models';
import { SubjectType } from '../../proxy/enums/donors/subject-type.enum';
import { Gender } from '../../proxy/enums/donors/gender.enum';
import { OrganizationType } from '../../proxy/enums/donors/organization-type.enum';
import { LegalForm } from '../../proxy/enums/donors/legal-form.enum';
import { DonorOrigin } from '../../proxy/enums/donors/donor-origin.enum';
import { TitleService } from '../../proxy/titles/title.service';
import type { TitleDto } from '../../proxy/titles/models';

@Component({
  selector: 'app-donor-form',
  standalone: false,
  templateUrl: './donor-form.component.html',
  styleUrls: ['./donor-form.component.scss']
})
export class DonorFormComponent implements OnInit {
  @Input() visible = false;
  @Input() donor: DonorDto | null = null;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() saved = new EventEmitter<void>();

  form!: FormGroup;
  saving = false;
  titles: TitleDto[] = [];
  
  SubjectType = SubjectType;
  Gender = Gender;
  OrganizationType = OrganizationType;
  LegalForm = LegalForm;
  DonorOrigin = DonorOrigin;

  get isEditMode(): boolean {
    return !!this.donor;
  }

  get isIndividual(): boolean {
    return this.form?.get('subjectType')?.value === SubjectType.Individual;
  }

  constructor(
    private fb: FormBuilder,
    private donorService: DonorService,
    private titleService: TitleService,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadTitles();
    if (this.donor) {
      this.patchForm();
    }
  }

  private loadTitles(): void {
    this.titleService.getAll().subscribe({
      next: (result) => {
        this.titles = result.items || [];
      },
      error: () => {
        // Se fallisce, usiamo titoli statici di fallback
        this.titles = [
          { id: '', code: 'SIG', name: 'Signore', abbreviation: 'Sig.' },
          { id: '', code: 'SIGRA', name: 'Signora', abbreviation: 'Sig.ra' },
          { id: '', code: 'DOTT', name: 'Dottore', abbreviation: 'Dott.' },
          { id: '', code: 'DOTTSSA', name: 'Dottoressa', abbreviation: 'Dott.ssa' },
          { id: '', code: 'ING', name: 'Ingegnere', abbreviation: 'Ing.' },
          { id: '', code: 'AVV', name: 'Avvocato', abbreviation: 'Avv.' },
          { id: '', code: 'PROF', name: 'Professore', abbreviation: 'Prof.' },
          { id: '', code: 'PROFSSA', name: 'Professoressa', abbreviation: 'Prof.ssa' }
        ] as TitleDto[];
      }
    });
  }

  private initForm(): void {
    this.form = this.fb.group({
      subjectType: [SubjectType.Individual, Validators.required],
      // Individuo
      titleId: [null],
      firstName: [''],
      lastName: [''],
      gender: [Gender.Unspecified],
      birthDate: [null],
      taxCode: [''],
      // Organizzazione
      companyName: [''],
      organizationType: [OrganizationType.Other],
      legalForm: [LegalForm.Other],
      vatNumber: [''],
      // Comune
      email: ['', Validators.email],
      phoneNumber: [''],
      origin: [DonorOrigin.Unknown],
      preferredLanguage: ['IT'],
      notes: ['']
    });
  }

  private patchForm(): void {
    if (!this.donor) return;
    
    this.form.patchValue({
      subjectType: this.donor.subjectType,
      titleId: this.donor.titleId,
      firstName: this.donor.firstName,
      lastName: this.donor.lastName,
      gender: this.donor.gender,
      birthDate: this.donor.birthDate ? new Date(this.donor.birthDate) : null,
      taxCode: this.donor.taxCode,
      companyName: this.donor.companyName,
      organizationType: this.donor.organizationType,
      legalForm: this.donor.legalForm,
      vatNumber: this.donor.vatNumber,
      origin: this.donor.origin,
      preferredLanguage: this.donor.preferredLanguage,
      notes: this.donor.notes
    });
  }

  close(): void {
    this.visibleChange.emit(false);
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
      const dto: UpdateDonorDto = {
        titleId: formValue.titleId,
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        gender: formValue.gender,
        birthDate: formValue.birthDate,
        companyName: formValue.companyName,
        organizationType: formValue.organizationType,
        legalForm: formValue.legalForm,
        preferredLanguage: formValue.preferredLanguage,
        notes: formValue.notes
      };

      this.donorService.update(this.donor!.id!, dto).subscribe({
        next: () => {
          this.message.success('Donatore aggiornato');
          this.saving = false;
          this.saved.emit();
        },
        error: (err) => {
          this.message.error('Errore nell\'aggiornamento');
          this.saving = false;
          console.error(err);
        }
      });
    } else {
      const dto: CreateDonorDto = {
        subjectType: formValue.subjectType,
        titleId: formValue.titleId,
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        gender: formValue.gender,
        birthDate: formValue.birthDate,
        taxCode: formValue.taxCode,
        companyName: formValue.companyName,
        organizationType: formValue.organizationType,
        legalForm: formValue.legalForm,
        vatNumber: formValue.vatNumber,
        email: formValue.email,
        phoneNumber: formValue.phoneNumber,
        origin: formValue.origin,
        preferredLanguage: formValue.preferredLanguage,
        notes: formValue.notes
      };

      this.donorService.create(dto).subscribe({
        next: () => {
          this.message.success('Donatore creato');
          this.saving = false;
          this.saved.emit();
        },
        error: (err) => {
          this.message.error('Errore nella creazione');
          this.saving = false;
          console.error(err);
        }
      });
    }
  }
}
