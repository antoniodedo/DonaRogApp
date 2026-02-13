import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'projects',
    loadChildren: () => import('../projects/projects.module').then(m => m.ProjectsModule)
  },
  {
    path: 'tags',
    loadChildren: () => import('./tags/tags.module').then(m => m.TagsModule)
  },
  {
    path: 'recurrences',
    loadChildren: () => import('./recurrences/recurrences.module').then(m => m.RecurrencesModule)
  },
  {
    path: 'tenants',
    loadChildren: () => import('./tenants/tenants.module').then(m => m.TenantsModule)
  },
  {
    path: 'users',
    loadChildren: () => import('./users/users.module').then(m => m.UsersModule)
  },
  {
    path: 'roles',
    loadChildren: () => import('./roles/roles.module').then(m => m.RolesModule)
  },
  {
    path: '',
    redirectTo: 'tenants',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
