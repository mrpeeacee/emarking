import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { S2S3ApprovalsComponent } from './s2-s3-approvals.component';
import { MarkersCompleteReportComponent } from './markers-complete-report/markers-complete-report.component';
import { AddionalStdScriptsComponent } from './addional-std-scripts/addional-std-scripts.component';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { Userrole } from 'src/app/modules/auth/userrole';

const routes: Routes = [
  {
    path: '',
    component: S2S3ApprovalsComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.EO, Userrole.AO, Userrole.CM, Userrole.ACM, Userrole.TL, Userrole.ATL],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'PHome',
          path: 'projects/dashboards/br_dashboard',
        },
        {
          label: 'Team Management',
          path: '#',
        },
      ],
    },
  },
  {
    path: ':qigid',
    component: S2S3ApprovalsComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.EO, Userrole.AO, Userrole.CM, Userrole.ACM, Userrole.TL, Userrole.ATL],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'PHome',
          path: 'projects/dashboards/br_dashboard/:qigid',
        },
        {
          label: 'Team Management',
          path: '#',
        },
      ],
    },
  },

  {
    path: ':qigid/:userroleid/MarkersCompleteReport',
    component: MarkersCompleteReportComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.EO, Userrole.AO, Userrole.CM, Userrole.ACM, Userrole.TL, Userrole.ATL],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'PHome',
          path: 'projects/dashboards/br_dashboard/:qigid',
        },
        {
          label: 'Team Management',
          path: 'projects/s2-s3-approvals/:qigid',
        },
        {
          label: "Marker's Complete Report",
          path: '#',
        },
      ],
    },
  },
  {
    path: ':qigid/:userroleid/AddionalStdScripts',
    component: AddionalStdScriptsComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.EO, Userrole.AO, Userrole.CM, Userrole.ACM, Userrole.TL, Userrole.ATL],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'PHome',
          path: 'projects/dashboards/br_dashboard/:qigid',
        },
        {
          label: 'Team Management',
          path: 'projects/s2-s3-approvals/:qigid',
        },
        {
          label: "Marker's Complete Report",
          path: 'projects/s2-s3-approvals/:qigid/:userroleid/MarkersCompleteReport',
        },
        {
          label: 'Additional Standardised Scripts',
          path: '#',
        },
      ],
    },
  }

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class S2S3ApprovalsRoutingModule { }
