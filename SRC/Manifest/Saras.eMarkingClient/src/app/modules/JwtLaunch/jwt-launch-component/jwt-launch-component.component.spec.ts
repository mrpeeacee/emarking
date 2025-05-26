import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JwtLaunchComponentComponent } from './jwt-launch-component.component';

describe('JwtLaunchComponentComponent', () => {
  let component: JwtLaunchComponentComponent;
  let fixture: ComponentFixture<JwtLaunchComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ JwtLaunchComponentComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JwtLaunchComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
