import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QigTabComponent } from './qig-tab.component';

describe('QigTabComponent', () => {
  let component: QigTabComponent;
  let fixture: ComponentFixture<QigTabComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QigTabComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(QigTabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
