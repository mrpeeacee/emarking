import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { ViewScriptComponent } from './view-script/view-script.component';
import { Userrole } from '../../auth/userrole';
import { ViewScriptDetailsComponent } from './view-script-details/view-script-details.component';

const routes: Routes = [{ 
  path: '', 
  component: ViewScriptComponent,
  canActivate: [RoleGuard],
  data: {
    expectedRole: [Userrole.AO,Userrole.SUPERADMIN,Userrole.SERVICEADMIN],
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
        label: 'View Script',
        path: '#'
      }]
  }
},
{
path: 'view-script-details', 
component: ViewScriptDetailsComponent,
canActivate: [RoleGuard],
data: {
  expectedRole: [Userrole.AO,Userrole.SUPERADMIN,Userrole.SERVICEADMIN],
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
      label: 'View Script',
      path: 'projects/view-script',
    },
    {
      label: 'Script Details',
      path: '#'
    }]
}
}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ViewScriptRoutingModule { }
