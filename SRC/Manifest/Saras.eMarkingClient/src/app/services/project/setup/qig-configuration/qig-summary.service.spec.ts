import { TestBed } from '@angular/core/testing';

import { QigSummaryService } from './qig-summary.service';

describe('QigSummaryService', () => {
  let service: QigSummaryService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(QigSummaryService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
