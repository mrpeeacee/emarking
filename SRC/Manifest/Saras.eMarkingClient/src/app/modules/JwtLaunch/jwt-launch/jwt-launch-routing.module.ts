import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { JwtLaunchComponentComponent } from '../jwt-launch-component/jwt-launch-component.component';

const routes: Routes = [
  {
    path: 'jwt/launch/:enc',
    component:JwtLaunchComponentComponent
  }


  
  
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class JwtLaunchRoutingModule { }
