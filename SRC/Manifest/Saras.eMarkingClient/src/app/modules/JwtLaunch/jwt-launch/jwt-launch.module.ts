import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { JwtLaunchRoutingModule } from './jwt-launch-routing.module';
import { JwtLaunchComponentComponent } from '../jwt-launch-component/jwt-launch-component.component';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/project/', '.json');
}

@NgModule({
  declarations: [
    JwtLaunchComponentComponent,


  ],
  imports: [
    CommonModule,
    JwtLaunchRoutingModule,
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
export class JwtLaunchModule { 
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
