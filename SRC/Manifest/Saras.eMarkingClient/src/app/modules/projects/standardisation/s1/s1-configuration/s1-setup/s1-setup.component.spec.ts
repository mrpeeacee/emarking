import { ComponentFixture, TestBed } from '@angular/core/testing';

import { S1SetupComponent } from './s1-setup.component';

describe('S1SetupComponent', () => {
  let component: S1SetupComponent;
  let fixture: ComponentFixture<S1SetupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ S1SetupComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(S1SetupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
