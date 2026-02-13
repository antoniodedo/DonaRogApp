import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { 
  IdentityRoleService,
  IdentityRoleDto,
  IdentityRoleCreateDto,
  IdentityRoleUpdateDto
} from '../../proxy/identity/identity.service';

@Component({
  selector: 'app-role-list',
  standalone: false,
  templateUrl: './role-list.component.html',
  styleUrls: ['./role-list.component.scss']
})
export class RoleListComponent implements OnInit {
  roles: IdentityRoleDto[] = [];
  loading = false;
  total = 0;
  pageIndex = 1;
  pageSize = 10;
  searchText = '';

  // Form
  isFormVisible = false;
  isEditMode = false;
  editingRole: IdentityRoleDto | null = null;
  form!: FormGroup;
  saving = false;

  constructor(
    private roleService: IdentityRoleService,
    private fb: FormBuilder,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadRoles();
  }

  initForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(256)]],
      isDefault: [false],
      isPublic: [true]
    });
  }

  loadRoles(): void {
    this.loading = true;
    this.roleService.getList({
      filter: this.searchText,
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize
    }).subscribe({
      next: (result) => {
        this.roles = result.items || [];
        this.total = result.totalCount || 0;
        this.loading = false;
      },
      error: () => {
        this.message.error('Errore nel caricamento ruoli');
        this.loading = false;
      }
    });
  }

  onPageIndexChange(index: number): void {
    this.pageIndex = index;
    this.loadRoles();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageIndex = 1;
    this.loadRoles();
  }

  search(): void {
    this.pageIndex = 1;
    this.loadRoles();
  }

  openCreateForm(): void {
    this.isEditMode = false;
    this.editingRole = null;
    this.form.reset({ isDefault: false, isPublic: true });
    this.isFormVisible = true;
  }

  openEditForm(role: IdentityRoleDto): void {
    this.isEditMode = true;
    this.editingRole = role;
    this.form.patchValue({
      name: role.name,
      isDefault: role.isDefault,
      isPublic: role.isPublic
    });
    this.isFormVisible = true;
  }

  closeForm(): void {
    this.isFormVisible = false;
    this.editingRole = null;
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

    if (this.isEditMode && this.editingRole) {
      const dto: IdentityRoleUpdateDto = {
        name: formValue.name,
        isDefault: formValue.isDefault,
        isPublic: formValue.isPublic,
        concurrencyStamp: this.editingRole.concurrencyStamp
      };

      this.roleService.update(this.editingRole.id, dto).subscribe({
        next: () => {
          this.message.success('Ruolo aggiornato');
          this.saving = false;
          this.closeForm();
          this.loadRoles();
        },
        error: (err) => {
          this.message.error(err.error?.error?.message || 'Errore nell\'aggiornamento');
          this.saving = false;
        }
      });
    } else {
      const dto: IdentityRoleCreateDto = {
        name: formValue.name,
        isDefault: formValue.isDefault,
        isPublic: formValue.isPublic
      };

      this.roleService.create(dto).subscribe({
        next: () => {
          this.message.success('Ruolo creato');
          this.saving = false;
          this.closeForm();
          this.loadRoles();
        },
        error: (err) => {
          this.message.error(err.error?.error?.message || 'Errore nella creazione');
          this.saving = false;
        }
      });
    }
  }

  deleteRole(role: IdentityRoleDto): void {
    if (role.isStatic) {
      this.message.warning('I ruoli di sistema non possono essere eliminati');
      return;
    }

    this.roleService.delete(role.id).subscribe({
      next: () => {
        this.message.success('Ruolo eliminato');
        this.loadRoles();
      },
      error: (err) => {
        this.message.error(err.error?.error?.message || 'Errore nell\'eliminazione');
      }
    });
  }
}
