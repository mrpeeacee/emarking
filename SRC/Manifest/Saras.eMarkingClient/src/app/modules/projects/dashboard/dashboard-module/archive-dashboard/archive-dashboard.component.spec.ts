import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ArchiveDashboardComponent } from './archive-dashboard.component';

describe('AoCmDashboardComponent', () => {
  let component: ArchiveDashboardComponent;
  let fixture: ComponentFixture<ArchiveDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ArchiveDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ArchiveDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
