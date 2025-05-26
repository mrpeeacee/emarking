import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationUsermanagementComponent } from './application-usermanagement.component';

describe('ApplicationUsermanagementComponent', () => {
  let component: ApplicationUsermanagementComponent;
  let fixture: ComponentFixture<ApplicationUsermanagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ApplicationUsermanagementComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ApplicationUsermanagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
