import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModerateScoreComponent } from './moderate-score.component';

describe('ModerateScoreComponent', () => {
  let component: ModerateScoreComponent;
  let fixture: ComponentFixture<ModerateScoreComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModerateScoreComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ModerateScoreComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
