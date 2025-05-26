import { Component, Injectable, OnInit } from '@angular/core';
import { UserLogin } from '../../auth/user';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth/auth.service';
import { first } from 'rxjs';
import { SharedserviceService } from '../../common-dashboard/Sso-Login/SharesServices/sharedservice.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { DashboardComponent } from '../../common-dashboard/common-dashboard/dashboard.component';

@Component({
  selector: 'emarking-jwt-launch-component',
  templateUrl: './jwt-launch-component.component.html',
  styleUrls: ['./jwt-launch-component.component.css']
})
@Injectable({
  providedIn:'root'
})
export class JwtLaunchComponentComponent implements OnInit {
  object!: UserLogin;
  public objectList!:string


  constructor(
    private activatedRoute: ActivatedRoute,
    public authService: AuthService,
    public router: Router,
    public sharedService:SharedserviceService,
    public alert:AlertService,
    public dashboard:DashboardComponent,
   

  ) {}

  ngOnInit(): void {
 
  this.activatedRoute.params.subscribe((params) => {
      
    let SsoParam = params['enc'];
    this.JwtSsoLogin(SsoParam);
  });

  }

  JwtSsoLogin(enc:any) {

   
    this.authService
      .JwtSsoLogin(enc)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          
          if (data != null && data != undefined) {
            this.object = data;
            if(this.object.Status == 'S001')
            {
              this.router.navigateByUrl('projects');
              this.authService.loginStatus.next(true);

            }
            else if(this.object.Status != 'S001')
            {
              this.RedirectToError();
            }
        
          } else {
            this.RedirectToError();
        
          }
        },
        error: () => {
          this.RedirectToError();
        },
      });
  }
  private RedirectToError() {
    this.router.navigateByUrl('error/403');
    this.authService.loginStatus.next(false);
  }

}
