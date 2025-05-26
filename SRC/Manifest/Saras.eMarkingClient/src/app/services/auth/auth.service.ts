import { Injectable } from '@angular/core';
import {
  UserLogin,
  PasswordChange,
  CaptchaModel
} from '../../modules/auth/user';
import { BehaviorSubject, EMPTY, Observable, throwError } from 'rxjs';
import { catchError, first, map } from 'rxjs/operators';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { BaseService } from '../base.service';
import { AlertService } from '../common/alert.service';
import { Tokenresult } from 'src/app/model/auth/tokenresult';
import { CommonService } from '../common/common.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends BaseService {
  currentUser = {};

  public dataSubject = new BehaviorSubject<boolean>(false);
  constructor(
    private http: HttpClient,
    public router: Router,
    public Alert: AlertService,
    public commonservice: CommonService
  ) {
    super();
  }

  Login(user: UserLogin) {
    return this.http
      .post<Tokenresult>(
        `${this.endpoint}/api/public/${this.apiVersion}/authenticate/login`,
        user,
        { headers: this.headers, observe: 'response' }
      )
      .pipe(
        catchError(err => {
          return throwError(err);
        }),
        map(result => {
          if (result != undefined) {
            if (result.body != null && result.headers != null) {
              var respx = result.headers.get('X-Token');
              if (respx != null) {
                sessionStorage.setItem('xHeader', respx);
              }
              if (!result.body.IsFirstTimeLoggedIn) {
                this.setToken(result.body);
                this.loginStatus.next(false);
              }
            }
          }
          return result.body;
        })
      );
  }

  Changepassword(objPasswordChange: PasswordChange) {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/authenticate/change-password`;
    return this.http
      .post<any>(api, objPasswordChange, { headers: this.headers })
      .pipe(
        catchError(err => {
          return throwError(err);
        }),
        map(result => {
          return result;
        })
      );
  }

  Forgotpassword(objForgotpassword: PasswordChange) {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/authenticate/forgot-password`;
    return this.http
      .post<any>(api, objForgotpassword, { headers: this.headers })
      .pipe(
        catchError(err => {
          return throwError(err);
        }),
        map(result => {
          return result;
        })
      );
  }

  SessionExpire() {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/authenticate/session-expire`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      catchError(err => {
        return throwError(err);
      }),
      map(result => {
        return result;
      })
    );
  }

  getCurrentRole(): string[] {
    let cntrole = [];
    let token = this.getToken();
    if (token != null) {
      if (token.Roles != null && token.Roles != undefined) {
        let roles = token.Roles;
        if (roles != null && roles != undefined && roles != '') {
          let str_array = roles.split(',');
          for (var i = 0; i < str_array.length; i++) {
            str_array[i] = str_array[i].replace(/^\s*/, '').replace(/\s*$/, '');
            if (str_array[i] != '') {
              cntrole.push(str_array[i]);
            }
          }
        }
      }
    }
    return cntrole;
  }

  reGenerateToken(projectId: number = 0): Observable<Tokenresult> {

   
    if (this.isLoggedIn) {
      let refeUrl = '';
      if (projectId >= -1) {
        refeUrl =
          `${this.endpoint}/api/public/${this.apiVersion}/authenticate/refresh-token/` +
          projectId;
      } else {
        refeUrl = `${this.endpoint}/api/public/${this.apiVersion}/authenticate/refresh-token`;
      }
      return this.http
        .post<Tokenresult>(refeUrl, {}, { headers: this.headers })
        .pipe(
          map(tokenresp => {
            this.setToken(tokenresp);
            return tokenresp;
          })
        );
    } else {
      return EMPTY;
    }
  }

  getToken(): Tokenresult | null {
   
    let token = localStorage.getItem('access_token');
    if (token != null && token != '' && token != undefined) {
      try {
        return JSON.parse(token);
      } catch {
        return null;
      }
    }
    return null;
  }

  setToken(token: Tokenresult) {
    if (token != null) {
      localStorage.setItem('access_token', JSON.stringify(token));
    }
  }

  clearAccessToken() {
    sessionStorage.removeItem('xHeader');
    localStorage.removeItem('access_token');
  }

  get isLoggedIn(): boolean {
   
    let authToken = this.getToken();
    return authToken !== null && authToken.RefreshInterval > 0;
  }

  public loginStatus = new BehaviorSubject<boolean>(this.isLoggedIn);
  get isAuthenticated() {
    return this.loginStatus.asObservable();
  }

  doLogout(redirectToLogin: boolean = true) {
    let isLoggedOut = false;
    this.apiLogout()
      .pipe(first())
      .subscribe({
        next: (res: any) => {
          if (res) {
            isLoggedOut = true;
            this.loginStatus.next(false);
          }
        },
        error: () => {
          isLoggedOut = true;
          this.loginStatus.next(false);
        },
        complete: () => {
          this.clearAccessToken();
          if (redirectToLogin) {
            this.router.navigate(['login']);
          }
        }
      });

    return isLoggedOut;
  }

  apiLogout() {
    return this.http
      .post<{ token: string }>(
        `${this.endpoint}/api/public/${this.apiVersion}/authenticate/LogoutAsync`,
        this.getToken(),
        { headers: this.headers }
      )
      .pipe(
        catchError(err => {
          return throwError(err);
        }),
        map(() => {
          return true;
        })
      );
  }

  // User profile
  getUserProfile(id: string): Observable<any> {
    let api = `${this.endpoint}/user-profile/${id}`;
    return this.http.get<Response>(api, { headers: this.headers }).pipe(
      map((res: Response) => {
        return res || {};
      })
    );
  }

  getCaptchaImage(): Observable<any> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/authenticate/get-captcha`;
    return this.http.get<CaptchaModel>(api, { headers: this.headers }).pipe(
      map((res: CaptchaModel) => {
        return res || {};
      })
    );
  }

  CommonChangepassword(objPasswordChange: PasswordChange, UserLoginId: string) {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/authenticate/change-password/${UserLoginId}`;
    return this.http
      .post<any>(api, objPasswordChange, { headers: this.headers })
      .pipe(
        catchError(err => {
          return throwError(err);
        }),
        map(result => {
          return result;
        })
      );
  }

  get getLoading() {
    return this.dataSubject.asObservable();
  }  

  setLoading(data: boolean) {
    this.dataSubject.next(data);
  }



  GenerateSSOJWTToken(): Observable<any> {
   
    let api = `${this.endpoint}/api/public/${this.apiVersion}/authenticate/SSOArchiveLogin`;
    return this.http.get<any>(api,{ headers: this.headers} ).pipe(
      catchError(err => {
        return throwError(err);
      }),
      map(result => {
        return result;
      })
    );
  }

  GenerateSSOJWTTokenLive(IsArchive:boolean=false): Observable<any> {
   
    let api = `${this.endpoint}/api/public/${this.apiVersion}/authenticate/SSOEmarkingLogin/${IsArchive}`;
    return this.http.get<any>(api,{ headers: this.headers} ).pipe(
      catchError(err => {
        return throwError(err);
      }),
      map(result => {
        return result;
      })
    );
  }

  


  
  

  


  JwtSsoLogin(enc: string) {
    return this.http
      .post<Tokenresult>(
        `${this.endpoint}/api/public/${this.apiVersion}/authenticate/jwt/launch/${enc}`,
        null,
        { headers: this.headers, observe: 'response' }
      )
      .pipe(
        catchError((err) => {
          throw err;
        }),
        map((result) => {
          this.LoginSuccess(result);
          return result.body;
        })
      );
  }


  LoginSuccess(result: HttpResponse<Tokenresult>) {
    if (result != undefined) {
      if (result.body != null && result.headers != null) {
        var respx = result.headers.get('X-Token');
        if (respx != null) {
          sessionStorage.setItem('xHeader', respx);
        }
        if (!result.body.IsFirstTimeLoggedIn) {
          this.setToken(result.body);
          this.loginStatus.next(false);
        }
      }
    }
  }



}
