import { Component, OnInit } from '@angular/core';
import {
  FormBuilder, FormGroup, Validators, AbstractControl,
  ValidationErrors
} from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'src/app/services/auth/auth.service';

import * as CryptoJS from 'crypto-js';
import { environment } from 'src/environments/environment';
import { first } from 'rxjs/operators';
import { I18nComponent } from 'src/app/i18n/container/i18n.component';
import * as fromI18n from '../../../i18n/reducers';
import { Store } from '@ngrx/store';
import { CaptchaModel, UserLogin } from '../user';
import { AlertService } from 'src/app/services/common/alert.service';
import { BrandingModel } from 'src/app/model/globalconst';
import { NotificationService } from 'src/app/services/common/notification.service';
import { AppComponent } from 'src/app/app.component';
import { MatDialog } from '@angular/material/dialog';
import { ChangePasswordComponent } from '../change-password/change-password.component';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'emarking-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent extends I18nComponent implements OnInit {
  public graphicLogin = 'assets/images/graphic-1.svg';
  LoginStatus!: boolean;

  signinForm: FormGroup;
  lcLanguage!: any;
  ValidationErrMsg: string = '';
  ValidationErrMsgUserBlock: string = '';
  UserinfoLst!: UserLogin;
  LoginName!: string;
  brand!: BrandingModel;
  status!: boolean;
  encyptionRequried: boolean = true;
  logininprogress: boolean = false;
  IsCaptchaEnabled: any;


  constructor(
    public fb: FormBuilder,
    public authService: AuthService,
    public router: Router,
    public translate: TranslateService,
    readonly store: Store<fromI18n.State>,
    public notificatonservice: NotificationService,
    public Alert: AlertService,
    public appComponent: AppComponent,
    private dialog: MatDialog,
    private _sanitizer: DomSanitizer,
  ) {
    super(store, translate);
    this.signinForm = this.fb.group({
      loginname: ['', Validators.required],
      password: ['', Validators.required], 
    });

    translate.addLangs(['en', 'zh-sg']);
    this.lcLanguage = localStorage.getItem('language');

    if (this.lcLanguage != null) {
      translate.setDefaultLang(this.lcLanguage);
      translate.use(this.lcLanguage);
    } else {
      translate.setDefaultLang('en');
      translate.use('en');
      localStorage.setItem('language', 'en');
    }
  }

  // Declare this key and iv values in declaration
  private key = CryptoJS.enc.Utf8.parse(environment.enyKey);
  private iv = CryptoJS.enc.Utf8.parse(environment.enyKey);

  // Methods for the encrypt and decrypt Using AES
  encryptUsingAES256(encString: string) {
    return CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(encString), this.key, {
      keySize: 128 / 8,
      iv: this.iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7,
    });
  }

  ngOnInit() {
    if (this.authService.isLoggedIn) {
      this.authService.doLogout();
      if (environment.IsArchive) {
        this.NavigateToMarking();
      }

    } else {
      this.authService.clearAccessToken();
      if (environment.IsArchive) {
        this.NavigateToMarking();
      }

    }
    localStorage.removeItem('PHome');
    this.notificatonservice.brand$.subscribe(
      (msg: BrandingModel) => (this.brand = msg)
    );

    this.translate
      .get('Login.IsPasswordEncryptedInClient')
      .subscribe((translated: string) => {
        this.encyptionRequried = JSON.parse(translated);
      });

      this.signinForm = this.fb.group(
        {
          loginname: ['', [Validators.required]],
          password: ['', [Validators.required, this.SpecialCharctValidate]],
          CaptchaText: [],
          GUID:[],
          IscaptchaRequired:Boolean,
        });
  }
  CaptchaData!: CaptchaModel;
  captchaimage!: any;
   
  loginUser() {
    if (!this.signinForm.valid) {
      return;
    }
    this.ValidationErrMsg = '';
    this.ValidationErrMsgUserBlock = '';
    if (this.encyptionRequried) {
      if( this.signinForm.get('password')?.value!=null)
      {
      this.signinForm.patchValue({
        password: this.encryptUsingAES256(
          this.signinForm.get('password')?.value
        ).toString(),
      });
    }
    }
    this.logininprogress = true; 
    this.signinForm.get('IscaptchaRequired')?.setValue(this.IsCaptchaEnabled);
    if (this.IsCaptchaEnabled) {

      this.setCaptureValidator(this.IsCaptchaEnabled);
      if(this.signinForm.get('CaptchaText')?.value!=null)
      {
      this.UserinfoLst.CaptchaText =this.signinForm.get('CaptchaText')?.value.toString();
      this.UserinfoLst.GUID = this.CaptchaData.GUID;
      this.signinForm.get('GUID')?.setValue(this.CaptchaData.GUID.toString());
      this.UserinfoLst.GUID= this.signinForm.get('GUID')?.value.toString();
      this.signinForm.get('IscaptchaRequired')?.setValue(this.IsCaptchaEnabled);
      this.UserinfoLst.IscaptchaRequired= this.signinForm.get('IscaptchaRequired')?.value.toString();
      }
    }
    this.authService
      .Login(this.signinForm.value)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null && data != undefined) {
            this.UserinfoLst = data;       
            this.IsCaptchaEnabled=false;
            if(this.UserinfoLst.LoginCount>=6)
            {
              this.IsCaptchaEnabled=true
              if (this.IsCaptchaEnabled ) {
                this.getCaptchaImage();
                this.signinForm.get('CaptchaText')?.addValidators(Validators.required);
              } else {
                this.signinForm.get('CaptchaText')?.clearValidators();
              }
          
            }
            switch (this.UserinfoLst.Status) {
              case 'E003':
                this.signinForm.controls['password'].reset();
                this.signinForm.get('password')?.setErrors(null);
                this.showUserBlockErrorMessage('userblocked');
                break;
              case 'E002':
                this.signinForm.controls['password'].reset();
                this.signinForm.get('password')?.setErrors(null);
                this.showErrorMessage('error');
                break;
              case 'S001':
                if (this.UserinfoLst.IsFirstTimeLoggedIn) {
                  this.RedirecttoChangePassword();
                } else {
                  this.checkroleandredirect();
                }
                break;
              case 'E004':
                this.Alert.warning(
                  'Your account is disabled. Please contact Admin.'
                );
                break;
              case 'E005':
                this.Alert.warning(
                  'Invalid User. Please contact Admin.'
                );
                this.signinForm.controls['loginname'].reset();
                this.signinForm.get('loginname')?.setErrors(null);
                this.signinForm.controls['password'].reset();
                this.signinForm.get('password')?.setErrors(null);
                break;
              case 'E001':
                this.status = true;
                break;
              case 'E006':
                const totalMinutes: number = this.UserinfoLst.Timeleft;
                const formattedTimeLeft: string = totalMinutes > 0 ? totalMinutes.toFixed(2) : '0.00';
              this.Alert.warning('Your account is temporarily locked due to multiple failed login attempts. Please try again in'+" " +formattedTimeLeft+' minutes.');
              break;
              case 'INVALCAP':
                this.Alert.warning("Invalid Captcha")
                break;
              default:
                break;
            }
          } else {
            this.errorlogins();
          }
        },
        error: () => {
          this.errorlogins();
        },
        complete: () => {
          this.logininprogress = false;
        },
      });
  }
  setCaptureValidator(isCaptchaRequired: boolean) {
    const captureControl = this.signinForm.get('CaptchaText');
    if (isCaptchaRequired) {
      captureControl?.setValidators([Validators.required]); // Add required validator
    } else {
      captureControl?.clearValidators(); // Remove validators
    }
    captureControl?.updateValueAndValidity(); // Update the control's validity
  }
  
  checkroleandredirect() {
    this.router.navigateByUrl('projects');
    this.authService.loginStatus.next(true);
  }
  RedirecttoChangePassword() {
    this.signinForm.controls['loginname'].reset();
    this.signinForm.get('loginname')?.setErrors(null);
    this.signinForm.controls['password'].reset();
    this.signinForm.get('password')?.setErrors(null);
    const editorDialog = this.dialog.open(ChangePasswordComponent, {
      data: {
        isSaveClicked: 0,
      },
      panelClass: 'fullviewpop',
    });
    editorDialog.afterClosed().subscribe((result) => {
      if (result.data == 1) {
        //Empty on purpose
      }
    });
  }
  RedirecttoForgotPassword() {
    if (!this.logininprogress) {
      this.router.navigateByUrl('ForgotPassword');
      this.authService.loginStatus.next(false);
    }
  }
  Btnchangepassword() {
    this.router.navigateByUrl('change-password');
    this.authService.loginStatus.next(false);
  }

  onLangSelect(lang: string) {
    localStorage.setItem('language', lang);
    this.translate.use(lang);
  }
  showErrorMessage(message: string) {
    this.ValidationErrMsg = message;
  }
  showUserBlockErrorMessage(message: string) {
    this.ValidationErrMsgUserBlock = message;
  }

  errorlogins() {
    this.logininprogress = false;
    this.signinForm.controls['password'].reset();
    this.signinForm.get('password')?.setErrors(null);
    this.showErrorMessage('INVLGN');
  }

  SpecialCharctValidate = function (
    control: AbstractControl
  ): ValidationErrors | null {
   
    let value: string = control.value || '';

    if (!value) {
      return null;
    }

    if ((control.value as string).indexOf(' ') >= 0) {
      return { cannotContainSpace: true };
    }
    if (control.value.length < 12) {
      return { passwordStrengthlength: `lenthlessthan12` };
    }

    return null;
  };

  NavigateToMarking() {
    this.authService.GenerateSSOJWTTokenLive(environment.IsArchive).subscribe({
      next: (data: any) => {
        if (data != null && data != undefined && data != "") {
          window.location.href = data;  
        }
        else {
          this.RedirectToError();
        }
      }, error: () => {
        this.RedirectToError();
      }

    })
  }


  private RedirectToError() {
    this.router.navigateByUrl('error/403');
    this.authService.loginStatus.next(false);
  }
  getCaptchaImage() {
    this.IsCaptchaEnabled=true;
    this.authService.getCaptchaImage().
      subscribe({
        next: (data: any) => {
          this.CaptchaData = data;
          if (this.CaptchaData != null && this.CaptchaData.GUID != null) {
            this.captchaimage = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpg;base64,' + this.CaptchaData.CaptchaImage);
          }
        },
        error: (a: any) => {
          throw (a);
        },
      });
}
}
