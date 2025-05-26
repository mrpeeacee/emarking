import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageQigComponent } from './manage-qig.component';

describe('ManageQigComponent', () => {
  let component: ManageQigComponent;
  let fixture: ComponentFixture<ManageQigComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageQigComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageQigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
