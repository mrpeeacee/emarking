import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Userrole } from 'src/app/modules/auth/userrole';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { BasicDetailsComponent } from './basic-details/basic-details.component';
import { ProjectClosureComponent } from './project-closure/project-closure.component';
import { ProjectLevelConfigurationComponent } from './project-level-configuration/project-level-configuration.component';
import { ProjectScheduleComponent } from './project-schedule/project-schedule.component';
import { ResolutionOfCoiComponent } from './resolution-of-coi/resolution-of-coi.component';

const routes: Routes = [{
  path: 'basic-details',
  component: BasicDetailsComponent,
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
        label: 'Basic Details',
        path: '#'
      }]
  }
},
{
  path: 'project-level-configuration', component: ProjectLevelConfigurationComponent,
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
        label: 'Project Level Configuration',
        path: '#'
      }]
  }

},
{
  path: 'project-schedule',
  component: ProjectScheduleComponent,
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
        label: 'Project Schedule',
        path: '#'
      }]
  }
},
{
  path: 'resolution-of-coi',
  component: ResolutionOfCoiComponent,
  canActivate: [RoleGuard],
  data: {
    expectedRole: [Userrole.AO, Userrole.CM, Userrole.EO,Userrole.EM,Userrole.SUPERADMIN,Userrole.SERVICEADMIN],
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
        label: 'Resolution of Conflict of Interest(COI)',
        path: '#'
      }]
  }
},
{
  path: 'project-closure',
  component: ProjectClosureComponent,
  canActivate: [RoleGuard],
  data: {
    expectedRole: [Userrole.AO, Userrole.CM, Userrole.EO,Userrole.EM,Userrole.SUPERADMIN,Userrole.SERVICEADMIN],
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
        label: 'Project Closure',
        path: '#'
      }]
  }
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProjectSetupRoutingModule { }
