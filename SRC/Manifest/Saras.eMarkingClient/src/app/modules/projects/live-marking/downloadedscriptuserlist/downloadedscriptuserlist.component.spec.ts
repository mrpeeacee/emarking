import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DownloadedscriptuserlistComponent } from './downloadedscriptuserlist.component';

describe('DownloadedscriptuserlistComponent', () => {
  let component: DownloadedscriptuserlistComponent;
  let fixture: ComponentFixture<DownloadedscriptuserlistComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DownloadedscriptuserlistComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DownloadedscriptuserlistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
