import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NoteService } from '../../proxy/notes/note.service';
import type { NoteDto, CreateUpdateNoteDto } from '../../proxy/notes/dto/models';

@Component({
  selector: 'app-donor-notes',
  standalone: false,
  templateUrl: './donor-notes.component.html',
  styleUrls: ['./donor-notes.component.scss']
})
export class DonorNotesComponent implements OnInit {
  @Input() donorId!: string;

  notes: NoteDto[] = [];
  loading = false;
  totalCount = 0;

  // Pagination
  pageIndex = 1;
  pageSize = 5;

  // Form
  isFormVisible = false;
  isEditMode = false;
  editingNote: NoteDto | null = null;
  form!: FormGroup;
  saving = false;

  // Categories
  categories = [
    { value: 'phone', label: 'Telefonata' },
    { value: 'meeting', label: 'Incontro' },
    { value: 'email', label: 'Email' },
    { value: 'event', label: 'Evento' },
    { value: 'donation', label: 'Donazione' },
    { value: 'other', label: 'Altro' }
  ];

  constructor(
    private noteService: NoteService,
    private fb: FormBuilder,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadNotes();
  }

  initForm(): void {
    this.form = this.fb.group({
      content: ['', Validators.required],
      category: [null],
      interactionDate: [new Date()],
      isImportant: [false],
      isPrivate: [false]
    });
  }

  loadNotes(): void {
    this.loading = true;
    const params = {
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize
    };
    
    this.noteService.getListByDonor(this.donorId, params).subscribe({
      next: (result) => {
        this.notes = result.items || [];
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: () => {
        this.message.error('Errore nel caricamento delle note');
        this.loading = false;
      }
    });
  }

  onPageChange(pageIndex: number): void {
    this.pageIndex = pageIndex;
    this.loadNotes();
  }

  onPageSizeChange(pageSize: number): void {
    this.pageSize = pageSize;
    this.pageIndex = 1;
    this.loadNotes();
  }

  openCreateForm(): void {
    this.isEditMode = false;
    this.editingNote = null;
    this.form.reset({
      isImportant: false,
      isPrivate: false,
      interactionDate: new Date()
    });
    this.isFormVisible = true;
  }

  openEditForm(note: NoteDto): void {
    this.isEditMode = true;
    this.editingNote = note;
    this.form.patchValue({
      content: note.content,
      category: note.category,
      interactionDate: note.interactionDate ? new Date(note.interactionDate) : null,
      isImportant: note.isImportant,
      isPrivate: note.isPrivate
    });
    this.isFormVisible = true;
  }

  closeForm(): void {
    this.isFormVisible = false;
    this.editingNote = null;
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

    const dto: CreateUpdateNoteDto = {
      content: formValue.content,
      category: formValue.category,
      interactionDate: formValue.interactionDate?.toISOString(),
      isImportant: formValue.isImportant || false,
      isPrivate: formValue.isPrivate || false
    };

    if (this.isEditMode && this.editingNote) {
      this.noteService.update(this.editingNote.id, dto).subscribe({
        next: () => {
          this.message.success('Nota aggiornata');
          this.saving = false;
          this.closeForm();
          this.loadNotes();
        },
        error: (err) => {
          this.message.error(err.error?.error?.message || 'Errore nell\'aggiornamento');
          this.saving = false;
        }
      });
    } else {
      this.noteService.createForDonor(this.donorId, dto).subscribe({
        next: () => {
          this.message.success('Nota creata');
          this.saving = false;
          this.closeForm();
          this.pageIndex = 1; // Torna alla prima pagina per vedere la nuova nota
          this.loadNotes();
        },
        error: (err) => {
          this.message.error(err.error?.error?.message || 'Errore nella creazione');
          this.saving = false;
        }
      });
    }
  }

  deleteNote(note: NoteDto): void {
    this.noteService.delete(note.id).subscribe({
      next: () => {
        this.message.success('Nota eliminata');
        this.loadNotes();
      },
      error: () => {
        this.message.error('Errore nell\'eliminazione');
      }
    });
  }

  toggleImportant(note: NoteDto): void {
    this.noteService.toggleImportant(note.id).subscribe({
      next: () => {
        this.loadNotes();
      },
      error: () => {
        this.message.error('Errore');
      }
    });
  }

  getCategoryLabel(category: string | undefined): string {
    if (!category) return '-';
    const cat = this.categories.find(c => c.value === category);
    return cat?.label || category;
  }

  getCategoryColor(category: string | undefined): string {
    switch (category) {
      case 'phone': return 'blue';
      case 'meeting': return 'green';
      case 'email': return 'cyan';
      case 'event': return 'purple';
      case 'donation': return 'gold';
      default: return 'default';
    }
  }

  formatDate(date: string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('it-IT');
  }

  formatDateTime(date: string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleString('it-IT', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  // Tronca il testo per la preview
  truncateText(text: string, maxLength: number = 100): string {
    if (!text || text.length <= maxLength) return text;
    return text.substring(0, maxLength) + '...';
  }
}
