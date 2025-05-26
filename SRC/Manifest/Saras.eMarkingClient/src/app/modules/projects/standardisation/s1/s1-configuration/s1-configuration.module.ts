import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MarkingPlayerModule } from 'src/app/modules/projects/marking-player/marking-player.module'
import { S1ConfigurationRoutingModule } from './s1-configuration-routing.module';
import { CategorisationPoolComponent } from './categorisation-pool/categorisation-pool.component';
import { FinaliseAsDefinitiveComponent } from './finalise-as-definitive/finalise-as-definitive.component';
import { S1SetupComponent } from './s1-setup/s1-setup.component';
import { S2S3configurationsComponent } from './s2-s3configurations/s2-s3configurations.component';
import { MatButtonModule } from '@angular/material/button';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { HttpClient } from '@angular/common/http';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { NgChartsModule } from 'ng2-charts';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { PerfectScrollbarConfigInterface, PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/standardization/', '.json');
}

@NgModule({
  declarations: [
    CategorisationPoolComponent,
    FinaliseAsDefinitiveComponent,
    S1SetupComponent,
    S2S3configurationsComponent
  ],
  imports: [
    CommonModule,
    S1ConfigurationRoutingModule,
    MatButtonModule,
    SharedModule,
    PerfectScrollbarModule,
    MarkingPlayerModule,
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

  providers: [
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }
  ]
})
export class S1ConfigurationModule {
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
