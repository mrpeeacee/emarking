import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CategorisationPoolComponent } from './categorisation-pool/categorisation-pool.component';
import { S1SetupComponent } from './s1-setup/s1-setup.component';
import { S2S3configurationsComponent } from './s2-s3configurations/s2-s3configurations.component';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { Userrole } from 'src/app/modules/auth/userrole';
import { FinaliseAsDefinitiveComponent } from './finalise-as-definitive/finalise-as-definitive.component';

const generateBreadcrumb = (paths: { label: string; path: string }[]) => [
  {
    label: 'Home',
    path: 'projects',
  },
  {
    label: 'PHome',
    path: 'projects/dashboards/ao-cm',
  },
  ...paths,
];

const routes: Routes = [
  {
    path: 's1-setup',
    component: S1SetupComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.CM],
      breadcrumb: generateBreadcrumb([
        {
          label: 'Setup',
          path: '#',
        },
      ]),
    },
  },
  {
    path: 'categorisation-pool',
    component: CategorisationPoolComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.CM, Userrole.ACM],
      breadcrumb: generateBreadcrumb([
        {
          label: 'Categorisation',
          path: '#',
        },
      ]),
    },
  },
  {
    path: 'categorisation-pool/:qigid',
    component: CategorisationPoolComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.CM, Userrole.ACM],
      breadcrumb: generateBreadcrumb([
        {
          label: 'Categorisation',
          path: '#',
        },
      ]),
    },
  },
  {
    path: 'categorisation/:qigid/:scriptId',
    component: FinaliseAsDefinitiveComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.CM, Userrole.ACM],
      breadcrumb: generateBreadcrumb([
        {
          label: 'Categorisation',
          path: 'projects/configurations/categorisation-pool/:qigid',
        },
        {
          label: 'Categorisation',
          path: '#',
        },
      ]),
    },
  },
  {
    path: 's2-s3',
    component: S2S3configurationsComponent,
    canActivate: [RoleGuard],
    data: {
      expectedRole: [Userrole.AO, Userrole.CM, Userrole.ACM],
      breadcrumb: generateBreadcrumb([
        {
          label: 'Qualifying Assessment Creation',
          path: '#',
        },
      ]),
    },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class S1ConfigurationRoutingModule {}
