import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { S2S3ApprovalsRoutingModule } from './s2-s3-approvals-routing.module';
import { S2S3ApprovalsComponent } from './s2-s3-approvals.component';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { MatButtonModule } from '@angular/material/button';
import { MarkersCompleteReportComponent } from './markers-complete-report/markers-complete-report.component';
import { AddionalStdScriptsComponent } from './addional-std-scripts/addional-std-scripts.component';
import { MarkingPlayerModule } from 'src/app/modules/projects/marking-player/marking-player.module';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/project/standardisation/S2S3Approval/', '.json');
}

@NgModule({
  declarations: [
    S2S3ApprovalsComponent,
    MarkersCompleteReportComponent,
    AddionalStdScriptsComponent
  ],
  imports: [
    CommonModule,
    S2S3ApprovalsRoutingModule,
    SharedModule,
    MatButtonModule,
    MarkingPlayerModule,
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
export class S2S3ApprovalsModule { 
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
