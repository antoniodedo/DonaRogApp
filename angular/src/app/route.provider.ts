import { RoutesService, eLayoutType } from '@abp/ng.core';
import { APP_INITIALIZER } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  {
    provide: APP_INITIALIZER,
    useFactory: configureRoutes,
    deps: [RoutesService],
    multi: true,
  },
];

function configureRoutes(routes: RoutesService) {
  return () => {
    console.log('Configuring routes with APP_INITIALIZER...');
    routes.add([
      {
        path: '/',
        name: '::Menu:Home',
        iconClass: 'fas fa-home',
        order: 1,
        layout: eLayoutType.application,
      },
      {
        path: '/communications/print-batches',
        name: 'Batch Stampa',
        iconClass: 'fas fa-print',
        order: 1.1,
        layout: eLayoutType.application,
      },
      {
        path: '/communications/thank-you-rules',
        name: 'Regole Ringraziamenti',
        iconClass: 'fas fa-robot',
        order: 1.2,
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
        path: '/admin/segmentation-rules',
        name: '::Menu:SegmentationRules',
        iconClass: 'fas fa-chart-pie',
        order: 3,
        layout: eLayoutType.application,
      },
      {
        path: '/letter-templates',
        name: 'Templates',
        iconClass: 'fas fa-file-alt',
        order: 5,
        layout: eLayoutType.application,
      },
      {
        path: '/donations',
        name: 'Donazioni',
        iconClass: 'fas fa-hand-holding-heart',
        order: 6,
        layout: eLayoutType.application,
      }
    ]);
    console.log('Routes configured successfully, including Communications routes');
  };
}
