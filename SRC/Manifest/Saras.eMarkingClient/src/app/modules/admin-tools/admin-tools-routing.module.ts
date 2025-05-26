import { RouterModule, Routes } from '@angular/router';
import { LiveMarkingProgressReportComponent } from './livemarking-progress-report/livemarking-progress-report.component';
import { Userrole } from '../auth/userrole';
import { NgModule } from '@angular/core';
import { QualityCheckReportComponent } from './qualitycheck-report/qualitycheck-report.component';
import { CandidateScriptDetailsComponent } from './candidate-scriptdetails-report/candidate-scriptdetails-report.component';
import { FrequencyDistributionReportComponent } from './frequencydistribution-report/frequencydistribution-report.component';
import { AnswerKeysReportComponent } from './answer-keys-report/answer-keys-report.component';
import { MailSentReportComponent } from './mail-sent-report/mail-sent-report.component';
import { FIDIReportComponent } from './FIDI-report/FIDI-report.component';
import { MarkerPerformanceReportComponent } from './marker-performance-report/marker-performance-report.component';
const routes: Routes = [
  {
    path: 'livemarkingprogress',
    component: LiveMarkingProgressReportComponent,
    data: {
      expectedRole: [Userrole.EO, Userrole.AO, Userrole.SERVICEADMIN, Userrole.SUPERADMIN, Userrole.KP],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'Admin-Tools',
          path: 'projects',
        },
        {
          label: 'Live Marking Progress',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'qualitycheck',
    component: QualityCheckReportComponent,
    data: {
      expectedRole: [Userrole.EO, Userrole.AO, Userrole.SERVICEADMIN, Userrole.SUPERADMIN, Userrole.KP],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'Admin-Tools',
          path: 'projects',
        },
        {
          label: 'Quality Check Report',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'candidatescriptdetails',
    component: CandidateScriptDetailsComponent,
    data: {
      expectedRole: [Userrole.EO, Userrole.AO, Userrole.SERVICEADMIN, Userrole.SUPERADMIN, Userrole.KP],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'Admin-Tools',
          path: 'projects',
        },
        {
          label: 'Candidates Script Details',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'frequencydistribution',
    component: FrequencyDistributionReportComponent,
    data: {
      expectedRole: [Userrole.EO, Userrole.AO, Userrole.SERVICEADMIN, Userrole.SUPERADMIN, Userrole.KP],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'Admin-Tools',
          path: 'projects',
        },
        {
          label: 'Frequency Distribution',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'fidireport',
    component: FIDIReportComponent,
    data: {
      expectedRole: [Userrole.EO, Userrole.EM, Userrole.SERVICEADMIN, Userrole.SUPERADMIN],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'Admin-Tools',
          path: 'projects',
        },
        {
          label: 'FIDI Report',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'allanswerkeys',
    component: AnswerKeysReportComponent,
    data: {
      expectedRole: [Userrole.EO, Userrole.AO, Userrole.SERVICEADMIN, Userrole.SUPERADMIN, Userrole.KP],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'Admin-Tools',
          path: 'projects',
        },
        {
          label: 'All Answer Keys Report',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'mailsentreport',
    component: MailSentReportComponent,
    data: {
      expectedRole: [Userrole.All],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'Admin-Tools',
          path: 'projects',
        },
        {
          label: 'Mail Sent Report',
          path: '#',
        },
      ],
    },
  },
  {
    path: 'marker-performance-report',
    component: MarkerPerformanceReportComponent,
    data: {
      expectedRole: [Userrole.EO, Userrole.EM, Userrole.SERVICEADMIN, Userrole.SUPERADMIN],
      breadcrumb: [
        {
          label: 'Home',
          path: 'projects',
        },
        {
          label: 'Admin-Tools',
          path: 'projects',
        },
        {
          label: 'Marker Performance Report',
          path: '#',
        },
      ],
    },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminToolsRoutingModule {}
