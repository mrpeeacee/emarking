import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScriptListViewComponent } from './script-list-view.component';

describe('ScriptListViewComponent', () => {
  let component: ScriptListViewComponent;
  let fixture: ComponentFixture<ScriptListViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScriptListViewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScriptListViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
