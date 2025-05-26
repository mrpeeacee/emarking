import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentreportsComponent } from './studentreports.component';

describe('StudentreportsComponent', () => {
  let component: StudentreportsComponent;
  let fixture: ComponentFixture<StudentreportsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StudentreportsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentreportsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
