import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MarkingTypeConfigComponent } from './marking-type-config.component';

describe('MarkingTypeConfigComponent', () => {
  let component: MarkingTypeConfigComponent;
  let fixture: ComponentFixture<MarkingTypeConfigComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MarkingTypeConfigComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MarkingTypeConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
