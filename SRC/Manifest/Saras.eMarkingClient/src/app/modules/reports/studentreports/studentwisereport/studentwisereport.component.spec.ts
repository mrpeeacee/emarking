import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentwisereportComponent } from './studentwisereport.component';

describe('StudentwisereportComponent', () => {
  let component: StudentwisereportComponent;
  let fixture: ComponentFixture<StudentwisereportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StudentwisereportComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentwisereportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
