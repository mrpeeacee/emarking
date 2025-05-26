import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RecommendTrialMarkRoutingModule } from './recommend-trial-mark-routing.module';
import { TrialMarkingPoolComponent } from './trial-marking-pool/trial-marking-pool.component';
import { RecommendationPoolComponent } from './recommendation-pool/recommendation-pool.component';
import { BandingComponent } from './banding/banding.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { HttpClient } from '@angular/common/http';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { NgChartsModule } from 'ng2-charts';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { PerfectScrollbarConfigInterface, PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { MarkingPlayerModule } from '../../../marking-player/marking-player.module';
const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/standardization/', '.json');
}
@NgModule({
  declarations: [
    TrialMarkingPoolComponent,
    RecommendationPoolComponent,
    BandingComponent
  ],
  imports: [
    CommonModule,
    RecommendTrialMarkRoutingModule,
    SharedModule,
    PerfectScrollbarModule,
    InfiniteScrollModule,
    DragDropModule,
    NgChartsModule,
    MarkingPlayerModule,
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
export class RecommendTrialMarkModule {
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
