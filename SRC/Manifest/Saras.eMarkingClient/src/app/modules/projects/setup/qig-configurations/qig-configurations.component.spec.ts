import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QigConfigurationsComponent } from './qig-configurations.component';

describe('QigConfigurationsComponent', () => {
  let component: QigConfigurationsComponent;
  let fixture: ComponentFixture<QigConfigurationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QigConfigurationsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QigConfigurationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
