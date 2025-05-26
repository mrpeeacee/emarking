import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MarkersCompleteReportComponent } from './markers-complete-report.component';

describe('MarkersCompleteReportComponent', () => {
  let component: MarkersCompleteReportComponent;
  let fixture: ComponentFixture<MarkersCompleteReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MarkersCompleteReportComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MarkersCompleteReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
