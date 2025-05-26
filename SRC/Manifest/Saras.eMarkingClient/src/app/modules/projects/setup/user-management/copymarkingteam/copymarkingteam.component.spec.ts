import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CopymarkingteamComponent } from './copymarkingteam.component';

describe('CopymarkingteamComponent', () => {
  let component: CopymarkingteamComponent;
  let fixture: ComponentFixture<CopymarkingteamComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CopymarkingteamComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CopymarkingteamComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
