import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UnMappedUsersComponent } from './un-mapped-users.component';

describe('UnMappedUsersComponent', () => {
  let component: UnMappedUsersComponent;
  let fixture: ComponentFixture<UnMappedUsersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UnMappedUsersComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UnMappedUsersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
