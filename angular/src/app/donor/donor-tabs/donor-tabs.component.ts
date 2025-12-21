/* eslint-disable @angular-eslint/prefer-standalone */
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-donor-tabs',
  standalone: false,
  templateUrl: './donor-tabs.component.html',
  styleUrl: './donor-tabs.component.scss'
})
export class DonorTabsComponent {
  // This component will handle the tabs for donor details.
  // You can implement the logic to switch between different donor-related views here.
  @Input() donorId?: string;
  active = 'activity';
  constructor() {}


  // Methods to handle tab switching can be added here.

}
