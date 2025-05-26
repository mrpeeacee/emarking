import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditScoringComponentComponent } from './edit-scoring-component.component';

describe('EditScoringComponentComponent', () => {
  let component: EditScoringComponentComponent;
  let fixture: ComponentFixture<EditScoringComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditScoringComponentComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditScoringComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
