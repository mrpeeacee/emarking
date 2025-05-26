import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardModuleRoutingModule } from './dashboard-module-routing.module';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { NgChartsModule } from 'ng2-charts';
import { AoCmDashboardComponent } from './ao-cm-dashboard/ao-cm-dashboard.component';
import { TlAtlDashboardComponent } from './tl-atl-dashboard/tl-atl-dashboard.component';
import { MarkerDashboardComponent } from './marker-dashboard/marker-dashboard.component';
import { StandardisationAssessmentsComponent } from './standardisation-assessments/standardisation-assessments.component';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { MarkingPlayerModule } from '../../marking-player/marking-player.module';
import { ProfileComponent } from './profile/profile.component';


export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/project/', '.json');
}

@NgModule({
  declarations: [
    AoCmDashboardComponent,
    TlAtlDashboardComponent,
    MarkerDashboardComponent,
    StandardisationAssessmentsComponent,
    ProfileComponent,
    
  ],
  imports: [
    CommonModule,
    DashboardModuleRoutingModule,
    SharedModule,
    MarkingPlayerModule,
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
  ]
})
export class DashboardModuleModule {
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
