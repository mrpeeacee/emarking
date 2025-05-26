import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MarkersreportsComponent } from './markersreports.component';

describe('MarkersreportsComponent', () => {
  let component: MarkersreportsComponent;
  let fixture: ComponentFixture<MarkersreportsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MarkersreportsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MarkersreportsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
