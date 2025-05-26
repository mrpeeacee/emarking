import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateqigComponent } from './createqig.component';

describe('CreateqigComponent', () => {
  let component: CreateqigComponent;
  let fixture: ComponentFixture<CreateqigComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateqigComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateqigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
