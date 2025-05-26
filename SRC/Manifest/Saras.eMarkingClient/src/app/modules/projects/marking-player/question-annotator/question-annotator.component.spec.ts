import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuestionAnnotatorComponent } from './question-annotator.component';

describe('QuestionAnnotatorComponent', () => {
  let component: QuestionAnnotatorComponent;
  let fixture: ComponentFixture<QuestionAnnotatorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QuestionAnnotatorComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QuestionAnnotatorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
