import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RandomCheckComponent } from './random-check.component';

describe('RandomCheckComponent', () => {
  let component: RandomCheckComponent;
  let fixture: ComponentFixture<RandomCheckComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RandomCheckComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RandomCheckComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
