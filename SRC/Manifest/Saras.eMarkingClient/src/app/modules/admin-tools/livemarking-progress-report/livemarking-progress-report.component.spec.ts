import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LiveMarkingProgressReportComponent } from './livemarking-progress-report.component';

describe('LiveMarkingProgressReportComponent', () => {
  let component: LiveMarkingProgressReportComponent;
  let fixture: ComponentFixture<LiveMarkingProgressReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LiveMarkingProgressReportComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LiveMarkingProgressReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
