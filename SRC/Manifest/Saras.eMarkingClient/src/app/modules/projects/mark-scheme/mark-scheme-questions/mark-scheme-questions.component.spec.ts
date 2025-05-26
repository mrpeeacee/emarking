import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MarkSchemeQuestionsComponent } from './mark-scheme-questions.component';

describe('MarkSchemeQuestionsComponent', () => {
  let component: MarkSchemeQuestionsComponent;
  let fixture: ComponentFixture<MarkSchemeQuestionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MarkSchemeQuestionsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MarkSchemeQuestionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
