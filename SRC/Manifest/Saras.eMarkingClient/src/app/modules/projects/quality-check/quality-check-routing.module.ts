import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { QualityCheckComponent } from './quality-check.component';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { Userrole } from '../../auth/userrole';

const routes: Routes = [{
  path: '', component: QualityCheckComponent,
  canActivate: [RoleGuard],
  data: {
    title: 'Live Marking',
    expectedRole: [Userrole.All],
    breadcrumb: [
      {
        label: 'Home',
        path: 'projects'
      },
      {
        label: 'PHome',
        path: 'projects/dashboards/br_dashboard'
      },
      {
        label: 'Quality Check',
        path: '#'
      }
    ]
  }
},
{
  path: ':qigid', component: QualityCheckComponent,
  canActivate: [RoleGuard],
  data: {
    title: 'Live Marking',
    expectedRole: [Userrole.All],
    breadcrumb: [
      {
        label: 'Home',
        path: 'projects'
      },
      {
        label: 'PHome',
        path: 'projects/dashboards/br_dashboard/:qigid'
      },
      {
        label: 'Quality Check',
        path: '#'
      }
    ]
  }
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class QualityCheckRoutingModule { }
