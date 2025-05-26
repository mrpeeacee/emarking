import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from '../base.service';
import { AlertService } from '../common/alert.service';

@Injectable()
export class EncryptInterceptor extends BaseService implements HttpInterceptor {

  constructor(public Alert: AlertService) {
    super();
  }

  intercept(request: HttpRequest<unknown>,
    next: HttpHandler
  ):
    Observable<HttpEvent<unknown>> {
    if (request.url.indexOf(`${this.endpoint}/api/public/${this.apiVersion}/authenticate/login`) === -1) {
      return next.handle(request);
    }
    return next.handle(request);
  }
}
