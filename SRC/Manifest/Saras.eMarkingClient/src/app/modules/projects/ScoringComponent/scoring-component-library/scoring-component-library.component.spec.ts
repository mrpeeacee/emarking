import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScoringComponentLibraryComponent } from './scoring-component-library.component';

describe('ScoringComponentLibraryComponent', () => {
  let component: ScoringComponentLibraryComponent;
  let fixture: ComponentFixture<ScoringComponentLibraryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScoringComponentLibraryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScoringComponentLibraryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
