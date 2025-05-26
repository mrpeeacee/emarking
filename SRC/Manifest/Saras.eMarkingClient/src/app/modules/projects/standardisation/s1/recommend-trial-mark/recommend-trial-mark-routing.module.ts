import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Userrole } from 'src/app/modules/auth/userrole';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { RecommendationPoolComponent } from './recommendation-pool/recommendation-pool.component';
import { TrialMarkingPoolComponent } from './trial-marking-pool/trial-marking-pool.component';

const routes: Routes = [
  {
    path: 'recommendation-pool',
    component: RecommendationPoolComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.KP],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'PHome',
          path: 'projects',
        },
        {
          label: 'Sampling',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'trial-marking-pool',
    component: TrialMarkingPoolComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.KP],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'PHome',
          path: 'projects',
        },
        {
          label: 'Trial Marking',
          path: '#',
        },
      ],
    },
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class RecommendTrialMarkRoutingModule {}
