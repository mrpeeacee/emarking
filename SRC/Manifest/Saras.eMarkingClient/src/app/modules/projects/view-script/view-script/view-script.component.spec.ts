import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewScriptComponent } from './view-script.component';

describe('ViewScriptComponent', () => {
  let component: ViewScriptComponent;
  let fixture: ComponentFixture<ViewScriptComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewScriptComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewScriptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
