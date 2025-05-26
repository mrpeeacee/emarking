import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MarkSchemeLibraryComponent } from './mark-scheme-library.component';

describe('MarkSchemeLibraryComponent', () => {
  let component: MarkSchemeLibraryComponent;
  let fixture: ComponentFixture<MarkSchemeLibraryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MarkSchemeLibraryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MarkSchemeLibraryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
