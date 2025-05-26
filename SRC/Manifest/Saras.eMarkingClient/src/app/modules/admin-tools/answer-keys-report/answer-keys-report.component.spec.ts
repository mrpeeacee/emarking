import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AnswerKeysReportComponent } from './answer-keys-report.component';

describe('AnswerKeysReportComponent', () => {
  let component: AnswerKeysReportComponent;
  let fixture: ComponentFixture<AnswerKeysReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AnswerKeysReportComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AnswerKeysReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
