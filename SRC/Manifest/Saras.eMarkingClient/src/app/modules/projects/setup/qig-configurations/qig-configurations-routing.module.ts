import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Userrole } from 'src/app/modules/auth/userrole';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { QigConfigurationsComponent } from './qig-configurations.component';

const routes: Routes = [{
  path: '', component: QigConfigurationsComponent,
  canActivate: [RoleGuard],
  data: {
    expectedRole: [Userrole.EO, Userrole.AO,Userrole.EM,Userrole.SUPERADMIN,Userrole.SERVICEADMIN],
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
        label: 'QIG Configuration',
        path: '#'
      }]
  }
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class QigConfigurationsRoutingModule { }
