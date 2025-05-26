import { TestBed } from '@angular/core/testing';

import { QigService } from './qig.service';

describe('QigService', () => {
  let service: QigService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(QigService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
