import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateMarkSchemeComponent } from './create-mark-scheme.component';

describe('CreateMarkSchemeComponent', () => {
  let component: CreateMarkSchemeComponent;
  let fixture: ComponentFixture<CreateMarkSchemeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateMarkSchemeComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateMarkSchemeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
