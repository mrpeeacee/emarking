import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Userrole } from 'src/app/modules/auth/userrole';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { UserImportComponent } from './user-import/user-import.component';
import { UserManagementComponent } from './user-management.component';
import { MappedUsersComponent } from './Project_User_Management/mapped-users/mapped-users.component';
import { UnMappedUsersComponent } from './Project_User_Management/un-mapped-users/un-mapped-users.component';
import { UserWithdrawComponent } from './user-withdraw/user-withdraw.component';


const routes: Routes = [

  {
    path: 'MappedUsers', component: MappedUsersComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.EO, Userrole.EM, Userrole.SUPERADMIN, Userrole.SERVICEADMIN],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: 'projects/dashboards/ao-cm'
        },
        {
          label: 'Project User Management',
          path: 'projects/setup/user-management/MappedUsers'
        }]
    }
  },
  {
    path: 'AvaliableUsers', component: UnMappedUsersComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.EO, Userrole.EM, Userrole.SUPERADMIN, Userrole.SERVICEADMIN],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: 'projects/dashboards/ao-cm'
        },
        {
          label: 'Project User Management',
          path: 'projects/setup/user-management/AvaliableUsers'
        }]
    }
  },
  {
    path: 'UserWithdraw',
    component: UserWithdrawComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.EO, Userrole.EM, Userrole.SUPERADMIN, Userrole.SERVICEADMIN],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: 'projects/dashboards/ao-cm'
        },
        {
          label: 'Candidate Withdraw',
          path: '#'
        }]
    }
  },
  {
    path: '', component: UserManagementComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.EO, Userrole.EM, Userrole.SUPERADMIN, Userrole.SERVICEADMIN],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: 'projects/dashboards/ao-cm'
        },
        {
          label: 'QIG Team Management',
          path: '#'
        }]
    }
  },
  {
    path: ':qigid', component: UserManagementComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.EO, Userrole.EM, Userrole.SUPERADMIN, Userrole.SERVICEADMIN],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: 'projects/dashboards/ao-cm'
        },
        {
          label: 'QIG Team Management',
          path: '#'
        }]
    }
  },
  {
    path: ':qigid/user-import', component: UserImportComponent,

    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.EO, Userrole.EM, Userrole.SUPERADMIN, Userrole.SERVICEADMIN],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: 'projects/dashboards/ao-cm'
        },
        {
          label: 'QIG Team Management',
          path: 'projects/setup/user-management/:qigid'
        },
        {
          label: 'User Import',
          path: '#'
        }]
    }
  },
  {
    path: 'UserWithdraw',
    component: UserWithdrawComponent,

  }
];



@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserManagementRoutingModule { }
