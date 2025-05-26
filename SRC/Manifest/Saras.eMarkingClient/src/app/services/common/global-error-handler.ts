import { HttpErrorResponse } from '@angular/common/http';
import { ErrorHandler, Injectable } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { AlertService } from './alert.service';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(
    public Alert: AlertService,
    public authService: AuthService  ) {}

  handleError(error: any) {
    console.log(error);
    if (error instanceof HttpErrorResponse) {
      if (error.status == 403 || error.status == 401) {
        this.Alert.warning('Session expired.');
        this.authService.loginStatus.next(false);
        location.href = 'error/401';
      } else if (error.status == 400) {
        this.Alert.warning('Bad Request. Please check your input.');
      } else if (error.status == 500) {
        this.Alert.error('Internal Server Error. ' + error.error.Detail);
      } else if (error.status == 504) {
        this.Alert.error(error.statusText);
      } else {
        this.Alert.error('An unexpected error occurred. Please try again.');
      }
    } //else {
      //  (error.error instanceof ErrorEvent)
      //  this.Alert.error('An error occurred: ', error.error.message)
      //  else
      //  this.Alert.error('An error occurred: ' + error.message)
      //
    //}
  }
}
