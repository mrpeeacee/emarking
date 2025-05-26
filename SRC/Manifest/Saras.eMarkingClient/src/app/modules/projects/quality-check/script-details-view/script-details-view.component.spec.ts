import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScriptDetailsViewComponent } from './script-details-view.component';

describe('ScriptDetailsViewComponent', () => {
  let component: ScriptDetailsViewComponent;
  let fixture: ComponentFixture<ScriptDetailsViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScriptDetailsViewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScriptDetailsViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
