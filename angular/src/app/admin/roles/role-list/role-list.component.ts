import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { IdentityRoleService } from '@abp/ng.identity/proxy';
import type { 
  IdentityRoleDto,
  IdentityRoleCreateDto,
  IdentityRoleUpdateDto
} from '@abp/ng.identity/proxy';

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

  // Modal
  isModalVisible = false;
  isEditMode = false;
  roleForm!: FormGroup;
  selectedRole: IdentityRoleDto | null = null;
  modalLoading = false;

  constructor(
    private roleService: IdentityRoleService,
    private fb: FormBuilder,
    private message: NzMessageService
  ) {
    this.createForm();
  }

  ngOnInit(): void {
    this.loadRoles();
  }

  createForm(): void {
    this.roleForm = this.fb.group({
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
      maxResultCount: this.pageSize,
      sorting: 'name'
    }).subscribe({
      next: (response) => {
        this.roles = response.items || [];
        this.total = response.totalCount || 0;
        this.loading = false;
      },
      error: (err) => {
        console.error('Errore nel caricamento ruoli:', err);
        this.message.error('Errore nel caricamento dei ruoli');
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    this.pageIndex = 1;
    this.loadRoles();
  }

  clearSearch(): void {
    this.searchText = '';
    this.pageIndex = 1;
    this.loadRoles();
  }

  getDefaultCount(): number {
    return this.roles.filter(r => r.isDefault).length;
  }

  getStaticCount(): number {
    return this.roles.filter(r => r.isStatic).length;
  }

  onPageChange(pageIndex: number): void {
    this.pageIndex = pageIndex;
    this.loadRoles();
  }

  onPageSizeChange(pageSize: number): void {
    this.pageSize = pageSize;
    this.pageIndex = 1;
    this.loadRoles();
  }

  openCreateModal(): void {
    this.isEditMode = false;
    this.selectedRole = null;
    this.roleForm.reset({
      isDefault: false,
      isPublic: true
    });
    this.isModalVisible = true;
  }

  openEditModal(role: IdentityRoleDto): void {
    this.isEditMode = true;
    this.selectedRole = role;
    
    this.roleForm.patchValue({
      name: role.name,
      isDefault: role.isDefault,
      isPublic: role.isPublic
    });

    this.isModalVisible = true;
  }

  handleCancel(): void {
    this.isModalVisible = false;
    this.roleForm.reset();
    this.selectedRole = null;
  }

  handleOk(): void {
    if (this.roleForm.invalid) {
      Object.values(this.roleForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      return;
    }

    this.modalLoading = true;

    if (this.isEditMode && this.selectedRole) {
      this.updateRole();
    } else {
      this.createRole();
    }
  }

  createRole(): void {
    const formValue = this.roleForm.value;
    const input: IdentityRoleCreateDto = {
      name: formValue.name,
      isDefault: formValue.isDefault,
      isPublic: formValue.isPublic
    };

    this.roleService.create(input).subscribe({
      next: () => {
        this.message.success('Ruolo creato con successo');
        this.isModalVisible = false;
        this.modalLoading = false;
        this.loadRoles();
      },
      error: (err) => {
        console.error('Errore nella creazione del ruolo:', err);
        this.message.error(err.error?.error?.message || 'Errore nella creazione del ruolo');
        this.modalLoading = false;
      }
    });
  }

  updateRole(): void {
    if (!this.selectedRole) return;

    const formValue = this.roleForm.value;
    const input: IdentityRoleUpdateDto = {
      name: formValue.name,
      isDefault: formValue.isDefault,
      isPublic: formValue.isPublic,
      concurrencyStamp: this.selectedRole.concurrencyStamp
    };

    this.roleService.update(this.selectedRole.id, input).subscribe({
      next: () => {
        this.message.success('Ruolo aggiornato con successo');
        this.isModalVisible = false;
        this.modalLoading = false;
        this.loadRoles();
      },
      error: (err) => {
        console.error('Errore nell\'aggiornamento del ruolo:', err);
        this.message.error(err.error?.error?.message || 'Errore nell\'aggiornamento del ruolo');
        this.modalLoading = false;
      }
    });
  }

  deleteRole(role: IdentityRoleDto): void {
    if (role.isStatic) {
      this.message.warning('Non è possibile eliminare un ruolo di sistema');
      return;
    }

    this.roleService.delete(role.id).subscribe({
      next: () => {
        this.message.success('Ruolo eliminato con successo');
        this.loadRoles();
      },
      error: (err) => {
        console.error('Errore nell\'eliminazione del ruolo:', err);
        this.message.error(err.error?.error?.message || 'Errore nell\'eliminazione del ruolo');
      }
    });
  }
}
