import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { DonationService } from '../../proxy/donations/donation.service';
import { DonationChannel } from '../../proxy/donations/models';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-donation-form',
  templateUrl: './donation-form.component.html',
  styleUrls: ['./donation-form.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NzFormModule, NzInputModule, NzButtonModule, NzSelectModule, NzDatePickerModule, NzCardModule, NzInputNumberModule, NzGridModule],
})
export class DonationFormComponent implements OnInit {
  form!: FormGroup;
  loading = false;
  isEditMode = false;
  donationId?: string;

  channelOptions = [
    { value: DonationChannel.BankTransfer, label: 'Bonifico Bancario' },
    { value: DonationChannel.PostalOrder, label: 'Bollettino Postale' },
    { value: DonationChannel.PostalOrderTelematic, label: 'Bollettino Telematico' },
    { value: DonationChannel.PayPal, label: 'PayPal' },
    { value: DonationChannel.Cash, label: 'Contanti' },
    { value: DonationChannel.Check, label: 'Assegno' },
    { value: DonationChannel.Bequest, label: 'Lasciti' },
    { value: DonationChannel.Other, label: 'Altro' },
  ];

  constructor(
    private fb: FormBuilder,
    private donationService: DonationService,
    private router: Router,
    private route: ActivatedRoute,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.initForm();
    
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.donationId = params['id'];
        this.isEditMode = true;
        this.loadDonation();
      }
    });
  }

  initForm(): void {
    this.form = this.fb.group({
      donorId: ['', [Validators.required]],
      channel: [null, [Validators.required]],
      totalAmount: [null, [Validators.required, Validators.min(0.01)]],
      donationDate: [new Date(), [Validators.required]],
      creditDate: [null],
      campaignId: [null],
      bankAccountId: [null],
      thankYouTemplateId: [null],
      notes: [''],
      internalNotes: [''],
    });
  }

  loadDonation(): void {
    if (!this.donationId) return;
    
    this.loading = true;
    this.donationService.get(this.donationId).subscribe({
      next: (donation) => {
        this.form.patchValue({
          donorId: donation.donorId,
          channel: donation.channel,
          totalAmount: donation.totalAmount,
          donationDate: new Date(donation.donationDate),
          creditDate: donation.creditDate ? new Date(donation.creditDate) : null,
          campaignId: donation.campaignId,
          bankAccountId: donation.bankAccountId,
          thankYouTemplateId: donation.thankYouTemplateId,
          notes: donation.notes,
          internalNotes: donation.internalNotes,
        });
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.message.error('Errore caricamento donazione');
      },
    });
  }

  submit(): void {
    if (this.form.invalid) {
      Object.values(this.form.controls).forEach(control => {
        control.markAsDirty();
        control.updateValueAndValidity();
      });
      return;
    }

    this.loading = true;
    const formValue = this.form.value;
    
    const dto = {
      ...formValue,
      donationDate: formValue.donationDate.toISOString(),
      creditDate: formValue.creditDate ? formValue.creditDate.toISOString() : undefined,
      projectAllocations: [],
    };

    this.donationService.create(dto).subscribe({
      next: () => {
        this.message.success('Donazione salvata con successo');
        this.router.navigate(['/donations']);
      },
      error: () => {
        this.loading = false;
        this.message.error('Errore salvataggio donazione');
      },
    });
  }

  cancel(): void {
    this.router.navigate(['/donations']);
  }
}
