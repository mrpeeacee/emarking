import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BlockedUsersComponent } from './blocked-users/blocked-users.component';
import { ApplicationUsermanagementComponent } from './application-usermanagement/application-usermanagement.component'
import { CreateUserComponent } from './create-user/create-user.component'
import { PassPhraseComponent } from './pass-phrase/pass-phrase.component'
import { ImportUserComponent } from './import-user/import-user.component'
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { Userrole } from 'src/app/modules/auth/userrole';
const routes: Routes = [
  {
    path: 'ApplicationUsermanagement',
    component: ApplicationUsermanagementComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.SUPERADMIN, Userrole.SERVICEADMIN, Userrole.EM, Userrole.EO],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'Application User Management',
          path: '#'
        }
      ]
    }
  },

  {
    path: 'CreateUser',
    component: CreateUserComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.SUPERADMIN, Userrole.SERVICEADMIN,Userrole.EM, Userrole.EO],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'Application User Management',
          path: 'userManagement/ApplicationUsermanagement'
        },
        {
          label: 'Create User',
          path: '#'
        }
      ]
    }
  },
  {
    path: 'CreateUser/:userId',
    component: CreateUserComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.SUPERADMIN, Userrole.SERVICEADMIN,Userrole.EM, Userrole.EO],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'Application User Management',
          path: 'userManagement/ApplicationUsermanagement'
        },
        {
          label: 'Edit User',
          path: '#'
        }
      ]
    }
  },
  {
    path: 'ImportUser',
    component: ImportUserComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.SUPERADMIN, Userrole.SERVICEADMIN,Userrole.EM, Userrole.EO],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'Application User Management ',
          path: 'userManagement/ApplicationUsermanagement'
        },
        {
          label: 'Import User',
          path: '#'
        }
      ]
    }
  },
  {
    path: 'blocked-users',
    component: BlockedUsersComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.SUPERADMIN, Userrole.SERVICEADMIN,Userrole.EM, Userrole.EO],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'Application User Management',
          path: 'userManagement/ApplicationUsermanagement'
        },
        {
          label: 'Blocked Users',
          path: 'userManagement/blocked-users'
        },
        {
          label: 'Blocked Users',
          path: '#'
        }
      ]
    }
  },
  {
    path: 'PassPhrase',
    component: PassPhraseComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.SUPERADMIN, Userrole.SERVICEADMIN,Userrole.EM, Userrole.EO],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'Passphrase',
          path: '#'
        }
      ]
    }
  },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserManagementroutingModule { }
