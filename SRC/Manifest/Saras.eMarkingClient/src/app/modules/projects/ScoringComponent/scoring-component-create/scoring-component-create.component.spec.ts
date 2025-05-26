import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScoringComponentCreateComponent } from './scoring-component-create.component';

describe('ScoringComponentCreateComponent', () => {
  let component: ScoringComponentCreateComponent;
  let fixture: ComponentFixture<ScoringComponentCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScoringComponentCreateComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScoringComponentCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
