import { RoutesService, eLayoutType } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routes = inject(RoutesService);
  routes.add([
      {
        path: '/',
        name: '::Menu:Home',
        iconClass: 'fas fa-home',
        order: 1,
        layout: eLayoutType.application,
      },
      {
        path: '/donors',
        name: '::Menu:Donors',
        iconClass: 'fas fa-users',
        order: 2,
        layout: eLayoutType.application,
      },
      {
        path: 'donors/donor-title',
        name: 'Titoli Donatori',
        iconClass: 'fas fa-users',
        order: 3,
        layout: eLayoutType.application,
      },
      {
        path: 'letter-templates',
        name: 'Templates',
        iconClass: 'fas fa-file-alt',
        order: 4,
        layout: eLayoutType.application,
      }
  ]);
}
