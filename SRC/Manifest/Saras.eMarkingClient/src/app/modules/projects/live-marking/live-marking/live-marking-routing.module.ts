import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LiveMarkingComponent } from './live-marking.component';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { Userrole } from 'src/app/modules/auth/userrole';
import { DownloadedscriptuserlistComponent } from '../downloadedscriptuserlist/downloadedscriptuserlist.component';

const routes: Routes = [
  { path: ':qig/:QigId', 
    component: LiveMarkingComponent,
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
          path: 'projects/dashboards/br_dashboard/:QigId'
        },
        {
          label: ':qig',
          path: 'projects/dashboards/br_dashboard/:QigId',
        },
        {
          label: 'Live Marking',
          path: '#'
        }
      ]
    }
  },

  { path: 'downloadedscriptuserlist/:qig/:QigId', 
    component: DownloadedscriptuserlistComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'Downloaded script user list',
      expectedRole: [Userrole.AO],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: 'projects/dashboards/br_dashboard/:QigId'
        },
        {
          label: ':qig',
          path: 'projects/dashboards/br_dashboard/:QigId',
        },
        {
          label: 'Live Marking',
          path: 'projects/live-marking/:qig/:QigId',
        },
        {
          label: 'Downloaded Script Details',
          path: '#'
        }
      ]
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LiveMarkingRoutingModule { }
