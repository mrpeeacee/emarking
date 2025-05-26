import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { SemiAutomaticQuestionsService } from 'src/app/services/project/response-processing/semi-automatic.service';
import {
  ViewFrequencyDistributionModel,
  ViewAllBlankSummaryModel,
  BlankOption,
  ModerateMarks,
  EnableManualMarkigModel,
  FibDiscrepencyReportModel
} from 'src/app/model/project/response-processing/semi-automatic-model';
import { first } from 'rxjs/operators';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { Router, ActivatedRoute } from '@angular/router';
import { FIBDiscrepencyReportComponent } from '../fib-discrepency-report/fib-discrepency-report.component';
import { ProjectClosureModel } from 'src/app/model/project/setup/project-closure-model';
import { QigService } from 'src/app/services/project/qig.service';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';

@Component({
  selector: 'emarking-frequency-distribution',
  templateUrl: './frequency-distribution.component.html',
  styleUrls: ['./frequency-distribution.component.css']
})
export class FrequencyDistributionComponent implements OnInit {
  @ViewChild('closeComponentManualmarking') closemodalpopup: any;
  Viewfrequencydistributionlst!: ViewFrequencyDistributionModel;
  Remarks!: string;
  ParentQuestionId!: number;
  CandidateAnswer!: string;
  showhide: boolean = false;
  Frequencydisthideshow: boolean = false;
  index: any;
  ViewallBlanksummarylst!: ViewAllBlankSummaryModel[];
  ObjFibDiscrepencyReportModel!: FibDiscrepencyReportModel;
  selectedblankindex: number = 0;
  frequencysLoading: boolean = true;
  allblankLoading: boolean = true;
  Id!: number;
  MarksAwarded!: number;
  Score?: number;
  Standardization!: boolean;
  Noofkeyword!: number;
  CorrectAnswer?: string;
  isChecked: boolean = false;
  IsManualMarkingRequired: boolean = false;
  updatemoderateLoading: boolean = false;
  intMessages: any = {
    Confirmwarning: '',
    discexist: '',
    disexistin: '',
    nodata: ''
  };
  Status!: string;
  QigId?: number;
  FreId!: number;
  ProjectQuestionId!: number;
  enablemanualmarking: boolean = false;
  Qiglst!: ViewAllBlankSummaryModel[];
  NoOfResponses!: number;
  count: number = 0;
  panelOpenState = true;
  closureData!: ProjectClosureModel;
  isClosed: any;

  constructor(
    public translate: TranslateService,
    public commonService: CommonService,
    public Alert: AlertService,
    public semiautomaticquestionsservice: SemiAutomaticQuestionsService,
    private dialog: MatDialog,
    private route: Router,
    private router: ActivatedRoute,
    public qigservice: QigService
  ) { }

  ngOnInit(): void {
    this.translate
      .get('SetUp.SemiAutomatic.FrequencyDistribution')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });
    this.translate
      .get('SetUp.SemiAutomatic.FreqPageDescription')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
    this.translate
      .get('SetUp.SemiAutomatic.Moderatescore')
      .subscribe((translated: string) => {
        this.intMessages.Moderatescore = translated;
      });
    this.translate
      .get('SetUp.SemiAutomatic.ReModeratescore')
      .subscribe((translated: string) => {
        this.intMessages.ReModeratescore = translated;
      });
    this.translate
      .get('SetUp.SemiAutomatic.Updatemoderatealert')
      .subscribe((translated: string) => {
        this.intMessages.Updatemoderatealert = translated;
      });
    this.translate
      .get('SetUp.SemiAutomatic.Updateremoderatealert')
      .subscribe((translated: string) => {
        this.intMessages.Updateremoderatealert = translated;
      });
    this.translate
      .get('SetUp.SemiAutomatic.Manualmarkingalert')
      .subscribe((translated: string) => {
        this.intMessages.Manualmarkingalert = translated;
      });
    this.translate
      .get('Std.SetUp.Confirmwarning')
      .subscribe((translated: string) => {
        this.intMessages.Confirmwarning = translated;
      });
    this.translate
      .get('SetUp.SemiAutomatic.moderateall')
      .subscribe((translated: string) => {
        this.intMessages.moderateall = translated;
      });
    this.translate
      .get('SetUp.SemiAutomatic.discexist')
      .subscribe((translated: string) => {
        this.intMessages.discexist = translated;
      });
    this.translate
      .get('SetUp.SemiAutomatic.disexistin')
      .subscribe((translated: string) => {
        this.intMessages.disexistin = translated;
      });
    this.translate
      .get('SetUp.SemiAutomatic.datafnd')
      .subscribe((translated: string) => {
        this.intMessages.nodata = translated;
      });

    this.translate
      .get('SetUp.SemiAutomatic.AllresponsesetoManualmarkingalert')
      .subscribe((translated: string) => {
        this.intMessages.AllresponsesetoManualmarkingalert = translated;
      });

    this.ProjectQuestionId = this.router.snapshot.params['ProjectQuestionId'];
    this.GetFrequencyDistribution(this.ProjectQuestionId);
    this.GetAllBlankSummary(this.ProjectQuestionId);
    this.Getqigworkflowtracking();
  }
  GetFrequencyDistribution(parentQuestionId: number) {
    this.frequencysLoading = true;
    this.ParentQuestionId = parentQuestionId;
    this.semiautomaticquestionsservice
      .GetFrequencyDistribution(parentQuestionId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.Viewfrequencydistributionlst = data;
          this.Viewfrequencydistributionlst?.BlankOption?.forEach(item => {
            item.disablemanualmark =
              item.CandidateAnswer.filter(
                a => a.MarkingType == null || a.MarkingType == 3
              )?.length == 0
                ? true
                : false;
            item.disablemoderate =
              item.CandidateAnswer.filter(a => a.MarkingType == null)?.length ==
                0
                ? true
                : false;
          });

          this.Viewfrequencydistributionlst?.BlankOption?.forEach(item => {
            this.Viewfrequencydistributionlst?.BlankOption?.filter(
              a => a.QIGId == item.QIGId && item.MarkingType == 3
            ).forEach(item1 => {
              item1.isenabledmanualmarking = true;
            });
          });
        },
        error: (err: any) => {
          this.translate
            .get('SetUp.SemiAutomatic.ErrorFrequencyDistribution')
            .subscribe((translated: string) => {
              this.Alert.clear();
              this.Alert.error(translated);
            });
          this.frequencysLoading = false;
          throw err;
        },
        complete: () => {
          this.frequencysLoading = false;
        }
      });
  }

  GetAllBlankSummary(parentQuestionId: number) {
    this.allblankLoading = true;
    this.ParentQuestionId = parentQuestionId;
    this.semiautomaticquestionsservice
      .GetAllBlankSummary(parentQuestionId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.ViewallBlanksummarylst = data;
        },
        error: (err: any) => {
          this.translate
            .get('SetUp.SemiAutomatic.errorblanksummary')
            .subscribe((translated: string) => {
              this.Alert.clear();
              this.Alert.error(translated);
            });
          this.allblankLoading = false;
          throw err;
        },
        complete: () => {
          this.allblankLoading = false;
        }
      });
  }

  EnableManual(blankoption: BlankOption) {
    this.Noofkeyword = 0;
    this.NoOfResponses = 0;
    this.Qiglst = this.ViewallBlanksummarylst.filter(
      a => a.QigId == blankoption?.QIGId
    );
    this.Remarks = this.Qiglst[0]?.Remarks;

    this.Viewfrequencydistributionlst?.BlankOption?.filter(
      a => a?.QIGId == blankoption?.QIGId
    ).forEach(item => {
      this.Noofkeyword += item.CandidateAnswer.reduce((accumulator, obj) => {
        if (obj.MarkingType == null) {
          return accumulator + 1;
        }
        return accumulator;
      }, 0);
      this.NoOfResponses += item.CandidateAnswer.filter(
        item2 => item2.Responses != null
      ).reduce((sum, current) => sum + current.Responses, 0);
    });
  }

  closeFreqDist() {
    this.route.navigateByUrl('projects/semi-automatic-questions');
  }
  SaveUpdateModerate(
    Id: number,
    MarksAwarded: number,
    CandidateAnswer: string,
    ProjectQuestionId: number,
    IsmoderateOrRemoderate: boolean
  ) {
    this.Id = Id;
    this.MarksAwarded = MarksAwarded;

    if (MarksAwarded != null && Id > 0) {
      let objModerateMarks = new ModerateMarks();
      objModerateMarks.Id = Id;
      objModerateMarks.MarkingType = 2;
      objModerateMarks.MarksAwarded = this.MarksAwarded;
      objModerateMarks.CandidatesAnswer = CandidateAnswer;
      objModerateMarks.ProjectQuestionId = ProjectQuestionId;
      objModerateMarks.ParentQuestionId = this.ParentQuestionId;

      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: IsmoderateOrRemoderate ? this.intMessages.Moderatescore : this.intMessages.ReModeratescore
        }
      });
      confirmDialog.afterClosed().subscribe(result => {
        this.Alert.clear();
        if (result === true) {
          this.updatemoderateLoading = true;
          this.updateModerateMarks(objModerateMarks, IsmoderateOrRemoderate);
        }
      });
    } else {
      this.showModerateScoreError();
    }
  }

  updateModerateMarks(objModerateMarks: ModerateMarks, IsmoderateOrRemoderate: boolean) {
    this.Alert.clear();
    this.semiautomaticquestionsservice
      .UpdateModerateMarks(objModerateMarks)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data == 'S001') {
            this.showUpdateModerateSuccess(IsmoderateOrRemoderate);
          } else if (data == 'SERROR') {
            this.showSecurityError();
          } else {
            this.showModerateScoreError();
          }
        },
        complete: () => {
          this.updatemoderateLoading = false;
        }
      });
  }

  showUpdateModerateSuccess(IsmoderateOrRemoderate: boolean) {
    this.Alert.clear();
    if (IsmoderateOrRemoderate) {
      this.Alert.success(this.intMessages.Updatemoderatealert);
    }
    else {
      this.Alert.success(this.intMessages.Updateremoderatealert);
    }
    this.GetFrequencyDistribution(this.ParentQuestionId);
    this.GetAllBlankSummary(this.ParentQuestionId);
  }

  showSecurityError() {
    this.translate
      .get('SetUp.SemiAutomatic.Securityerror')
      .subscribe((translated: string) => {
        this.Alert.clear();
        this.Alert.warning(translated);
      });
  }

  showModerateScoreError() {
    this.translate
      .get('SetUp.SemiAutomatic.Moderatescoreerror')
      .subscribe((translated: string) => {
        this.Alert.clear();
        this.updatemoderateLoading = false;
        this.Alert.warning(translated);
      });
  }

  UpdateOverallModerate(ProjectQuestionId: number) {
    if (ProjectQuestionId != null || ProjectQuestionId != undefined) {
      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: this.intMessages.moderateall
        }
      });
      confirmDialog.afterClosed().subscribe(result => {
        this.Alert.clear();
        if (result === true) {
          this.Alert.clear();
          this.updatemoderateLoading = true;
          this.semiautomaticquestionsservice
            .UpdateOverallModerateMarks(ProjectQuestionId)
            .pipe(first())
            .subscribe({
              next: (data: any) => {
                if (data == 'S001') {
                  this.showUpdateModerateSuccess(true);
                } else {
                  this.handleModeratedFailed();
                }
              },
              complete: () => {
                this.updatemoderateLoading = false;
              }
            });
        }
      });
    } else {
      this.showModerateScoreError();
    }
  }

  handleModeratedFailed() {
    this.translate.get('SetUp.SemiAutomatic.moderatedfailed')
      .subscribe((translated: string) => {
        this.Alert.clear();
        this.updatemoderateLoading = false;
        this.Alert.warning(translated);
      });
  }

  UpdateManualMarkig(blankoption: BlankOption) {
    this.enablemanualmarking = true;
    if (
      this.Remarks == '' ||
      this.Remarks == null ||
      this.Remarks.trim() == ''
    ) {
      this.showRemarksNullWarning();
      return;
    }

    this.isChecked = true;
    let objManualMarkig = new EnableManualMarkigModel();
    objManualMarkig.Remarks = this.Remarks;
    objManualMarkig.StandardizationRequired =
      this.Standardization == null ? false : this.Standardization;
    objManualMarkig.QigId = blankoption?.QIGId;
    objManualMarkig.Id = this.FreId;
    objManualMarkig.ProjectQuestionId = blankoption?.ProjectQuestionId;

    const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        message: this.intMessages.Manualmarkingalert
      }
    });

    confirmDialog.afterClosed().subscribe(result => {
      this.Alert.clear();
      if (result === true) {
        this.frequencysLoading = true;
        this.allblankLoading = true;
        this.enablemanualmarking = false;
        this.Alert.clear();
        this.closemodalpopup.nativeElement.click();

        this.updateManualMarkig(objManualMarkig);
      }
      this.enablemanualmarking = false;
    });
  }

  updateManualMarkig(objManualMarkig: EnableManualMarkigModel) {
    this.semiautomaticquestionsservice
      .UpdateManualMarkig(objManualMarkig)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.Status = data;
          if (this.Status == 'S001') {
            this.showUpdateManualMarkingSuccess();
          } else if (this.Status == 'SERROR') {
            this.showManualSecurityError();
          } else {
            this.showManualMarkingFailed();
          }
        },
        error: (a: any) => {
          this.frequencysLoading = false;
          this.allblankLoading = false;
          throw a;
        },
        complete: () => {
          this.frequencysLoading = false;
          this.allblankLoading = false;
        }
      });
  }

  showRemarksNullWarning() {
    this.translate
      .get('SetUp.SemiAutomatic.Remarksnullwarning')
      .subscribe((translated: string) => {
        this.enablemanualmarking = false;
        this.Alert.clear();
        this.Alert.warning(translated);
      });
  }

  showUpdateManualMarkingSuccess() {
    this.translate
      .get('SetUp.SemiAutomatic.Updatemanualmarking')
      .subscribe((translated: string) => {
        this.Alert.clear();
        this.Alert.success(translated);
        this.GetAllBlankSummary(this.ParentQuestionId);
        this.GetFrequencyDistribution(this.ParentQuestionId);
      });
  }

  ShowUpdateAllresponsestoManualMarkingSuccess() {
    this.translate
      .get('SetUp.SemiAutomatic.SuccessAllresponsesetoManualmarking')
      .subscribe((translated: string) => {
        this.Alert.clear();
        this.Alert.success(translated);
        this.GetAllBlankSummary(this.ParentQuestionId);
        this.GetFrequencyDistribution(this.ParentQuestionId);
      });
  }

  showManualSecurityError() {
    this.translate
      .get('SetUp.SemiAutomatic.Securityerror')
      .subscribe((translated: string) => {
        this.frequencysLoading = false;
        this.allblankLoading = false;
        this.Alert.clear();
        this.Alert.warning(translated);
      });
  }

  showManualMarkingFailed() {
    this.translate
      .get('SetUp.SemiAutomatic.manualmarkingfailed')
      .subscribe((translated: string) => {
        this.Alert.clear();
        this.updatemoderateLoading = false;
        this.Alert.warning(translated);
      });
  }

  validateNumber(event: any) {
    if (event.key != 'ArrowUp' && event.key != 'ArrowDown') {
      event.preventDefault();
    }
  }

  getDescripency(CandidateAnswer: string, ProjectQuestionId: number, QigId: number, ID: number) {
    const editorDialog = this.dialog.open(FIBDiscrepencyReportComponent, {
      data: {
        projectquestionid: 0,
        candidateAnswer: CandidateAnswer,
        projectQuestionId: ProjectQuestionId,
        qigId: QigId,
        Id: ID,
        isSaveClicked: 0
      },
      panelClass: 'md-pop'
    });
    editorDialog.afterClosed().subscribe(result => {
      if (result?.data == 1) {
        this.GetFrequencyDistribution(this.ProjectQuestionId);
        this.GetAllBlankSummary(this.ProjectQuestionId);
      }
    });
  }

  BtnCheckDescrepancy(projectQuestionId: number) {
    this.frequencysLoading = true;
    this.semiautomaticquestionsservice
      .CheckDiscrepancy(projectQuestionId)
      .pipe(first())
      .subscribe({
        next: (data: ProjectClosureModel) => {
          if (data != null && data != undefined) {
            this.closureData = data;
            if (this.closureData?.DiscrepancyModels?.length > 0) {
              var disqiglist = this.closureData?.DiscrepancyModels?.filter(
                (x: any) => x.IsDiscrepancyExist
              );
              var disqignames = '';
              if (disqiglist != undefined && disqiglist != null) {
                disqignames = Array.prototype.map
                  .call(disqiglist, s => s.QigName)
                  .toString();
              }
              this.Alert.warning(
                this.intMessages.disexistin +
                ' : ' +
                this.ViewallBlanksummarylst[this.selectedblankindex]
                  .BlankName +
                ' of ' +
                disqignames
              );
              this.GetFrequencyDistribution(this.ParentQuestionId);
              this.GetAllBlankSummary(this.ParentQuestionId);
            } else {
              this.Alert.success(this.intMessages.discexist);
              this.GetFrequencyDistribution(this.ParentQuestionId);
              this.GetAllBlankSummary(this.ParentQuestionId);
            }
          } else {
            this.Alert.warning(this.intMessages.nodata);
          }
        },
        error: (a: any) => {
          this.frequencysLoading = false;
          throw a;
        },
        complete: () => {
          this.frequencysLoading = false;
        }
      });
  }


  Getqigworkflowtracking() {
    this.qigservice
      .Getqigworkflowtracking(0, AppSettingEntityType.Project)
      .subscribe((data) => {
        if (data != null && data != undefined) {
          let WorkFlowStatusTracking = data;
          if (WorkFlowStatusTracking.length > 0) {
            this.isClosed = WorkFlowStatusTracking[0].ProjectStatus;
          }
          else {
            this.isClosed = 0;
          }
        }
      });
  }

  SaveUpdateAllResponsestoManualMarking(blankoption: BlankOption) {

    const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        message: this.intMessages.AllresponsesetoManualmarkingalert
      }
    });

    confirmDialog.afterClosed().subscribe(result => {
      this.Alert.clear();
      if (result === true) {
        this.frequencysLoading = true;
        this.allblankLoading = true;
        this.enablemanualmarking = false;
        this.Alert.clear();
        this.closemodalpopup.nativeElement.click();

        this.UpdateAllResponsestoManualMarkig(blankoption?.ParentQuestionId);
      }
      this.enablemanualmarking = false;
    });


  }

  UpdateAllResponsestoManualMarkig(ParentQuestionId: number) {
    this.semiautomaticquestionsservice
      .UpdateAllResponsestoManualMarkig(ParentQuestionId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.Status = data;
          if (this.Status == 'S001') {
            this.ShowUpdateAllresponsestoManualMarkingSuccess();
          } else if (this.Status == 'SERROR') {
            this.showManualSecurityError();
          } else {
            this.showManualMarkingFailed();
          }
        },
        error: (a: any) => {
          this.frequencysLoading = false;
          this.allblankLoading = false;
          throw a;
        },
        complete: () => {
          this.frequencysLoading = false;
          this.allblankLoading = false;
        }
      });
  }
}
