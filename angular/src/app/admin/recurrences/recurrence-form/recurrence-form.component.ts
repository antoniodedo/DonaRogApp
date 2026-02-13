import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RecurrenceService } from 'src/app/proxy/application/recurrences';
import { CreateRecurrenceDto, RecurrenceDto, UpdateRecurrenceDto } from 'src/app/proxy/application/contracts/recurrences/dto/models';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-recurrence-form',
  standalone: false,
  templateUrl: './recurrence-form.component.html',
  styleUrls: ['./recurrence-form.component.scss']
})
export class RecurrenceFormComponent implements OnInit {
  @Input() recurrenceId?: string;
  @Output() onSave = new EventEmitter<void>();
  @Output() onCancel = new EventEmitter<void>();

  form: FormGroup;
  loading = false;
  isEditMode = false;

  constructor(
    private fb: FormBuilder,
    private recurrenceService: RecurrenceService,
    private message: NzMessageService
  ) {
      this.form = this.fb.group({
        name: ['', [Validators.required, Validators.maxLength(200)]],
        code: ['', [Validators.maxLength(50)]], // Non obbligatorio
        description: ['', [Validators.maxLength(2000)]],
        recurrenceDay: [null, [Validators.min(1), Validators.max(31)]], // Giorno (1-31)
        recurrenceMonth: [null, [Validators.min(1), Validators.max(12)]], // Mese (1-12)
        daysBeforeRecurrence: [30, [Validators.min(0), Validators.max(365)]], // Non obbligatorio
        daysAfterRecurrence: [7, [Validators.min(0), Validators.max(365)]], // Non obbligatorio
        notes: ['', [Validators.maxLength(2000)]]
      });
  }

  ngOnInit(): void {
    if (this.recurrenceId) {
      this.isEditMode = true;
      this.loadRecurrence();
    }
  }

  loadRecurrence(): void {
    if (!this.recurrenceId) return;

    this.loading = true;
    this.recurrenceService.get(this.recurrenceId).subscribe({
      next: (recurrence: RecurrenceDto) => {
        this.form.patchValue({
          name: recurrence.name,
          code: recurrence.code,
          description: recurrence.description,
          recurrenceDay: recurrence.recurrenceDay,
          recurrenceMonth: recurrence.recurrenceMonth,
          daysBeforeRecurrence: recurrence.daysBeforeRecurrence,
          daysAfterRecurrence: recurrence.daysAfterRecurrence,
          notes: recurrence.notes
        });
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.message.error('Errore nel caricamento della ricorrenza');
      }
    });
  }

  save(): void {
    if (this.form.invalid) {
      Object.values(this.form.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      return;
    }

    this.loading = true;

    if (this.isEditMode && this.recurrenceId) {
      const updateDto: UpdateRecurrenceDto = {
        name: this.form.value.name,
        description: this.form.value.description,
        recurrenceDay: this.form.value.recurrenceDay || undefined,
        recurrenceMonth: this.form.value.recurrenceMonth || undefined,
        daysBeforeRecurrence: this.form.value.daysBeforeRecurrence,
        daysAfterRecurrence: this.form.value.daysAfterRecurrence,
        notes: this.form.value.notes
      };

      this.recurrenceService.update(this.recurrenceId, updateDto).subscribe({
        next: () => {
          this.message.success('Ricorrenza aggiornata con successo');
          this.onSave.emit();
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.message.error('Errore durante l\'aggiornamento della ricorrenza');
        }
      });
    } else {
      const createDto: CreateRecurrenceDto = {
        name: this.form.value.name,
        code: this.form.value.code,
        description: this.form.value.description,
        recurrenceDay: this.form.value.recurrenceDay || undefined,
        recurrenceMonth: this.form.value.recurrenceMonth || undefined,
        daysBeforeRecurrence: this.form.value.daysBeforeRecurrence,
        daysAfterRecurrence: this.form.value.daysAfterRecurrence,
        notes: this.form.value.notes
      };

      this.recurrenceService.create(createDto).subscribe({
        next: () => {
          this.message.success('Ricorrenza creata con successo');
          this.onSave.emit();
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.message.error('Errore durante la creazione della ricorrenza');
        }
      });
    }
  }

  cancel(): void {
    this.onCancel.emit();
  }
}
