import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { QigConfigurationsRoutingModule } from './qig-configurations-routing.module';
import { QigConfigurationsComponent } from './qig-configurations.component';
import { AnnotationSettingsComponent } from './annotation-settings/annotation-settings.component';
import { RandomCheckComponent } from './random-check/random-check.component'; 
import { StatndardisationSettingsComponent } from './statndardisation-settings/statndardisation-settings.component';
import { PerfectScrollbarConfigInterface, PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { HttpClient } from '@angular/common/http';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { MatMenuModule } from '@angular/material/menu';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { MarkingTypeConfigComponent } from './marking-type-config/marking-type-config.component';
import { QigQuestionsComponent } from './qig-questions/qig-questions.component';
import { OtherQigSettingsComponent } from './other-qig-settings/other-qig-settings.component';
import { LiveMarkingSettingsComponent } from './live-marking-settings/live-marking-settings.component';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import { QigSummaryComponent } from './qig-summary/qig-summary.component';
import { MatTreeModule } from '@angular/material/tree';
import { MatCardModule } from '@angular/material/card';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/project/', '.json');
}


@NgModule({
  declarations: [
    QigConfigurationsComponent,
    AnnotationSettingsComponent, 
    RandomCheckComponent,
    LiveMarkingSettingsComponent,
    StatndardisationSettingsComponent,
    MarkingTypeConfigComponent,
    QigQuestionsComponent,
    OtherQigSettingsComponent,
    QigSummaryComponent
  ],
  imports: [
    CommonModule,
    QigConfigurationsRoutingModule,
    SharedModule,
    PerfectScrollbarModule,
    MatMenuModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatTreeModule,
    MatCardModule,
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
export class QigConfigurationsModule { 
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }}
