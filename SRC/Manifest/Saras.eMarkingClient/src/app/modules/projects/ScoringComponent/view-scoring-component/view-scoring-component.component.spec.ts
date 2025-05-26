import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewScoringComponentComponent } from './view-scoring-component.component';

describe('ViewScoringComponentComponent', () => {
  let component: ViewScoringComponentComponent;
  let fixture: ComponentFixture<ViewScoringComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewScoringComponentComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewScoringComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
