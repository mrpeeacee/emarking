import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { BrandingModel } from 'src/app/model/globalconst';
import { AuthService } from 'src/app/services/auth/auth.service';
import { NotificationService } from 'src/app/services/common/notification.service';

@Component({
  selector: 'emarking-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css'],

})
export class FooterComponent implements OnInit {
  constructor(public notificatonservice: NotificationService, public authService: AuthService) { }
  LoginStatus$!: Observable<boolean>;
  buildVersionNumber = "";
  isBuildNumberDisplayEnabled = false;
  serverTime = new Date();
  brand!: BrandingModel;
  ngOnInit(): void {
    this.LoginStatus$ = this.authService.isAuthenticated;
    this.setServerDateTime();
    this.notificatonservice.setBuildnumber();
    this.notificatonservice.brand$.subscribe(msg => this.brand = msg); 
  }

  setServerDateTime() {
    this.LoginStatus$.subscribe(() => {
      this.notificatonservice.getServerDatetime();      
      this.notificatonservice.CurrentDateTime$.subscribe(val => {
        this.serverTime = val;
      });
    });
  }

}
