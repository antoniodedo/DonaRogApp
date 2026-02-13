import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { DonorService } from '../../proxy/donors/donor.service';
import { DonorEmailDto, CreateDonorEmailDto } from '../../proxy/donors/dtos/models';
import { EmailType, emailTypeOptions } from '../../proxy/enums/shared/email-type.enum';

@Component({
  selector: 'app-donor-emails',
  standalone: false,
  templateUrl: './donor-emails.component.html',
  styleUrls: ['./donor-emails.component.scss']
})
export class DonorEmailsComponent implements OnInit {
  @Input() donorId!: string;

  emails: DonorEmailDto[] = [];
  loading = false;
  
  // Modal aggiungi email
  isAddModalVisible = false;
  addForm!: FormGroup;
  saving = false;

  emailTypeOptions = emailTypeOptions;
  EmailType = EmailType;

  emailTypeLabels: Record<number, string> = {
    [EmailType.Personal]: 'Personale',
    [EmailType.Work]: 'Lavoro',
    [EmailType.Other]: 'Altro'
  };

  constructor(
    private donorService: DonorService,
    private message: NzMessageService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.loadEmails();
    this.initForm();
  }

  private initForm(): void {
    this.addForm = this.fb.group({
      emailAddress: ['', [Validators.required, Validators.email]],
      type: [EmailType.Personal]
    });
  }

  loadEmails(): void {
    this.loading = true;
    this.donorService.getEmails(this.donorId).subscribe({
      next: (emails) => {
        this.emails = emails;
        this.loading = false;
      },
      error: (err) => {
        this.message.error('Errore nel caricamento delle email');
        this.loading = false;
        console.error(err);
      }
    });
  }

  openAddModal(): void {
    this.addForm.reset({ type: EmailType.Personal });
    this.isAddModalVisible = true;
  }

  closeAddModal(): void {
    this.isAddModalVisible = false;
  }

  addEmail(): void {
    if (this.addForm.invalid) {
      Object.keys(this.addForm.controls).forEach(key => {
        const control = this.addForm.get(key);
        control?.markAsDirty();
        control?.updateValueAndValidity();
      });
      return;
    }

    this.saving = true;
    const dto: CreateDonorEmailDto = this.addForm.value;
    
    this.donorService.addEmail(this.donorId, dto).subscribe({
      next: () => {
        this.message.success('Email aggiunta');
        this.closeAddModal();
        this.loadEmails();
        this.saving = false;
      },
      error: (err) => {
        this.message.error('Errore nell\'aggiunta dell\'email');
        this.saving = false;
        console.error(err);
      }
    });
  }

  setDefault(email: DonorEmailDto): void {
    this.donorService.setDefaultEmail(this.donorId, email.emailAddress).subscribe({
      next: () => {
        this.message.success('Email predefinita impostata');
        this.loadEmails();
      },
      error: () => this.message.error('Errore')
    });
  }

  verifyEmail(email: DonorEmailDto): void {
    this.donorService.verifyEmail(this.donorId, email.emailAddress).subscribe({
      next: () => {
        this.message.success('Email verificata');
        this.loadEmails();
      },
      error: () => this.message.error('Errore')
    });
  }

  removeEmail(email: DonorEmailDto): void {
    this.donorService.removeEmail(this.donorId, email.emailAddress).subscribe({
      next: () => {
        this.message.success('Email rimossa');
        this.loadEmails();
      },
      error: () => this.message.error('Errore')
    });
  }

  getEmailTypeLabel(type: EmailType | undefined): string {
    if (type === undefined) return '-';
    return this.emailTypeLabels[type] || '-';
  }
}
