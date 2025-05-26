import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseValidationSummaryComponent } from './course-validation-summary.component';

describe('CourseValidationSummaryComponent', () => {
  let component: CourseValidationSummaryComponent;
  let fixture: ComponentFixture<CourseValidationSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CourseValidationSummaryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CourseValidationSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
