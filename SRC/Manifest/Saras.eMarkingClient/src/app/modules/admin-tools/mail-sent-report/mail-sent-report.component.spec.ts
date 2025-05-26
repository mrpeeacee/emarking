import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MailSentReportComponent } from './mail-sent-report.component';

describe('MailSentReportComponent', () => {
  let component: MailSentReportComponent;
  let fixture: ComponentFixture<MailSentReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MailSentReportComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MailSentReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
