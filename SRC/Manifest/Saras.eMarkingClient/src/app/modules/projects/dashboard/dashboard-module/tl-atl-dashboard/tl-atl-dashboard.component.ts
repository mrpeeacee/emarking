import { Component, OnInit } from '@angular/core';
import { QigUserModel } from 'src/app/model/project/qig';
import { CommonService } from 'src/app/services/common/common.service';
import { TranslateService } from '@ngx-translate/core';
import { PracticeQualifyingEnable, WorkflowStatusTrackingModel } from 'src/app/model/standardisation/Assessment';
import { AlertService } from 'src/app/services/common/alert.service';
import { MarkingOverviewsModel, PrivilegesModel } from 'src/app/model/dashboard/dashboard';
import { DashboardService } from 'src/app/services/project/dashboard/dashboard.service';
import { UserprivilegeService } from 'src/app/services/project/privileges.service';
import { ActivatedRoute, Router } from '@angular/router';
import { first } from 'rxjs';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import { WorkflowStatus, WorkflowProcessStatus } from 'src/app/model/common-model';
import { QigService } from 'src/app/services/project/qig.service';
import { QualityCheckService } from 'src/app/services/project/quality-check/quality-check.service';

@Component({
  templateUrl: './tl-atl-dashboard.component.html',
  styleUrls: ['./tl-atl-dashboard.component.css']
})
export class TlAtlDashboardComponent implements OnInit {

  IsPracticeQualifyingEnable!: PracticeQualifyingEnable;
  QigId!: number;
  activeQig!: QigUserModel;
  QIgName: string = "";
  IsPracticeDisabled!: boolean;
  IsQualifyDisabled!: boolean;
  title!: string;
  statusval: number = 0;
  IsKp: boolean = false;
  TodayOverviews!: MarkingOverviewsModel;
  quicklinks!: PrivilegesModel[];
  enablelivemarking: boolean = true;
  mrkoverviewloading: boolean = true;
  linksloading: boolean = true;
  IsS1Required: boolean = false;

  IsQigPause: boolean = false;
  WorkFlowStatusTracking: WorkflowStatusTrackingModel[] = [];
  intMessages: any = {
    Markingprocesspaused: '',
  };
  ProjectUserRoleId: any;

  constructor(public commonService: CommonService,
    public Alert: AlertService,
    public translate: TranslateService,
    private dashboardService: DashboardService,
    private router: Router,
    private usPrivilegeService: UserprivilegeService,
    public qigservice: QigService,
    private qaCheckService: QualityCheckService, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.translate.get('Home.S2pageDesc').subscribe((translated: string) => {
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
    this.Alert.clear();
    this.IsQigPause = false;
    this.enablelivemarking = false;
    if (selectedqig != null && selectedqig.QigId > 0) {
      this.QigId = selectedqig.QigId;
      this.IsS1Required = selectedqig.IsS1Available;
      this.QIgName = selectedqig.QigName;
      this.IsKp = selectedqig.IsKp;
      this.IsEligibleForLiveMarking(this.QigId);
      this.GetAllOverView(this.QigId);
      this.GetUserPrivileges();
      this.CheckingQigPause();
      this.activatedRoute.params.subscribe((params) => {
        let QigId = params['qigid'];
        this.router.navigateByUrl(
          this.router.url.replace(QigId, this.QigId.toString())
        );
      });
    }
  }

  GetAllOverView(QigId: number) {
    this.dashboardService.GetAllOverView(QigId, 0).
      subscribe({
        next: (data: any) => {
          this.TodayOverviews = data;
        },
        error: (a: any) => {
          throw (a);
        },
        complete: () => {
          this.mrkoverviewloading = false;
        }
      });
  }


  GetUserPrivileges() {
    this.linksloading = true
    this.usPrivilegeService.GetUserPrivileges(4).
      subscribe({
        next: (data: any) => {
          this.quicklinks = data;
        },
        error: (a: any) => {
          throw (a);
        },
        complete: () => {
          this.linksloading = false;
        }
      });
  }

  navtoQuickLink(qlink: PrivilegesModel) {
    if (qlink.PrivilegeCode == "QL_LIVEMARKING") {
      this.navTolLive(qlink);
    } else {
      this.router.navigate([qlink.Url+'/'+this.QigId]);
    }
  }

  navTolLive(qlink: any) {
    if (this.enablelivemarking) {
      this.IsQigPause = false;
      this.qigservice.Getqigworkflowtracking(this.QigId, AppSettingEntityType.QIG).pipe(first()).subscribe({
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
                this.Alert.warning(this.intMessages.Markingprocesspaused + '<br>' + 'Remarks : ' + awf[0].Remark+'.');
              });
          } else {
            this.router.navigate([qlink.Url, this.QIgName, this.QigId]);
          }
        },
        error: (ae: any) => {
          throw (ae);
        },
        complete: () => {
          this.mrkoverviewloading = false;
        }
      });
    }
  }

  setAssessmentStatus(statusval: number) {
    //this.enablelivemarking = statusval != 5;
  }

  QS1completed!: any;
  CheckingQigPause() {
    this.mrkoverviewloading = true;
    this.IsQigPause = false;
    this.qigservice.Getqigworkflowtracking(this.QigId, AppSettingEntityType.QIG).pipe(first()).subscribe({
      next: (result: any) => {
        this.WorkFlowStatusTracking = result;
        let awt = this.WorkFlowStatusTracking.filter(
          (a) =>
            a.WorkflowStatusCode == WorkflowStatus.Pause &&
            a.ProcessStatus == WorkflowProcessStatus.OnHold
        );

        this.QS1completed = this.WorkFlowStatusTracking.findIndex(
          (a) =>
            a.WorkflowStatusCode == WorkflowStatus.Standardization_1 &&
            a.ProcessStatus == WorkflowProcessStatus.Completed
        );
        
        this.Alert.clear();

        if (awt.length > 0) {
          this.IsQigPause = true;
          this.translate
            .get('General.qigpause')
            .subscribe((translated: string) => {
              this.intMessages.Markingprocesspaused = translated;
              this.Alert.warning(this.intMessages.Markingprocesspaused + '<br>' + 'Remarks : ' + awt[0].Remark+'.');
            });
        }


      },
      error: (at: any) => {
        throw (at);
      },
      complete: () => {
        this.mrkoverviewloading = false;
      }
    });
  }

  IsEligibleForLiveMarking(QigId: number) {
    this.qaCheckService.IsEligibleForLiveMarking(QigId).pipe(first()).subscribe({
      next: (result: any) => { 
        this.enablelivemarking = result; 
      },
      error: (a: any) => {
        throw (a);
      },
      complete: () => {
        this.mrkoverviewloading = false;
      }
    });
  }
}
