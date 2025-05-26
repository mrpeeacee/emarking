import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors, FormGroupDirective } from "@angular/forms";
import * as CryptoJS from 'crypto-js';
import { AuthService } from 'src/app/services/auth/auth.service';
import { TranslateService } from '@ngx-translate/core';
import { PasswordChange, CaptchaModel } from '../user';
import { first } from 'rxjs/operators';
import { AlertService } from 'src/app/services/common/alert.service';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'emarking-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit {

  public graphicLogin = "assets/images/graphic-1.svg";
  show_buttonNric: boolean = true;
  show_eyeNric!: boolean;
  show_buttonnewPassword: boolean = true;

  show_eyenewPassword!: boolean;
  show_buttonConfirmNewPassword: boolean = true;
  show_eyeconfirmNewPassword!: boolean;
  password!: string;
  ForgotpwdsigninForm!: FormGroup;
  submitted: boolean = false;
  ValidationErrMsg: string = "";
  status!: boolean;
  IsCaptchaEnabled: boolean = true;
  encyptionRequried: boolean = true;
  data1: any;

  constructor(public router: Router,
    public fb: FormBuilder,
    public authService: AuthService,
    public translate: TranslateService,
    public Alert: AlertService,
    private _sanitizer: DomSanitizer) { }


  // Declare this key and iv values in declaration
  private key = CryptoJS.enc.Utf8.parse(environment.enyKey);
  private iv = CryptoJS.enc.Utf8.parse(environment.enyKey);

  // Methods for the encrypt and decrypt Using AES
  encryptUsingAES256(encString: string) {
    return CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(encString), this.key, {
      keySize: 128 / 8,
      iv: this.iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    });
  }

  ngOnInit(): void {
    this.ForgotpwdsigninForm = this.fb.group({
      loginname: ['', Validators.required],
      nric: ['', [Validators.required, this.WhitespaceValidate]],
      fnewpassword: ['', [Validators.required, this.SpecialCharctValidate]],
      fcnfnewpassword: ['', Validators.required],
      Capture: ['', [Validators.required, Validators.maxLength(4), Validators.min(4)]]
    },
      {
        validator: this.ConfirmPasswordValidator("fnewpassword", "fcnfnewpassword")
      }
    );

    this.show_eyeNric = true;
    this.show_eyenewPassword = true;
    this.show_eyeconfirmNewPassword = true;
    this.translate.get('Login.IsPasswordEncryptedInClient').subscribe((translated: string) => {
      this.encyptionRequried = JSON.parse(translated);
    });

    this.translate
      .get('Login.IsCaptchaEnabled')
      .subscribe((translated: string) => {
        this.IsCaptchaEnabled = JSON.parse(translated);
        if (this.IsCaptchaEnabled) {
          this.getCaptchaImage();
          this.ForgotpwdsigninForm.get('Capture')?.addValidators(Validators.required);
        } else {
          this.ForgotpwdsigninForm.get('Capture')?.clearValidators();
        }
      });
  }
 
  SpecialCharctValidate = function (control: AbstractControl): ValidationErrors | null {
    let value: string = control.value || '';

    if (!value) {
      return null
    }

    if ((control.value as string).indexOf(' ') >= 0) {
      return { cannotContainSpace: true }
    }
    let onlyalphabets = /^(?=.*[a-z])(?=.*[A-Z])/g
    let NumSplChar = /^(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?])(?=.*\d)/g
    let SplCharalpha = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?](?=.*[a-zA-Z])/g
    let alphaNum = /^(?=.*[a-zA-Z])(?=.*\d).{11,24}/g
    let alphanumsplChar = /(?=.*[a-zA-Z0-9])(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?])/g

    let matchalphanumsplChar = control.value.match(alphanumsplChar);
    let matchOnlyAlpha = control.value.match(onlyalphabets);
    let matchNumSplChar = control.value.match(NumSplChar);
    let matchalphaNum = control.value.match(alphaNum);
    let matchSplCharalpha = control.value.match(SplCharalpha);

    if (matchOnlyAlpha == null && matchNumSplChar == null && matchalphaNum == null && matchSplCharalpha == null && matchalphanumsplChar == null) {
      return { passwordStrength: `Special char required` };
    }
    if (control.value.length < 12) {
      return { passwordStrengthlength: `lenthlessthan12` };
    }
    if (control.value.length > 50) {
      return { passwordStrengthlength: `lenthmorethan50` };
    }
    return null;

  }


  WhitespaceValidate = function (control: AbstractControl): ValidationErrors | null {
    let value: string = control.value || '';

    if (!value) {
      return null
    }

    if ((control.value as string).indexOf(' ') >= 0) {
      return { cannotContainSpace: true }
    }
    return null;
  }

  ConfirmPasswordValidator(controlName: string, matchingControlName: string) {
    return (formGroup: FormGroup) => {
      let control = formGroup.controls[controlName];
      let matchingControl = formGroup.controls[matchingControlName]
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

  BtnbacktoLoginPage() {
    this.router.navigateByUrl('login');
  }

  CaptchaData!: CaptchaModel;
  captchaimage!: any;
  getCaptchaImage() {
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



  ForgotPassword(formData: any, formDirective: FormGroupDirective) {
    this.submitted = true;
  
    if ((this.ForgotpwdsigninForm.get('Capture')?.value.toString() == '') && !this.IsCaptchaEnabled) {
      this.resetCaptureControl();
    }
  
    if (!this.ForgotpwdsigninForm.valid) {
      return;
    }
  
    this.ValidationErrMsg = "";
  
    if (!this.arePasswordsValid()) {
      return;
    }
  
    if (this.IsCaptchaEnabled && !this.isCaptchaValid()) {
      return;
    }
  
    const ForgotpasswordClassobj = this.createPasswordChangeObject();
  
    this.authService.Forgotpassword(ForgotpasswordClassobj)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.handleForgotPasswordResponse(data);
        },
        error: (a) => {
          this.Alert.error('Error while Changing Password.');
        },
      });
  }
  
  private resetCaptureControl() {
    this.ForgotpwdsigninForm.controls['Capture'].reset();
    this.ForgotpwdsigninForm.get("Capture")?.setErrors(null);
  }
  
  private arePasswordsValid(): boolean {
    const newPassword = this.ForgotpwdsigninForm.value.fnewpassword;
    const confirmPassword = this.ForgotpwdsigninForm.value.fcnfnewpassword;
  
    if (newPassword === '' || newPassword === null || confirmPassword === '' || confirmPassword === null) {
      return false;
    }
  
    if (newPassword !== confirmPassword) {
      return false;
    }
  
    return true;
  }
  
  private isCaptchaValid(): boolean {
    if (this.CaptchaData === undefined || this.ForgotpwdsigninForm.value.Capture === '' || this.ForgotpwdsigninForm.value.Capture === null || this.ForgotpwdsigninForm.value.Capture === undefined) {
      return false;
    }
  
    return true;
  }
  
  private createPasswordChangeObject(): PasswordChange {
    const ForgotpasswordClassobj = new PasswordChange();
  
    if (this.encyptionRequried) {
      ForgotpasswordClassobj.Newpassword = this.encryptUsingAES256(this.ForgotpwdsigninForm.get('fnewpassword')?.value).toString();
      ForgotpasswordClassobj.Cnfnewpassword = this.encryptUsingAES256(this.ForgotpwdsigninForm.get('fcnfnewpassword')?.value).toString();
    } else {
      ForgotpasswordClassobj.Newpassword = this.ForgotpwdsigninForm.get('fcnfnewpassword')?.value.toString();
      ForgotpasswordClassobj.Cnfnewpassword = this.ForgotpwdsigninForm.get('fcnfnewpassword')?.value.toString();
    }
  
    ForgotpasswordClassobj.LoginID = this.ForgotpwdsigninForm.get('loginname')?.value.toString();
    ForgotpasswordClassobj.NRIC = this.ForgotpwdsigninForm.get('nric')?.value.toString();
  
    if (this.IsCaptchaEnabled) {
      ForgotpasswordClassobj.CaptchaText = this.ForgotpwdsigninForm.get('Capture')?.value.toString();
      ForgotpasswordClassobj.GUID = this.CaptchaData.GUID;
    }
  
    return ForgotpasswordClassobj;
  }
  
  private handleForgotPasswordResponse(data: any) {
   
    this.data1=data
    switch (data.status) {
      case "U001":
        this.status = true;
        break;
      case "Disabled":
        this.status = false;
        this.Alert.clear();
        this.Alert.warning("Your account is disabled. Please contact Admin.");
        break;
      case "Suspended":
          this.status = false;
        this.Alert.clear();
        const totalMinutes: number = this.data1.Timeleft;
         const formattedTimeLeft: string = totalMinutes > 0 ? totalMinutes.toFixed(2) : '0.00';
      this.Alert.warning('Your account is temporarily locked due to multiple failed login attempts. Please try again in'+" " +formattedTimeLeft+' minutes.');
        break;
      case "InvalidNRICorLoginId":
        this.status = false;
        this.translate.get('Login.Passwordupdatefailed').subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
        break;
      case "BLOCKED":
        this.status = false;
        this.translate.get('Login.Blockederror').subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
        break;
      case "U004":
        this.translate.get('Login.Notsamepassword').subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.error(translated);
        });
        break;
      case "SERROR":
        this.translate.get('Login.Securityerror').subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
        break;
      case "INVALCAP":
        this.translate.get('Login.WrongCaptchaText').subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
        break;
      default:
        this.status = false;
        this.translate.get('Login.Securityerror').subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
        break;
    }
  }
  

}
