import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DonorTitleTableComponent } from './donor-title-table.component';

describe('DonorTitleTableComponent', () => {
  let component: DonorTitleTableComponent;
  let fixture: ComponentFixture<DonorTitleTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DonorTitleTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DonorTitleTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
