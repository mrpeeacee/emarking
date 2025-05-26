import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StandardisationAssessmentsComponent } from './standardisation-assessments.component';

describe('StandardisationAssessmentsComponent', () => {
  let component: StandardisationAssessmentsComponent;
  let fixture: ComponentFixture<StandardisationAssessmentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StandardisationAssessmentsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StandardisationAssessmentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
