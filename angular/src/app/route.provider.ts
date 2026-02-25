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
        path: '/letter-templates',
        name: 'Templates',
        iconClass: 'fas fa-file-alt',
        order: 4,
        layout: eLayoutType.application,
      },
      {
        path: '/donations',
        name: 'Donazioni',
        iconClass: 'fas fa-hand-holding-heart',
        order: 5,
        layout: eLayoutType.application,
      },
      {
        path: '/communications/print-batches',
        name: 'Batch Stampa',
        iconClass: 'fas fa-print',
        order: 6,
        layout: eLayoutType.application,
      },
      {
        path: '/communications/thank-you-rules',
        name: 'Regole Ringraziamenti',
        iconClass: 'fas fa-robot',
        order: 7,
        layout: eLayoutType.application,
      }
  ]);
}
