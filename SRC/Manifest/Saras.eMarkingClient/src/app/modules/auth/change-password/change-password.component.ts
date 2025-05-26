import { Component, OnInit, Inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ValidationErrors,
  FormGroupDirective,
} from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'src/app/services/auth/auth.service';

import * as CryptoJS from 'crypto-js';
import { environment } from 'src/environments/environment';
import { first } from 'rxjs/operators';
import { I18nComponent } from 'src/app/i18n/container/i18n.component';
import * as fromI18n from 'src/app/i18n/reducers';
import { Store } from '@ngrx/store';
import { AlertService } from 'src/app/services/common/alert.service';
import { BrandingModel } from 'src/app/model/globalconst';
import { NotificationService } from 'src/app/services/common/notification.service';
import { PasswordChange } from 'src/app/modules/auth/user';
import {
  MatDialogRef,
  MatDialog,
  MAT_DIALOG_DATA,
} from '@angular/material/dialog';
import { CreateEditUser } from 'src/app/model/Global/UserManagement/UserManagementModel';

@Component({
  templateUrl: './change-password.component.html',
  styleUrls: ['../login/login.component.css'],
})
export class ChangePasswordComponent extends I18nComponent implements OnInit {
  public logo = 'assets/images/saras-logo.png';

  public graphicLogin = 'assets/images/graphic-1.svg';
  LoginStatus!: boolean;
  show_buttonoldPassword: boolean = true;
  show_eyeoldPassword: boolean = false;
  show_buttonnewPassword: boolean = true;
  show_eyenewPassword: boolean = false;
  show_buttonConfirmPassword: boolean = true;
  show_eyeconfirmPassword: boolean = false;
  signinForm!: FormGroup;
  submitted: boolean = false;
  lcLanguage!: any;
  ValidationErrMsg: string = '';
  LoginName!: string;
  status!: boolean;
  Issession!: any;
  brand!: BrandingModel;

  password!: string;
  hideoldpassword!: boolean;
  hidenewpassword!: boolean;
  hideconfirmpassword!: boolean;
  show = false;
  encyptionRequried: boolean = true;
  Objuserdetails!: CreateEditUser;

  constructor(
    public fb: FormBuilder,
    public authService: AuthService,
    public router: Router,
    public translate: TranslateService,
    readonly store: Store<fromI18n.State>,
    public notificatonservice: NotificationService,
    public Alert: AlertService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<ChangePasswordComponent>,
    private dialog: MatDialog
  ) {
    super(store, translate);
    this.initializeLanguage();
  }

  private initializeLanguage() {
    this.translate.addLangs(['en', 'zh-sg']);
    this.lcLanguage = localStorage.getItem('language');
    if (this.lcLanguage != null) {
      this.translate.setDefaultLang(this.lcLanguage);
      this.translate.use(this.lcLanguage);
    } else {
      this.translate.setDefaultLang('en');
      this.translate.use('en');
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
    this.SessionExpire();
    this.signinForm = this.fb.group(
      {
        oldpassword: ['', Validators.required],
        newpassword: ['', [Validators.required, this.SpecialCharctValidate]],
        cnfnewpassword: ['', Validators.required],
      },
      {
        validator: this.ConfirmPasswordValidator(
          'newpassword',
          'cnfnewpassword'
        ),
      }
    );
    this.notificatonservice.brand$.subscribe(
      (msg: BrandingModel) => (this.brand = msg)
    );

    this.hideoldpassword = true;
    this.hidenewpassword = true;
    this.hideconfirmpassword = true;
    this.translate
      .get('Login.IsPasswordEncryptedInClient')
      .subscribe((translated: string) => {
        this.encyptionRequried = JSON.parse(translated);
      });

    this.Objuserdetails = this.data.CreateEditUser;
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
    let onlyalphabets = /^(?=.*[a-z])(?=.*[A-Z])/g;
    let NumSplChar = /^(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?])(?=.*\d)/g;
    let SplCharalpha = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?](?=.*[a-zA-Z])/g;
    let alphaNum = /^(?=.*[a-zA-Z])(?=.*\d).{11,24}/g;
    let alphanumsplChar =
      /(?=.*[a-zA-Z0-9])(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?])/g;

    let matchalphanumsplChar = control.value.match(alphanumsplChar);
    let matchOnlyAlpha = control.value.match(onlyalphabets);
    let matchNumSplChar = control.value.match(NumSplChar);
    let matchalphaNum = control.value.match(alphaNum);
    let matchSplCharalpha = control.value.match(SplCharalpha);

    if (control.value.length < 12) {
      return { passwordStrengthlength: `lenthlessthan12` };
    }
    if (control.value.length > 50) {
      return { passwordStrengthlength: `lenthmorethan12` };
    }
    if (
      matchOnlyAlpha == null &&
      matchNumSplChar == null &&
      matchalphaNum == null &&
      matchSplCharalpha == null &&
      matchalphanumsplChar == null
    ) {
      return { passwordStrength: `Special char required` };
    }    
    return null;
  };

  ConfirmPasswordValidator(controlName: string, matchingControlName: string) {
    return (formGroup: FormGroup) => {
      let control = formGroup.controls[controlName];
      let matchingControl = formGroup.controls[matchingControlName];
      if (
        matchingControl.errors &&
        !matchingControl.errors.confirmPasswordValidator
      ) {
        return;
      }
      if (control.value !== matchingControl.value) {
        matchingControl.setErrors({ confirmPasswordValidator: true });
      } else {
        matchingControl.setErrors(null);
      }
    };
  }

  SessionExpire() {
    this.authService
      .SessionExpire()
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null) {
            this.Issession = data;
            if (!this.Issession) {
              this.authService.doLogout();
            }
          } else {
            this.authService.doLogout();
          }
        },
        error: (a: any) => {
          throw a;
        },
      });
  }

  btnChangePassword(formData: any, formDirective: FormGroupDirective) {
    this.submitted = true;

    if (!this.signinForm.valid) {
      return;
    }

    this.ValidationErrMsg = '';

    if (this.isFormValuesEmpty()) {
      return;
    }

    if (!this.isNewPasswordsMatch()) {
      return;
    }

    let passwordClassobj = this.createPasswordChangeObject();

    if (this.Objuserdetails != null || this.Objuserdetails != undefined) {
      this.authService
        .CommonChangepassword(passwordClassobj, this.Objuserdetails.Loginname)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.handlePasswordChangeResponse(data);
          },
          error: (a) => {
            this.redirecttologin();
            this.showErrorMessage(a.message);
          },
        });
    } else {
      this.authService
        .Changepassword(passwordClassobj)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.handlePasswordChangeResponse(data);
          },
          error: (a) => {
            this.redirecttologin();
            this.showErrorMessage(a.message);
          },
        });
    }
  }

  isFormValuesEmpty(): boolean {
    const { oldpassword, newpassword, cnfnewpassword } = this.signinForm.value;
    return !oldpassword || !newpassword || !cnfnewpassword;
  }

  isNewPasswordsMatch(): boolean {
    return (
      this.signinForm.value.newpassword === this.signinForm.value.cnfnewpassword
    );
  }

  createPasswordChangeObject(): PasswordChange {
    let passwordClassobj = new PasswordChange();
    if (this.encyptionRequried) {
      passwordClassobj.Oldpassword = this.encryptUsingAES256(
        this.signinForm.get('oldpassword')?.value
      ).toString();
      passwordClassobj.Newpassword = this.encryptUsingAES256(
        this.signinForm.get('newpassword')?.value
      ).toString();
      passwordClassobj.Cnfnewpassword = this.encryptUsingAES256(
        this.signinForm.get('cnfnewpassword')?.value
      ).toString();
    } else {
      passwordClassobj.Oldpassword = this.signinForm.get('oldpassword')?.value;
      passwordClassobj.Newpassword = this.signinForm.get('newpassword')?.value;
      passwordClassobj.Cnfnewpassword =
        this.signinForm.get('cnfnewpassword')?.value;
    }
    return passwordClassobj;
  }

  handlePasswordChangeResponse(data: any) {
    if (data == 'U001') {
      this.status = true;
    } else if (data == 'U003') {
      this.translate
        .get('Login.OldPaaswordntmatch')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.error(translated);
        });
    } else if (data == 'U004') {
      this.translate
        .get('Login.Notsamepassword')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.error(translated);
        });
    } else if (data == 'SERROR') {
      this.translate
        .get('Login.Securityerror')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
    } else {
      this.status = false;
      this.redirecttologin();
    }
  }

  BacktoLoginPage() {
    this.authService.doLogout();
    setTimeout(() => {
      this.dialogRef.close({ status: 0 });
    }, 1000);
  }

  showOldPassword() {
    this.show_buttonoldPassword = !this.show_buttonoldPassword;
    this.show_eyeoldPassword = !this.show_eyeoldPassword;
  }

  showNewPassword() {
    this.show_buttonnewPassword = !this.show_buttonnewPassword;
    this.show_eyenewPassword = !this.show_eyenewPassword;
  }

  showConfirmPassword() {
    this.show_buttonConfirmPassword = !this.show_buttonConfirmPassword;
    this.show_eyeconfirmPassword = !this.show_eyeconfirmPassword;
  }

  redirecttologin() {
    this.router.navigateByUrl('login');
    this.authService.loginStatus.next(false);
  }

  showErrorMessage(message: string) {
    this.ValidationErrMsg = message;
  }

  closeMethod(evnt: any) {
    this.dialogRef.close({ status: 0 });
  }
}
