import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MarkerDashboardComponent } from './marker-dashboard.component';

describe('MarkerDashboardComponent', () => {
  let component: MarkerDashboardComponent;
  let fixture: ComponentFixture<MarkerDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MarkerDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MarkerDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
