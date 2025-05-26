import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QnsQigMappingComponent } from './qns-qig-mapping.component';

describe('QnsQigMappingComponent', () => {
  let component: QnsQigMappingComponent;
  let fixture: ComponentFixture<QnsQigMappingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QnsQigMappingComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QnsQigMappingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
