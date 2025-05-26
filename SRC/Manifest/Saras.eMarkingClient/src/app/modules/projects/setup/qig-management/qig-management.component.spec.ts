import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QigManagementComponent } from './qig-management.component';

describe('QigManagementComponent', () => {
  let component: QigManagementComponent;
  let fixture: ComponentFixture<QigManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QigManagementComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QigManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
