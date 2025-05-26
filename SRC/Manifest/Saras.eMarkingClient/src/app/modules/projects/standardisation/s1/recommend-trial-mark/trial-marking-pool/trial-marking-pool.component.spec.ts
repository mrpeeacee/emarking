import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrialMarkingPoolComponent } from './trial-marking-pool.component';

describe('TrialMarkingPoolComponent', () => {
  let component: TrialMarkingPoolComponent;
  let fixture: ComponentFixture<TrialMarkingPoolComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TrialMarkingPoolComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TrialMarkingPoolComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
