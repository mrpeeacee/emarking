import { ComponentFixture, TestBed } from '@angular/core/testing';

import { S2S3ApprovalsComponent } from './s2-s3-approvals.component';

describe('S2S3ApprovalsComponent', () => {
  let component: S2S3ApprovalsComponent;
  let fixture: ComponentFixture<S2S3ApprovalsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ S2S3ApprovalsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(S2S3ApprovalsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
