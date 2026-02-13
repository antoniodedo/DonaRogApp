import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { 
  IdentityUserService, 
  IdentityRoleService,
  IdentityUserDto, 
  IdentityRoleDto,
  IdentityUserCreateDto,
  IdentityUserUpdateDto
} from '../../proxy/identity/identity.service';

@Component({
  selector: 'app-user-list',
  standalone: false,
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {
  users: IdentityUserDto[] = [];
  roles: IdentityRoleDto[] = [];
  loading = false;
  total = 0;
  pageIndex = 1;
  pageSize = 10;
  searchText = '';

  // Form
  isFormVisible = false;
  isEditMode = false;
  editingUser: IdentityUserDto | null = null;
  form!: FormGroup;
  saving = false;
  selectedRoles: string[] = [];

  constructor(
    private userService: IdentityUserService,
    private roleService: IdentityRoleService,
    private fb: FormBuilder,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadUsers();
    this.loadRoles();
  }

  initForm(): void {
    this.form = this.fb.group({
      userName: ['', [Validators.required, Validators.maxLength(256)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
      name: ['', Validators.maxLength(64)],
      surname: ['', Validators.maxLength(64)],
      phoneNumber: ['', Validators.maxLength(16)],
      password: ['', [Validators.minLength(6)]],
      isActive: [true],
      lockoutEnabled: [true]
    });
  }

  loadUsers(): void {
    this.loading = true;
    this.userService.getList({
      filter: this.searchText,
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize
    }).subscribe({
      next: (result) => {
        this.users = result.items || [];
        this.total = result.totalCount || 0;
        this.loading = false;
      },
      error: () => {
        this.message.error('Errore nel caricamento utenti');
        this.loading = false;
      }
    });
  }

  loadRoles(): void {
    this.roleService.getAllList().subscribe({
      next: (result) => {
        this.roles = result.items || [];
      },
      error: () => {
        this.message.warning('Impossibile caricare i ruoli');
      }
    });
  }

  onPageIndexChange(index: number): void {
    this.pageIndex = index;
    this.loadUsers();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageIndex = 1;
    this.loadUsers();
  }

  search(): void {
    this.pageIndex = 1;
    this.loadUsers();
  }

  openCreateForm(): void {
    this.isEditMode = false;
    this.editingUser = null;
    this.selectedRoles = [];
    this.form.reset({ isActive: true, lockoutEnabled: true });
    this.form.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.form.get('password')?.updateValueAndValidity();
    this.isFormVisible = true;
  }

  openEditForm(user: IdentityUserDto): void {
    this.isEditMode = true;
    this.editingUser = user;
    this.form.patchValue({
      userName: user.userName,
      email: user.email,
      name: user.name,
      surname: user.surname,
      phoneNumber: user.phoneNumber,
      isActive: user.isActive,
      lockoutEnabled: user.lockoutEnabled,
      password: ''
    });
    this.form.get('password')?.clearValidators();
    this.form.get('password')?.setValidators([Validators.minLength(6)]);
    this.form.get('password')?.updateValueAndValidity();
    
    // Carica ruoli utente
    this.userService.getRoles(user.id).subscribe({
      next: (result) => {
        this.selectedRoles = (result.items || []).map(r => r.name);
      }
    });
    
    this.isFormVisible = true;
  }

  closeForm(): void {
    this.isFormVisible = false;
    this.editingUser = null;
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

    if (this.isEditMode && this.editingUser) {
      const dto: IdentityUserUpdateDto = {
        userName: formValue.userName,
        email: formValue.email,
        name: formValue.name,
        surname: formValue.surname,
        phoneNumber: formValue.phoneNumber,
        isActive: formValue.isActive,
        lockoutEnabled: formValue.lockoutEnabled,
        roleNames: this.selectedRoles,
        password: formValue.password || undefined,
        concurrencyStamp: this.editingUser.concurrencyStamp
      };

      this.userService.update(this.editingUser.id, dto).subscribe({
        next: () => {
          this.message.success('Utente aggiornato');
          this.saving = false;
          this.closeForm();
          this.loadUsers();
        },
        error: (err) => {
          this.message.error(err.error?.error?.message || 'Errore nell\'aggiornamento');
          this.saving = false;
        }
      });
    } else {
      const dto: IdentityUserCreateDto = {
        userName: formValue.userName,
        email: formValue.email,
        name: formValue.name,
        surname: formValue.surname,
        phoneNumber: formValue.phoneNumber,
        isActive: formValue.isActive,
        lockoutEnabled: formValue.lockoutEnabled,
        roleNames: this.selectedRoles,
        password: formValue.password
      };

      this.userService.create(dto).subscribe({
        next: () => {
          this.message.success('Utente creato');
          this.saving = false;
          this.closeForm();
          this.loadUsers();
        },
        error: (err) => {
          this.message.error(err.error?.error?.message || 'Errore nella creazione');
          this.saving = false;
        }
      });
    }
  }

  deleteUser(user: IdentityUserDto): void {
    this.userService.delete(user.id).subscribe({
      next: () => {
        this.message.success('Utente eliminato');
        this.loadUsers();
      },
      error: (err) => {
        this.message.error(err.error?.error?.message || 'Errore nell\'eliminazione');
      }
    });
  }

  getInitials(user: IdentityUserDto): string {
    if (user.name && user.surname) {
      return (user.name[0] + user.surname[0]).toUpperCase();
    }
    return user.userName.substring(0, 2).toUpperCase();
  }
}
