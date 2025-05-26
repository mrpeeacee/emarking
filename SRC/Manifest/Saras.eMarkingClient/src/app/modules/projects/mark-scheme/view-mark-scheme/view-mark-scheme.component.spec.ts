import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewMarkSchemeComponent } from './view-mark-scheme.component';

describe('ViewMarkSchemeComponent', () => {
  let component: ViewMarkSchemeComponent;
  let fixture: ComponentFixture<ViewMarkSchemeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewMarkSchemeComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewMarkSchemeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
