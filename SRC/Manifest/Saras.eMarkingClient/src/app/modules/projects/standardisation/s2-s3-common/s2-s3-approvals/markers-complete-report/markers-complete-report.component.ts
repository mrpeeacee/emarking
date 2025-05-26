import { Component, OnInit } from '@angular/core';
import {
  PracticeQuestionDetails,
  StandardisationQualifyAssessmentModel,
  StandardisationScriptClass,
  WorkflowStatusTrackingModel,
} from 'src/app/model/standardisation/Assessment';
import { QualifyingAssessmentService } from 'src/app/services/project/standardisation/std-two/qualifying-assessment.service';
import { PracticeBenchmarkService } from 'src/app/services/project/standardisation/std-two/practice-assessment.service';
import { first } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { S2S3ApprovalsService } from 'src/app/services/project/standardisation/s2-s3-approvals.service';
import { IRecommedData } from 'src/app/model/standardisation/recommendation';
import { QigService } from 'src/app/services/project/qig.service';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import { QuestionAnnotatorComponent } from 'src/app/modules/projects/marking-player/question-annotator/question-annotator.component';
import {
  WorkflowProcessStatus,
  WorkflowStatus,
} from 'src/app/model/common-model';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { ApprovalModel } from 'src/app/model/project/standardisation/s2-s3-approvals';
import { AlertService } from 'src/app/services/common/alert.service';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
@Component({
  selector: 'emarking-markers-complete-report',
  templateUrl: './markers-complete-report.component.html',
  styleUrls: ['./markers-complete-report.component.css'],
})
export class MarkersCompleteReportComponent implements OnInit {
  panelOpenState = false;
  stdScripts!: StandardisationQualifyAssessmentModel;
  stdScrpClass!: StandardisationScriptClass[];
  practiceQusDtls!: PracticeQuestionDetails[];
  markingPersonal!: string;
  result!: number;
  ToleranceCount!: number;
  approvalStatus!: string;
  approvalType!: string;
  sumofDef!: number;
  sumofTot!: number;
  qigId!: number;
  userRoleId!: number;
  scpType!: number;
  Ispause: any;
  ProcessStatus!: number;
  paused!: boolean;
  res!: any;
  IsAdditionaldone!: boolean;
  temp!: ApprovalModel;
  WorkflowId!: number;
  remark!: string;
  ScriptLoading: boolean = false;
  MPCountLoading: boolean = false;
  QusDetailLoading: boolean = false;
  WorkFlowStatusTracking: WorkflowStatusTrackingModel[] = [];
  indexExpanded: number = -1;

  constructor(
    public qServices: QualifyingAssessmentService,
    public pServices: PracticeBenchmarkService,
    private S2S3AppService: S2S3ApprovalsService,
    private qigservice: QigService,
    private route: ActivatedRoute,
    private router: Router,
    public dialog: MatDialog,
    public Alert: AlertService,
    public translate: TranslateService,
    public commonService: CommonService
  ) {}

  ngOnInit(): void {
    this.setTitles();
    this.qigId = this.route.snapshot.params['qigid'];
    this.userRoleId = this.route.snapshot.params['userroleid'];

    setTimeout(() => {
      this.Getqigworkflowtracking();
    }, 500);
    this.getStandardisationScripts();
    this.getMPName();
  }

  valMessage: any = {
    cnfrm: '',
    succss: '',
    rmkfield: '',
    alrdyapp: '',
    appfield: '',
  };

  setTitles() {
    this.translate.get('markcomp.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('markcomp.desc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.translate
      .get('markcomp.confirmmsg')
      .subscribe((translated: string) => {
        this.valMessage.cnfrm = translated;
      });
    this.translate.get('markcomp.approved').subscribe((translated: string) => {
      this.valMessage.succss = translated;
    });
    this.translate.get('markcomp.rmarkbln').subscribe((translated: string) => {
      this.valMessage.rmkfield = translated;
    });
    this.translate.get('markcomp.alapp').subscribe((translated: string) => {
      this.valMessage.alrdyapp = translated;
    });
    this.translate.get('markcomp.appfail').subscribe((translated: string) => {
      this.valMessage.appfield = translated;
    });
  }

  getStandardisationScripts() {
    this.ScriptLoading = true;
    this.qServices
      .GetS2StandardisationScript(this.qigId, this.userRoleId)
      .pipe(first())
      .subscribe({
        next: (data: StandardisationQualifyAssessmentModel) => {
          if (data != null || data != undefined) {
            this.stdScrpClass = data.Scripts;
            this.remark = data.Remarks;
            this.ProcessStatus = data.ProcessStatus;
            this.scpType = data.WorkflowId;
            this.paused = data.IsQigPaused;
            this.res = data.Result;
            this.IsAdditionaldone = data.IsAdditionalDone;
          }
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.ScriptLoading = false;
        },
      });
  }

  togglePanels(index: number) {
    if (!this.ScriptLoading) {
      this.indexExpanded = index == this.indexExpanded ? -1 : index;
    }
  }

  getNxtStandardisationScripts() {
    this.Getqigworkflowtracking();
    this.indexExpanded = -1;
    this.togglePanels(-1);
    this.qServices
      .GetS2StandardisationScript(this.qigId, this.userRoleId)
      .pipe(first())
      .subscribe({
        next: (data: StandardisationQualifyAssessmentModel) => {
          if (data != null || data != undefined) {
            this.stdScrpClass = data.Scripts;
            this.ProcessStatus = data.ProcessStatus;
            this.scpType = data.WorkflowId;
            this.paused = data.IsQigPaused;
            this.res = data.Result;
            this.IsAdditionaldone = data.IsAdditionalDone;
          }
        },
        error: (a: any) => {
          throw a;
        },
      });
  }

  getAdditionalScripts() {
    this.Getqigworkflowtracking();
    this.indexExpanded = -1;
    this.togglePanels(-1);
    this.qServices
      .GetS2StandardisationScript(this.qigId, this.userRoleId, true)
      .pipe(first())
      .subscribe({
        next: (data: StandardisationQualifyAssessmentModel) => {
          if (data != null || data != undefined) {
            const {
              Scripts,
              ProcessStatus,
              WorkflowId,
              IsQigPaused,
              Result,
              IsAdditionalDone,
            } = data;
            this.stdScrpClass = Scripts;
            this.ProcessStatus = ProcessStatus;
            this.scpType = WorkflowId;
            this.paused = IsQigPaused;
            this.res = Result;
            this.IsAdditionaldone = IsAdditionalDone;
          }
        },
        error: (a: any) => {
          throw a;
        },
      });
  }

  getMPName() {
    this.S2S3AppService.GetMarkingPersonal(this.qigId, this.userRoleId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.markingPersonal = data.find((x: any) => x.UserRoleId).MPName;
          this.result = data.find((x: any) => x.UserRoleId).Result;
          this.ToleranceCount = data.find(
            (x: any) => x.UserRoleId
          ).ToleranceCount;
          this.approvalStatus = data.find(
            (x: any) => x.UserRoleId
          ).ApprovalStatus;
          this.approvalType = data.find((x: any) => x.UserRoleId).ApprovalType;
        },
        error: (a: any) => {
          throw a;
        },
      });
  }

  PracticeQuestionDetails(scriptId: number, iscompleted: boolean) {
    if (!this.ScriptLoading) {
      this.QusDetailLoading = true;
    }
    this.pServices
      .S2PracticeQuestionDetails(
        this.qigId,
        scriptId,
        iscompleted,
        this.userRoleId
      )
      .pipe(first())
      .subscribe({
        next: (result: any) => {
          this.practiceQusDtls = result;
          this.sumofDef = this.practiceQusDtls.reduce(
            (sum, current) => sum + current.DefenetiveMarks,
            0
          );
          this.sumofTot = this.practiceQusDtls.reduce(
            (sum, current) => sum + current.AwardedMarks,
            0
          );
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.QusDetailLoading = false;
        },
      });
  }

  playerOpening: boolean = false;
  NavigateToMarkingPlayer(makingdata: IRecommedData) {
    makingdata.QigId = this.qigId;
    if (!this.playerOpening) {
      this.playerOpening = true;
      this.qigservice
        .Getqigworkflowtracking(this.qigId, AppSettingEntityType.QIG)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.WorkFlowStatusTracking = data;

            let b = this.WorkFlowStatusTracking.findIndex((a) => {
              return (
                a.WorkflowStatusCode == WorkflowStatus.Pause &&
                a.ProcessStatus == WorkflowProcessStatus.OnHold
              );
            });

            const dialogRef = this.dialog.open(QuestionAnnotatorComponent, {
              data: makingdata,
              panelClass: 'fullviewpop',
            });
            dialogRef.afterClosed().subscribe(() => {
              if (makingdata.Workflowid == 21) {
                this.getStandardisationScripts();
              } else if (makingdata.Workflowid == 22) {
                this.getAdditionalScripts();
              }
            });
          },
          error: (a: any) => {
            throw a;
          },
          complete: () => {
            this.playerOpening = false;
          },
        });
    }
  }

  scriptApproval(approvalDtls: ApprovalModel) { 
    approvalDtls.markingPersonal = this.markingPersonal;
    this.qigservice
      .Getqigworkflowtracking(this.qigId, AppSettingEntityType.QIG)
      .subscribe((data) => {
        if (data != null && data != undefined) {
          this.processWorkflowStatusTracking(data, approvalDtls);
        }
      });
  }

  processWorkflowStatusTracking(data: any, approvalDtls: ApprovalModel) {
    let WorkFlowStatusTracking = data;
    this.Ispause = WorkFlowStatusTracking.filter(
      (a: {
        WorkflowStatusCode: WorkflowStatus;
        ProcessStatus: WorkflowProcessStatus;
      }) =>
        a.WorkflowStatusCode == WorkflowStatus.Pause &&
        a.ProcessStatus == WorkflowProcessStatus.OnHold
    );
  
    if (this.Ispause.length > 0) {
      this.handlePauseCase();
    } else {
      this.handleNonPauseCase(approvalDtls);
    }
  }

  handlePauseCase() {
    this.translate
      .get('com.ispaused')
      .subscribe((translated: string) => {
        this.Alert.warning(
          translated +
            '<br>' +
            'Remarks : ' +
            this.Ispause[0].Remark +
            '.'
        );
      });
    this.getStandardisationScripts();
  }

  handleNonPauseCase(approvalDtls: ApprovalModel) {
    this.temp = approvalDtls;
    if (
      approvalDtls.Remark != null &&
      approvalDtls.Remark != '' &&
      approvalDtls.Remark.trim()
    ) {
      this.openConfirmationDialog();
    } else {
      this.Alert.warning(this.valMessage.rmkfield);
    }
  }

  openConfirmationDialog() {
    const confirmDialog = this.dialog.open(
      ConfirmationDialogComponent,
      {
        data: {
          message: this.valMessage.cnfrm,
        },
      }
    );
    confirmDialog.afterClosed().subscribe((res) => {
      if (res) {
        this.getscriptapproval();
      }
    });
  }

  getscriptapproval() {
    this.S2S3AppService.scriptApproval(this.qigId, this.userRoleId, this.temp)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data == 'S001') {
            this.Alert.success(this.valMessage.succss);
          } else if (data == 'R001') {
            this.Alert.warning(this.valMessage.rmkfield);
          } else if (data == 'A001') {
            this.Alert.warning(this.valMessage.alrdyapp);
          } else {
            this.Alert.warning(this.valMessage.appfield);
          }
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          if (this.scpType == 21) {
            this.getStandardisationScripts();
          } else {
            this.getAdditionalScripts();
          }
          this.getMPName();
        },
      });
  }

  navigateToAssignAddSp() {
    this.qigservice
      .Getqigworkflowtracking(this.qigId, AppSettingEntityType.QIG)
      .subscribe((data) => {
        if (data != null && data != undefined) {
          this.processWorkflowTracking(data);
        }
      });
  }

  processWorkflowTracking(data: any) {
    let WorkFlowStatusTracking = data;
    this.Ispause = WorkFlowStatusTracking.filter(
      (a: {
        WorkflowStatusCode: WorkflowStatus;
        ProcessStatus: WorkflowProcessStatus;
      }) =>
        a.WorkflowStatusCode == WorkflowStatus.Pause &&
        a.ProcessStatus == WorkflowProcessStatus.OnHold
    );
  
    if (this.Ispause.length > 0) {
      this.AssignAddSpPauseCase();
    } else {
      this.navigateToAdditionalStdScripts();
    }
  }

  AssignAddSpPauseCase() {
    this.translate
      .get('com.ispaused')
      .subscribe((translated: string) => {
        this.Alert.warning(
          translated +
            '<br>' +
            'Remarks : ' +
            this.Ispause[0].Remark +
            '.'
        );
      });
    this.getStandardisationScripts();
  }

  navigateToAdditionalStdScripts() {
    this.router.navigate([
      '/projects/s2-s3-approvals/' +
        this.qigId +
        '/' +
        this.userRoleId +
        '/AddionalStdScripts',
    ]);
  }

  backToS2S3Approval() {
    this.router.navigate(['/projects/s2-s3-approvals/'+this.qigId]);
  }

  Getqigworkflowtracking() {
    this.qigservice
      .Getqigworkflowtracking(this.qigId, AppSettingEntityType.QIG)
      .subscribe((data) => {
        if (data != null && data != undefined) {
          this.GetqigworkflowTracking(data);
        }
        this.setTitles();
      });
  }

  GetqigworkflowTracking(data: any) {
    let WorkFlowStatusTracking = data;
    this.Ispause = WorkFlowStatusTracking.filter(
      (a: {
        WorkflowStatusCode: WorkflowStatus;
        ProcessStatus: WorkflowProcessStatus;
      }) =>
        a.WorkflowStatusCode == WorkflowStatus.Pause &&
        a.ProcessStatus == WorkflowProcessStatus.OnHold
    );
  
    if (this.Ispause.length > 0) {
      this.GetqigworkPauseCase();
    }
  }

  GetqigworkPauseCase() {
    this.translate
      .get('com.ispaused')
      .subscribe((translated: string) => {
        this.Alert.warning(
          translated +
            '<br>' +
            'Remarks : ' +
            this.Ispause[0].Remark +
            '.'
        );
      });
  }
}
