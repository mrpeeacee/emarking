import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MarkingPlayerComponent } from './marking-player.component';

describe('MarkingPlayerComponent', () => {
  let component: MarkingPlayerComponent;
  let fixture: ComponentFixture<MarkingPlayerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MarkingPlayerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MarkingPlayerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
