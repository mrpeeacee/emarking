import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SsoLoginLiveComponent } from './sso-login-live.component';

describe('SsoLoginLiveComponent', () => {
  let component: SsoLoginLiveComponent;
  let fixture: ComponentFixture<SsoLoginLiveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SsoLoginLiveComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SsoLoginLiveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
