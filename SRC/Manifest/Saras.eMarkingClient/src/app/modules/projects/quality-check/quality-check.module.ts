import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { QualityCheckRoutingModule } from './quality-check-routing.module';
import { QualityCheckComponent } from './quality-check.component';
import { ScriptListViewComponent } from './script-list-view/script-list-view.component';
import { ScriptDetailsViewComponent } from './script-details-view/script-details-view.component';
import { SharedModule } from '../../shared/shared.module';
import { MarkingPlayerModule } from '../marking-player/marking-player.module'
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { PerfectScrollbarModule, PerfectScrollbarConfigInterface, PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/project/quality-check/', '.json');
  
}

@NgModule({
  declarations: [
    QualityCheckComponent,
    ScriptListViewComponent,
    ScriptDetailsViewComponent
  ],
  imports: [
    CommonModule,
    QualityCheckRoutingModule,
    SharedModule,
    MarkingPlayerModule,
    InfiniteScrollModule,
    PerfectScrollbarModule,
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
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }
  ]
})
export class QualityCheckModule {
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
