import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DonorNotesComponent } from './donor-notes.component';

describe('DonorNotesComponent', () => {
  let component: DonorNotesComponent;
  let fixture: ComponentFixture<DonorNotesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DonorNotesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DonorNotesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
