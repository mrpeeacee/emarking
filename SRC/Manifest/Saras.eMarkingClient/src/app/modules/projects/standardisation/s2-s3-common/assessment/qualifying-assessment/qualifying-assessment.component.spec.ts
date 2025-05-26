import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QualifyingAssessmentComponent } from './qualifying-assessment.component';

describe('QualifyingAssessmentComponent', () => {
  let component: QualifyingAssessmentComponent;
  let fixture: ComponentFixture<QualifyingAssessmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QualifyingAssessmentComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QualifyingAssessmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
