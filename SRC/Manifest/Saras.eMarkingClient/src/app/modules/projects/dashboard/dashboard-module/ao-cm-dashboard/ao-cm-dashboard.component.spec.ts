import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AoCmDashboardComponent } from './ao-cm-dashboard.component';

describe('AoCmDashboardComponent', () => {
  let component: AoCmDashboardComponent;
  let fixture: ComponentFixture<AoCmDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AoCmDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AoCmDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
