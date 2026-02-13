import { Component } from '@angular/core';
import { AuthService, ConfigStateService, PermissionService } from '@abp/ng.core';
import { OAuthService } from 'angular-oauth2-oidc';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  isCollapsed = false;

  get isAuthenticated(): boolean {
    return this.oAuthService.hasValidAccessToken();
  }

  get currentUserName(): string {
    const user = this.configState.getOne('currentUser');
    return user?.userName || '';
  }

  get isHostUser(): boolean {
    const user = this.configState.getOne('currentUser');
    return !user?.tenantId;
  }

  get isTenantUser(): boolean {
    return !this.isHostUser;
  }

  get canManageUsers(): boolean {
    return this.permissionService.getGrantedPolicy('AbpIdentity.Users');
  }

  get canManageRoles(): boolean {
    return this.permissionService.getGrantedPolicy('AbpIdentity.Roles');
  }

  get canManageTenants(): boolean {
    return this.permissionService.getGrantedPolicy('AbpTenantManagement.Tenants');
  }

  get showAdminMenu(): boolean {
    return this.canManageUsers || this.canManageRoles || this.canManageTenants;
  }

  get showTenantMenu(): boolean {
    return this.isTenantUser;
  }

  constructor(
    private authService: AuthService,
    private oAuthService: OAuthService,
    private configState: ConfigStateService,
    private permissionService: PermissionService
  ) {}

  login(): void {
    this.authService.navigateToLogin();
  }

  logout(): void {
    this.authService.logout().subscribe();
  }
}
