import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditMarkSchemeComponent } from './edit-mark-scheme.component';

describe('EditMarkSchemeComponent', () => {
  let component: EditMarkSchemeComponent;
  let fixture: ComponentFixture<EditMarkSchemeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditMarkSchemeComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditMarkSchemeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
