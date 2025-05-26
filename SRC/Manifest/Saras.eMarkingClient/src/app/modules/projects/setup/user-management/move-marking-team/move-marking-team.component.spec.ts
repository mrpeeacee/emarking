import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MoveMarkingTeamComponent } from './move-marking-team.component';

describe('MoveMarkingTeamComponent', () => {
  let component: MoveMarkingTeamComponent;
  let fixture: ComponentFixture<MoveMarkingTeamComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MoveMarkingTeamComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MoveMarkingTeamComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
