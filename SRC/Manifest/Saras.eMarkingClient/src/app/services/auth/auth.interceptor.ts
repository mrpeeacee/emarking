import { HTTP_INTERCEPTORS, HttpErrorResponse, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, throwError } from 'rxjs';
import { catchError, filter, concatMap, take } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable()

export class AuthInterceptor implements HttpInterceptor {

  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(private authService: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    req = req.clone({
      withCredentials: true
    });

    return next.handle(req).pipe(catchError(error => {
      if (error instanceof HttpErrorResponse && !req.url.includes('/authenticate/login') && !req.url.includes('/authenticate/refresh-token') && !req.url.includes('/authenticate/LogoutAsync') && error.status === 401) {
        return this.handle401Error(req, next);
      }
      return throwError(() => error);
    }));
  }


  private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.authService.reGenerateToken(0).pipe(
        concatMap(() => {
          this.isRefreshing = false;
          this.refreshTokenSubject.next(true);
          return next.handle(this.addTokenHeader(request));
        }),
        catchError((err) => {
          this.isRefreshing = false; 
          return throwError(() => err);
        })
      );

    }

    return this.refreshTokenSubject.pipe(
      filter(token => token !== null),
      take(1),
      concatMap(() => {
        return next.handle(this.addTokenHeader(request));
      })
    );
  }

  private addTokenHeader(request: HttpRequest<any>) {
    return request.clone({
      withCredentials: true
    });
  }
}

export const authInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
];
