import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TlAtlDashboardComponent } from './tl-atl-dashboard.component';

describe('TlAtlDashboardComponent', () => {
  let component: TlAtlDashboardComponent;
  let fixture: ComponentFixture<TlAtlDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TlAtlDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TlAtlDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
