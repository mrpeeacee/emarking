import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FrequencyDistributionReportComponent } from './frequencydistribution-report.component';

describe('FrequencyDistributionReportComponent', () => {
  let component: FrequencyDistributionReportComponent;
  let fixture: ComponentFixture<FrequencyDistributionReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FrequencyDistributionReportComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FrequencyDistributionReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
