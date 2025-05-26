import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Userrole } from 'src/app/modules/auth/userrole';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { AoCmDashboardComponent } from './ao-cm-dashboard/ao-cm-dashboard.component';
import { MarkerDashboardComponent } from './marker-dashboard/marker-dashboard.component';
import { TlAtlDashboardComponent } from './tl-atl-dashboard/tl-atl-dashboard.component';
import { ProfileComponent } from './profile/profile.component';


const routes: Routes = [

  {
    path: 'MyProfile', component: ProfileComponent,
  },

  {
    path: 'ao-cm',
    component: AoCmDashboardComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'AO CM Dashboard',
      id: 10,
      expectedRole: [Userrole.EO, Userrole.AO, Userrole.CM, Userrole.ACM,Userrole.EM,Userrole.SUPERADMIN,Userrole.SERVICEADMIN],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: '#'
        }]
    }
  },
  {
    
    path: 'ao-cm/:qigid',
    component: AoCmDashboardComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'AO CM Dashboard',
      id: 10,
      expectedRole: [Userrole.EO, Userrole.AO, Userrole.CM, Userrole.ACM,Userrole.EM,Userrole.SUPERADMIN,Userrole.SERVICEADMIN],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: '#'
        }]
    }
  },
  {
    path: 'tl-atl',
    component: TlAtlDashboardComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'Tl ATL Dashboard',
      id: 10,
      expectedRole: [Userrole.TL, Userrole.ATL],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: '#'
        }]
    }
  },
  {
    path: 'tl-atl/:qigid',
    component: TlAtlDashboardComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'TL ATL Dashboard',
      id: 10,
      expectedRole: [Userrole.TL, Userrole.ATL],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: '#'
        }]
    }
  },
  {
    path: 'marker',
    component: MarkerDashboardComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'Marker Dashboard',
      id: 10,
      expectedRole: [Userrole.MARKER],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: '#'
        }]
    }
  },
  {
    path: 'marker/:qigid',
    component: MarkerDashboardComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'Marker Dashboard',
      id: 10,
      expectedRole: [Userrole.MARKER],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects'
        },
        {
          label: 'PHome',
          path: '#'
        }]
    }
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardModuleRoutingModule { }
