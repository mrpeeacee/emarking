import { TestBed } from '@angular/core/testing';

import { LiveMarkingService } from './live-marking.service';

describe('LiveMarkingService', () => {
  let service: LiveMarkingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LiveMarkingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
