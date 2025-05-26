import { NgModule, NO_ERRORS_SCHEMA, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { UserManagementRoutingModule } from './user-management-routing.module';
import { UserManagementComponent } from './user-management.component';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { UserCountsComponent } from './user-counts/user-counts.component';
import { UserDataViewComponent } from './user-data-view/user-data-view.component';
import { UserHierarchicalViewComponent } from './user-hierarchical-view/user-hierarchical-view.component';
import { MatTreeModule } from '@angular/material/tree';
import { UserViewlistComponent } from './user-viewlist/user-viewlist.component';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { PerfectScrollbarConfigInterface, PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { UserImportComponent } from './user-import/user-import.component';
import { UserCreationComponent} from './user-creation/user-creation.component';
import { CopymarkingteamComponent } from './copymarkingteam/copymarkingteam.component';
import { MoveMarkingTeamComponent } from './move-marking-team/move-marking-team.component';
import { MappedUsersComponent } from './Project_User_Management/mapped-users/mapped-users.component';
import { UnMappedUsersComponent } from './Project_User_Management/un-mapped-users/un-mapped-users.component';
import{UserWithdrawComponent} from './user-withdraw/user-withdraw.component';
import { MatPaginatorModule } from '@angular/material/paginator';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/project/setup/user-management/', '.json');
}

@NgModule({
  schemas: [NO_ERRORS_SCHEMA,CUSTOM_ELEMENTS_SCHEMA],
  declarations: [
    UserManagementComponent,
    UserCountsComponent,
    UserDataViewComponent,
    UserHierarchicalViewComponent,
    UserViewlistComponent,
    UserImportComponent,
    UserHierarchicalViewComponent,
    UserCreationComponent,
    CopymarkingteamComponent,
    MoveMarkingTeamComponent,
    MappedUsersComponent,
    UnMappedUsersComponent,
    UserWithdrawComponent  

  ],
  imports: [
    CommonModule,
    SharedModule,
    UserManagementRoutingModule,
    MatTreeModule , 
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
  ], providers: [
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }
  ]
})
export class UserManagementModule { 
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
