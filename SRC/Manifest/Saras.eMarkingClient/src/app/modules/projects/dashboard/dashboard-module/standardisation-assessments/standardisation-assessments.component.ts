import { Component, Input, OnChanges, EventEmitter, Output } from '@angular/core';
import { Router } from '@angular/router';
import {
  PracticeQualifyingEnable,
  WorkflowStatusTrackingModel,
} from 'src/app/model/standardisation/Assessment';
import { CommonService } from 'src/app/services/common/common.service';
import { TranslateService } from '@ngx-translate/core';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import {
  WorkflowStatus,
  WorkflowProcessStatus,
} from 'src/app/model/common-model';
import { AlertService } from 'src/app/services/common/alert.service';
import { first } from 'rxjs/operators';
import { StdAssessmentService } from 'src/app/services/project/standardisation/std-two/std-assessment.service';
import { QigService } from 'src/app/services/project/qig.service';
import { MarkingOverviewsModel, PrivilegesModel } from 'src/app/model/dashboard/dashboard';
import { UserprivilegeService } from 'src/app/services/project/privileges.service';
import { DashboardService } from 'src/app/services/project/dashboard/dashboard.service';

@Component({
  selector: 'emarking-standardisation-assessments',
  templateUrl: './standardisation-assessments.component.html',
  styleUrls: ['./standardisation-assessments.component.css']
})
export class StandardisationAssessmentsComponent implements OnChanges {
  constructor(
    public stdAssessmentservice: StdAssessmentService,
    public _dashboardService: DashboardService,
    public qigservice: QigService,
    private routers: Router,
    private commonservice: CommonService,
    public translate: TranslateService,
    public Alert: AlertService,
    public usrPrivilegeService: UserprivilegeService,
  ) {}


  @Input() QigId!: number;

  @Input() QIgName!: string;

  @Input() QigIsKp!: boolean;

  @Input() IsS1Available!: boolean;

  @Output() assessmentStatusEmit = new EventEmitter<number>();


  //S2 = 2 || s3 = 3
  @Input() process!: number;
  IsPracticeQualifyingEnable!: PracticeQualifyingEnable;
  IsPracticeDisplay: boolean = true;
  IsQualifyDisplay: boolean = true;
  IsKp: boolean = false;
  stdloading: boolean = false;
  status: number = 0;
  IsQigPause: boolean = false; 
  enablelivemarking: boolean = true;
  S1Avilable: boolean = false;
  quicklinks!: PrivilegesModel[];
  markingOverview!: MarkingOverviewsModel;
  WorkFlowStatusTracking: WorkflowStatusTrackingModel[] = [];
  isLiveMarkingStarted: boolean = true;
  QigPause: string = '';
  alertmsg: any = {
    S1enabled: '',
    S2enabled: '',
    S3enabled: '',
    S1notcompleted: '',
  };

  intMessages: any = {
    Markingprocesspaused: '',
  };

  ngOnChanges(): void {
    if (this.QigId > 0) {
      this.IsKp = this.QigIsKp;
      this.S1Avilable = this.IsS1Available;


      if (!this.QigIsKp) {
        if ((this.process == 2 || this.process == 3)) {
          this.IsKp = this.commonservice.IsKp;
            this.IsEnablePracticeQualifyingButton(this.QigId);
            this.AssessmentStatus();
        }
      }
      this.translate
        .get('General.s1notavailable')
        .subscribe((translated: string) => {
          this.alertmsg.S1enabled = translated;
        });
      this.translate
        .get('General.S2notavailable')
        .subscribe((translated: string) => {
          this.alertmsg.S2enabled = translated;
        });
      this.translate
        .get('General.S3notavailable')
        .subscribe((translated: string) => {
          this.alertmsg.S3enabled = translated;
        });
      this.translate
        .get('General.s1notcmplted')
        .subscribe((translated: string) => {
          this.alertmsg.S1notcompleted = translated;
        });
      this.translate
        .get('Std.SetUp.Markingprocesspaused')
        .subscribe((translated: string) => {
          this.intMessages.Markingprocesspaused = translated;
        });
      this.CheckingQigPause();
    }
  }

  IsEnablePracticeQualifyingButton(QigID: number) {
    this.stdloading = true;
    if (this.process == 2) {
      this.stdAssessmentservice.IsS2PracticeQualifyingEnable(QigID).pipe(first()).subscribe({
        next: (result: any) => {
          this.IsPracticeQualifyingEnable = result;
          if (result != null) {
            let levelvalid = true;
            if (this.process == 2 || this.process == 3) {
              levelvalid =
                this.process == 2
                  ? this.IsPracticeQualifyingEnable.IsS2Enable
                  : this.IsPracticeQualifyingEnable.IsS3Enable;
            }
            this.IsPracticeDisplay =
              levelvalid &&
              !this.IsKp &&
              this.IsPracticeQualifyingEnable.S1Completed == 3;
            this.IsQualifyDisplay =
              levelvalid &&
              this.IsPracticeQualifyingEnable.IsQualifyingEnable &&
              !this.IsKp &&
              this.IsPracticeQualifyingEnable.S1Completed == 3;
          }
        },
        error: (a: any) => {
          throw (a);
        },
        complete: () => {
          this.stdloading = false;
        }
      });
    } else if (this.process == 3) {
      this.stdAssessmentservice.IsS3PracticeQualifyingEnable(QigID).pipe(first()).subscribe({
        next: (result: any) => {
          this.IsPracticeQualifyingEnable = result;
          if (result != null) {
            let levelvalid = true;
            if (this.process == 2 || this.process == 3) {
              levelvalid =
                this.process == 2
                  ? this.IsPracticeQualifyingEnable.IsS2Enable
                  : this.IsPracticeQualifyingEnable.IsS3Enable;
            }
            this.IsPracticeDisplay =
              levelvalid &&
              !this.IsKp &&
              this.IsPracticeQualifyingEnable.S1Completed == 3;
            this.IsQualifyDisplay =
              levelvalid &&
              this.IsPracticeQualifyingEnable.IsQualifyingEnable &&
              !this.IsKp &&
              this.IsPracticeQualifyingEnable.S1Completed == 3;
          }
        },
        error: (a: any) => {
          throw (a);
        },
        complete: () => {
          this.stdloading = false;
        }
      });
    }
  }

  NavigateToPracticePage() {
    this.qigservice.Getqigworkflowtracking(this.QigId, AppSettingEntityType.QIG).pipe(first()).subscribe({
      next: (result: any) => {
        this.WorkFlowStatusTracking = result;

        let b = this.WorkFlowStatusTracking.filter(
          (a) =>
            a.WorkflowStatusCode == WorkflowStatus.Pause &&
            a.ProcessStatus == WorkflowProcessStatus.OnHold
        );
        this.Alert.clear();
        if (b.length > 0) {
          this.translate
            .get('General.qigpause')
            .subscribe((translated: string) => {
              this.intMessages.Markingprocesspaused = translated;
              this.Alert.warning(this.intMessages.Markingprocesspaused + '<br>' + 'Remarks : ' + b[0].Remark + '.');
            });
        } else {
          if (this.IsPracticeDisplay && !this.QigIsKp) {
            this.routers.navigate([
              `/projects/assessments/`,
              this.QIgName,
              this.QigId,
              this.process == 2 ? 'tl-atl':'marker',
              'practice',
              this.process
            ]);
          }
        }
      },
      error: (a: any) => {
        throw (a);
      },
      complete: () => {
        this.stdloading = false;
      }
    });
  }

  NavigateToQualifyingPage() {
    this.qigservice.Getqigworkflowtracking(this.QigId, AppSettingEntityType.QIG).pipe(first()).subscribe({
      next: (result: any) => {
        this.WorkFlowStatusTracking = result;

        let b = this.WorkFlowStatusTracking.filter(
          (a) =>
            a.WorkflowStatusCode == WorkflowStatus.Pause &&
            a.ProcessStatus == WorkflowProcessStatus.OnHold
        );
        this.Alert.clear();
        if (b.length > 0) {
          this.translate
            .get('General.qigpause')
            .subscribe((translated: string) => {
              this.intMessages.Markingprocesspaused = translated;
              this.Alert.warning(this.intMessages.Markingprocesspaused + '<br>' + 'Remarks : ' + b[0].Remark + '.');
            });
        } else {
          if (this.IsQualifyDisplay && !this.QigIsKp) {
            this.routers.navigate([
              `/projects/assessments/`,
              this.QIgName,
              this.QigId,
              this.process == 2 ? 'tl-atl':'marker',
              'qualify',
              this.process
            ]);
          }
        }
      },
      error: (a: any) => {
        throw (a);
      },
      complete: () => {
        this.stdloading = false;
      }
    });
  }

  AssessmentStatus() {
    if (this.process == 2) {
      this.stdAssessmentservice.S2AssessmentStatus(this.QigId).subscribe(
        (data) => {
          this.status = data;
        }
      );
    } else if (this.process == 3) {
      this.stdAssessmentservice.S3AssessmentStatus(this.QigId).subscribe(
        (data) => {
          this.status = data;

          if (data != null || data != undefined) {
            this.enablelivemarking = data != 5 && data != 11 && this.S1Avilable;
          } else {
            this.enablelivemarking = true;
          }

          this.assessmentStatusEmit.emit(this.status);

        }
      );
    }
  }


  GetStatusMessage() { 
    let message = '';
    if (!this.IsPracticeQualifyingEnable?.IsS1Enable) {
      this.assessmentStatusEmit.emit(5);
      return this.alertmsg.S1enabled;

    }
    if (this.IsPracticeQualifyingEnable.S1Completed != 3) {
      this.assessmentStatusEmit.emit(0);
      return this.alertmsg.S1notcompleted;
    }
    if (
      !this.IsPracticeDisplay &&
      !this.IsQualifyDisplay &&
      this.process == 2
    ) {
      return this.alertmsg.S2enabled;
    }
    if (
      !this.IsPracticeDisplay &&
      !this.IsQualifyDisplay &&
      this.process == 3
    ) {
      this.assessmentStatusEmit.emit(5);
      return this.alertmsg.S3enabled;
    }
   
    return message;
  }

  CheckingQigPause() { 
    this.IsQigPause = false;
    this.qigservice.Getqigworkflowtracking(this.QigId, AppSettingEntityType.QIG).pipe(first()).subscribe({
      next: (result: any) => {
        this.WorkFlowStatusTracking = result;

        let b = this.WorkFlowStatusTracking.filter(
          (a) =>
            a.WorkflowStatusCode == WorkflowStatus.Pause &&
            a.ProcessStatus == WorkflowProcessStatus.OnHold
        );
         
        this.Alert.clear();
        if (b.length > 0) {
          this.IsQigPause = true;
          this.translate
            .get('General.qigpause')
            .subscribe((translated: string) => {
              this.intMessages.Markingprocesspaused = translated;
              this.Alert.warning(this.intMessages.Markingprocesspaused + '<br>' + 'Remarks : ' + b[0].Remark+'.');
            });
        }
      },
      error: (a: any) => {
        throw (a);
      },
      complete: () => {
        this.stdloading = false;
      }
    });
  }

  StatusAssessment() {
    this.stdAssessmentservice.S3AssessmentStatus(this.QigId).subscribe((data) => {
      if (data != null || data != undefined) {
        this.enablelivemarking = data != 5 && data != 11 && this.S1Avilable;
      } else {
        this.enablelivemarking = true;
      }
    });
  }

  GetUserPrivileges() {
    this.usrPrivilegeService.GetUserPrivileges(4).subscribe({
      next: (data: any) => {
        this.quicklinks = data;
      },
      error: (a: any) => {
        throw a;
      },
    });
  }

  navTolLive() {
    if (!this.enablelivemarking) {
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
              this.routers.navigate(['/projects/live-marking', this.QIgName, this.QigId]);
            }
          },
          error: (aew: any) => {
            throw aew;
          },
        });
    }
  }

  GetAllOverView(QigId: number, ProjectUserRoleId: number) {
    this._dashboardService.GetAllOverView(QigId, ProjectUserRoleId).subscribe({
      next: (ddata: any) => {
        if (ddata != null) {
          this.markingOverview = ddata;
        }
      },
      error: (ar: any) => {
        throw ar;
      },
    });
  }

}
