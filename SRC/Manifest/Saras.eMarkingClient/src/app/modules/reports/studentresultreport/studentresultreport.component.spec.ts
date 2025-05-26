import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentresultreportComponent } from './studentresultreport.component';

describe('StudentresultreportComponent', () => {
  let component: StudentresultreportComponent;
  let fixture: ComponentFixture<StudentresultreportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StudentresultreportComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentresultreportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
