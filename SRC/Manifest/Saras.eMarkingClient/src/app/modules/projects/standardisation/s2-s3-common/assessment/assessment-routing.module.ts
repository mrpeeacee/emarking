import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PracticeAssessmentComponent } from './practice-assessment/practice-assessment.component';
import { QualifyingAssessmentComponent } from './qualifying-assessment/qualifying-assessment.component';

const generateBreadcrumb = (qig: string, qigid: string, process: string, page: string) => [
  {
    label: 'Home',
    path: 'projects',
  },
  {
    label: 'PHome',
    path: `projects/dashboards/${process}/${qigid}`,
  },
  {
    label: qig,
    path: `projects/dashboards/${process}/${qigid}`,
  },
  {
    label: process === 'marker' && page === 'practice' ? 'Practice of Benchmark Scripts' : 'Standardisation',
    path: '#',
  },
];

const routes: Routes = [
  {
    path: ':qig/:qigid/marker/practice/:process',
    component: PracticeAssessmentComponent,
    data: {
      breadcrumb: generateBreadcrumb(':qig', ':qigid', 'marker', 'practice'),
    },
  },
  {
    path: ':qig/:qigid/tl-atl/practice/:process',
    component: PracticeAssessmentComponent,
    data: {
      breadcrumb: generateBreadcrumb(':qig', ':qigid', 'tl-atl', 'practice'),
    },
  },
  {
    path: ':qig/:qigid/marker/qualify/:process',
    component: QualifyingAssessmentComponent,
    data: {
      breadcrumb: generateBreadcrumb(':qig', ':qigid', 'marker', 'qualify'),
    },
  },
  {
    path: ':qig/:qigid/tl-atl/qualify/:process',
    component: QualifyingAssessmentComponent,
    data: {
      breadcrumb: generateBreadcrumb(':qig', ':qigid', 'tl-atl', 'qualify'),
    },
  },
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AssessmentRoutingModule {}
