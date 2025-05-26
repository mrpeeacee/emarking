import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CommonDashboardRoutingModule } from './common-dashboard-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { NgChartsModule } from 'ng2-charts';

import { DashboardComponent } from './dashboard.component';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { SsoLoginComponent } from '../Sso-Login/sso-login.component';
import { SsoLoginLiveComponent } from '../sso-login-live/sso-login-live.component';



export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/project/', '.json');
}

@NgModule({
  declarations: [
    DashboardComponent,
    SsoLoginComponent,
    SsoLoginLiveComponent,
    
  ],
  imports: [
    CommonModule,
    CommonDashboardRoutingModule,
    SharedModule,
    PerfectScrollbarModule,
    NgChartsModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient]
      },
      isolate: true
    })
  ],
  exports: [
    TranslateModule
  ]
})
export class CommonDashboardModule {
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
