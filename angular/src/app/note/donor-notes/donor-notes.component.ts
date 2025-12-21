/* eslint-disable @angular-eslint/prefer-standalone */
import { Component, Input, OnInit, input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NoteService } from '@proxy/notes';
import { NoteDto } from '@proxy/notes/dto';
import { ListService, PagedResultDto } from '@abp/ng.core';

@Component({
  selector: 'app-donor-notes',
  standalone: false,
  templateUrl: './donor-notes.component.html',
  styleUrl: './donor-notes.component.scss',
  providers: [ListService]
})
export class DonorNotesComponent implements OnInit {

 @Input() donorId?: string;
  notes = { items: [], totalCount: 0 } as PagedResultDto<NoteDto>;
  form: FormGroup;
  
    selectedNote = {} as NoteDto; 
    isModalOpen = false; 
  
    constructor(
      public readonly list: ListService, 
      private noteService: NoteService, 
      private fb: FormBuilder) {}
  
    ngOnInit() {
      if (!this.donorId) {
    console.warn('Donor ID is not provided.');
    return;
  }

      const donorStreamCreator = (query) => this.noteService.getListByDonor(this.donorId!, query);
  
      this.list.hookToQuery(donorStreamCreator).subscribe((response) => {
        this.notes = response;
      });
    }
  
  
    createNote() {
      this.buildForm(); 
      this.isModalOpen = true;
    }
  
     editNote(id: string) {
      this.noteService.get(id).subscribe((note) => {
        this.selectedNote = note;
        this.buildForm();
        this.isModalOpen = true;
      });
    }
  
    buildForm() {
      this.form = this.fb.group({
        content: [this.selectedNote.content || '', Validators.required],
        isImportant: [this.selectedNote.isImportant || '', Validators.required],
      });
    }
  
    save() {
      if (this.form.invalid) {
        return;
      }
  
      const request = this.selectedNote.id
        ? this.noteService.updateByDonor(this.donorId, this.selectedNote.id, this.form.value)
        : this.noteService.createByDonor(this.donorId, this.form.value);
  
      request.subscribe(() => {
        this.isModalOpen = false;
        this.form.reset();
        this.list.get();
      });
    }  
}
