import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LiveMarkingComponent } from './live-marking.component';

describe('LiveMarkingComponent', () => {
  let component: LiveMarkingComponent;
  let fixture: ComponentFixture<LiveMarkingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LiveMarkingComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LiveMarkingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
