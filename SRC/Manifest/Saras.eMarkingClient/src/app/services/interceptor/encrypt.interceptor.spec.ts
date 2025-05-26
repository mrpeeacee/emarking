import { TestBed } from '@angular/core/testing';

import { EncryptInterceptor } from './encrypt.interceptor';

describe('EncryptInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      EncryptInterceptor
      ]
  }));

  it('should be created', () => {
    const interceptor: EncryptInterceptor = TestBed.inject(EncryptInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
