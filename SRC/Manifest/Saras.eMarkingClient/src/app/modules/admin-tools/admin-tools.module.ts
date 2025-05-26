import { NgModule } from "@angular/core";
import { LiveMarkingProgressReportComponent } from "./livemarking-progress-report/livemarking-progress-report.component";
import { CommonModule, DatePipe } from "@angular/common";
import { InfiniteScrollModule } from "ngx-infinite-scroll";
import { DragDropModule } from "@angular/cdk/drag-drop";
import { NgChartsModule } from "ng2-charts";
import { AdminToolsRoutingModule } from "./admin-tools-routing.module";
import { SharedModule } from "../shared/shared.module";
import { QualityCheckReportComponent } from "./qualitycheck-report/qualitycheck-report.component";
import { CandidateScriptDetailsComponent } from "./candidate-scriptdetails-report/candidate-scriptdetails-report.component";
import { FrequencyDistributionReportComponent } from "./frequencydistribution-report/frequencydistribution-report.component";
import { MatMenuModule } from "@angular/material/menu";
import { MatPaginatorModule } from "@angular/material/paginator";
import { HttpClient } from "@angular/common/http";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { TranslateLoader, TranslateModule, TranslateService } from "@ngx-translate/core";
import { AnswerKeysReportComponent } from "./answer-keys-report/answer-keys-report.component";
import { MailSentReportComponent } from "./mail-sent-report/mail-sent-report.component";
import { FIDIReportComponent } from "./FIDI-report/FIDI-report.component";
import { MarkerPerformanceReportComponent } from './marker-performance-report/marker-performance-report.component';
export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/admin-tools/', '.json');
}  

  @NgModule({
    declarations: [
        LiveMarkingProgressReportComponent,
        QualityCheckReportComponent,
        CandidateScriptDetailsComponent,
        FrequencyDistributionReportComponent,
      AnswerKeysReportComponent,
      MailSentReportComponent, FIDIReportComponent, MarkerPerformanceReportComponent
    ],
    imports: [
      CommonModule,
      AdminToolsRoutingModule,
      SharedModule,
      InfiniteScrollModule,
      DragDropModule,
      NgChartsModule,
      MatMenuModule,
      MatPaginatorModule,
      TranslateModule.forChild({
        loader: {
          provide: TranslateLoader,
          useFactory: createTranslateLoader,
          deps: [HttpClient]
        },
        isolate: true
      })
    ],
    providers: [DatePipe]
  })
  export class AdminTools {
    constructor(translate: TranslateService) {
      translate.addLangs(["en", "fr"]);
      translate.setDefaultLang('en');
      translate.use('en');
    }
}
