import { Component, Input, OnChanges, OnInit } from '@angular/core';
import { first } from 'rxjs/operators';
import {
  CatContentScore,
  CategorisationModel,
  CategorisationTrialMarkModel,
  CategorisationTrialMarkModel1,
  CatQuestionDetails,
} from 'src/app/model/project/standardisation/std-one/categorisaton/categorisation';
import { CategorisationService } from 'src/app/services/project/standardisation/std-one/categorisation/categorisation.service';
import { ActivatedRoute, Router } from '@angular/router';
import {
  ScriptCategorizationPoolType,
  WorkflowProcessStatus,
  WorkflowStatus,
} from 'src/app/model/common-model';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { AlertService } from 'src/app/services/common/alert.service';
import { QuestionAnnotatorComponent } from 'src/app/modules/projects/marking-player/question-annotator/question-annotator.component';
import { IRecommedData } from 'src/app/model/standardisation/recommendation';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { QigService } from 'src/app/services/project/qig.service';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import { WorkflowStatusTrackingModel } from 'src/app/model/standardisation/Assessment';
@Component({
  templateUrl: './finalise-as-definitive.component.html',
  styleUrls: ['./finalise-as-definitive.component.css'],
})
export class FinaliseAsDefinitiveComponent implements OnInit, OnChanges {
  constructor(
    private categorisationService: CategorisationService,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog,
    public Alert: AlertService,
    public translate: TranslateService,
    public commonService: CommonService,
    public qigservice: QigService
  ) { }

  @Input() QigId!: number;
  @Input() QigIsKp!: boolean;

  trialMarkedScript!: CategorisationTrialMarkModel1;
  markingScript!: CategorisationTrialMarkModel;

  selectedPoolType = '0';

  trialcount: CategorisationTrialMarkModel1[] = [];
  expandedscript!: CategorisationModel | null;
  trialmrkloading: boolean = true;
  scrpbtnloading: boolean = false;
  kplstloading: boolean = false;
  IsQigPaused: boolean = false;
  IsS1Completed: any;
  Ispause: any;
  qigId!: number;
  scriptId!: number;
  IsKp: boolean = false;
  loaddata: any = [{}, {}, {}];
  statusTracking: WorkflowStatusTrackingModel[] = [];
  scoringcomponents: CatContentScore[] = [];

  intMessages: any = {
    CatSuccss: '',
    reCatSucc: '',
    ScrAlrInQa: '',
    SelFinalize: '',
    SelPlType: '',
    CatError: '',
    uncatsucc: '',
    RecCnfirm: '',
    RecCatOthCnfm: '',
    Markingprocesspaused: '',
    stdscriptcompted: '',
    scriptunrecmnded: ''
  };

  ngOnChanges(): void {
    if (this.QigId > 0) {
      this.IsKp = this.QigIsKp;
    }
  }

  ngOnInit() {
    this.qigId = this.route.snapshot.params['qigid'];
    this.scriptId = this.route.snapshot.params['scriptId'];
    this.textIntenationalization();
    this.getTrialMarkedScript(this.qigId, this.scriptId);
  }

  private textIntenationalization() {
    this.translate.get('reCat.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });

    this.translate.get('reCat.pgedesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });

    this.translate.get('reCat.catsuccs').subscribe((translated: string) => {
      this.intMessages.CatSuccss = translated;
    });

    this.translate.get('reCat.recatsucc').subscribe((translated: string) => {
      this.intMessages.reCatSucc = translated;
    });

    this.translate.get('reCat.scinqa').subscribe((translated: string) => {
      this.intMessages.ScrAlrInQa = translated;
    });

    this.translate.get('reCat.selasfnl').subscribe((translated: string) => {
      this.intMessages.SelFinalize = translated;
    });

    this.translate.get('reCat.selpltpe').subscribe((translated: string) => {
      this.intMessages.SelPlType = translated;
    });

    this.translate.get('reCat.caterr').subscribe((translated: string) => {
      this.intMessages.CatError = translated;
    });

    this.translate.get('reCat.uncatsucc').subscribe((translated: string) => {
      this.intMessages.uncatsucc = translated;
    });

    this.translate.get('reCat.catcnfm').subscribe((translated: string) => {
      this.intMessages.RecCnfirm = translated;
    });

    this.translate.get('reCat.scriptunrcmnded').subscribe((translated: string) => {
      this.intMessages.scriptunrecmnded = translated;
    })

    this.translate
      .get('reCat.RecCatOthCnfm')
      .subscribe((translated: string) => {
        this.intMessages.RecCatOthCnfm = translated;
      });

    this.translate
      .get('Std.SetUp.Markingprocesspaused')
      .subscribe((translated: string) => {
        this.intMessages.Markingprocesspaused = translated;
      });
    this.translate
      .get('reCat.stdscriptcompted')
      .subscribe((translated: string) => {
        this.intMessages.stdscriptcompted = translated;
      });
  }

  getTrialMarkedScript(qigId: number, catModel: number,UserScriptMarkingRefID?:number ) {
    
    this.trialmrkloading = true;
    this.Getqigworkflowtracking();
    this.categorisationService.getTrialMarkedScript(qigId, catModel, UserScriptMarkingRefID).subscribe(
      
      (data: CategorisationTrialMarkModel1) => {
        
        this.trialMarkedScript = data;
        this.selectedPoolType = this.trialMarkedScript ?.PoolType.toString();
      },
      (err: any) => {
        throw err;
      },
      () => {
        this.trialmrkloading = false;
        this.textIntenationalization();
      }
    );
  }

  Getqigworkflowtracking() {
    this.qigservice
      .Getqigworkflowtracking(this.qigId, AppSettingEntityType.QIG)
      .subscribe((data) => {
        if (data != null && data != undefined) {
          let WorkFlowStatusTracking = data;

          this.Ispause = WorkFlowStatusTracking.filter((a: { WorkflowStatusCode: WorkflowStatus; ProcessStatus: WorkflowProcessStatus; }) =>
            a.WorkflowStatusCode == WorkflowStatus.Pause &&
            a.ProcessStatus == WorkflowProcessStatus.OnHold);

          this.IsS1Completed = WorkFlowStatusTracking.filter((a: { WorkflowStatusCode: WorkflowStatus; ProcessStatus: WorkflowProcessStatus; }) =>
            a.WorkflowStatusCode == WorkflowStatus.Standardization_1 &&
            a.ProcessStatus == WorkflowProcessStatus.Completed);

          if (this.Ispause.length > 0) {
            this.translate
              .get('Std.QuaAsseCrea.Qualifyingpaused')
              .subscribe((translated: string) => {
                this.Alert.warning(translated + '<br>' + 'Remarks : ' + this.Ispause[0].Remark + '.');
              });
          } else if (this.IsS1Completed.length > 0) {
            this.translate
              .get('Std.QuaAsseCrea.s1closure')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          }
          this.textIntenationalization();
        }
      });
  }

  getMrkforcomp(cttitle: CatContentScore, datacnt: CatContentScore[]) {
    let mrk = 'NA';
    let res = datacnt.find((a) => a.Name == cttitle.Name);
    if (res != null && res != undefined) {
      mrk = res?.Marks.toString();
    }
    return mrk;
  }
  getMrkforQuestion(cttitle: CatQuestionDetails, datacnt: CatQuestionDetails[]) {
    
    let mrk = 'NA';
    let res = datacnt.find((a) => a.QuestionCode == cttitle.QuestionCode);
    
    if (res != null && res != undefined) {
      if(res.Marks != null)
      {
      mrk = res?.Marks.toString();
      }
    }
    return mrk;
  }

  SetAsDefinitive(
    trialmark: CategorisationTrialMarkModel,
    trialmarks: CategorisationTrialMarkModel[],
    isInQfAsses: boolean,
    pooltype: number
  ) {
    if (
      !isInQfAsses &&
      pooltype <= 0 &&
      !this.trialMarkedScript.IsQigPaused &&
      !this.trialMarkedScript.IsS1Completed
    ) {
      trialmarks.forEach((elem) => {
        if (elem.MarkingRefId == trialmark.MarkingRefId) {
          elem.SelectAsDefinitive = !trialmark.SelectAsDefinitive;
        } else {
          elem.SelectAsDefinitive = false;
        }
      });
    }
  }

  scriptCategorise() {
    let script = this.trialMarkedScript;
    if (!this.IsQigPaused) {
      if (
        script.PoolType > 0 &&
        parseInt(this.selectedPoolType) == ScriptCategorizationPoolType.None
      ) {
        const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
          data: {
            message: this.intMessages.RecCnfirm,
          },
          panelClass: 'confirmationpop',
        });
        confirmDialog.afterClosed().subscribe((result) => {
          if (result) {
            this.SaveCatrgorise(script);
          }
        });
      } else {
        this.SaveCatrgorise(script);
      }
    }
  }
   tempscriptname!:string;
  private SaveCatrgorise(script: CategorisationTrialMarkModel1) {
    debugger
    this.tempscriptname = script.ScriptName;
    if (script != null && this.validateScriptSave(script)) {
      let catsScriptIndex = this.trialMarkedScript.TrailMarkedScripts.filter(
        (tr) => tr.SelectAsDefinitive
      );
      let catsScript = catsScriptIndex[0];
      this.scrpbtnloading = true;
      if (
        script.PoolType > 0 &&
        parseInt(this.selectedPoolType) == ScriptCategorizationPoolType.None
      ) {
        this.SubmitCategorisation(catsScript, script.ScriptId);
      } else {
        this.categorisationService
          .IsScriptCategorised(this.qigId, script.ScriptId)
          .subscribe(
            (data: boolean) => {
              if (data) {
                const confirmDialog = this.dialog.open(
                  ConfirmationDialogComponent,
                  {
                    data: {
                      message: this.intMessages.RecCatOthCnfm,
                    },
                    panelClass: 'confirmationpop',
                  }
                );
                confirmDialog.afterClosed().subscribe((result) => {
                  if (result) {
                    this.SubmitCategorisation(catsScript, script.ScriptId);
                  } else {
                    this.getTrialMarkedScript(this.qigId, script.ScriptId);
                    this.selectedPoolType = script.PoolType.toString();
                  }
                });
              } else {
                this.SubmitCategorisation(catsScript, script.ScriptId);
              }
            },
            (err: any) => {
              throw err;
            },
            () => {
              this.scrpbtnloading = false;
              this.textIntenationalization();
            }
          );
      }
    }
  }

  private validateScriptSave(script: CategorisationTrialMarkModel1): boolean {
    if (
      script.PoolType > 0 &&
      parseInt(this.selectedPoolType) == ScriptCategorizationPoolType.None
    ) {
      return true;
    }

    let itemindex = this.trialMarkedScript.TrailMarkedScripts.findIndex(
      (tr) => tr.SelectAsDefinitive
    );
    if (itemindex < 0) {
      this.Alert.warning(this.intMessages.SelFinalize);
      return false;
    }
    if (parseInt(this.selectedPoolType) <= ScriptCategorizationPoolType.None) {
      this.Alert.warning(this.intMessages.SelPlType);
      return false;
    }
    return true;
  }

  private SubmitCategorisation(
    catsScript: CategorisationTrialMarkModel,
    scriptid: number
  ) {
    this.scrpbtnloading = true;
    if( parseInt(this.selectedPoolType) == 0){
      catsScript.Poolcategory = "Uncategorised";
    }
    else if( parseInt(this.selectedPoolType) == 1){
      catsScript.Poolcategory = "Standardisation Script";
    }else if( parseInt(this.selectedPoolType) == 2){
      catsScript.Poolcategory = "Additional Standardised Script";
    }else if( parseInt(this.selectedPoolType) == 3){
      catsScript.Poolcategory = "Benchmark Script";
    }
    this.categorisationService
      .categorise({
        MarkingRefId: catsScript.MarkingRefId,
        MarkedBy: catsScript.MarkedBy,
        PoolType: parseInt(this.selectedPoolType),
        QigId: this.qigId,
        Poolcategory:catsScript.Poolcategory,
        ScriptName:this.tempscriptname,
        ScriptId: scriptid,
        SelectAsDefinitive: catsScript.SelectAsDefinitive,
      })
      .pipe(first())
      .subscribe({
        next: (result: any) => {
          if (result != null && result != undefined) {
            if (result == 'SU001') {
              this.Alert.success(this.intMessages.CatSuccss);
            }
            if (result == 'MVDNXTLVL') {
              this.Alert.warning(this.intMessages.ScrAlrInQa);
            }
            if (result == 'SELPTPE') {
              this.Alert.warning(this.intMessages.SelPlType);
            }
            if (result == 'SELSFIN') {
              this.Alert.warning(this.intMessages.SelFinalize);
            }
            if (result == 'S1Comp') {
              this.Alert.warning(this.intMessages.stdscriptcompted);
            }
            if (result == 'QIGPOS') {
              this.Alert.warning(this.intMessages.Markingprocesspaused);
            }
            if (result == 'UNCATS') {
              this.Alert.success(this.intMessages.uncatsucc);
            }
            if (result == 'ER001') {
              this.Alert.error(this.intMessages.caterr);
            }
            if (result == 'UNRCMNDED') {
              this.Alert.warning(this.intMessages.scriptunrecmnded);
            }
          } else {
            this.Alert.error(this.intMessages.caterr);
          }
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.getTrialMarkedScript(this.qigId, this.scriptId);
          this.scrpbtnloading = false;
        },
      });
  }
  playerOpening: boolean = false;
  trailmarking(
    script: CategorisationTrialMarkModel1,
    markingdata: IRecommedData
  ) {
    
    this.categorisationService
      .Getqigworkflowtracking(this.qigId, AppSettingEntityType.QIG)
      .subscribe({
        next: (dataqig: any) => {
          
          this.statusTracking = dataqig;

          let val = this.statusTracking.findIndex(
            (a) =>
              a.WorkflowStatusCode == WorkflowStatus.Pause &&
              a.ProcessStatus == WorkflowProcessStatus.OnHold
          );
          if (val >= 0) {
            this.getTrialMarkedScript(this.qigId, this.scriptId);
            if (!this.playerOpening) {
              this.playerOpening = true;
              this.categorisationService
                .IsQigInQualifying(this.qigId, this.scriptId)
                .subscribe({
                  next: (data: boolean) => {

                    markingdata.IsViewMode = true;

                    markingdata.QigId = this.qigId;
                    const dialogRef = this.dialog.open(
                      QuestionAnnotatorComponent,
                      {
                        data: markingdata,
                        panelClass: 'fullviewpop',
                      }
                    );
                    dialogRef.afterClosed().subscribe(() => {
                      this.textIntenationalization();
                      this.getTrialMarkedScript(this.qigId, this.scriptId);
                    });
                  },
                  error: (err: any) => {
                    throw err;
                  },
                  complete: () => {
                    this.playerOpening = false;
                  },
                });
            }
          } else {
            if (!this.playerOpening) {
              this.playerOpening = true;
              this.categorisationService
                .IsQigInQualifying(this.qigId, this.scriptId)
                .subscribe({
                  next: (data: boolean) => {
                    if (data || this.trialMarkedScript.PoolType > 0 || this.trialMarkedScript.IsInQfAsses) {
                      markingdata.IsViewMode = true;
                    }
                    markingdata.QigId = this.qigId;
                    const dialogRef = this.dialog.open(
                      QuestionAnnotatorComponent,
                      {
                        data: markingdata,
                        panelClass: 'fullviewpop',
                      }
                    );
                    dialogRef.afterClosed().subscribe(() => {
                      this.textIntenationalization();
                      this.getTrialMarkedScript(this.qigId, this.scriptId);
                    });
                  },
                  error: (err: any) => {
                    throw err;
                  },
                  complete: () => {
                    this.playerOpening = false;
                  },
                });
            }
          }
        },
      });
  }

  remarking(script: CategorisationTrialMarkModel, markingdata: IRecommedData) {
    if (!this.playerOpening) {
      
      this.playerOpening = true;
      this.categorisationService
        .Getqigworkflowtracking(this.qigId, AppSettingEntityType.QIG)
        .subscribe({
          next: (qdata: any) => {
            
            this.statusTracking = qdata;

            let val = this.statusTracking.findIndex(
              (a) =>

                a.ProcessStatus == WorkflowProcessStatus.OnHold
            );
            if (val > 0) {
              this.getTrialMarkedScript(this.qigId, this.scriptId);
            } else {
              let scriptdata = this.trialMarkedScript;
              if ((scriptdata != null) && (scriptdata.IsS1Completed || scriptdata.IsQigPaused)) {
                markingdata.IsViewMode = true;
                if (
                  script.SelectAsDefinitive &&
                  this.trialMarkedScript.PoolType > 0
                ) {
                  if(!scriptdata.IsQigPaused){
                    markingdata.IsViewMode = false;
                  }
                }
                markingdata.QigId = this.qigId;
                const dialogRef = this.dialog.open(QuestionAnnotatorComponent, {
                  data: markingdata,
                  panelClass: 'fullviewpop',
                });
                dialogRef.afterClosed().subscribe((result) => {
                  if( parseInt(this.selectedPoolType) == 0){
                    script.Poolcategory = "Uncategorised";
                  }
                  else if( parseInt(this.selectedPoolType) == 1){
                    script.Poolcategory = "Standardisation Script";
                  }else if( parseInt(this.selectedPoolType) == 2){
                    script.Poolcategory = "Additional Standardised Script";
                  }else if( parseInt(this.selectedPoolType) == 3){
                    script.Poolcategory = "Benchmark Script";
                  }
                  if (result.status == 1) {
                    this.categorisationService
                      .recategorise({
                        MarkingRefId: script.MarkingRefId,
                        MarkedBy: script.MarkedBy,
                        PoolType: parseInt(this.selectedPoolType),
                        QigId: this.qigId,
                        Poolcategory:script.Poolcategory,
                        ScriptName:this.tempscriptname,
                        ScriptId: this.scriptId,
                        SelectAsDefinitive: script.SelectAsDefinitive,
                      })
                      .pipe(first())
                      .subscribe({
                        next: (subresult: any) => {
                          this.ShowRemarkmessage(subresult);
                        },
                        error: (ar: any) => {
                          throw ar;
                        },
                        complete: () => {
                          setTimeout(() => {
                            this.getTrialMarkedScript(
                              this.qigId,
                              this.scriptId
                            );
                          }, 100);
                          this.playerOpening = false;
                        },
                      });
                  }
                  else {
                    this.getTrialMarkedScript(this.qigId, this.scriptId, script.MarkingRefId);
                  }
                  this.textIntenationalization();
                });
              }
              this.playerOpening = false;
            }
          },
          error: (a: any) =>{
            this.playerOpening = false;
            throw a;
          },
          complete: ()=>{
            this.playerOpening = false;
          }
        });
    }
  }

  private ShowRemarkmessage(result: any) {
    if (result != null && result != undefined) {
      if (result == 'SU001') {
        this.Alert.success(this.intMessages.reCatSucc);
      }
      if (result == 'MVDNXTLVL') {
        this.Alert.warning(this.intMessages.ScrAlrInQa);
      }
      if (result == 'SELPTPE') {
        this.Alert.warning(this.intMessages.SelPlType);
      }
      if (result == 'SELSFIN') {
        this.Alert.warning(this.intMessages.SelFinalize);
      }
      if (result == 'S1Comp') {
        this.Alert.warning(
          this.intMessages.stdscriptcompted
        );
      }
      if (result == 'QIGPOS') {
        this.Alert.warning(
          this.intMessages.Markingprocesspaused
        );
      }
      if (result == 'UNCATS') {
        this.Alert.success(this.intMessages.uncatsucc);
      }
      if (result == 'ER001') {
        this.Alert.error(this.intMessages.caterr);
      }
    } else {
      this.Alert.error(this.intMessages.caterr);
    }
  }

  BacktoCat() {
    this.router.navigate([
      `projects/configurations/categorisation-pool`,
      this.qigId,
    ]);
  }

  ViewQuestionDetails(data: CategorisationTrialMarkModel) {
    
    if (data != null || undefined) {
      var trialmarkeddata = data;
      this.trialMarkedScript.QuestionDetails = trialmarkeddata ?.QuestionDetails;
    }
  }
}
