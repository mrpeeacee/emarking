import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FinaliseAsDefinitiveComponent } from './finalise-as-definitive.component';

describe('FinaliseAsDefinitiveComponent', () => {
  let component: FinaliseAsDefinitiveComponent;
  let fixture: ComponentFixture<FinaliseAsDefinitiveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FinaliseAsDefinitiveComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FinaliseAsDefinitiveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
