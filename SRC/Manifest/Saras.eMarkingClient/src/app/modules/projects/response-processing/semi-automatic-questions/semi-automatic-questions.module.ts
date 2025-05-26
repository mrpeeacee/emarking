import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { SemiAutomaticQuestionsRoutingModule } from './semi-automatic-questions-routing.module';
import { SemiAutomaticQuestionsComponent } from './semi-automatic-questions.component';
import { FrequencyDistributionComponent } from './frequency-distribution/frequency-distribution.component';
import { PerfectScrollbarConfigInterface, PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { HttpClient } from '@angular/common/http';
import { FIBDiscrepencyReportComponent } from './fib-discrepency-report/fib-discrepency-report.component';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/project/response-processing/semi-automatic/', '.json');
}

@NgModule({
  declarations: [
    SemiAutomaticQuestionsComponent,
    FrequencyDistributionComponent,
    FIBDiscrepencyReportComponent
  ],
  imports: [
    CommonModule,
    SemiAutomaticQuestionsRoutingModule,
    SharedModule,
    PerfectScrollbarModule,
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
export class SemiAutomaticQuestionsModule {
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
