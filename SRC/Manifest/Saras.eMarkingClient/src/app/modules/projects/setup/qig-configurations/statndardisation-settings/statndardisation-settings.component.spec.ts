import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StatndardisationSettingsComponent } from './statndardisation-settings.component';

describe('StatndardisationSettingsComponent', () => {
  let component: StatndardisationSettingsComponent;
  let fixture: ComponentFixture<StatndardisationSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StatndardisationSettingsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StatndardisationSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
