import { Component, Input } from '@angular/core';
import { QualityCheckService } from '../../../../services/project/quality-check/quality-check.service';
import { QigService } from 'src/app/services/project/qig.service';
import { AlertService } from '../../../../services/common/alert.service';
import { IRecommedData } from '../../../../model/standardisation/recommendation';
import { QuestionAnnotatorComponent } from '../../marking-player/question-annotator/question-annotator.component';
import { MatDialog } from '@angular/material/dialog';
import { QualityCheckComponent } from '../quality-check.component';
import { first } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import {
  LivemarkingApproveModel,
  QualityCheckInitialScriptModel,
  ScriptChildModel,
  TrialmarkingScriptDetails,
} from '../../../../model/project/quality-check/marker-tree-view-model';
import { WorkflowStatusTrackingModel } from 'src/app/model/standardisation/Assessment';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import {
  WorkflowStatus,
  WorkflowProcessStatus,
} from 'src/app/model/common-model';
import { LiveMarkingService } from 'src/app/services/project/live-marking/live-marking.service';

@Component({
  selector: 'emarking-script-details-view',
  templateUrl: './script-details-view.component.html',
  styleUrls: ['./script-details-view.component.css'],
})
export class ScriptDetailsViewComponent {
  ScriptId: any;
  qigID: any;
  scriptlistdetails: any = [];
  UserScriptMarkingRefId: any;
  Markingprocesspaused: string = '';
  playerOpening: boolean = false;
  livemarkapprove!: LivemarkingApproveModel;
  qaScriptModel!: QualityCheckInitialScriptModel;
  activeMarking!: ScriptChildModel;
  activephase!: string;
  scriptchildmodel!: ScriptChildModel;
  qadtlsloading: boolean = false;
  statusUpdloading: boolean = false;
  Ispause: any;
  IsQigPause: boolean = false;
  PhaseTrackingId: number = 0;
  WorkFlowStatusTracking: WorkflowStatusTrackingModel[] = [];
  intMessages: any = {
    Markingprocesspaused: '',
  };
  viewbuttondisabled: boolean = false;
  PClosed: boolean = false;
  constructor(
    public qualityCheckService: QualityCheckService,
    public qualityCheckComponent: QualityCheckComponent,
    public qigservice: QigService,
    public Alert: AlertService,
    public dialog: MatDialog,
    public translate: TranslateService,
    private liveMarkingService: LiveMarkingService
  ) {}

  @Input() activeScriptId?: number;

  GetScriptInDetails(QigId: any, ScriptId: any) {
    this.qigID = QigId;
    this.ScriptId = ScriptId;
    this.qadtlsloading = true;
    this.CheckQigPauseStatus(QigId);
    this.qualityCheckService.GetScriptInDetails(QigId, ScriptId).subscribe({
      next: (data: any) => {
        this.qaScriptModel = data;

        if (this.qaScriptModel != null && this.qaScriptModel != undefined) {
          var scriptdetails = this.qaScriptModel.ScriptChildModel.filter(
            (q) => q.IsActive
          )[0];

          this.PhaseTrackingId = scriptdetails.PhaseStatusTrackingId;
          this.qaScriptModel.ScriptChildModel.forEach((element) => {
            if (element.IsActive) {
              this.activeMarking = element;
              if (element.IsScriptFinalised) {
                this.activephase = 'Ad hoc';
              } else if (
                element.Phase == 2 &&
                (element.Status == 4 ||
                  element.Status == 7 ||
                  element.Status == 6)
              ) {
                this.activephase = 'Rc-1';
              } else if (
                element.Phase == 3 &&
                (element.Status == 4 ||
                  element.Status == 7 ||
                  element.Status == 6)
              ) {
                this.activephase = 'Rc-2';
              } else {
                this.activephase = 'Ad hoc';
              }
            }
          });

          if (this.qaScriptModel.IsInGracePeriod) {
            this.translate
              .get('Quality-Check.ScriptDetailsView.scrptingraceperiod')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          }
        }
      },
      error: (a: any) => {
        this.qadtlsloading = false;
        throw a;
      },
      complete: () => {
        this.qadtlsloading = false;
      },
    });
  }

  getScriptDetails(QigId: any, ScriptId: any) {
    this.GetScriptInDetails(QigId, ScriptId);
  }

  RedirectToTrialMarking(markingdata: IRecommedData, isactive: boolean) {
    this.viewbuttondisabled = true;
    this.Alert.clear();
    this.qigservice
      .Getqigworkflowtracking(this.qigID, AppSettingEntityType.QIG)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.WorkFlowStatusTracking = data;

          let wfa = this.WorkFlowStatusTracking.filter(
            (a) =>
              a.WorkflowStatusCode == WorkflowStatus.Pause &&
              a.ProcessStatus == WorkflowProcessStatus.OnHold
          );
          let projectclosure = this.WorkFlowStatusTracking.filter(
            (a) => a.ProjectStatus == WorkflowProcessStatus.ProjectClosure
          );
          if (projectclosure.length > 0) {
            this.PClosed = true;
          } else if (wfa.length > 0) {
            this.IsQigPause = true;
          }

          this.liveMarkingService
            .CheckScriptIsLivePool(this.ScriptId, this.PhaseTrackingId)
            .pipe(first())
            .subscribe({
              next: (res: any) => {
                if (res == 'SU001') {
                  if (!this.qadtlsloading) {
                    this.qadtlsloading = true;
                    markingdata.QigId =
                      this.qualityCheckComponent.activeQig.QigId;
                    markingdata.ScriptId = this.ScriptId;
                    if (this.qaScriptModel.CheckedOutByMe && isactive) {
                      markingdata.IsViewMode = false;
                    } else {
                      markingdata.IsViewMode = true;
                    }
                    const dialogRef = this.dialog.open(
                      QuestionAnnotatorComponent,
                      {
                        data: markingdata,
                        panelClass: 'fullviewpop',
                      }
                    );

                    dialogRef.afterClosed().subscribe((retstatus) => {
                      this.viewbuttondisabled = false;

                      if (retstatus.status == 1) {
                        var trialmarkingScriptDetails =
                          new TrialmarkingScriptDetails();

                        trialmarkingScriptDetails.QigID =
                          this.qualityCheckComponent.activeQig.QigId;
                        trialmarkingScriptDetails.ScriptID = this.ScriptId;
                        trialmarkingScriptDetails.WorkflowstatusId =
                          this.qaScriptModel.WorkflowStatusID;

                        this.InsertMarkingRecord(trialmarkingScriptDetails);
                      }
                    });

                    this.qadtlsloading = false;
                    this.playerOpening = false;
                  }
                } else {
                  this.GetScriptInDetails(this.qigID, this.ScriptId);

                  this.translate
                    .get('Quality-Check.ScriptDetailsView.sriptisinlivepool')
                    .subscribe((translated: string) => {
                      this.Alert.warning(translated);
                    });
                }
              },
              error: (err: any) => {
                throw err;
              },
              complete: () => {
                this.statusUpdloading = false;
              },
            });

          // }
        },
        error: (ar: any) => {
          this.qadtlsloading = false;
          throw ar;
        },
        complete: () => {
          this.qadtlsloading = false;
        },
      });
  }

  Updatescriptstatus(isCheckout: boolean, IsTrue: boolean) {
    if (!this.statusUpdloading && !this.qadtlsloading) {
      this.statusUpdloading = true;
      this.Alert.clear();
      this.qigservice
        .Getqigworkflowtracking(this.qigID, AppSettingEntityType.QIG)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.WorkFlowStatusTracking = data;
            let wea = this.WorkFlowStatusTracking.filter(
              (a) =>
                a.WorkflowStatusCode == WorkflowStatus.Pause &&
                a.ProcessStatus == WorkflowProcessStatus.OnHold
            );
            let projectclosure = this.WorkFlowStatusTracking.filter(
              (a) => a.ProjectStatus == WorkflowProcessStatus.ProjectClosure
            );

            if (projectclosure.length > 0) {
              this.PClosed = true;
              this.translate
                .get('Std.SetUp.Markingprocesspaused')
                .subscribe((translated: string) => {
                  this.intMessages.Markingprocesspaused =
                    translated +
                    '<br>' +
                    'Remarks : ' +
                    projectclosure[0].Remark +
                    '.';
                });

              this.translate
                .get('Quality-Check.ScriptDetailsView.projectclosedalrtmsg')
                .subscribe((translated: string) => {
                  this.Alert.warning(translated);
                });

              this.statusUpdloading = false;
            } else if (wea.length > 0) {
              this.IsQigPause = true;
              this.translate
                .get('Std.SetUp.Markingprocesspaused')
                .subscribe((translated: string) => {
                  this.intMessages.Markingprocesspaused =
                    translated + '<br>' + 'Remarks : ' + wea[0].Remark + '.';
                });

              this.Alert.warning(
                'This QIG is currently under “Paused state”.' +
                  '<br>' +
                  'Remarks : ' +
                  wea[0].Remark +
                  '.'
              );
              this.statusUpdloading = false;
            } else {
              this.liveMarkingService
                .CheckScriptIsLivePool(this.ScriptId, this.PhaseTrackingId)
                .pipe(first())
                .subscribe({
                  next: (res: any) => {
                    if (res == 'SU001') {
                      this.IsQigPause = false;
                      var lvmrkg = new LivemarkingApproveModel();
                      lvmrkg.ScriptID = this.ScriptId;

                      lvmrkg.QigID = this.qualityCheckComponent.activeQig.QigId;

                      lvmrkg.IsCheckout = IsTrue;
                      lvmrkg.WorkflowstatusId =
                        this.qaScriptModel.WorkflowStatusID;

                      if (isCheckout) {
                        lvmrkg.Status = 0;
                      } else {
                        lvmrkg.Status = this.qaScriptModel.Status;
                      }

                      var rem = document.getElementById(
                        'w3review'
                      ) as HTMLInputElement;

                      if (rem != null) {
                        lvmrkg.Remark = rem.value.trim();
                      }

                      if (!isCheckout) {
                        var isValid = this.Validation(
                          rem.value.trim(),
                          lvmrkg.Status
                        );

                        if (isValid) {
                          this.LiveMarkingScriptApprovalStatus(lvmrkg);
                        } else {
                          this.statusUpdloading = false;
                        }
                      } else {
                        this.CheckoutScript(lvmrkg);
                      }
                    } else {
                      this.GetScriptInDetails(this.qigID, this.ScriptId);
                      this.translate
                        .get(
                          'Quality-Check.ScriptDetailsView.sriptisinlivepool'
                        )
                        .subscribe((translated: string) => {
                          this.Alert.warning(translated);
                        });
                    }
                  },
                  error: (err: any) => {
                    this.statusUpdloading = false;
                    throw err;
                  },
                });
            }
          },
          error: (a: any) => {
            this.statusUpdloading = false;
            throw a;
          },
        });
    }
  }

  private CheckQigPauseStatus(QigID: number) {
    this.qigservice
      .Getqigworkflowtracking(QigID, AppSettingEntityType.QIG)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.WorkFlowStatusTracking = data;

          let awf = this.WorkFlowStatusTracking.filter(
            (a) =>
              a.WorkflowStatusCode == WorkflowStatus.Pause &&
              a.ProcessStatus == WorkflowProcessStatus.OnHold
          );
          let projectclosure = this.WorkFlowStatusTracking.filter(
            (a) => a.ProjectStatus == WorkflowProcessStatus.ProjectClosure
          );

          if (projectclosure.length > 0) {
            this.PClosed = true;

            this.translate
              .get('Std.SetUp.Markingprocesspaused')
              .subscribe((translated: string) => {
                this.intMessages.Markingprocesspaused =
                  translated +
                  '<br>' +
                  'Remarks : ' +
                  projectclosure[0].Remark +
                  '.';
              });

            this.translate
              .get('Quality-Check.ScriptDetailsView.projectclosedalrtmsg')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          } else if (awf.length > 0) {
            this.IsQigPause = true;

            this.translate
              .get('Std.SetUp.Markingprocesspaused')
              .subscribe((translated: string) => {
                this.intMessages.Markingprocesspaused =
                  translated + '<br>' + 'Remarks : ' + awf[0].Remark + '.';
              });

            this.Alert.warning(
              'This QIG is currently under “Paused state”.' +
                '<br>' +
                'Remarks : ' +
                awf[0].Remark +
                '.'
            );
          } else {
            this.PClosed = false;
            this.IsQigPause = false;
          }
        },
        error: (ea: any) => {
          throw ea;
        },
      });
  }

  private Validation(remarks: string, status: number): boolean {
    var isTrue = true;
    if (status == 0 || status == undefined) {
      this.translate
        .get('Quality-Check.ScriptDetailsView.selectaprovedorrtntomrker')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
      isTrue = false;
    }
    this.qadtlsloading = false;
    return isTrue;
  }

  private LiveMarkingScriptApprovalStatus(
    lvmrkg: LivemarkingApproveModel
  ): any {
    this.statusUpdloading = true;
    this.qualityCheckService
      .LiveMarkingScriptApprovalStatus(lvmrkg)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.GetScriptInDetails(this.qigID, this.ScriptId);

          if (data == 'SU001') {
            this.translate
              .get('Quality-Check.ScriptDetailsView.scrptappvedsucc')
              .subscribe((translated: string) => {
                this.Alert.success(translated);
              });
          } else if (data == 'RTRNTMRKR') {
            this.translate
              .get('Quality-Check.ScriptDetailsView.scrptrtntomrkr')
              .subscribe((translated: string) => {
                this.Alert.success(translated);
              });
          } else if (data == 'ESCLT') {
            this.translate
              .get('Quality-Check.ScriptDetailsView.scptescltsucc')
              .subscribe((translated: string) => {
                this.Alert.success(translated);
              });
          }
          else{
            this.DisplayAlertMessages(data);
          }
          
        },
        error: (a: any) => {
          this.statusUpdloading = false;
          throw a;
        },
        complete: () => {
          this.statusUpdloading = false;
        },
      });
  }

  private CheckoutScript(lvmrkg: LivemarkingApproveModel): any {
    this.statusUpdloading = true;
    this.qualityCheckService
      .CheckedOutScript(lvmrkg)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
  
          this.DisplayAlertMessages(data);
          this.GetScriptInDetails(lvmrkg.QigID, lvmrkg.ScriptID);
        },
        error: (a: any) => {
          this.statusUpdloading = false;
          throw a;
        },
        complete: () => {
          this.statusUpdloading = false;
        },
      });
  }

  private InsertMarkingRecord(
    markingscriptdetails: TrialmarkingScriptDetails
  ): any {
    this.statusUpdloading = true;
    this.qualityCheckService
      .AddMarkingRecord(markingscriptdetails)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.GetScriptInDetails(
            markingscriptdetails.QigID,
            markingscriptdetails.ScriptID
          );
        },
        error: (a: any) => {
          this.statusUpdloading = false;
          throw a;
        },
        complete: () => {
          this.statusUpdloading = false;
        },
      });
  }

  DisplayAlertMessages(data:any){
    if (data == 'UNCHKED') {
      this.translate
        .get('Quality-Check.ScriptDetailsView.scrptreleasedsuccessfully')
        .subscribe((translated: string) => {
          this.Alert.success(translated);
        });
    } else if (data == 'CHCKED') {
      this.translate
        .get('Quality-Check.ScriptDetailsView.scrptchkoutsucc')
        .subscribe((translated: string) => {
          this.Alert.success(translated);
        });
    }
     else if (data == 'CHCKEDBY') {
      this.translate
        .get('Quality-Check.ScriptDetailsView.scrptchkoutbyothermp')
        .subscribe((translated: string) => {
          this.Alert.success(translated);
        });
    } else if (data == 'Disabled') {
      this.translate
        .get('Quality-Check.ScriptDetailsView.userdisablestatus')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    } else if (data == 'Inactive') {
      this.translate
        .get('Quality-Check.ScriptDetailsView.userinactivestatus')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    } else if (data == 'Untagged') {
      this.translate
        .get('Quality-Check.ScriptDetailsView.mpuntaggedfromqig')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    } else if (data == 'UNMAPPED') {
      this.translate
        .get('Quality-Check.ScriptDetailsView.mpunmappedfromqig')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    } else if (data == 'TaggedOtherUser') {
      this.translate
        .get('Quality-Check.ScriptDetailsView.taggedOtherUser')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    }
  }
}
