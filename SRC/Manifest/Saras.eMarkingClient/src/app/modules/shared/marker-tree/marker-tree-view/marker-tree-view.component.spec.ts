import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MarkerTreeViewComponent } from './marker-tree-view.component';

describe('MarkerTreeViewComponent', () => {
  let component: MarkerTreeViewComponent;
  let fixture: ComponentFixture<MarkerTreeViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MarkerTreeViewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MarkerTreeViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
