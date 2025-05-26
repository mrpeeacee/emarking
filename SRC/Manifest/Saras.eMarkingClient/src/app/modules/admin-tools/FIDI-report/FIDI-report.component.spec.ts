import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FDReportComponent, FIDIReportComponent } from './FIDI-report.component';

describe('FIDIReportComponent', () => {
  let component: FIDIReportComponent;
  let fixture: ComponentFixture<FDReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FIDIReportComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FIDIReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
