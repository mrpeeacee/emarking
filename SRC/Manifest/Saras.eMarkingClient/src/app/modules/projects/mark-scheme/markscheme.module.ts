import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MarkschemeRoutingModule } from './markscheme-routing.module';
import { MarkSchemeQuestionsComponent } from './mark-scheme-questions/mark-scheme-questions.component';
import { CreateMarkSchemeComponent } from './create-mark-scheme/create-mark-scheme.component';
import { EditMarkSchemeComponent } from './edit-mark-scheme/edit-mark-scheme.component';
import { ViewMarkSchemeComponent } from './view-mark-scheme/view-mark-scheme.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { HttpClient } from '@angular/common/http';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { NgChartsModule } from 'ng2-charts';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { PerfectScrollbarConfigInterface, PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { SharedModule } from '../../shared/shared.module';
import { MarkSchemeLibraryComponent } from './mark-scheme-library/mark-scheme-library.component';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/standardization/', '.json');
}

@NgModule({
  declarations: [
    MarkSchemeQuestionsComponent,
    CreateMarkSchemeComponent,
    EditMarkSchemeComponent,
    ViewMarkSchemeComponent,
    MarkSchemeLibraryComponent
  ],
  imports: [
    CommonModule,
    MarkschemeRoutingModule,
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
export class MarkschemeModule {
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}