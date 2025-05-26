import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecommendationPoolComponent } from './recommendation-pool.component';

describe('RecommendationPoolComponent', () => {
  let component: RecommendationPoolComponent;
  let fixture: ComponentFixture<RecommendationPoolComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RecommendationPoolComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecommendationPoolComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
