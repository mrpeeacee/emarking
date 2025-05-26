import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthenticatedLayoutComponent } from './authenticated-layout/authenticated-layout.component';
import { AuthGuard } from 'src/app/services/auth/auth.guard';
import { ErrorpageComponent } from '../shared/errorpage/errorpage.component';

const authenticatedRoutes: Routes = [
  {
    path: '',
    component: AuthenticatedLayoutComponent,
    children: [
      {
        path: 'projects/dashboards',
        loadChildren: () =>
          import(
            '../projects/dashboard/dashboard-module/dashboard-module.module'
          ).then((m) => m.DashboardModuleModule),
        canActivate: [AuthGuard],
      },
      {
        path: 'projects/setup',
        loadChildren: () =>
          import('../projects/setup/project-setup/project-setup.module').then(
            (m) => m.ProjectSetupModule
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'projects/setup/user-management',
        loadChildren: () =>
          import(
            '../projects/setup/user-management/user-management.module'
          ).then((m) => m.UserManagementModule),
        canActivate: [AuthGuard],
      },
      {
        path: 'projects/setup/qig-configurations',
        loadChildren: () =>
          import(
            '../projects/setup/qig-configurations/qig-configurations.module'
          ).then((m) => m.QigConfigurationsModule),
        canActivate: [AuthGuard],
      },
      {
        path: 'projects/mark-schemes',
        loadChildren: () =>
          import('../projects/mark-scheme/markscheme.module').then(
            (m) => m.MarkschemeModule
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'projects/ScoringComponent',
        loadChildren: () =>
          import('../projects/ScoringComponent/scoringcomponent.module').then(
            (m) => m.ScoringComponentModule
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'projects/assessments',
        loadChildren: () =>
          import(
            '../projects/standardisation/s2-s3-common/assessment/assessment.module'
          ).then((m) => m.AssessmentModule),
        canActivate: [AuthGuard],
      },
      {
        path: 'projects/s2-s3-approvals',
        loadChildren: () =>
          import(
            '../projects/standardisation/s2-s3-common/s2-s3-approvals/s2-s3-approvals.module'
          ).then((m) => m.S2S3ApprovalsModule),
        canActivate: [AuthGuard],
      },
      {
        path: 'projects/configurations',
        loadChildren: () =>
          import(
            '../projects/standardisation/s1/s1-configuration/s1-configuration.module'
          ).then((m) => m.S1ConfigurationModule),
        canActivate: [AuthGuard],
      },
      {
        path: 'projects/marking',
        loadChildren: () =>
          import(
            '../projects/standardisation/s1/recommend-trial-mark/recommend-trial-mark.module'
          ).then((m) => m.RecommendTrialMarkModule),
        canActivate: [AuthGuard],
      },
      {
        path: 'projects/live-marking',
        loadChildren: () =>
          import(
            '../projects/live-marking/live-marking/live-marking.module'
          ).then((m) => m.LiveMarkingModule),
        canActivate: [AuthGuard],
      },
      {
        path: 'projects',
        loadChildren: () =>
          import(
            '../common-dashboard/common-dashboard/common-dashboard.module'
          ).then((m) => m.CommonDashboardModule),
        canActivate: [AuthGuard],
      },
      {
        path: 'reports',
        loadChildren: () =>
          import('../reports/reports.module').then((m) => m.ReportsModule),
        canActivate: [AuthGuard],
      },
      {
        path: 'userManagement',
        loadChildren: () =>
          import('../Global/user-management/user-management.module').then(
            (m) => m.UserManagementModule
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'projects/quality-check',
        loadChildren: () =>
          import('../projects/quality-check/quality-check.module').then(
            (m) => m.QualityCheckModule
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'projects/automatic-questions',
        loadChildren: () =>
          import(
            '../projects/response-processing/automatic-questions/automatic-questions.module'
          ).then((m) => m.AutomaticQuestionsModule),
      },
      {
        path: 'projects/semi-automatic-questions',
        loadChildren: () =>
          import(
            '../projects/response-processing/semi-automatic-questions/semi-automatic-questions.module'
          ).then((m) => m.SemiAutomaticQuestionsModule),
      },
      {
        path: 'projects/setup/QigManagement',
        loadChildren: () =>
          import('../projects/setup/qig-management/qig-management.module').then(
            (m) => m.QigManagementModule
          ),
      },
      {
        path: 'admin-tools',
        loadChildren: () =>
          import('../admin-tools/admin-tools.module').then((m) => m.AdminTools),
        canActivate: [AuthGuard],
      },
      {
        path: 'projects/view-script',
        loadChildren: () =>import('../projects/view-script/view-script.module').then((m) => m.ViewScriptModule),
        canActivate: [AuthGuard],
      },

     
      {
        path: '**',
        component: ErrorpageComponent,
        data: {
          code: '404',
        },
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(authenticatedRoutes)],
  exports: [RouterModule],
})
export class AuthenticatedRoutingRoutingModule {}
