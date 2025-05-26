import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QigManagementRoutingModule } from './qig-management-routing.module';
import { QigManagementComponent } from './qig-management.component';
import { QnsQigMappingComponent } from './qns-qig-mapping/qns-qig-mapping.component';
import { ManageQigComponent } from './manage-qig/manage-qig.component';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { TagqigComponent } from './tagqig/tagqig.component';
import { CreateqigComponent } from './createqig/createqig.component';
import { PerfectScrollbarConfigInterface, PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { MatMenuModule } from '@angular/material/menu';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/project/setup/qig-configurations/', '.json');
}


@NgModule({
  declarations: [
    QigManagementComponent,
    QnsQigMappingComponent,
    ManageQigComponent,
    TagqigComponent,
    CreateqigComponent
  ],
  imports: [
    CommonModule,
    QigManagementRoutingModule,
    SharedModule,
    PerfectScrollbarModule,
    MatMenuModule,
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
export class QigManagementModule {
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
