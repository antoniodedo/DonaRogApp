import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { 
  TenantService,
  TenantDto,
  TenantCreateDto,
  TenantUpdateDto
} from '../../proxy/tenant-management/tenant.service';

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

  // Form
  isFormVisible = false;
  isEditMode = false;
  editingTenant: TenantDto | null = null;
  form!: FormGroup;
  saving = false;

  constructor(
    private tenantService: TenantService,
    private fb: FormBuilder,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadTenants();
  }

  initForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(64)]],
      adminEmailAddress: ['', [Validators.email, Validators.maxLength(256)]],
      adminPassword: ['', [Validators.minLength(6)]]
    });
  }

  loadTenants(): void {
    this.loading = true;
    this.tenantService.getList({
      filter: this.searchText,
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize
    }).subscribe({
      next: (result) => {
        this.tenants = result.items || [];
        this.total = result.totalCount || 0;
        this.loading = false;
      },
      error: () => {
        this.message.error('Errore nel caricamento tenant');
        this.loading = false;
      }
    });
  }

  onPageIndexChange(index: number): void {
    this.pageIndex = index;
    this.loadTenants();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageIndex = 1;
    this.loadTenants();
  }

  search(): void {
    this.pageIndex = 1;
    this.loadTenants();
  }

  openCreateForm(): void {
    this.isEditMode = false;
    this.editingTenant = null;
    this.form.reset();
    this.form.get('adminEmailAddress')?.setValidators([Validators.required, Validators.email]);
    this.form.get('adminPassword')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.form.get('adminEmailAddress')?.updateValueAndValidity();
    this.form.get('adminPassword')?.updateValueAndValidity();
    this.isFormVisible = true;
  }

  openEditForm(tenant: TenantDto): void {
    this.isEditMode = true;
    this.editingTenant = tenant;
    this.form.patchValue({
      name: tenant.name
    });
    this.form.get('adminEmailAddress')?.clearValidators();
    this.form.get('adminPassword')?.clearValidators();
    this.form.get('adminEmailAddress')?.updateValueAndValidity();
    this.form.get('adminPassword')?.updateValueAndValidity();
    this.isFormVisible = true;
  }

  closeForm(): void {
    this.isFormVisible = false;
    this.editingTenant = null;
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

    if (this.isEditMode && this.editingTenant) {
      const dto: TenantUpdateDto = {
        name: formValue.name,
        concurrencyStamp: this.editingTenant.concurrencyStamp
      };

      this.tenantService.update(this.editingTenant.id, dto).subscribe({
        next: () => {
          this.message.success('Tenant aggiornato');
          this.saving = false;
          this.closeForm();
          this.loadTenants();
        },
        error: (err) => {
          this.message.error(err.error?.error?.message || 'Errore nell\'aggiornamento');
          this.saving = false;
        }
      });
    } else {
      const dto: TenantCreateDto = {
        name: formValue.name,
        adminEmailAddress: formValue.adminEmailAddress,
        adminPassword: formValue.adminPassword
      };

      this.tenantService.create(dto).subscribe({
        next: () => {
          this.message.success('Tenant creato');
          this.saving = false;
          this.closeForm();
          this.loadTenants();
        },
        error: (err) => {
          this.message.error(err.error?.error?.message || 'Errore nella creazione');
          this.saving = false;
        }
      });
    }
  }

  deleteTenant(tenant: TenantDto): void {
    this.tenantService.delete(tenant.id).subscribe({
      next: () => {
        this.message.success('Tenant eliminato');
        this.loadTenants();
      },
      error: (err) => {
        this.message.error(err.error?.error?.message || 'Errore nell\'eliminazione');
      }
    });
  }

  getInitials(name: string): string {
    return name.substring(0, 2).toUpperCase();
  }
}
