import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportOutboundLogsComponent } from './report-outbound-logs.component';

describe('ReportOutboundLogsComponent', () => {
  let component: ReportOutboundLogsComponent;
  let fixture: ComponentFixture<ReportOutboundLogsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportOutboundLogsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReportOutboundLogsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
