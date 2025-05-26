import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserManagementroutingModule } from './user-management.routing.module'
import { SharedModule } from '../../shared/shared.module';
import { BlockedUsersComponent } from './blocked-users/blocked-users.component';
import { ApplicationUsermanagementComponent } from './application-usermanagement/application-usermanagement.component'
import { CreateUserComponent } from './create-user/create-user.component'
import { PassPhraseComponent } from './pass-phrase/pass-phrase.component'
import { ImportUserComponent } from './import-user/import-user.component'
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';


const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/Global/UserManagement/', '.json');
}


@NgModule({
  declarations: [
    BlockedUsersComponent,
    ApplicationUsermanagementComponent,
    CreateUserComponent,
    PassPhraseComponent,
    ImportUserComponent
  ],
  imports: [
    SharedModule,
    CommonModule,
    UserManagementroutingModule,
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
  ]
})
export class UserManagementModule {
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
