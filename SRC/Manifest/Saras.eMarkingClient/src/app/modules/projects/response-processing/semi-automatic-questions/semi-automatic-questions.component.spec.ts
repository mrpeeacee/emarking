import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SemiAutomaticQuestionsComponent } from './semi-automatic-questions.component';

describe('SemiAutomaticQuestionsComponent', () => {
  let component: SemiAutomaticQuestionsComponent;
  let fixture: ComponentFixture<SemiAutomaticQuestionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SemiAutomaticQuestionsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SemiAutomaticQuestionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
