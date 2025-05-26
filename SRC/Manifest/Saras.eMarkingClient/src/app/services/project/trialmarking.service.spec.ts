import { TestBed } from '@angular/core/testing';

import { TrialmarkingService } from './trialmarking.service';

describe('TrialmarkingService', () => {
  let service: TrialmarkingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TrialmarkingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
