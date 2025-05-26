import { Component, OnInit, ViewChild } from '@angular/core';
import { QigUserModel } from 'src/app/model/project/qig';
import { CommonService } from 'src/app/services/common/common.service';
import {
  PracticeQualifyingEnable,
  WorkflowStatusTrackingModel,
} from 'src/app/model/standardisation/Assessment';
import { TranslateService } from '@ngx-translate/core';
import { AlertService } from 'src/app/services/common/alert.service';
import {
  MarkingOverviewsModel,
  PrivilegesModel,
} from 'src/app/model/dashboard/dashboard';
import { UserprivilegeService } from 'src/app/services/project/privileges.service';
import { StdAssessmentService } from 'src/app/services/project/standardisation/std-two/std-assessment.service';
import { ChartConfiguration, ChartData, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import DatalabelsPlugin from 'chartjs-plugin-datalabels';
import { ActivatedRoute, Router } from '@angular/router';
import { QigService } from 'src/app/services/project/qig.service';
import { first } from 'rxjs';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import {
  WorkflowStatus,
  WorkflowProcessStatus,
} from 'src/app/model/common-model';
import { DashboardService } from 'src/app/services/project/dashboard/dashboard.service';

@Component({
  templateUrl: './marker-dashboard.component.html',
  styleUrls: ['./marker-dashboard.component.css'],
})
export class MarkerDashboardComponent implements OnInit {
  @ViewChild(BaseChartDirective) chart: BaseChartDirective | undefined;
  public pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'right',
        onClick: () => {
          return false;
        },
      },
    },
  };
  public pieChartData: ChartData<'pie', number[], string | string[]> = {
    labels: [['Total No. of Scripts'], ['Submitted'], ['Reallocated']],
    datasets: [
      {
        data: [0, 0, 0],
      },
    ],
  };
  public pieChartType: ChartType = 'pie';
  public pieChartPlugins = [DatalabelsPlugin];

  overviewloading: boolean = true;
  linksloading: boolean = true;
  markingOverview!: MarkingOverviewsModel;
  IsPracticeQualifyingEnable!: PracticeQualifyingEnable;
  QigId!: number;
  activeQig!: QigUserModel;
  QIgName: string = '';
  IsPracticeDisabled!: boolean;
  IsQualifyDisabled!: boolean;
  title!: string;
  IsTrue!: boolean;
  statusval: number = 0;
  quicklinks!: PrivilegesModel[];
  enablelivemarking: boolean = true;

  IsQigPause: boolean = false;
  WorkFlowStatusTracking: WorkflowStatusTrackingModel[] = [];
  IsS1Available: boolean = false;
  intMessages: any = {
    Markingprocesspaused: '',
  };
  ProjectUserRoleId: any;

  constructor(
    public commonService: CommonService,
    public _dashboardService: DashboardService,
    public usrPrivilegeService: UserprivilegeService,
    public stdAssessmentservice: StdAssessmentService,
    public Alert: AlertService,
    public translate: TranslateService,
    public router: Router,
    public qigservice: QigService,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.translate.get('Home.S3pageDesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.translate.get('General.Dashboad').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
      this.title = translated;
    });
    this.translate
      .get('Std.SetUp.Markingprocesspaused')
      .subscribe((translated: string) => {
        this.intMessages.Markingprocesspaused = translated;
      });
  }

  getQigDetails(selectedqig: QigUserModel) {
    this.IsS1Available = false;
    if (selectedqig != null && selectedqig.QigId > 0) {
      this.QigId = selectedqig.QigId;
      this.QIgName = selectedqig.QigName;
      this.IsS1Available = selectedqig.IsS1Available;
      this.GetAllOverView(this.QigId, 0);
      this.GetUserPrivileges();
      this.CheckingQigPause();
      this.StatusAssessment(this.QigId);
      this.activatedRoute.params.subscribe((params) => {
        let QigId = params['qigid'];
        this.router.navigateByUrl(
          this.router.url.replace(QigId, this.QigId.toString())
        );
      });
    }
  }

  GetAllOverView(QigId: number, ProjectUserRoleId: number) {
    this.overviewloading = true;
    this._dashboardService.GetAllOverView(QigId, ProjectUserRoleId).subscribe({
      next: (ddata: any) => {
        if (ddata != null) {
          this.markingOverview = ddata;
          this.pieChartData.datasets = [
            {
              data: [
                this.markingOverview.TotalScripts,
                this.markingOverview.Submitted,
                this.markingOverview.Reallocated,
              ],
            },
          ];
          this.chart?.update();
        }
      },
      error: (ar: any) => {
        throw ar;
      },
      complete: () => {
        this.overviewloading = false;
      },
    });
  }

  GetUserPrivileges() {
    this.linksloading = true;
    this.usrPrivilegeService.GetUserPrivileges(4).subscribe({
      next: (data: any) => {
        this.quicklinks = data;
      },
      error: (a: any) => {
        throw a;
      },
      complete: () => {
        this.linksloading = false;
      },
    });
  }
  navTolLive(qlink: any) {
    if (!this.enablelivemarking && !this.linksloading) {
      this.IsQigPause = false;
      this.qigservice
        .Getqigworkflowtracking(this.QigId, AppSettingEntityType.QIG)
        .pipe(first())
        .subscribe({
          next: (result: any) => {
            this.WorkFlowStatusTracking = result;
            let wfa = this.WorkFlowStatusTracking.filter(
              (a) =>
                a.WorkflowStatusCode == WorkflowStatus.Pause &&
                a.ProcessStatus == WorkflowProcessStatus.OnHold
            );
            if (wfa.length > 0) {
              this.IsQigPause = true;
              this.translate
                .get('General.qigpause')
                .subscribe((translated: string) => {
                  this.intMessages.Markingprocesspaused = translated;
                  this.Alert.warning(this.intMessages.Markingprocesspaused + '<br>' + 'Remarks : ' + wfa[0].Remark+'.');
                });
            } else {
              this.router.navigate([qlink.Url, this.QIgName, this.QigId]);
            }
          },
          error: (aew: any) => {
            throw aew;
          },
          complete: () => {
            this.overviewloading = false;
          },
        });
    }
  }
  setAssessmentStatus(statusval: number) {
    //  this.enablelivemarking = statusval != 5
  }

  StatusAssessment(QigId: number) {
    this.stdAssessmentservice.S3AssessmentStatus(QigId).subscribe((data) => {
      if (data != null || data != undefined) {
        this.enablelivemarking = data != 5 && this.IsS1Available;
      } else {
        this.enablelivemarking = true;
      }
    });
  }

  CheckingQigPause() {
    this.overviewloading = true;
    this.IsQigPause = false;
    this.qigservice
      .Getqigworkflowtracking(this.QigId, AppSettingEntityType.QIG)
      .pipe(first())
      .subscribe({
        next: (result: any) => {
          this.WorkFlowStatusTracking = result;
          let awfl = this.WorkFlowStatusTracking.filter(
            (a) =>
              a.WorkflowStatusCode == WorkflowStatus.Pause &&
              a.ProcessStatus == WorkflowProcessStatus.OnHold
          );
          if (awfl.length > 0) {
            this.IsQigPause = true;
            this.translate
              .get('General.qigpause')
              .subscribe((translated: string) => {
                this.intMessages.Markingprocesspaused = translated;
                this.Alert.warning(this.intMessages.Markingprocesspaused + '<br>' + 'Remarks : ' + awfl[0].Remark + '.');
              });
          }
        },
        error: (arw: any) => {
          throw arw;
        },
        complete: () => {
          this.overviewloading = false;
        },
      });
  }
}
