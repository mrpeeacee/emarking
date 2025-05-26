import {
  HttpHandler,
  HttpRequest,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, finalize} from 'rxjs';
import { Injectable } from '@angular/core';
import { AuthService } from '../auth/auth.service'; 

@Injectable()
export class HttpLoadingInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}
  RequestLength: number = 0;
  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    if(this.RequestLength < 0){
      this.RequestLength = 0;
    }
    this.RequestLength = this.RequestLength + 1;
    this.authService.setLoading(true);

    return next.handle(request).pipe(
      finalize(() => {
        this.RequestLength = this.RequestLength - 1;
        if (this.RequestLength <= 0) {
          this.authService.setLoading(false);          
        }
      })
    );
  }
}
