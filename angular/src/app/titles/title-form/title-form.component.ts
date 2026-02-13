import { Component, EventEmitter, Input, OnInit, Output, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { TitleService } from '../../proxy/titles/title.service';
import type { TitleDto, CreateUpdateTitleDto } from '../../proxy/titles/models';
import { Gender } from '../../proxy/enums/donors/gender.enum';

@Component({
  selector: 'app-title-form',
  standalone: false,
  templateUrl: './title-form.component.html',
  styleUrls: ['./title-form.component.scss']
})
export class TitleFormComponent implements OnInit, OnChanges {
  @Input() visible = false;
  @Input() title: TitleDto | null = null;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() saved = new EventEmitter<void>();

  form!: FormGroup;
  saving = false;

  Gender = Gender;

  get isEditMode(): boolean {
    return !!this.title;
  }

  constructor(
    private fb: FormBuilder,
    private titleService: TitleService,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.initForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['title'] || changes['visible']) {
      if (this.visible) {
        this.initForm();
        if (this.title) {
          this.patchForm();
        }
      }
    }
  }

  private initForm(): void {
    this.form = this.fb.group({
      code: ['', [Validators.required, Validators.maxLength(20)]],
      name: ['', [Validators.required, Validators.maxLength(100)]],
      abbreviation: ['', [Validators.required, Validators.maxLength(20)]],
      associatedGender: [null],
      displayOrder: [0, [Validators.required, Validators.min(0)]],
      notes: ['']
    });
  }

  private patchForm(): void {
    if (!this.title) return;

    this.form.patchValue({
      code: this.title.code,
      name: this.title.name,
      abbreviation: this.title.abbreviation,
      associatedGender: this.title.associatedGender,
      displayOrder: this.title.displayOrder,
      notes: this.title.notes
    });
  }

  close(): void {
    this.visibleChange.emit(false);
    this.form.reset();
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

    const dto: CreateUpdateTitleDto = {
      code: formValue.code,
      name: formValue.name,
      abbreviation: formValue.abbreviation,
      associatedGender: formValue.associatedGender,
      displayOrder: formValue.displayOrder,
      notes: formValue.notes
    };

    if (this.isEditMode) {
      this.titleService.update(this.title!.id, dto).subscribe({
        next: () => {
          this.message.success('Titolo aggiornato');
          this.saving = false;
          this.saved.emit();
        },
        error: (err) => {
          this.message.error(err.error?.error?.message || 'Errore nell\'aggiornamento');
          this.saving = false;
        }
      });
    } else {
      this.titleService.create(dto).subscribe({
        next: () => {
          this.message.success('Titolo creato');
          this.saving = false;
          this.saved.emit();
        },
        error: (err) => {
          this.message.error(err.error?.error?.message || 'Errore nella creazione');
          this.saving = false;
        }
      });
    }
  }
}
