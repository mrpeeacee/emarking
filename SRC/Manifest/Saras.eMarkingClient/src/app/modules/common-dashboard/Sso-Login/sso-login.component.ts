import { Component} from '@angular/core';
import { AuthService } from 'src/app/services/auth/auth.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { Router } from '@angular/router';
import { JwtLaunchComponentComponent } from '../../JwtLaunch/jwt-launch-component/jwt-launch-component.component';
import { SharedserviceService } from './SharesServices/sharedservice.service';




@Component({
  selector: 'emarking-sso-login',
  templateUrl: './sso-login.component.html',
  styleUrls: ['./sso-login.component.css']
})
export class SsoLoginComponent {
  public enc!: string

  constructor(
    public authservice: AuthService,
    public Alert: AlertService,
    public router: Router,
    public sharedService: SharedserviceService,
    public jetlaunch: JwtLaunchComponentComponent,
   
  ) { }

  public IsDisable:boolean=false

  ArchiveDashboard() {
    this.IsDisable=true
    this.authservice.GenerateSSOJWTToken().subscribe(result => {
      let enc = result;
      if (enc != null && enc != "" && enc != undefined) {
        window.location.href = enc;
      }
      else
      {
        this.IsDisable=false
        this.Alert.warning("User Does Not Exist...!")
      }
    })
  }

}
