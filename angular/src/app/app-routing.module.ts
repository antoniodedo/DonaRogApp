import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { authGuard, permissionGuard } from '@abp/ng.core';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/donors' },
  { 
    path: 'welcome', 
    loadChildren: () => import('./pages/welcome/welcome.module').then(m => m.WelcomeModule),
    canActivate: [authGuard]
  },
  { 
    path: 'donors', 
    loadChildren: () => import('./donors/donors.module').then(m => m.DonorsModule),
    canActivate: [authGuard]
  },
  { 
    path: 'campaigns', 
    loadChildren: () => import('./admin/campaigns/campaigns.module').then(m => m.CampaignsModule),
    canActivate: [authGuard]
  },
  { 
    path: 'donations', 
    loadChildren: () => import('./donations/donations.module').then(m => m.DonationsModule),
    canActivate: [authGuard]
  },
  { 
    path: 'admin/titles', 
    loadChildren: () => import('./titles/titles.module').then(m => m.TitlesModule),
    canActivate: [authGuard]
  },
  { 
    path: 'letter-templates', 
    loadChildren: () => import('./letter-templates/letter-templates.module').then(m => m.LetterTemplatesModule),
    canActivate: [authGuard]
  },
  { 
    path: 'admin/bank-accounts', 
    loadChildren: () => import('./admin/bank-accounts/bank-accounts.module').then(m => m.BankAccountsModule),
    canActivate: [authGuard]
  },
  { 
    path: 'admin', 
    loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule),
    canActivate: [authGuard]
  },
  { path: 'login', redirectTo: '/' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
