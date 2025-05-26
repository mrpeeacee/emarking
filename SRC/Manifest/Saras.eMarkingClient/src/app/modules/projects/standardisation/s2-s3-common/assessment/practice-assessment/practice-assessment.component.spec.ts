import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PracticeAssessmentComponent } from './practice-assessment.component';

describe('PracticeAssessmentComponent', () => {
  let component: PracticeAssessmentComponent;
  let fixture: ComponentFixture<PracticeAssessmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PracticeAssessmentComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PracticeAssessmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
