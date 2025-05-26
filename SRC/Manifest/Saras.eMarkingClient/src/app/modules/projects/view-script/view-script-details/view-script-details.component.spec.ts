import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewScriptDetailsComponent } from './view-script-details.component';

describe('ViewScriptDetailsComponent', () => {
  let component: ViewScriptDetailsComponent;
  let fixture: ComponentFixture<ViewScriptDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewScriptDetailsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewScriptDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
