import { Component, OnInit } from '@angular/core';
import { QigService } from 'src/app/services/project/qig.service';
import { QualifyingAssessmentService } from 'src/app/services/project/standardisation/std-two/qualifying-assessment.service';
import { ActivatedRoute, Router } from '@angular/router';
import { PracticeQuestionDetails, StandardisationQualifyAssessmentModel, StandardisationScriptClass, WorkflowStatusTrackingModel } from 'src/app/model/standardisation/Assessment';
import { IRecommedData } from 'src/app/model/standardisation/recommendation';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import { WorkflowStatus, WorkflowProcessStatus } from 'src/app/model/common-model';
import { first } from 'rxjs/operators';
import { AuthService } from 'src/app/services/auth/auth.service';
import { QuestionAnnotatorComponent } from 'src/app/modules/projects/marking-player/question-annotator/question-annotator.component';
import { PracticeBenchmarkService } from 'src/app/services/project/standardisation/std-two/practice-assessment.service';

@Component({
  templateUrl: './qualifying-assessment.component.html',
  styleUrls: ['./qualifying-assessment.component.css']
})
export class QualifyingAssessmentComponent implements OnInit {
  panelOpenState = false;
  QigId!: number;
  StdQlfyngAssModel!: StandardisationQualifyAssessmentModel;
  WorkFlowStatusTracking: WorkflowStatusTrackingModel[] = [];
  irecomenddata!: IRecommedData;
  process!: number;
  title!: string;
  QualifyLoader: boolean = false;
  qigId!: number;
  sumofDef!: number;
  sumofTot!: number;
  scriptloading: boolean = false;
  intMessages: any = {
    Markingprocesspaused: '',
  };
  
  stdOraddval: boolean = false;
  constructor(
    private Qualifyingassessmentservice: QualifyingAssessmentService,
    public services: PracticeBenchmarkService,
    private qigservice: QigService,
    private router: ActivatedRoute,
    public dialog: MatDialog,
    public translate: TranslateService,
    public commonService: CommonService,
    public Alert: AlertService,
    private route: Router,
    private authService: AuthService
  ) {
  }
  ngOnInit(): void {
    this.GetQualifyingAssessment(this.stdOraddval);
    this.qigId = this.router.snapshot.params['qigid'];
  }

  playerOpening: boolean = false;
  NavigateToMarkingPlayer(makingdata: IRecommedData) {
    makingdata.QigId = this.QigId;
    if (!this.playerOpening) {
      this.playerOpening = true;
      this.qigservice.Getqigworkflowtracking(this.QigId, AppSettingEntityType.QIG).pipe(first()).subscribe({
        next: (data: any) => {

          this.WorkFlowStatusTracking = data;

          let a = this.WorkFlowStatusTracking.findIndex(
            b => b.WorkflowStatusCode == WorkflowStatus.Pause 
            && b.ProcessStatus == WorkflowProcessStatus.OnHold);

          if (a >= 0) {
            this.GetQualifyingAssessment(this.stdOraddval);
          }
          else {
            const dialogRef = this.dialog.open(QuestionAnnotatorComponent, {
              data: makingdata,
              panelClass: 'fullviewpop',
            });
            dialogRef.afterClosed().subscribe((retval:any) => {
              if (!this.stdOraddval) {
                if (retval.status == 1) {
                  this.Qualifyingassessmentservice.QualifyingAssessmentUpdateSummery(this.QigId).subscribe();
                }
              }
              this.GetQualifyingAssessment(this.stdOraddval);
           });
          }
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.QualifyLoader = false;
          this.playerOpening = false;
        }
      });
    }
  }

  NavigateToDashboard() {
    if (this.process == 2) {
      this.route.navigate([`projects/dashboards/tl-atl`, this.QigId]);
    } else if (this.process == 3) {
      this.route.navigate([`projects/dashboards/marker`, this.QigId]);
    }
  }
iscompCount: number = 0;
starttest: boolean = false;
  GetQualifyingAssessment(stdOradd: boolean) {

    this.stdOraddval = stdOradd;

    this.translate
      .get('qualify.pagedescription')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
    this.translate
      .get('Std.SetUp.Markingprocesspaused')
      .subscribe((translated: string) => {
        this.intMessages.Markingprocesspaused = translated;
      });

    this.QigId = this.router.snapshot.params['qigid'];
    this.process = 0;
    let curntrole = this.authService.getCurrentRole();
    curntrole.forEach((role) => {
      if (role == 'TL' || role == 'ATL') {
        this.process = 2;
      }
      if (role == 'MARKER') {
        this.process = 3;
      }
    });
    if (!stdOradd) {
      this.StdQlfyngAssModel = {} as StandardisationQualifyAssessmentModel;
      this.StdQlfyngAssModel.Scripts = [{}, {}, {}] as StandardisationScriptClass[];
    }

    if (this.process == 2) {
      this.translate.get('qualify.S2title').subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
        this.title = translated;
      });

      this.QualifyLoader = true;
      this.Qualifyingassessmentservice.GetS2StandardisationScript(this.QigId, 0, this.stdOraddval).pipe(first()).subscribe({
        next: (data: any) => {
          this.StdQlfyngAssModel = data;

          if (this.StdQlfyngAssModel.ProcessStatus == 4) {
            this.Alert.warning(this.intMessages.Markingprocesspaused + '<br>' + 'Remarks : ' + this.StdQlfyngAssModel.PauseRemarks+'.');
          }
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.QualifyLoader = false;
        }
      });

    }
    else if (this.process == 3) {
      this.translate.get('qualify.S3title').subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
        this.title = translated;
      });

      this.QualifyLoader = true;
      this.Qualifyingassessmentservice.GetS3StandardisationScript(this.QigId, this.stdOraddval).pipe(first()).subscribe({
        next: (data: any) => {
          if (this.stdOraddval) {

            if (this.StdQlfyngAssModel != null && this.StdQlfyngAssModel != undefined) {
              this.StdQlfyngAssModel.Isopened = data.Isopened;
              this.StdQlfyngAssModel.Markedscript = data.Markedscript;
              this.StdQlfyngAssModel.Noofscripts = data.Noofscripts;
              this.StdQlfyngAssModel.ProcessStatus = data.ProcessStatus;
              this.StdQlfyngAssModel.Qigname = data.Qigname;
              this.StdQlfyngAssModel.Scripts = data.Scripts;
              this.StdQlfyngAssModel.TotalMarks = data.TotalMarks;
              this.StdQlfyngAssModel.UserScriptMarkingRefID = data.UserScriptMarkingRefID;
              this.StdQlfyngAssModel.WorkflowId = data.WorkflowId;
              this.StdQlfyngAssModel.AddResult = data.Result;
              this.StdQlfyngAssModel.ApprovalStatus = data.ApprovalStatus;  
              this.StdQlfyngAssModel.IsAdditionalDone = data.IsAdditionalDone;   
            }
            else {
              this.StdQlfyngAssModel = data;
            }
          }
          else {
            this.StdQlfyngAssModel = data;
          }


          if (this.StdQlfyngAssModel.ProcessStatus == 4) {
            this.Alert.warning(this.intMessages.MarkingprocesspausedStdQlfyngAssModel + '<br>' + 'Remarks : ' + this.StdQlfyngAssModel.PauseRemarks+'.');
          }
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.QualifyLoader = false;
        }
      });
    }
  }

  indexExpanded: number = -1;

  togglePanels(index: number) {
    if (!this.scriptloading) {
      this.indexExpanded = index == this.indexExpanded ? -1 : index;
    }
  }

  qualifyQusDtls!: PracticeQuestionDetails[];
  QualifyingQuestionDetails(scriptId: number = 0, iscompleted: boolean = false) {
    this.qualifyQusDtls = [{}, {}, {}] as PracticeQuestionDetails[];
    if (this.process == 2) {
      this.services
        .S2PracticeQuestionDetails(this.qigId, scriptId, iscompleted)
        .pipe(first())
        .subscribe({
          next: (result: any) => {
            this.qualifyQusDtls = result;
            this.sumofDef = this.qualifyQusDtls.reduce(
              (sum, current) => sum + current.DefenetiveMarks,
              0
            );
            this.sumofTot = this.qualifyQusDtls.reduce(
              (sum, current) => sum + current.AwardedMarks,
              0
            );
          },
          error: (a: any) => {
            throw (a);
          },
        });
    } else if (this.process == 3) {
      this.services
        .S3PracticeQuestionDetails(this.qigId, scriptId, iscompleted)
        .pipe(first())
        .subscribe({
          next: (result: any) => {
            this.qualifyQusDtls = result;
            this.sumofDef = this.qualifyQusDtls.reduce(
              (sum, current) => sum + current.DefenetiveMarks,
              0
            );
            this.sumofTot = this.qualifyQusDtls.reduce(
              (sum, current) => sum + current.AwardedMarks,
              0
            );
          },
          error: (a: any) => {
            throw (a);
          },
        });
    }
  }
}
