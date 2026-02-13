import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { TenantService } from '@abp/ng.tenant-management/proxy';
import type { 
  TenantDto,
  TenantCreateDto,
  TenantUpdateDto
} from '@abp/ng.tenant-management/proxy';

@Component({
  selector: 'app-tenant-list',
  standalone: false,
  templateUrl: './tenant-list.component.html',
  styleUrls: ['./tenant-list.component.scss']
})
export class TenantListComponent implements OnInit {
  tenants: TenantDto[] = [];
  loading = false;
  total = 0;
  pageIndex = 1;
  pageSize = 10;
  searchText = '';

  // Modal
  isModalVisible = false;
  isEditMode = false;
  tenantForm!: FormGroup;
  selectedTenant: TenantDto | null = null;
  modalLoading = false;

  constructor(
    private tenantService: TenantService,
    private fb: FormBuilder,
    private message: NzMessageService
  ) {
    this.createForm();
  }

  ngOnInit(): void {
    this.loadTenants();
  }

  createForm(): void {
    this.tenantForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(256)]],
      adminEmailAddress: ['', [Validators.required, Validators.email]],
      adminPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  loadTenants(): void {
    this.loading = true;
    this.tenantService.getList({
      filter: this.searchText,
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize,
      sorting: 'name'
    }).subscribe({
      next: (response) => {
        this.tenants = response.items || [];
        this.total = response.totalCount || 0;
        this.loading = false;
      },
      error: (err) => {
        console.error('Errore nel caricamento tenant:', err);
        this.message.error('Errore nel caricamento dei tenant');
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    this.pageIndex = 1;
    this.loadTenants();
  }

  onPageChange(pageIndex: number): void {
    this.pageIndex = pageIndex;
    this.loadTenants();
  }

  onPageSizeChange(pageSize: number): void {
    this.pageSize = pageSize;
    this.pageIndex = 1;
    this.loadTenants();
  }

  openCreateModal(): void {
    this.isEditMode = false;
    this.selectedTenant = null;
    this.tenantForm.reset();
    
    // Mostra i campi admin solo in creazione
    this.tenantForm.get('adminEmailAddress')?.enable();
    this.tenantForm.get('adminPassword')?.enable();
    this.tenantForm.get('adminEmailAddress')?.setValidators([Validators.required, Validators.email]);
    this.tenantForm.get('adminPassword')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.tenantForm.get('adminEmailAddress')?.updateValueAndValidity();
    this.tenantForm.get('adminPassword')?.updateValueAndValidity();
    
    this.isModalVisible = true;
  }

  openEditModal(tenant: TenantDto): void {
    this.isEditMode = true;
    this.selectedTenant = tenant;
    
    this.tenantForm.patchValue({
      name: tenant.name
    });

    // Nascondi i campi admin in modifica
    this.tenantForm.get('adminEmailAddress')?.disable();
    this.tenantForm.get('adminPassword')?.disable();
    this.tenantForm.get('adminEmailAddress')?.clearValidators();
    this.tenantForm.get('adminPassword')?.clearValidators();
    this.tenantForm.get('adminEmailAddress')?.updateValueAndValidity();
    this.tenantForm.get('adminPassword')?.updateValueAndValidity();

    this.isModalVisible = true;
  }

  handleCancel(): void {
    this.isModalVisible = false;
    this.tenantForm.reset();
    this.selectedTenant = null;
  }

  handleOk(): void {
    if (this.tenantForm.invalid) {
      Object.values(this.tenantForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      return;
    }

    this.modalLoading = true;

    if (this.isEditMode && this.selectedTenant) {
      this.updateTenant();
    } else {
      this.createTenant();
    }
  }

  createTenant(): void {
    const formValue = this.tenantForm.getRawValue();
    const input: TenantCreateDto = {
      name: formValue.name,
      adminEmailAddress: formValue.adminEmailAddress,
      adminPassword: formValue.adminPassword
    };

    this.tenantService.create(input).subscribe({
      next: () => {
        this.message.success('Tenant creato con successo');
        this.isModalVisible = false;
        this.modalLoading = false;
        this.loadTenants();
      },
      error: (err) => {
        console.error('Errore nella creazione del tenant:', err);
        this.message.error(err.error?.error?.message || 'Errore nella creazione del tenant');
        this.modalLoading = false;
      }
    });
  }

  updateTenant(): void {
    if (!this.selectedTenant) return;

    const formValue = this.tenantForm.getRawValue();
    const input: TenantUpdateDto = {
      name: formValue.name,
      concurrencyStamp: this.selectedTenant.concurrencyStamp
    };

    this.tenantService.update(this.selectedTenant.id, input).subscribe({
      next: () => {
        this.message.success('Tenant aggiornato con successo');
        this.isModalVisible = false;
        this.modalLoading = false;
        this.loadTenants();
      },
      error: (err) => {
        console.error('Errore nell\'aggiornamento del tenant:', err);
        this.message.error(err.error?.error?.message || 'Errore nell\'aggiornamento del tenant');
        this.modalLoading = false;
      }
    });
  }

  deleteTenant(tenant: TenantDto): void {
    this.tenantService.delete(tenant.id).subscribe({
      next: () => {
        this.message.success('Tenant eliminato con successo');
        this.loadTenants();
      },
      error: (err) => {
        console.error('Errore nell\'eliminazione del tenant:', err);
        this.message.error(err.error?.error?.message || 'Errore nell\'eliminazione del tenant');
      }
    });
  }
}
