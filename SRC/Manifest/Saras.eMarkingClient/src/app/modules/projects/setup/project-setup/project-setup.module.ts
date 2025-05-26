import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProjectSetupRoutingModule } from './project-setup-routing.module';
import { BasicDetailsComponent } from './basic-details/basic-details.component';
import { ProjectLevelConfigurationComponent } from './project-level-configuration/project-level-configuration.component';
import { ProjectScheduleComponent } from './project-schedule/project-schedule.component';
import { ResolutionOfCoiComponent } from './resolution-of-coi/resolution-of-coi.component';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { PerfectScrollbarConfigInterface, PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { MatMenuModule } from '@angular/material/menu';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ProjectClosureComponent } from './project-closure/project-closure.component';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/project/', '.json');
}

@NgModule({
  declarations: [
    BasicDetailsComponent,
    ProjectLevelConfigurationComponent,
    ProjectScheduleComponent,
    ResolutionOfCoiComponent,
    ProjectClosureComponent
  ],
  imports: [
    CommonModule,
    ProjectSetupRoutingModule,
    SharedModule,
    PerfectScrollbarModule,
    MatMenuModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient]
      },
      isolate: true
    })
  ], providers: [
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }
  ]
})
export class ProjectSetupModule {
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
