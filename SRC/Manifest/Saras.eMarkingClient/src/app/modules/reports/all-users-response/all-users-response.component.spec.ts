import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AllUsersResponseComponent } from './all-users-response.component';

describe('AllUsersResponseComponent', () => {
  let component: AllUsersResponseComponent;
  let fixture: ComponentFixture<AllUsersResponseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AllUsersResponseComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AllUsersResponseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
