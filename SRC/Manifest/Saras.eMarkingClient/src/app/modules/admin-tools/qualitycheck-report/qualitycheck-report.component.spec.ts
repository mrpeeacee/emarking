import { ComponentFixture, TestBed } from '@angular/core/testing';
import { QualityCheckReportComponent } from './qualitycheck-report.component';


describe('QualityCheckReportComponent', () => {
  let component: QualityCheckReportComponent;
  let fixture: ComponentFixture<QualityCheckReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QualityCheckReportComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QualityCheckReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
