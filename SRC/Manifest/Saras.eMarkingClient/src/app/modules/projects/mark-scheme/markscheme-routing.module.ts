import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CreateMarkSchemeComponent } from './create-mark-scheme/create-mark-scheme.component';
import { EditMarkSchemeComponent } from './edit-mark-scheme/edit-mark-scheme.component';
import { MarkSchemeLibraryComponent } from './mark-scheme-library/mark-scheme-library.component';
import { MarkSchemeQuestionsComponent } from './mark-scheme-questions/mark-scheme-questions.component';
import { ViewMarkSchemeComponent } from './view-mark-scheme/view-mark-scheme.component';
import { Userrole } from '../../auth/userrole';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';

const routes: Routes = [{ path: '', component: MarkSchemeLibraryComponent,
canActivate: [RoleGuard],
data: {
  expectedRole: [Userrole.AO, Userrole.CM,Userrole.EM,Userrole.EO,Userrole.SUPERADMIN,Userrole.SERVICEADMIN,Userrole.KP],
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
      label: 'Mark Scheme Library',
      path: '#'
    }]
  }  
},
{ path: 'create', component: CreateMarkSchemeComponent,
canActivate: [RoleGuard],
data: {
  expectedRole: [Userrole.AO, Userrole.CM,Userrole.EM,Userrole.EO,Userrole.SUPERADMIN,Userrole.SERVICEADMIN,Userrole.KP],
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
      label: 'Mark Scheme Library',
      path: 'projects/mark-schemes'
    },
    {
      label: 'Create Mark Scheme',
      path: '#'
    }]
  } 
 },
{ path: ':schemeid/edit', component: EditMarkSchemeComponent,
canActivate: [RoleGuard],
data: {
  expectedRole: [Userrole.AO, Userrole.CM,Userrole.EM,Userrole.EO,Userrole.SUPERADMIN,Userrole.SERVICEADMIN,Userrole.KP],
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
      label: 'Mark Scheme Library',
      path: 'projects/mark-schemes'
    },
    {
      label: 'Edit Mark Scheme',
      path: '#'
    }]
  }  
},
{ path: ':schemeid', component: ViewMarkSchemeComponent,
canActivate: [RoleGuard],
data: {
  expectedRole: [Userrole.AO, Userrole.CM,Userrole.EM,Userrole.EO,Userrole.SUPERADMIN,Userrole.SERVICEADMIN,Userrole.KP],
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
      label: 'Mark Scheme Library',
      path: 'projects/mark-schemes'
    },
    {
      label: 'View Mark Scheme',
      path: '#'
    }]
  } 
},
{ path: 'questions', component: MarkSchemeQuestionsComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MarkschemeRoutingModule { }
