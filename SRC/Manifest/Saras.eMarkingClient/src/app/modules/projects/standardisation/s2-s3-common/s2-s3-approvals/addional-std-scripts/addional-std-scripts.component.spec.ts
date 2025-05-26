import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddionalStdScriptsComponent } from './addional-std-scripts.component';

describe('AddionalStdScriptsComponent', () => {
  let component: AddionalStdScriptsComponent;
  let fixture: ComponentFixture<AddionalStdScriptsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddionalStdScriptsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddionalStdScriptsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
