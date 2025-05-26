import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MarkerPerformanceReportComponent } from './marker-performance-report.component';

describe('MarkerPerformanceReportComponent', () => {
  let component: MarkerPerformanceReportComponent;
  let fixture: ComponentFixture<MarkerPerformanceReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MarkerPerformanceReportComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MarkerPerformanceReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
