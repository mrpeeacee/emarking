import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReportsRoutingModule } from './reports-routing.module';
import { SharedModule } from '../shared/shared.module';
import { PerfectScrollbarModule, PerfectScrollbarConfigInterface, PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { NgChartsModule } from 'ng2-charts';
import { StudentreportsComponent } from './studentreports/studentreports.component'
import { StudentwisereportComponent } from './studentreports/studentwisereport/studentwisereport.component'
 
import { MarkersreportsComponent } from './markersreports/markersreports.component'
import { ReportsComponent } from './reports.component';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { NgxSliderModule } from '@angular-slider/ngx-slider';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { EmsReportComponent } from './ems-report/ems-report.component';
import { AuditReportComponent } from './audit-report/audit-report.component';
import { CourseValidationSummaryComponent } from './course-validation-summary/course-validation-summary.component';
import { StudentresultreportComponent } from './studentresultreport/studentresultreport.component';
import { UserResponseComponent} from './UserResponse/user-response.component';
import { ReportOutboundLogsComponent } from './report-outbound-logs/report-outbound-logs.component';
import { DataReportComponent } from './data-report/data-report.component';
import { TestCenterResponsesComponent } from './test-center-responses/test-center-responses.component';
import { AllUsersResponseComponent } from './all-users-response/all-users-response.component';
import { MatPaginatorModule, MatPaginatorIntl} from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/reports/', '.json');
}

@NgModule({
  declarations: [
    ReportsComponent,
    StudentreportsComponent,
    MarkersreportsComponent,
    StudentwisereportComponent, 
    EmsReportComponent,
    AuditReportComponent,
    CourseValidationSummaryComponent,
    StudentresultreportComponent,
    UserResponseComponent,
    ReportOutboundLogsComponent,
    DataReportComponent,
    TestCenterResponsesComponent,
    AllUsersResponseComponent
  ],
  imports: [
    CommonModule,
    ReportsRoutingModule,
    SharedModule,
    PerfectScrollbarModule,
    InfiniteScrollModule,
    NgChartsModule,
    NgxSliderModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient]
      },
      isolate: true
    })
  ],

  providers: [
    { provide: MatPaginatorIntl, useClass: AllUsersResponseComponent },
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }
  ],
  exports: [
    TranslateModule
  ]
})
export class ReportsModule {
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
