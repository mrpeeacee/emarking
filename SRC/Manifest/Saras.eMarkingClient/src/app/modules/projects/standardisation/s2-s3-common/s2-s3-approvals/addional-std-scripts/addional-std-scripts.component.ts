import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import {
  WorkflowProcessStatus,
  WorkflowStatus,
} from 'src/app/model/common-model';
import {
  AdditionalStdScriptsModel,
  AssignAdditionalStdScriptsModel,
} from 'src/app/model/project/standardisation/s2-s3-approvals';
import { WorkflowStatusTrackingModel } from 'src/app/model/standardisation/Assessment';
import { IRecommedData } from 'src/app/model/standardisation/recommendation';
import { QuestionAnnotatorComponent } from 'src/app/modules/projects/marking-player/question-annotator/question-annotator.component';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { QigService } from 'src/app/services/project/qig.service';
import { S2S3ApprovalsService } from 'src/app/services/project/standardisation/s2-s3-approvals.service';
import { QualifyingAssessmentService } from 'src/app/services/project/standardisation/std-two/qualifying-assessment.service';

@Component({
  selector: 'emarking-addional-std-scripts',
  templateUrl: './addional-std-scripts.component.html',
  styleUrls: ['./addional-std-scripts.component.css'],
})
export class AddionalStdScriptsComponent implements OnInit {
  AddstdscriptsList!: AdditionalStdScriptsModel[];
  qigId!: number;
  projectuserroleid!: number;
  AssignAddstdscripts!: AssignAdditionalStdScriptsModel[];
  WorkFlowStatusTracking: WorkflowStatusTrackingModel[] = [];
  Ispause: any;
  QidPaused: boolean = false;
  IsQigPaused!: string;

  constructor(
    public s2s3approvalservice: S2S3ApprovalsService,
    private route: ActivatedRoute,
    public Alert: AlertService,
    private router: Router,
    public translate: TranslateService,
    public commonService: CommonService,
    private qigservice: QigService,
    public dialog: MatDialog,
    public qServices: QualifyingAssessmentService
  ) {}

  ngOnInit(): void {
    this.qigId = this.route.snapshot.params['qigid'];
    this.projectuserroleid = this.route.snapshot.params['userroleid'];
    this.setTitles();
    this.Getqigworkflowtracking();
    this.GetAssignAdditionalStdScripts();
    this.getStandardisationScripts();
  }

  valMessage: any = {
    plselect: '',
    confrmadd: '',
    succs: '',
    erras: '',
  };

  setTitles() {
    this.translate.get('addscp.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('addscp.desc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.translate.get('addscp.plssel').subscribe((translated: string) => {
      this.valMessage.plselect = translated;
    });
    this.translate.get('addscp.confirmadd').subscribe((translated: string) => {
      this.valMessage.confrmadd = translated;
    });
    this.translate.get('addscp.success').subscribe((translated: string) => {
      this.valMessage.succs = translated;
    });
    this.translate.get('addscp.erras').subscribe((translated: string) => {
      this.valMessage.erras = translated;
    });
  }

  GetAssignAdditionalStdScripts() {
    this.s2s3approvalservice
      .GetAssignAdditionalStdScripts(this.qigId, this.projectuserroleid)
      .subscribe({
        next: (ddata: any) => {
          if (ddata.length > 0 || ddata != null || ddata != undefined) {
            this.AddstdscriptsList = ddata;
          }
        },
        error: (ad: any) => {
          throw ad;
        },
      });
  }

  getStandardisationScripts() {
    this.qServices
      .GetS2StandardisationScript(this.qigId, this.projectuserroleid)
      .subscribe((data: any) => {
        this.IsQigPaused = data.ProcessStatus;
      });
  }

  AssignAddScripts() {
    this.qigservice
      .Getqigworkflowtracking(this.qigId, AppSettingEntityType.QIG)
      .subscribe((data) => {
        if (data != null && data != undefined) {
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
            this.translate
              .get('com.ispaused')
              .subscribe((translated: string) => {
                this.Alert.warning(translated + '<br>' + 'Remarks : ' + this.Ispause[0].Remark+'.');
              });
              this.QidPaused = true;
              this.GetAssignAdditionalStdScripts()
          } else {
            let qnsIndex = this.AddstdscriptsList.findIndex(
              (a) => a.IsSelected
            );
            if (qnsIndex < 0) {
              this.Alert.warning(this.valMessage.plselect);
            } else {
              const confirmDialog = this.dialog.open(
                ConfirmationDialogComponent,
                {
                  data: {
                    message: this.valMessage.confrmadd,
                  },
                }
              );

              confirmDialog.afterClosed().subscribe((res) => {
                if (res) {
                  var assignadditional = new AssignAdditionalStdScriptsModel();
                  assignadditional.QIGID = this.qigId;
                  assignadditional.ProjectUserRoleID = this.projectuserroleid;
                  assignadditional.ScriptIDs = this.AddstdscriptsList;

                  this.s2s3approvalservice
                    .AssignAdditionalStdScripts(assignadditional)
                    .pipe(first())
                    .subscribe({
                      next: (outdata: any) => {
                        if (outdata != null || outdata != undefined) {
                          if (outdata == 'S001') {
                            this.Alert.success(this.valMessage.succs);
                            setTimeout(() => {
                              this.backToMarkerComplete();
                            }, 1000);
                            this.GetAssignAdditionalStdScripts();
                          }
                          if (outdata == 'E001') {
                            this.Alert.success(this.valMessage.erras);
                          }
                        }
                      },
                      error: (err: any) => {
                        throw err;
                      },
                    });
                }
              });
            }
          }
        }
      });
  }

  backToMarkerComplete() {
    this.router.navigate([
      '/projects/s2-s3-approvals/' +
        this.qigId +
        '/' +
        this.projectuserroleid +
        '/MarkersCompleteReport',
    ]);
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

            let wfa = this.WorkFlowStatusTracking.findIndex(
              (a) =>
                a.WorkflowStatusCode == WorkflowStatus.Pause &&
                a.ProcessStatus == WorkflowProcessStatus.OnHold
            );

            if (wfa >= 0) {
              this.GetAssignAdditionalStdScripts();
            } else {
              const dialogRef = this.dialog.open(QuestionAnnotatorComponent, {
                data: makingdata,
                panelClass: 'fullviewpop',
              });
              dialogRef.afterClosed().subscribe(() => {
                this.GetAssignAdditionalStdScripts();
              });
            }
          },
          error: (ae: any) => {
            throw ae;
          },
          complete: () => {
            this.playerOpening = false;
          },
        });
    }
  }

  Getqigworkflowtracking() {
    this.qigservice
      .Getqigworkflowtracking(this.qigId, AppSettingEntityType.QIG)
      .subscribe((data) => {
        if (data != null && data != undefined) {
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
            this.translate
              .get('com.ispaused')
              .subscribe((translated: string) => {
                this.Alert.warning(translated + '<br>' + 'Remarks : ' + this.Ispause[0].Remark+'.');
              });
          }
          this.setTitles();
        }
      });
  }
}
