import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AnnotationSettingsComponent } from './annotation-settings.component';

describe('AnnotationSettingsComponent', () => {
  let component: AnnotationSettingsComponent;
  let fixture: ComponentFixture<AnnotationSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AnnotationSettingsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AnnotationSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
