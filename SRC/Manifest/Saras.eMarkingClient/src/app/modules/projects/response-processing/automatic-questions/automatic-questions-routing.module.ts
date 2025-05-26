import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AutomaticQuestionsComponent } from './automatic-questions.component';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { Userrole } from 'src/app/modules/auth/userrole';
import { ModerateScoreComponent } from './moderate-score/moderate-score.component';

const routes: Routes = [{
  path: '', component: AutomaticQuestionsComponent,
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
        label: 'Automatic Questions',
        path: '#'
      }]
  }
},
{
  path: ':pqid', component: AutomaticQuestionsComponent,
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
        label: 'Automatic Questions',
        path: '#'
      }]
  }
},
{
  path: ':pqid/moderate-score', component: ModerateScoreComponent,

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
        label: 'Automatic Questions',
        path: 'projects/automatic-questions/:pqid'
      },
      {
        label: 'Moderate Score',
        path: '#'
      }]
  }
},
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AutomaticQuestionsRoutingModule { }
