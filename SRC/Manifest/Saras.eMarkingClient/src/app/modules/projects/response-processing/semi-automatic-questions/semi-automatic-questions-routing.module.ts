import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SemiAutomaticQuestionsComponent } from './semi-automatic-questions.component';
import { FrequencyDistributionComponent } from './frequency-distribution/frequency-distribution.component';
import { FIBDiscrepencyReportComponent } from './fib-discrepency-report/fib-discrepency-report.component';
import { Userrole } from 'src/app/modules/auth/userrole';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';

const routes: Routes = [{
  path: '', component: SemiAutomaticQuestionsComponent,
  canActivate: [RoleGuard],
  data: {
    expectedRole: [Userrole.AO, Userrole.CM],
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
        label: 'Semi Automatic Questions',
        path: '#'
      },
    
    ]
  }
},
{
  path: ':ProjectQuestionId/frequency-distribution', component: FrequencyDistributionComponent,

  canActivate: [RoleGuard],
  data: {
    expectedRole: [Userrole.AO, Userrole.CM],
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
        label: 'Semi Automatic Questions',
        path: 'projects/semi-automatic-questions'
      },
      {
        label: 'Frequency Distribution',
        path: '#'
      }]
  }
},

{
  path: 'fib-discrepency-report', component: FIBDiscrepencyReportComponent,

  canActivate: [RoleGuard],
  data: {
    expectedRole: [Userrole.AO, Userrole.CM],
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
        label: 'Semi Automatic Questions',
        path: 'projects/semi-automatic-questions'
      },
      {
        label: 'FIB Discrepency Report',
        path: '#'
      }]
  }
},



];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SemiAutomaticQuestionsRoutingModule { }
