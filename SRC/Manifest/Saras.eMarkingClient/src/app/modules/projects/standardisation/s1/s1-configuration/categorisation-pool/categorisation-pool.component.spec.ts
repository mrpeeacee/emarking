import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategorisationPoolComponent } from './categorisation-pool.component';

describe('CategorisationPoolComponent', () => {
  let component: CategorisationPoolComponent;
  let fixture: ComponentFixture<CategorisationPoolComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CategorisationPoolComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CategorisationPoolComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
