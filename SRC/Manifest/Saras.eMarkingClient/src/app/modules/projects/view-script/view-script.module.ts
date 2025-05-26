import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../shared/shared.module';
import { ViewScriptRoutingModule } from './view-script-routing.module';
import { ViewScriptComponent } from './view-script/view-script.component';
import { ViewScriptDetailsComponent } from './view-script-details/view-script-details.component';
import { TranslateService, TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SharedService } from './shared.service';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader'; 
import { MarkingPlayerModule } from '../marking-player/marking-player.module';

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/project/quality-check/', '.json');
}

@NgModule({
  declarations: [
    ViewScriptComponent,
    ViewScriptDetailsComponent,   
  ],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    SharedModule,
    MarkingPlayerModule,
    ViewScriptRoutingModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient],
      },
      isolate: true,
    }),
  ],
  providers:[SharedService]
})
export class ViewScriptModule {
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
 }
