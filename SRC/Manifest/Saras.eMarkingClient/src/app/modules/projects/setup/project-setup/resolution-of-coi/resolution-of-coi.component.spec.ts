import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResolutionOfCoiComponent } from './resolution-of-coi.component';

describe('ResolutionOfCoiComponent', () => {
  let component: ResolutionOfCoiComponent;
  let fixture: ComponentFixture<ResolutionOfCoiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ResolutionOfCoiComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ResolutionOfCoiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
