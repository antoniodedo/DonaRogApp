import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';
import { BankAccountService } from '../../../proxy/bank-accounts/bank-account.service';
import { BankAccountListDto, CreateUpdateBankAccountDto } from '../../../proxy/bank-accounts/models';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-bank-accounts-list',
  templateUrl: './bank-accounts-list.component.html',
  styleUrls: ['./bank-accounts-list.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NzTableModule, NzButtonModule, NzFormModule, NzInputModule, NzCardModule, NzModalModule, NzTagModule, NzPopconfirmModule],
})
export class BankAccountsListComponent implements OnInit {
  accounts: BankAccountListDto[] = [];
  loading = false;
  isModalVisible = false;
  form!: FormGroup;
  editingId?: string;

  constructor(
    private fb: FormBuilder,
    private bankAccountService: BankAccountService,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadAccounts();
  }

  initForm(): void {
    this.form = this.fb.group({
      accountName: ['', [Validators.required]],
      iban: ['', [Validators.required]],
      bankName: [''],
      swift: [''],
      notes: [''],
    });
  }

  loadAccounts(): void {
    this.loading = true;
    this.bankAccountService.getList({ skipCount: 0, maxResultCount: 100 }).subscribe({
      next: (result) => {
        this.accounts = result.items || [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  showCreateModal(): void {
    this.editingId = undefined;
    this.form.reset();
    this.isModalVisible = true;
  }

  handleOk(): void {
    if (this.form.invalid) return;

    this.loading = true;
    const dto: CreateUpdateBankAccountDto = this.form.value;

    const operation = this.editingId
      ? this.bankAccountService.update(this.editingId, dto)
      : this.bankAccountService.create(dto);

    operation.subscribe({
      next: () => {
        this.message.success('Conto salvato con successo');
        this.isModalVisible = false;
        this.loadAccounts();
      },
      error: () => {
        this.loading = false;
        this.message.error('Errore salvataggio conto');
      },
    });
  }

  handleCancel(): void {
    this.isModalVisible = false;
  }

  activate(id: string): void {
    this.bankAccountService.activate(id).subscribe({
      next: () => {
        this.message.success('Conto attivato');
        this.loadAccounts();
      },
    });
  }

  deactivate(id: string): void {
    this.bankAccountService.deactivate(id).subscribe({
      next: () => {
        this.message.success('Conto disattivato');
        this.loadAccounts();
      },
      error: (error) => {
        this.message.error(error.error?.error?.message || 'Impossibile disattivare');
      },
    });
  }

  setAsDefault(id: string): void {
    this.bankAccountService.setAsDefault(id).subscribe({
      next: () => {
        this.message.success('Conto impostato come predefinito');
        this.loadAccounts();
      },
    });
  }

  delete(id: string): void {
    this.bankAccountService.delete(id).subscribe({
      next: () => {
        this.message.success('Conto eliminato');
        this.loadAccounts();
      },
      error: (error) => {
        this.message.error(error.error?.error?.message || 'Impossibile eliminare');
      },
    });
  }
}
