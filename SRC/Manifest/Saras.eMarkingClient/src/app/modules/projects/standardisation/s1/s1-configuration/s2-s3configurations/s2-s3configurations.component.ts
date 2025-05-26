import { Component, OnInit } from '@angular/core';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { QigUserModel } from 'src/app/model/project/qig';
import { CommonService } from 'src/app/services/common/common.service';
import {
  CategorizationPoolModel,
  QulAssessmentModel,
  S1CompletedModel,
} from 'src/app/model/standardisation/QualifyingAssessment';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import { AlertService } from 'src/app/services/common/alert.service';
import {
  WorkflowStatus,
  WorkflowProcessStatus,
} from 'src/app/model/common-model';
import { MatDialog } from '@angular/material/dialog';
import { IRecommedData } from 'src/app/model/standardisation/recommendation';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs/operators';
import { CategorisationStasticsModel } from 'src/app/model/project/standardisation/std-one/categorisaton/categorisation';
import { CategorisationService } from 'src/app/services/project/standardisation/std-one/categorisation/categorisation.service';
import { S2S3ConfigService } from 'src/app/services/project/standardisation/std-one/std-two-std-three-config.service';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { QuestionAnnotatorComponent } from 'src/app/modules/projects/marking-player/question-annotator/question-annotator.component';

@Component({
  templateUrl: './s2-s3configurations.component.html',
  styleUrls: ['./s2-s3configurations.component.css']
})
export class S2S3configurationsComponent implements OnInit {
  ScriptCategorizationList!: CategorizationPoolModel[];
  ActiveQigId: any;
  ActiveQigName!:string;
  activeQig!: QigUserModel;
  NoofStandardisedScripts: number = 0;
  SelectedScripts: boolean = true;
  S1Checked: boolean = false;
  isDisabledToggle: boolean = false;
  ShowMeS1: boolean = false;
  projectworkflowtarckinglst!: S1CompletedModel[];
  QAssessmentscriptsModel!: QulAssessmentModel;
  QScriptCategorizationList: CategorizationPoolModel[] = [];
  assessmentstatus: string = '';
  remarks: string = '';
  SelectedscripCount: number = 0;
  max_total: number = 0;
  min: number = 1;
  ViewRandom: boolean = true;
  ViewSequential: boolean = false;
  mode: any = 2;
  approvaltype: any = 2;
  toleranceCount: number = 1;
  QualifiedAssessmentID: number = 0;
  Status: string = '';
  remarksbutton: boolean = true;
  selectedQIGdetail!: QigUserModel;
  IsS1Available: boolean = true;
  qassessmentloading: boolean = false;
  Ispause: any;
  IsClosure: any;
  StandaridsationScriptPoolCount: number = 0;
  AdditionalStandaridsationScriptPoolCount: number = 0;
  BenchMarkScriptPoolCount: number = 0;
  StandaridsationScriptPoolCategorized: number = 0;
  AdditionalStandaridsationScriptPoolCategorized: number = 0;
  BenchMarkScriptPoolCategorized: number = 0;
  categorisedloading: boolean = false;
  ScriptCategorizationFilter: any = [];

  IsCategorisedScript: boolean = true;
  stasticsloading: boolean = false;
  S1completed: any;
  constructor(
    public s2s3configService: S2S3ConfigService,
    public translate: TranslateService,
    public commonService: CommonService,
    public Alert: AlertService,
    public categorisationService: CategorisationService,
    public dialog: MatDialog
  ) {
  }

  qigsummary!: CategorisationStasticsModel;

  intMessages: any = {
    Markingprocesspaused: '',
    Markingprocessclosure: '',
    Selectscriptswarning: '',
    Tolerancecountwarning: '',
    Qualassessscriptupdatedsuc: '',
    Qualassessscriptnotsavedsuc: '',
    Qualassesssavedsuc: '',
    Qualassessupdatedsuc: '',
    Qualassessnotcreatedwarning: '',
    Qualassesscreateerror: '',
    S1completedsucc: '',
    Qualassessnotcreated: '',
    S1comerror: '',
    S1compltdsucfly: '',
    S1recomndsucfy: '',
    S1comp: ''
  };

  ScriptSearchValue: string = '';
  selected: any[] = [];

  ngOnInit(): void {

    this.translate
      .get('Std.QuaAsseCrea.Title')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });
    this.translate
      .get('Std.QuaAsseCrea.PageDesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
    this.translate
      .get('Std.SetUp.Markingprocesspaused')
      .subscribe((translated: string) => {
        this.intMessages.Markingprocesspaused = translated;
      });
    this.translate.get('Std.SetUp.Markingprocessclosure').subscribe((translated: string) => {
      this.intMessages.Markingprocessclosure = translated;
    });
    this.translate.get('Std.SetUp.S1comp').subscribe((translated: string) => {
      this.intMessages.S1comp = translated;
    });
    this.translate
      .get('Std.QuaAsseCrea.Selectscriptswarning')
      .subscribe((translated: string) => {
        this.intMessages.Selectscriptswarning = translated;
      });
    this.translate
      .get('Std.QuaAsseCrea.Tolerancecountwarning')
      .subscribe((translated: string) => {
        this.intMessages.Tolerancecountwarning = translated;
      });
    this.translate
      .get('Std.QuaAsseCrea.Qualassessscriptupdatedsuc')
      .subscribe((translated: string) => {
        this.intMessages.Qualassessscriptupdatedsuc = translated;
      });
    this.translate
      .get('Std.QuaAsseCrea.Qualassessscriptnotsavedsuc')
      .subscribe((translated: string) => {
        this.intMessages.Qualassessscriptnotsavedsuc = translated;
      });
    this.translate
      .get('Std.QuaAsseCrea.Qualassesssavedsuc')
      .subscribe((translated: string) => {
        this.intMessages.Qualassesssavedsuc = translated;
      });
    this.translate
      .get('Std.QuaAsseCrea.Qualassessupdatedsuc')
      .subscribe((translated: string) => {
        this.intMessages.Qualassessupdatedsuc = translated;
      });
    this.translate
      .get('Std.QuaAsseCrea.Qualassessnotcreatedwarning')
      .subscribe((translated: string) => {
        this.intMessages.Qualassessnotcreatedwarning = translated;
      });
    this.translate
      .get('Std.QuaAsseCrea.Qualassesscreateerror')
      .subscribe((translated: string) => {
        this.intMessages.Qualassesscreateerror = translated;
      });
    this.translate
      .get('Std.QuaAsseCrea.S1completedsucc')
      .subscribe((translated: string) => {
        this.intMessages.S1completedsucc = translated;
      });
    this.translate
      .get('Std.QuaAsseCrea.Qualassessnotcreated')
      .subscribe((translated: string) => {
        this.intMessages.Qualassessnotcreated = translated;
      });
    this.translate
      .get('Std.QuaAsseCrea.S1comerror')
      .subscribe((translated: string) => {
        this.intMessages.S1comerror = translated;
      });
    this.translate
      .get('Std.QuaAsseCrea.S1compltdsucfly')
      .subscribe((translated: string) => {
        this.intMessages.S1compltdsucfly = translated;
      });

    this.translate
      .get('Std.QuaAsseCrea.S1recomndsucfy')
      .subscribe((translated: string) => {
        this.intMessages.S1recomndsucfy = translated;
      });

      this.translate
      .get('Std.QuaAsseCrea.SelectTolerancewarning')
      .subscribe((translated: string) => {
        this.intMessages.SelectTolerancewarning = translated;
      });
  

    this.QAssessmentscriptsModel = {} as QulAssessmentModel;
    this.QAssessmentscriptsModel.QScriptDetails = [
      {},
    ] as CategorizationPoolModel[];
    this.ScriptSearchValue = '';
  }

  getQigDetails(selectedqig: QigUserModel) {
    this.activeQig = selectedqig;
    this.fnmodeSelection(2);
    this.fnapprovalType(2);
    this.IsS1Available = selectedqig.IsS1Available;
    this.ActiveQigId = selectedqig.QigId;
    this.ActiveQigName = selectedqig.QigName;
    this.GetS1CompletedRemarks();
    this.GetQualifyScriptdetails(this.ActiveQigId);
    this.Getqigworkflowtracking(this.ActiveQigId);
    this.getscriptreponses(this.ActiveQigId);
    this.ScriptSearchValue = '';

  }
  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(
      this.ScriptCategorizationList,
      event.previousIndex,
      event.currentIndex
    );
  }
  toogleS1Completed(evnt: any) {
    if (this.IsCategorisedScript) {
      this.ShowMeS1 = !this.ShowMeS1;
      if (!this.ShowMeS1) {
        this.S1Checked = true;
      } else {
        this.S1Checked = false;
      }
    }
  }
  scrollWin(evnt: any) {
    window.scrollBy(0, 500);
  }

  onScritsChecked(objscriptcategory: any) {
    this.SelectedscripCount = 0;
    var scriptcatlst = this.ScriptCategorizationList;

    for (var i = 0; i < this.ScriptCategorizationList?.length; i++) {
      if (scriptcatlst[i]?.IsSelected) {
        this.SelectedscripCount = this.SelectedscripCount + 1;
      }
    }

    if (this.SelectedscripCount == 0) {
      this.max_total = 1;
    } else {
      this.max_total = this.SelectedscripCount;
    }
  }

  fnmodeSelection(evnt: any) {
    if (evnt == 1) {
      this.ViewSequential = true;
      this.ViewRandom = false;
    } else {
      this.ViewSequential = false;
      this.ViewRandom = true;
    }
    this.mode = evnt;
  }
  fnapprovalType(evnt: any) {
    this.approvaltype = evnt;
  }

  Getqigworkflowtracking(QIGID: number) {
    this.qassessmentloading = true;
    this.s2s3configService
      .Getqigworkflowtracking(this.ActiveQigId, AppSettingEntityType.QIG)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null && data != undefined) {
            let WorkFlowStatusTracking = data;

            this.Ispause = WorkFlowStatusTracking?.filter((a: { WorkflowStatusCode: WorkflowStatus; ProcessStatus: WorkflowProcessStatus; }) =>
              a.WorkflowStatusCode == WorkflowStatus.Pause &&
              a.ProcessStatus == WorkflowProcessStatus.OnHold);

            this.IsClosure = WorkFlowStatusTracking?.filter((a: { WorkflowStatusCode: WorkflowStatus; ProcessStatus: WorkflowProcessStatus; }) =>
              a.WorkflowStatusCode == WorkflowStatus.Closure &&
              a.ProcessStatus == WorkflowProcessStatus.Closure);

            this.S1completed = WorkFlowStatusTracking?.findIndex(
              (a: {
                WorkflowStatusCode: WorkflowStatus;
                ProcessStatus: WorkflowProcessStatus;
              }) =>
                a.WorkflowStatusCode == WorkflowStatus.Standardization_1 &&
                a.ProcessStatus == WorkflowProcessStatus.Completed
            );

            this.Alert.clear();
            if (this.IsClosure.length > 0) {
              this.Alert.warning(this.intMessages.Markingprocessclosure + '<br>' + 'Remarks : ' + this.IsClosure[0].Remark + '.');
            }
            else if (this.Ispause.length > 0) {
              this.Alert.warning(this.intMessages.Markingprocesspaused + '<br>' + 'Remarks : ' + this.Ispause[0].Remark + '.');
            }
          }
          else {
            this.S1completed = -1;
          }
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.qassessmentloading = false;
          this.ScriptSearchValue = '';
        },
      });
  }

  AddUpdateQualifyingAssessment(evnt: any) {
    if (evnt != null) {
      (evnt.target as HTMLButtonElement).disabled = true;
      setTimeout(
        () => ((evnt.target as HTMLButtonElement).disabled = false),
        5000
      );
    }
    this.SelectedscripCount = 0;

    if(this.toleranceCount<=0){
      this.Alert.warning(this.intMessages.SelectTolerancewarning);
      return;
    }

    this.ScriptCategorizationList?.forEach((element) => {
      if (element.IsSelected) {
        this.SelectedscripCount = this.SelectedscripCount + 1;
      }
    });

    if (this.SelectedscripCount <= 0) {
      this.Alert.warning(this.intMessages.Selectscriptswarning);
      return;
    }
    if (this.SelectedscripCount < this.toleranceCount) {
      this.Alert.warning(this.intMessages.Tolerancecountwarning);
      return;
    }
if(this.mode == 2 && this.approvaltype == 2){
  this.QAssessmentscriptsModel.ScriptPresentationTypeName = 'Random';
  this.QAssessmentscriptsModel.ApprovalTypeName = 'Auto';
}
else{
  this.QAssessmentscriptsModel.ScriptPresentationTypeName = 'Sequential';
  this.QAssessmentscriptsModel.ApprovalTypeName = 'Manual';
}
    if (this.QualifiedAssessmentID > 0) {
      this.QAssessmentscriptsModel.IsActive = true;
      this.QAssessmentscriptsModel.IsTagged = false;
      this.QAssessmentscriptsModel.TotalNoOfScripts = this.NoofStandardisedScripts;
      this.QAssessmentscriptsModel.NoOfScriptSelected = this.SelectedscripCount;
      this.QAssessmentscriptsModel.ScriptPresentationType = this.mode;
      this.QAssessmentscriptsModel.ApprovalType = this.approvaltype;
      this.QAssessmentscriptsModel.ToleranceCount = this.toleranceCount;
      this.QAssessmentscriptsModel.QScriptDetails = this.ScriptCategorizationList;
      this.QAssessmentscriptsModel.QIGID = this.ActiveQigId;
      this.QAssessmentscriptsModel.QigName = this.ActiveQigName;
      this.qassessmentloading = true;
      this.s2s3configService
        .QualifyingAssessmentupdate(
          //  this.ActiveQigId,
          this.QAssessmentscriptsModel
        )
        .pipe(first())
        .subscribe({
          next: (data: any) => {

            this.Status = data;
            if (this.Status == 'U001') {
              this.Alert.success(this.intMessages.Qualassessscriptupdatedsuc);
              this.Getqigworkflowtracking(this.ActiveQigId);
              this.GetQualifyScriptdetails(this.ActiveQigId);
              this.getscriptreponses(this.ActiveQigId);
            }
            else if (this.Status == 'S1Comp') {
              this.Alert.warning(this.intMessages.S1comp);
            }
            else {
              this.Alert.error(this.intMessages.Qualassessscriptnotsavedsuc);
            }
          },
          error: (a: any) => {
            throw a;
          },
          complete: () => {
            this.qassessmentloading = false;
            this.ScriptSearchValue = '';
          },
        });
    } else { 
      this.QAssessmentscriptsModel.IsActive = true;
      this.QAssessmentscriptsModel.IsTagged = false;
      this.QAssessmentscriptsModel.TotalNoOfScripts =
        this.NoofStandardisedScripts;
      this.QAssessmentscriptsModel.NoOfScriptSelected = this.SelectedscripCount;
      this.QAssessmentscriptsModel.ScriptPresentationType = this.mode;
      this.QAssessmentscriptsModel.ApprovalType = this.approvaltype;
      this.QAssessmentscriptsModel.ToleranceCount = this.toleranceCount;
      this.QAssessmentscriptsModel.QScriptDetails =
        this.ScriptCategorizationList;
        this.QAssessmentscriptsModel.QigName = this.ActiveQigName;
      this.qassessmentloading = true;
      this.s2s3configService
        .QualifyingAssessmentInsert(
          this.ActiveQigId,
          this.QAssessmentscriptsModel
        )
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.Status = data;
            if (this.Status == 'S001') {
              this.Alert.success(this.intMessages.Qualassesssavedsuc);
              this.Getqigworkflowtracking(this.ActiveQigId);
              this.GetQualifyScriptdetails(this.ActiveQigId);
            }
            else if (this.Status == 'U001') {
              this.Alert.success(this.intMessages.Qualassessupdatedsuc);
              this.Getqigworkflowtracking(this.ActiveQigId);
            } else if (this.Status == 'S1Comp') {
              this.Alert.warning(this.intMessages.Qualassessnotcreatedwarning);
            }
            else {
              this.Alert.error(this.intMessages.Qualassesscreateerror);
            }
          },
          error: (a: any) => {
            throw a;
          },
          complete: () => {
            this.qassessmentloading = false;
            this.ScriptSearchValue = '';
          },
        });
    }
  }
  GetQualifyScriptdetails(QIGID: number) {
    this.ScriptCategorizationList = [];
    this.QualifiedAssessmentID = 0;
    this.qassessmentloading = true;
    this.s2s3configService
      .GetQualifyScriptdetails(QIGID)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null && data != undefined) {
            this.QualifiedAssessmentID = data.QualifyingAssessmentId;
            this.approvaltype = data.ApprovalType;
            this.mode = data.ScriptPresentationType;
            this.ViewRandom = data.ScriptPresentationType != 1;
            this.ViewSequential = data.ScriptPresentationType == 1;
            this.toleranceCount = data.ToleranceCount;
            this.NoofStandardisedScripts = data.TotalNoOfScripts;
            this.SelectedscripCount = data.NoOfScriptSelected;
            this.ScriptCategorizationList = data.Qscriptdetails;
            this.ScriptCategorizationFilter = data.Qscriptdetails;
            this.max_total = data.NoOfScriptSelected;
            this.min = 1;
            this.StandaridsationScriptPoolCount = data.StandardizationScriptPoolCount;
            this.AdditionalStandaridsationScriptPoolCount = data.AdditionalStandizationScriptPoolCount;
            this.BenchMarkScriptPoolCount = data.BenchMarkScriptPoolCount;
            this.StandaridsationScriptPoolCategorized = data.NoOfStandardizationScriptCategorized;
            this.AdditionalStandaridsationScriptPoolCategorized = data.NoOfAdditionalStandizationScriptPoolCategorized;
            this.BenchMarkScriptPoolCategorized = data.NoOfBenchMarkScriptPoolCategorized;

            if (this.BenchMarkScriptPoolCategorized >= this.BenchMarkScriptPoolCount && this.AdditionalStandaridsationScriptPoolCategorized >= this.AdditionalStandaridsationScriptPoolCount
              && this.StandaridsationScriptPoolCategorized >= this.StandaridsationScriptPoolCount) {
              this.IsCategorisedScript = true;
            }
            else {
              this.IsCategorisedScript = false;
            }
          }
          else {
            this.NoofStandardisedScripts = 0;
          }
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.qassessmentloading = false;
          this.ScriptSearchValue = '';
        },
      });
  }
  GetS1CompletedRemarks() {
    this.qassessmentloading = true;
    this.s2s3configService
      .S1CompletedRemarks(
        this.ActiveQigId,
        AppSettingEntityType.QIG,
        WorkflowStatus.Standardization_1
      )
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null) {
            if (data.length > 0) {
              for (let dataobj of data) {
                if (
                  (dataobj.WorkflowStatusCode ==
                    WorkflowStatus.Standardization_1 ||
                    dataobj.WorkflowStatusCode == WorkflowStatus.Pause) &&
                  dataobj.EntityID == this.ActiveQigId
                ) {
                  this.S1Checked = true;
                  this.isDisabledToggle = true;
                  this.ShowMeS1 = true;
                  this.remarks = dataobj.Remarks;
                  this.remarksbutton = true;
                } else {
                  this.ShowMeS1 = false;
                  this.S1Checked = false;
                  this.isDisabledToggle = false;
                  this.remarks = '';
                  this.remarksbutton = false;
                }
              }
            } else {
              this.ShowMeS1 = false;
              this.S1Checked = false;
              this.isDisabledToggle = false;
              this.remarks = '';
              this.remarksbutton = false;
            }
          }
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.qassessmentloading = false;
          this.ScriptSearchValue = '';
        },
      });
  }

  AddUpdateRemarks(evnt: any) {
    this.getscriptreponses(this.ActiveQigId);
    this.GetQualifyScriptdetails(this.ActiveQigId);
    setTimeout(() => {
      if (this.qigsummary?.QigAdlStandardisedScript > this.qigsummary?.AdlStandardisedScript || this.qigsummary?.QigStandardisedScript > this.qigsummary?.StandardisedScript || this.qigsummary?.QigBenchMarkScript > this.qigsummary?.BenchMarkScript) {
        this.Alert.warning("Script categorisation target not reached!");
      }

      else {
        let confmessage = this.intMessages.S1compltdsucfly;
        if (this.qigsummary?.RecommendedCount < this.qigsummary?.RecommendationPoolCount) {
          confmessage = this.intMessages.S1recomndsucfy;
        }
        const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
          data: {
            message: confmessage

          }
        });
        confirmDialog.afterClosed().subscribe((res: any) => {
          if (res === true) {
            if (evnt != null) {
              (evnt.target as HTMLButtonElement).disabled = true;
              setTimeout(
                () => ((evnt.target as HTMLButtonElement).disabled = false),
                5000
              );
            }
            let S1lstobj = new S1CompletedModel();
            S1lstobj.EntityID = this.ActiveQigId;
            S1lstobj.EntityType = AppSettingEntityType.QIG;
            S1lstobj.ProcessStatus = 3;
            S1lstobj.Remarks = this.remarks;
            this.qassessmentloading = true;
            S1lstobj.Buttonremarks = this.remarksbutton;
            S1lstobj.ScriptCategorizedList = this.ScriptCategorizationList;

            this.s2s3configService.ProjectWorkflowStatusTracking(WorkflowStatus.Standardization_1, S1lstobj).subscribe(data => {
              if (data == "P001") {
                this.Alert.success(this.intMessages.S1completedsucc);
                this.GetS1CompletedRemarks();
                this.getscriptreponses(this.ActiveQigId);
                this.Getqigworkflowtracking(this.ActiveQigId);
              } else if (data == "ASMNTC") {
                this.Alert.warning(this.intMessages.Qualassessnotcreated);
              }
              else if (data == "TRGTNOTREACH") {
                this.Alert.warning("Script categorisation target not reached!");
              } else
                if (this.Status == "S1Comp") {
                  this.Alert.warning(this.intMessages.Qualassessnotcreatedwarning);
                } else if (this.Status == "Paused") {
                  this.Alert.warning(this.intMessages.Markingprocesspaused);
                }
                else {
                  this.Alert.warning("S1 Already Completed for the Script!.");
                }
            },
              (err: any) => {
                throw (err)
              },
              () => {
                this.qassessmentloading = false;
                this.ScriptSearchValue = '';
              });
          }
        });
      }
    }, 500);
  }

  getscriptreponses(qigId: number): void {
    this.categorisedloading = true;
    this.categorisationService.getCategorisationStatistics(qigId).subscribe(
      (data: CategorisationStasticsModel) => {
        this.qigsummary = data;
      },
      (err: any) => {
        throw err;
      },
      () => {
        this.categorisedloading = false;
        this.ScriptSearchValue = '';
      }
    );
  }

  playerOpening: boolean = false;
  RedirectToQAssesmentPlayer(objscriptcategory: any) {

    if (!this.playerOpening) {
      this.playerOpening = true;
      var markingdata: IRecommedData = {
        ProjectId: 0,
        ScriptId: objscriptcategory.ScriptID,
        ScriptName: '',
        QigId: this.ActiveQigId,
        QigName: '',
        IsViewMode: true,
        Markedby: objscriptcategory.MarkedBy,
        UserScriptMarkingRefId: objscriptcategory.UserScriptMarkingRefID,
        Status: 0,
        RcType: 0,
        PhaseStatusTrackingId: 0
      };
      const dialogRef = this.dialog.open(QuestionAnnotatorComponent, {
        data: markingdata,
        panelClass: 'fullviewpop',
      });
      dialogRef.afterClosed().subscribe((result) => {
        
        this.translate
          .get('Std.QuaAsseCrea.PageDesc')
          .subscribe((translated: string) => {
            this.commonService.setPageDescription(translated);
          });
        this.translate
          .get('Std.QuaAsseCrea.Title')
          .subscribe((translated: string) => {
            this.commonService.setHeaderName(translated);
          });
      });
      this.playerOpening = false;      
      this.Getqigworkflowtracking(this.ActiveQigId);
      this.GetQualifyScriptdetails(this.ActiveQigId);
      this.ScriptSearchValue = '';
    }
  }

  SearchScript() {
    var ScriptSearchValue = this.ScriptSearchValue;
    this.ScriptCategorizationList = this.ScriptCategorizationFilter.filter(function (el: { ScriptName: string; }) {
      return el.ScriptName.toLowerCase().includes(
        ScriptSearchValue.trim().toLowerCase()
      );
    });
  }

  validateNumber(event: any) {
    var invalidChars = ["-", "e", "+", "E", "."];
    if (invalidChars.includes(event.key)) {
      event.preventDefault();
    }
  }

}
