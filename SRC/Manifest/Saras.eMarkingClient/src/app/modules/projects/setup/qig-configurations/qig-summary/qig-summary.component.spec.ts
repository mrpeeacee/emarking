import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QigSummaryComponent } from './qig-summary.component';

describe('QigSummaryComponent', () => {
  let component: QigSummaryComponent;
  let fixture: ComponentFixture<QigSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QigSummaryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QigSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
