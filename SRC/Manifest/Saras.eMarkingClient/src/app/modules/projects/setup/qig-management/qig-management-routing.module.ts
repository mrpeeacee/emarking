import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { QnsQigMappingComponent } from './qns-qig-mapping/qns-qig-mapping.component';
import { ManageQigComponent } from './manage-qig/manage-qig.component';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { Userrole } from '../../../auth/userrole';
import { CreateqigComponent } from './createqig/createqig.component';

const routes: Routes = [
  { 
    path: '', component: QnsQigMappingComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.EO,Userrole.EM,Userrole.SUPERADMIN,Userrole.SERVICEADMIN],
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
          label: 'Question-QIG Mapping',
          path: '#'
        }]
    } 
  },
  { 
    path: 'manage-qig', component: ManageQigComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.EO,Userrole.EM,Userrole.SUPERADMIN,Userrole.SERVICEADMIN],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: 'projects/dashboards/ao-cm'
        }
       ,
        {
          label: "Manage QIG's",
          path: '#'
        }
      ,]
    }
  },
  { 
    path: 'createqig', component: CreateqigComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.EO,Userrole.EM,Userrole.SUPERADMIN,Userrole.SERVICEADMIN],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: 'projects/dashboards/ao-cm'
        }
        ,       
        {
          label: "Manage QIG's",
          path: 'projects/setup/QigManagement/manage-qig'
        }
        ,
        {
          label: "Create QIG",
          path: '#'
        }
      ,]
    }
  }
  ,
  { 
    path: ':QigId/editqig', component: CreateqigComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.EO,Userrole.EM,Userrole.SUPERADMIN,Userrole.SERVICEADMIN],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: 'projects/dashboards/ao-cm'
        }
        ,       
        {
          label: "Manage QIG's",
          path: 'projects/setup/QigManagement/manage-qig'
        }
        ,
        {
          label: "Edit QIG",
          path: '#'
        }
      ,]
    }
  }

]
  
  ;

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class QigManagementRoutingModule { }
