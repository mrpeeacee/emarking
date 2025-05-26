import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserCountsComponent } from './user-counts.component';

describe('UserCountsComponent', () => {
  let component: UserCountsComponent;
  let fixture: ComponentFixture<UserCountsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserCountsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserCountsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
