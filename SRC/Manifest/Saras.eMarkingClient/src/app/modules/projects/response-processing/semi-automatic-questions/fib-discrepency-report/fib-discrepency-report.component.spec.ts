import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FIBDiscrepencyReportComponent } from './fib-discrepency-report.component';

describe('FIBDiscrepencyReportComponent', () => {
  let component: FIBDiscrepencyReportComponent;
  let fixture: ComponentFixture<FIBDiscrepencyReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FIBDiscrepencyReportComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FIBDiscrepencyReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
