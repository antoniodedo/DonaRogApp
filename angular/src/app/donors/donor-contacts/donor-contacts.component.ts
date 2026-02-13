import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { DonorService } from '../../proxy/donors/donor.service';
import type { DonorContactDto, CreateDonorContactDto } from '../../proxy/donors/models';
import { ContactType, contactTypeOptions } from '../../proxy/enums/shared/contact-type.enum';

@Component({
  selector: 'app-donor-contacts',
  standalone: false,
  templateUrl: './donor-contacts.component.html',
  styleUrls: ['./donor-contacts.component.scss']
})
export class DonorContactsComponent implements OnInit {
  @Input() donorId!: string;

  contacts: DonorContactDto[] = [];
  loading = false;
  
  // Modal aggiungi contatto
  isAddModalVisible = false;
  addForm!: FormGroup;
  saving = false;

  contactTypeOptions = contactTypeOptions;
  ContactType = ContactType;

  contactTypeLabels: Record<number, string> = {
    [ContactType.Mobile]: 'Cellulare',
    [ContactType.HomeLandline]: 'Fisso Casa',
    [ContactType.WorkLandline]: 'Fisso Lavoro',
    [ContactType.Fax]: 'Fax',
    [ContactType.WhatsApp]: 'WhatsApp',
    [ContactType.Other]: 'Altro'
  };

  constructor(
    private donorService: DonorService,
    private message: NzMessageService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.loadContacts();
    this.initForm();
  }

  private initForm(): void {
    this.addForm = this.fb.group({
      phoneNumber: ['', [Validators.required]],
      type: [ContactType.Mobile]
    });
  }

  loadContacts(): void {
    this.loading = true;
    this.donorService.getContacts(this.donorId).subscribe({
      next: (contacts) => {
        this.contacts = contacts;
        this.loading = false;
      },
      error: (err) => {
        this.message.error('Errore nel caricamento dei contatti');
        this.loading = false;
        console.error(err);
      }
    });
  }

  openAddModal(): void {
    this.addForm.reset({ type: ContactType.Mobile });
    this.isAddModalVisible = true;
  }

  closeAddModal(): void {
    this.isAddModalVisible = false;
  }

  addContact(): void {
    if (this.addForm.invalid) {
      Object.keys(this.addForm.controls).forEach(key => {
        const control = this.addForm.get(key);
        control?.markAsDirty();
        control?.updateValueAndValidity();
      });
      return;
    }

    this.saving = true;
    const dto: CreateDonorContactDto = this.addForm.value;
    
    this.donorService.addContact(this.donorId, dto).subscribe({
      next: () => {
        this.message.success('Contatto aggiunto');
        this.closeAddModal();
        this.loadContacts();
        this.saving = false;
      },
      error: (err) => {
        this.message.error('Errore nell\'aggiunta del contatto');
        this.saving = false;
        console.error(err);
      }
    });
  }

  setDefault(contact: DonorContactDto): void {
    this.donorService.setDefaultContact(this.donorId, contact.phoneNumber!).subscribe({
      next: () => {
        this.message.success('Contatto predefinito impostato');
        this.loadContacts();
      },
      error: () => this.message.error('Errore')
    });
  }

  removeContact(contact: DonorContactDto): void {
    this.donorService.removeContact(this.donorId, contact.phoneNumber!).subscribe({
      next: () => {
        this.message.success('Contatto rimosso');
        this.loadContacts();
      },
      error: () => this.message.error('Errore')
    });
  }

  getContactTypeLabel(type: ContactType | undefined): string {
    if (type === undefined) return '-';
    return this.contactTypeLabels[type] || '-';
  }
}
