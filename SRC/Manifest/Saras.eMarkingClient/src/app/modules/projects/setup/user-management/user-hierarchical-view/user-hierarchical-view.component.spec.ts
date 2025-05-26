import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserHierarchicalViewComponent } from './user-hierarchical-view.component';

describe('UserHierarchicalViewComponent', () => {
  let component: UserHierarchicalViewComponent;
  let fixture: ComponentFixture<UserHierarchicalViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserHierarchicalViewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserHierarchicalViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
