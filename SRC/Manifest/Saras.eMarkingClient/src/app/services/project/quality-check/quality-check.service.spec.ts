import { TestBed } from '@angular/core/testing';

import { QualityCheckService } from './quality-check.service';

describe('QualityCheckService', () => {
  let service: QualityCheckService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(QualityCheckService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
