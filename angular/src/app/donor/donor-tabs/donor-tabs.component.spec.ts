import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DonorTabsComponent } from './donor-tabs.component';

describe('DonorTabsComponent', () => {
  let component: DonorTabsComponent;
  let fixture: ComponentFixture<DonorTabsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DonorTabsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DonorTabsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
