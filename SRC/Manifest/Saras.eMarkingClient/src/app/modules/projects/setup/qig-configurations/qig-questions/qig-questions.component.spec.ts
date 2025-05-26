import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QigQuestionsComponent } from './qig-questions.component';

describe('QigQuestionsComponent', () => {
  let component: QigQuestionsComponent;
  let fixture: ComponentFixture<QigQuestionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QigQuestionsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QigQuestionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
