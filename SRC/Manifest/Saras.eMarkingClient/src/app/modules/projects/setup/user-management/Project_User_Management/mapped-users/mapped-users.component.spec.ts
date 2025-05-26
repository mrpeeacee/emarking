import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MappedUsersComponent } from './mapped-users.component';

describe('MappedUsersComponent', () => {
  let component: MappedUsersComponent;
  let fixture: ComponentFixture<MappedUsersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MappedUsersComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MappedUsersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
