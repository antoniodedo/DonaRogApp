import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { DonationService } from '../../proxy/donations/donation.service';
import { DonationChannel, DonationListDto, DonationStatus, GetDonationsInput } from '../../proxy/donations/models';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-external-donations-demo',
  templateUrl: './external-donations-demo.component.html',
  styleUrls: ['./external-donations-demo.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterModule, NzFormModule, NzInputModule, NzButtonModule, NzSelectModule, NzDatePickerModule, NzCardModule, NzInputNumberModule, NzAlertModule, NzDividerModule, NzTableModule, NzGridModule, NzIconModule],
})
export class ExternalDonationsDemoComponent implements OnInit {
  form!: FormGroup;
  loading = false;
  pendingDonations: DonationListDto[] = [];
  
  channelOptions = [
    { value: DonationChannel.PostalOrderTelematic, label: 'Bollettino Telematico' },
    { value: DonationChannel.PayPal, label: 'PayPal' },
    { value: DonationChannel.BankTransfer, label: 'Bonifico' },
  ];

  constructor(
    private fb: FormBuilder,
    private donationService: DonationService,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadPendingDonations();
  }

  initForm(): void {
    this.form = this.fb.group({
      externalId: [this.generateExternalId(), [Validators.required]],
      channel: [DonationChannel.PostalOrderTelematic, [Validators.required]],
      amount: [50, [Validators.required, Validators.min(0.01)]],
      donationDate: [new Date(), [Validators.required]],
      creditDate: [null],
      donorId: ['', [Validators.required]], // TODO: Autocomplete
      notes: [''],
    });
  }

  generateExternalId(): string {
    return 'EXT-' + Date.now();
  }

  simulateSend(): void {
    if (this.form.invalid) {
      Object.values(this.form.controls).forEach(control => {
        control.markAsDirty();
        control.updateValueAndValidity();
      });
      return;
    }

    this.loading = true;
    const formValue = this.form.value;
    
    // Map ExternalDonationDto to CreateDonationDto
    const createDto: any = {
      donorId: formValue.donorId,
      channel: formValue.channel,
      totalAmount: formValue.amount,
      donationDate: formValue.donationDate.toISOString(),
      creditDate: formValue.creditDate ? formValue.creditDate.toISOString() : undefined,
      externalId: formValue.externalId,
      notes: formValue.notes,
      status: DonationStatus.Pending, // Create as Pending (needs verification)
    };

    this.donationService.create(createDto).subscribe({
      next: () => {
        this.message.success('Donazione importata! Visibile nella lista "Da Verificare"');
        this.form.patchValue({
          externalId: this.generateExternalId(),
          amount: 50,
          notes: '',
        });
        this.loadPendingDonations();
        this.loading = false;
      },
      error: (error) => {
        this.loading = false;
        this.message.error('Errore importazione: ' + (error.error?.error?.message || 'Errore sconosciuto'));
      },
    });
  }

  loadPendingDonations(): void {
    const input: GetDonationsInput = {
      status: DonationStatus.Pending,
      skipCount: 0,
      maxResultCount: 10,
      sorting: 'donationDate DESC',
    };

    this.donationService.getList(input).subscribe({
      next: (result) => {
        this.pendingDonations = result.items || [];
      },
    });
  }
}
