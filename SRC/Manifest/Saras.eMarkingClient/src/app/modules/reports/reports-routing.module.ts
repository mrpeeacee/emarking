import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { Userrole } from '../auth/userrole';
import { EmsReportComponent } from './ems-report/ems-report.component';
import { MarkersreportsComponent } from './markersreports/markersreports.component';
import { AuditReportComponent } from './audit-report/audit-report.component';
import { CourseValidationSummaryComponent } from './course-validation-summary/course-validation-summary.component';
import { StudentresultreportComponent } from './studentresultreport/studentresultreport.component';
import{UserResponseComponent} from './UserResponse/user-response.component';
import { DataReportComponent } from './data-report/data-report.component';
import { TestCenterResponsesComponent } from './test-center-responses/test-center-responses.component';
import { AllUsersResponseComponent } from './all-users-response/all-users-response.component';
const routes: Routes = [
  {
    path: 'students-result',
    component: StudentresultreportComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.All],
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
          label: 'Student Result Reports',
          path: '#',
        },
      ],
    },
  },

  {
    path: 'Testcenter-responsesReport',
    component: TestCenterResponsesComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.All],
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
          label: 'Testcenter-responsesReport',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'students-result-report',
    component: StudentresultreportComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.All],
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
          label: 'Students Results',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'Audit-Report',
    component: AuditReportComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.EO, Userrole.EM, Userrole.SUPERADMIN, Userrole.SERVICEADMIN],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        // {
        //   label: 'PHome',
        //   path: 'projects/dashboards/br_dashboard',
        // },
        {
          label: 'Audit Report',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'markers-performance',
    component: MarkersreportsComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.All],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        }
      ],
    },
  },
  {
    path: 'ems',
    component: EmsReportComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.EO,Userrole.EM,Userrole.SUPERADMIN,Userrole.SERVICEADMIN],
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
          label: 'Outbound Reports',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'course-validation-summary',
    component: CourseValidationSummaryComponent,
    
    data: {
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'Course Validation Summary',
          path: '#',
        },
      ],
    },
  },
  {
    path: ':candidateid/user-response',
    component: UserResponseComponent,
    canActivate: [RoleGuard],
    data: {
   
      expectedRole: [Userrole.AO,Userrole.CM, Userrole.ACM,Userrole.EO, Userrole.EM, Userrole.SUPERADMIN, Userrole.SERVICEADMIN],
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
          label: 'Candidate Withdraw',
          path: 'projects/setup/user-management/UserWithdraw',
        },
        {
          label: 'View Responses',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'all-user-response',
    component: AllUsersResponseComponent,
    canActivate: [RoleGuard],
    data: {

      expectedRole: [Userrole.AO, Userrole.EO, Userrole.EM, Userrole.SUPERADMIN, Userrole.SERVICEADMIN],
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
          label: 'Download Responses',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'all-user-response/withdraw',
    component: AllUsersResponseComponent,
    canActivate: [RoleGuard],
    data: {
   
      expectedRole: [Userrole.AO,Userrole.EO, Userrole.EM, Userrole.SUPERADMIN, Userrole.SERVICEADMIN],
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
          label: 'Candidate Withdraw',
          path: 'projects/setup/user-management/UserWithdraw',
        },
        {
          label: 'All User Response',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'data-report',
    component: DataReportComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.EO, Userrole.EM, Userrole.SERVICEADMIN, Userrole.SUPERADMIN],
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
          label: 'Data Report',
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
export class ReportsRoutingModule { }
