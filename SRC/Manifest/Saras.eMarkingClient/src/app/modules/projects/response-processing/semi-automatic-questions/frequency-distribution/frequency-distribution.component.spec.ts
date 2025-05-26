import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FrequencyDistributionComponent } from './frequency-distribution.component';

describe('FrequencyDistributionComponent', () => {
  let component: FrequencyDistributionComponent;
  let fixture: ComponentFixture<FrequencyDistributionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FrequencyDistributionComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FrequencyDistributionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
