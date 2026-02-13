import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { IdentityUserService } from '@abp/ng.identity/proxy';
import type { 
  IdentityUserDto,
  IdentityUserCreateDto,
  IdentityUserUpdateDto
} from '@abp/ng.identity/proxy';
import type { IdentityRoleDto } from '@abp/ng.identity/proxy';

@Component({
  selector: 'app-user-list',
  standalone: false,
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {
  users: IdentityUserDto[] = [];
  loading = false;
  total = 0;
  pageIndex = 1;
  pageSize = 10;
  searchText = '';

  // Ruoli disponibili
  availableRoles: IdentityRoleDto[] = [];

  // Modal
  isModalVisible = false;
  isEditMode = false;
  userForm!: FormGroup;
  selectedUser: IdentityUserDto | null = null;
  modalLoading = false;

  constructor(
    private userService: IdentityUserService,
    private fb: FormBuilder,
    private message: NzMessageService
  ) {
    this.createForm();
  }

  ngOnInit(): void {
    this.loadUsers();
    this.loadRoles();
  }

  createForm(): void {
    this.userForm = this.fb.group({
      userName: ['', [Validators.required, Validators.maxLength(256)]],
      name: ['', [Validators.maxLength(64)]],
      surname: ['', [Validators.maxLength(64)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [''],
      password: ['', [Validators.required, Validators.minLength(6)]],
      isActive: [true],
      lockoutEnabled: [true],
      roleNames: [[]]
    });
  }

  loadUsers(): void {
    this.loading = true;
    this.userService.getList({
      filter: this.searchText,
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize,
      sorting: 'userName'
    }).subscribe({
      next: (response) => {
        this.users = response.items || [];
        this.total = response.totalCount || 0;
        this.loading = false;
      },
      error: (err) => {
        console.error('Errore nel caricamento utenti:', err);
        this.message.error('Errore nel caricamento degli utenti');
        this.loading = false;
      }
    });
  }

  loadRoles(): void {
    this.userService.getAssignableRoles().subscribe({
      next: (response) => {
        this.availableRoles = response.items || [];
      },
      error: (err) => {
        console.error('Errore nel caricamento ruoli:', err);
      }
    });
  }

  onSearch(): void {
    this.pageIndex = 1;
    this.loadUsers();
  }

  clearSearch(): void {
    this.searchText = '';
    this.pageIndex = 1;
    this.loadUsers();
  }

  getActiveCount(): number {
    return this.users.filter(u => u.isActive).length;
  }

  getInactiveCount(): number {
    return this.users.filter(u => !u.isActive).length;
  }

  onPageChange(pageIndex: number): void {
    this.pageIndex = pageIndex;
    this.loadUsers();
  }

  onPageSizeChange(pageSize: number): void {
    this.pageSize = pageSize;
    this.pageIndex = 1;
    this.loadUsers();
  }

  openCreateModal(): void {
    this.isEditMode = false;
    this.selectedUser = null;
    this.userForm.reset({
      isActive: true,
      lockoutEnabled: true,
      roleNames: []
    });
    
    // Password obbligatoria in creazione
    this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.userForm.get('password')?.updateValueAndValidity();
    
    this.isModalVisible = true;
  }

  openEditModal(user: IdentityUserDto): void {
    this.isEditMode = true;
    this.selectedUser = user;
    
    // Carica i ruoli dell'utente
    this.userService.getRoles(user.id).subscribe({
      next: (response) => {
        const roleNames = (response.items || []).map(r => r.name);
        
        this.userForm.patchValue({
          userName: user.userName,
          name: user.name,
          surname: user.surname,
          email: user.email,
          phoneNumber: user.phoneNumber,
          isActive: user.isActive,
          lockoutEnabled: user.lockoutEnabled,
          roleNames: roleNames
        });

        // Password opzionale in modifica
        this.userForm.get('password')?.clearValidators();
        this.userForm.get('password')?.updateValueAndValidity();

        this.isModalVisible = true;
      },
      error: (err) => {
        console.error('Errore nel caricamento ruoli utente:', err);
        this.message.error('Errore nel caricamento dei ruoli');
      }
    });
  }

  handleCancel(): void {
    this.isModalVisible = false;
    this.userForm.reset();
    this.selectedUser = null;
  }

  handleOk(): void {
    if (this.userForm.invalid) {
      Object.values(this.userForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      return;
    }

    this.modalLoading = true;

    if (this.isEditMode && this.selectedUser) {
      this.updateUser();
    } else {
      this.createUser();
    }
  }

  createUser(): void {
    const formValue = this.userForm.value;
    const input: IdentityUserCreateDto = {
      userName: formValue.userName,
      name: formValue.name,
      surname: formValue.surname,
      email: formValue.email,
      phoneNumber: formValue.phoneNumber,
      password: formValue.password,
      isActive: formValue.isActive,
      lockoutEnabled: formValue.lockoutEnabled,
      roleNames: formValue.roleNames || []
    };

    this.userService.create(input).subscribe({
      next: () => {
        this.message.success('Utente creato con successo');
        this.isModalVisible = false;
        this.modalLoading = false;
        this.loadUsers();
      },
      error: (err) => {
        console.error('Errore nella creazione dell\'utente:', err);
        this.message.error(err.error?.error?.message || 'Errore nella creazione dell\'utente');
        this.modalLoading = false;
      }
    });
  }

  updateUser(): void {
    if (!this.selectedUser) return;

    const formValue = this.userForm.value;
    const input: IdentityUserUpdateDto = {
      userName: formValue.userName,
      name: formValue.name,
      surname: formValue.surname,
      email: formValue.email,
      phoneNumber: formValue.phoneNumber,
      isActive: formValue.isActive,
      lockoutEnabled: formValue.lockoutEnabled,
      roleNames: formValue.roleNames || [],
      password: formValue.password || undefined,
      concurrencyStamp: this.selectedUser.concurrencyStamp
    };

    this.userService.update(this.selectedUser.id, input).subscribe({
      next: () => {
        this.message.success('Utente aggiornato con successo');
        this.isModalVisible = false;
        this.modalLoading = false;
        this.loadUsers();
      },
      error: (err) => {
        console.error('Errore nell\'aggiornamento dell\'utente:', err);
        this.message.error(err.error?.error?.message || 'Errore nell\'aggiornamento dell\'utente');
        this.modalLoading = false;
      }
    });
  }

  deleteUser(user: IdentityUserDto): void {
    this.userService.delete(user.id).subscribe({
      next: () => {
        this.message.success('Utente eliminato con successo');
        this.loadUsers();
      },
      error: (err) => {
        console.error('Errore nell\'eliminazione dell\'utente:', err);
        this.message.error(err.error?.error?.message || 'Errore nell\'eliminazione dell\'utente');
      }
    });
  }

  getUserRoleNames(user: IdentityUserDto): string[] {
    // Questo è un placeholder. In un'implementazione reale, 
    // caricheresti i ruoli per ogni utente o li includeresti nella risposta getList
    return [];
  }
}
