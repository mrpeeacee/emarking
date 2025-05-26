import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewDownloadMarksSchemeComponent } from './view-download-marks-scheme.component';

describe('ViewDownloadMarksSchemeComponent', () => {
  let component: ViewDownloadMarksSchemeComponent;
  let fixture: ComponentFixture<ViewDownloadMarksSchemeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewDownloadMarksSchemeComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewDownloadMarksSchemeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
