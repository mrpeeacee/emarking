import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoleGuard } from 'src/app/services/auth/role-guard.guard';
import { ScoringComponentLibraryComponent } from './scoring-component-library/scoring-component-library.component';
import { Userrole } from '../../auth/userrole';
import { ScoringComponentCreateComponent } from './scoring-component-create/scoring-component-create.component';
import { EditScoringComponentComponent } from './edit-scoring-component/edit-scoring-component.component';
import { ViewScoringComponentComponent } from './view-scoring-component/view-scoring-component.component';

const routes: Routes = [{ path: '', component: ScoringComponentLibraryComponent,
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
        label: 'scoring  component  Library',
        path: '#'
      }]
    }  
  },
  { path: 'create', component: ScoringComponentCreateComponent,
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
          label: 'scoring component create',
          path: '#'
        }]
      } 
     },
     { path: ':ComponentId/edit', component: EditScoringComponentComponent,
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
            label: 'Edit scoring component',
            path: '#'
          }]
        } 
       },
       { path: ':ComponentId', component: ViewScoringComponentComponent,
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
              label: 'View scoring component',
              path: '#'
            }]
          } 
         }      
]
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ScoringComponentRoutingModule { }
