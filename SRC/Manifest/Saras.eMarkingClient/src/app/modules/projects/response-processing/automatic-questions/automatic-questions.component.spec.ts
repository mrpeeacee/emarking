import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AutomaticQuestionsComponent } from './automatic-questions.component';

describe('AutomaticQuestionsComponent', () => {
  let component: AutomaticQuestionsComponent;
  let fixture: ComponentFixture<AutomaticQuestionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AutomaticQuestionsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AutomaticQuestionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
