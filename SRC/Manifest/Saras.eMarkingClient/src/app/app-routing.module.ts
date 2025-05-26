import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './modules/auth/login/login.component';
import { ForgotPasswordComponent } from './modules/auth/forgot-password/forgot-password.component';
import { Userrole } from './modules/auth/userrole';
import { ErrorpageComponent } from './modules/shared/errorpage/errorpage.component';
import { ChangePasswordComponent } from './modules/auth/change-password/change-password.component';

const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    data: {
      title: 'eMarking',
      id: 10,
      expectedRole: Userrole.All,
    },
  },
  {
    path: 'ForgotPassword',
    component: ForgotPasswordComponent,
    data: {
      title: 'eMarking',
      id: 10,
      expectedRole: Userrole.All,
    },
  },
  {
    path: 'change-password',
    component: ChangePasswordComponent,
    data: {
      title: 'eMarking',
      id: 10,
      expectedRole: Userrole.All,
    },
  },

  {
    path: 'sso',
    loadChildren: () =>
      import('./modules/JwtLaunch/jwt-launch/jwt-launch.module').then(
        (m) => m.JwtLaunchModule
      ),
  },
  
  {
    path: 'error',
    component: ErrorpageComponent,
    data: {
      title: 'eMarking',
      id: 11,
      expectedRole: Userrole.All,
    },
  },
  {
    path: 'error/:code',
    component: ErrorpageComponent,
    data: {
      title: 'eMarking',
      id: 11,
      expectedRole: Userrole.All,
    },
  },

 
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
