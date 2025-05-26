import { Component, OnInit } from '@angular/core';
import { CommonService } from 'src/app/services/common/common.service';
import { DashboardService } from 'src/app/services/project/dashboard/dashboard.service';
import { QigUserModel } from 'src/app/model/project/qig';
import {
  LiveMarkingOverviewsModel,
  StandardisationOverviewModel,
  StandardisationApprovalCountsModel,
} from 'src/app/model/project/dashbaord/ao-cm-dashboard';
import {
  MarkingOverviewsModel,
  PrivilegesModel,
} from 'src/app/model/dashboard/dashboard';
import { ActivatedRoute, Router } from '@angular/router';
import { UserprivilegeService } from 'src/app/services/project/privileges.service';
import { WorkflowStatusTrackingModel } from 'src/app/model/standardisation/Assessment';
import { first } from 'rxjs';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import {
  WorkflowStatus,
  WorkflowProcessStatus,
} from 'src/app/model/common-model';
import { QigService } from 'src/app/services/project/qig.service';
import { TranslateService } from '@ngx-translate/core';
import { AlertService } from 'src/app/services/common/alert.service';
import { StdSetupService } from 'src/app/services/project/standardisation/std-one/std-setup.service';
import { AuthService } from 'src/app/services/auth/auth.service';

@Component({
  templateUrl: './ao-cm-dashboard.component.html',
  styleUrls: ['./ao-cm-dashboard.component.css'],
})
export class AoCmDashboardComponent implements OnInit {
  constructor(
    public commonService: CommonService,
    public stdsetupservice: StdSetupService,
    private dashboardService: DashboardService,
    private router: Router,
    private authService: AuthService,
    private usrPrivilegeService: UserprivilegeService,
    public qigservice: QigService,
    public translate: TranslateService,
    public Alert: AlertService,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.textinternationalization();
    this.translate.get('General.qigpause').subscribe((translated: string) => {
      this.intMessages.Markingprocesspaused = translated;
    });
  }
  activeQig!: QigUserModel;
  StandardisationOverview!: StandardisationOverviewModel;
  StandardisationApprovals!: StandardisationApprovalCountsModel;
  LiveMarkingOverviews!: LiveMarkingOverviewsModel;
  TodayOverviews!: MarkingOverviewsModel;
  quicklinks!: PrivilegesModel[];
  IsQigPause: boolean = false;
  WorkFlowStatusTracking: WorkflowStatusTrackingModel[] = [];
  intMessages: any = {
    Markingprocesspaused: '',
  };
  QigConfiguration!: any;
  Ispauseoronholdors1completed: string = '';
  IsS1Required: boolean = false;
  QS1completed!: any;
  curntrole!: any;
  currentloggedinRole !:any;
  isClosed: any;

  getQigDetails(selectedqig: QigUserModel) {
    this.IsQigPause = false;
    this.activeQig = selectedqig;
    this.IsS1Required = selectedqig.IsS1Available;
    if (this.activeQig != null && this.activeQig.QigId > 0) {
      this.getPauseDetails();
      this.getStandardisationOverview(this.activeQig.QigId);
      this.getStandardisationApprovals(this.activeQig.QigId);
      this.getLiveMarkingOverviews(this.activeQig.QigId);
      this.GetAllOverView(this.activeQig.QigId);
      this.curntrole = this.authService.getCurrentRole();
      this.currentloggedinRole = this.curntrole[0];
      this.GetUserPrivileges();
      this.activatedRoute.params.subscribe((params) => {
        let QigId = params['qigid'];
        this.router.navigateByUrl(
          this.router.url.replace(QigId, this.activeQig.QigId.toString())
        );
      });
    }
  }

  getPauseDetails() {
    this.qigservice
      .Getqigworkflowtracking(this.activeQig.QigId, AppSettingEntityType.QIG)
      .pipe(first())
      .subscribe({
        next: (result: any) => {
          this.WorkFlowStatusTracking = result;
          let WorkFlowStatus = result;
          this.isClosed = WorkFlowStatus.find(
            (x: any) => x.ProjectStatus
          )?.ProjectStatus;

          if (this.isClosed == 3) {
            this.translate
              .get('SetUp.dashboard.projectclosed')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          } else {
            let awef = this.WorkFlowStatusTracking.filter(
              (a) =>
                a.WorkflowStatusCode == WorkflowStatus.Pause &&
                a.ProcessStatus == WorkflowProcessStatus.OnHold
            );

            this.QS1completed = this.WorkFlowStatusTracking.findIndex(
              (a) =>
                a.WorkflowStatusCode == WorkflowStatus.Standardization_1 &&
                a.ProcessStatus == WorkflowProcessStatus.Completed
            );

            if (awef.length > 0) {
              this.IsQigPause = true;
              this.translate
                .get('General.qigpause')
                .subscribe((translated: string) => {
                  this.intMessages.Markingprocesspaused = translated;
                  this.Alert.warning(
                    this.intMessages.Markingprocesspaused +
                      '<br>' +
                      'Remark : ' +
                      awef[0].Remark +
                      '.'
                  );
                });
            }
          }
        },
        error: (aer: any) => {
          throw aer;
        },
      });
  }

  textinternationalization() {
    this.commonService.setHeaderName('Dashboard');
    this.translate
      .get(
        'SetUp.dashboard.dashboarddiscription'
      )
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
  }

  mrkoverviewloading: boolean = true;
  GetAllOverView(QigId: number) {
    this.dashboardService.GetAllOverView(QigId, 0).subscribe({
      next: (data: any) => {
        this.TodayOverviews = data;
      },
      error: (a: any) => {
        throw a;
      },
      complete: () => {
        this.mrkoverviewloading = false;
      },
    });
  }

  OverviewLoading: boolean = true;
  private getStandardisationOverview(QigId: number) {
    this.OverviewLoading = true;
    this.dashboardService.getStandardisationOverview(QigId).subscribe({
      next: (data: any) => {
        if (data != null) {
          this.StandardisationOverview = data;
        }
      },
      error: (a: any) => {
        throw a;
      },
      complete: () => {
        this.OverviewLoading = false;
      },
    });
  }

  ApprovalsLoading: boolean = true;
  private getStandardisationApprovals(QigId: number) {
    this.ApprovalsLoading = true;
    this.dashboardService.getStandardisationApprovals(QigId).subscribe({
      next: (data: any) => {
        if (data != null) {
          this.StandardisationApprovals = data;
        }
      },
      error: (a: any) => {
        throw a;
      },
      complete: () => {
        this.ApprovalsLoading = false;
      },
    });
  }

  LiveMarkingLoading: boolean = true;
  private getLiveMarkingOverviews(QigId: number) {
    this.LiveMarkingLoading = true;
    this.dashboardService.getLiveMarkingOverviews(QigId).subscribe({
      next: (data: any) => {
        if (data != null) {
          this.LiveMarkingOverviews = data;
        }
      },
      error: (a: any) => {
        throw a;
      },
      complete: () => {
        this.LiveMarkingLoading = false;
      },
    });
  }

  linksloading: boolean = true;
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

  navtoQuickLink(qlink: PrivilegesModel) {
    if (qlink.PrivilegeCode == 'QL_LIVEMARKING') {
      this.router.navigate([
        qlink.Url,
        this.activeQig.QigName,
        this.activeQig.QigId,
      ]);
    } else {
      this.router.navigate([qlink.Url+'/'+this.activeQig.QigId]);
    }
  }

  private navTolLive(qlink: any) {
    if (!this.linksloading) {
      this.IsQigPause = false;
      this.qigservice
        .Getqigworkflowtracking(this.activeQig.QigId, AppSettingEntityType.QIG)
        .pipe(first())
        .subscribe({
          next: (result: any) => {
            this.WorkFlowStatusTracking = result;
            let awf = this.WorkFlowStatusTracking.filter(
              (a) =>
                a.WorkflowStatusCode == WorkflowStatus.Pause &&
                a.ProcessStatus == WorkflowProcessStatus.OnHold
            );
            if (awf.length > 0) {
              this.IsQigPause = true;
              this.translate
                .get('General.qigpause')
                .subscribe((translated: string) => {
                  this.intMessages.Markingprocesspaused = translated;
                  this.Alert.warning(
                    this.intMessages.Markingprocesspaused +
                      '<br>' +
                      'Remark : ' +
                      awf[0].Remark +
                      '.'
                  );
                });
            } else {
              this.router.navigate([
                qlink.Url,
                this.activeQig.QigName,
                this.activeQig.QigId,
              ]);
            }
          },
          error: (ar: any) => {
            throw ar;
          },
          complete: () => {
            this.mrkoverviewloading = false;
          },
        });
    }
  }
}
