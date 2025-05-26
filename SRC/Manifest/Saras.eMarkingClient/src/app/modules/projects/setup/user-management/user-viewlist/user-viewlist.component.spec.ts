import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserViewlistComponent } from './user-viewlist.component';

describe('UserViewlistComponent', () => {
  let component: UserViewlistComponent;
  let fixture: ComponentFixture<UserViewlistComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserViewlistComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserViewlistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
