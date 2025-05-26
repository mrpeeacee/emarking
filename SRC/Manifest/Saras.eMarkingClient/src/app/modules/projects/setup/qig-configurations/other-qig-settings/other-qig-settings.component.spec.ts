import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OtherQigSettingsComponent } from './other-qig-settings.component';

describe('OtherQigSettingsComponent', () => {
  let component: OtherQigSettingsComponent;
  let fixture: ComponentFixture<OtherQigSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OtherQigSettingsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OtherQigSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
