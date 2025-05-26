import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestCenterResponsesComponent } from './test-center-responses.component';

describe('TestCenterResponsesComponent', () => {
  let component: TestCenterResponsesComponent;
  let fixture: ComponentFixture<TestCenterResponsesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TestCenterResponsesComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestCenterResponsesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
