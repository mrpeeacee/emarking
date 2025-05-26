import { Component } from '@angular/core';
import { AuthService } from 'src/app/services/auth/auth.service';
import { AlertService } from 'src/app/services/common/alert.service';

@Component({
  selector: 'emarking-sso-login-live',
  templateUrl: './sso-login-live.component.html',
  styleUrls: ['./sso-login-live.component.css']
})
export class SsoLoginLiveComponent  {

  constructor(
    public authservice: AuthService,
    public Alert: AlertService,
  ) { }

  public IsDisable: boolean = false


  LiveDashBoard() {
    this.IsDisable = true
    this.authservice.GenerateSSOJWTTokenLive().subscribe(result => {
      let enc = result;
      if (enc != null && enc != "" && enc != undefined) {
        window.location.href = enc;
      }
      else {
        this.IsDisable = false
        this.Alert.warning("User Does Not Exist...!")
      }
    })
  }

}
