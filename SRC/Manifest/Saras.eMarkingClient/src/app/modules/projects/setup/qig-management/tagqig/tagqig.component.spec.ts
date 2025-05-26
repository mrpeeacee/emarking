import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TagqigComponent } from './tagqig.component';

describe('TagqigComponent', () => {
  let component: TagqigComponent;
  let fixture: ComponentFixture<TagqigComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TagqigComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TagqigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
