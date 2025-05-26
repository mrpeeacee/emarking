import { ComponentFixture, TestBed } from '@angular/core/testing';

import { S2S3configurationsComponent } from './s2-s3configurations.component';

describe('S2S3configurationsComponent', () => {
  let component: S2S3configurationsComponent;
  let fixture: ComponentFixture<S2S3configurationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ S2S3configurationsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(S2S3configurationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
