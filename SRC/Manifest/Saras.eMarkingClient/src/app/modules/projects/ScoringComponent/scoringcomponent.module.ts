import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ScoringComponentLibraryComponent } from './scoring-component-library/scoring-component-library.component';
import { ScoringComponentRoutingModule } from './scoringcomponent-routing.module';
import { PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface, PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { SharedModule } from '../../shared/shared.module';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { NgChartsModule } from 'ng2-charts';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { ScoringComponentCreateComponent } from './scoring-component-create/scoring-component-create.component';
import { ViewScoringComponentComponent } from './view-scoring-component/view-scoring-component.component';
import { EditScoringComponentComponent } from './edit-scoring-component/edit-scoring-component.component';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/standardization/', '.json');
}

@NgModule({
  declarations: [
    ScoringComponentLibraryComponent,
    ScoringComponentCreateComponent,
    ViewScoringComponentComponent,
    EditScoringComponentComponent,
  ],
  imports: [
    CommonModule,
    ScoringComponentRoutingModule,
    SharedModule,
    PerfectScrollbarModule,
    InfiniteScrollModule,
    DragDropModule,
    NgChartsModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient]
      },
      isolate: true
    })
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ],
  providers: [
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }
  ]
})
export class ScoringComponentModule { 
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
