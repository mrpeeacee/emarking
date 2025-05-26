import { TestBed } from '@angular/core/testing';

import { QigconfigService } from './qigconfig.service';

describe('QigconfigService', () => {
  let service: QigconfigService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(QigconfigService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
