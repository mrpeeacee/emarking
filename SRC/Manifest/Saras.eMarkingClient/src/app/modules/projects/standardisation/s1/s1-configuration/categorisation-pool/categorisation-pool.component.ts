import { Component, OnInit, ViewChild } from '@angular/core';
import { QigUserModel } from 'src/app/model/project/qig';
import { AlertService } from 'src/app/services/common/alert.service';
import {
  ScriptCategorizationPoolType,
  WorkflowStatus,
  WorkflowProcessStatus,
} from 'src/app/model/common-model';
import { IRecommedData } from 'src/app/model/standardisation/recommendation';
import { MatDialog } from '@angular/material/dialog';
import { CommonService } from 'src/app/services/common/common.service';
import { TranslateService } from '@ngx-translate/core';
import { QigService } from 'src/app/services/project/qig.service';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';

import { ChartConfiguration, ChartData, ChartType } from 'chart.js';
import DataLabelsPlugin from 'chartjs-plugin-datalabels';
import { CategorisationService } from 'src/app/services/project/standardisation/std-one/categorisation/categorisation.service';
import { CategorisationModel, CategorisationStasticsModel, CategorisationTrialMarkModel } from 'src/app/model/project/standardisation/std-one/categorisaton/categorisation';
import { QuestionAnnotatorComponent } from 'src/app/modules/projects/marking-player/question-annotator/question-annotator.component';
import { Router, ActivatedRoute } from '@angular/router';
import { BaseChartDirective } from 'ng2-charts';
import { RecommendationService } from 'src/app/services/project/standardisation/std-one/recommendation/recommendation.service';
import { UnrecommandedScript } from 'src/app/model/project/standardisation/std-one/recommendation/recommendation-model';
import { first } from 'rxjs';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';


@Component({
  templateUrl: './categorisation-pool.component.html',
  styleUrls: ['./categorisation-pool.component.css']
})
export class CategorisationPoolComponent implements OnInit {
  Ispause: any;
  constructor(
    private categorisationService: CategorisationService,
    public Alert: AlertService,
    private dialog: MatDialog,
    public commonService: CommonService,
    public translate: TranslateService,
    public qigservice: QigService,
    private route: Router,
    private activatedRoute: ActivatedRoute,
    private reommandservice: RecommendationService
  ) {
  }

  panelOpenState = false;
  selectedPoolType = '0';
  activeQig!: QigUserModel;
  pooltypes: any;

  stasticsloading: boolean = false;
  scriptsloading: boolean = false;
  scriptsloadingdata!: CategorisationModel[];
  trialmrkloading: boolean = false;
  trialmrkloaddata!: CategorisationTrialMarkModel[];
  expandedscript!: CategorisationModel | null;
  IsS1Available: boolean = true;
  intMessages: any = {
    CatSuccss: '',
    ScrAlrInQa: '',
    SelFinalize: '',
    SelPlType: '',
    CatError: '',
    uncatsucc: '',
    RecCnfirm: '',
    RecCatOthCnfm: '',
    Markingprocesspaused: '',
    stdscriptcompted: '',
    Markingprocessclosure: ''
  };

  qigsummary!: CategorisationStasticsModel;
  categorisationScripts!: CategorisationModel[];
  trialMarkedScript!: CategorisationTrialMarkModel[];
  IsS1Completed: any;
  IsClosure: any;
  qigId!: number;
  ScriptSearchValue: string = '';
  FiltercategorisationScripts: any;
  s1closure: boolean = false;
  qigpause: boolean = false;

  ngOnInit(): void {
    this.scriptsloadingdata = [{}, {}, {}] as CategorisationModel[];
    this.trialmrkloaddata = [{}, {}, {}] as CategorisationTrialMarkModel[];
    this.textIntenationalization();
    this.pooltypes = [
      {
        Id: 5,
        Text: 'unctsct',
        Value: ScriptCategorizationPoolType.None,
        Selected: true,
      },
      {
        Id: 4,
        Text: 'bncscpt',
        Value: ScriptCategorizationPoolType.BenchMarkScript,
        Selected: true,
      },
      {
        Id: 2,
        Text: 'stdscpt',
        Value: ScriptCategorizationPoolType.StandardizationScript,
        Selected: true,
      },
      {
        Id: 3,
        Text: 'adstscpt',
        Value: ScriptCategorizationPoolType.AdditionalStandardizationScript,
        Selected: true,
      },
    ];
    this.ScriptSearchValue = '';
  }

  private textIntenationalization() {
    this.translate.get('cat.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });

    this.translate.get('cat.pgedesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });

    this.translate.get('cat.catsuccs').subscribe((translated: string) => {
      this.intMessages.CatSuccss = translated;
    });
    this.translate.get('cat.scinqa').subscribe((translated: string) => {
      this.intMessages.ScrAlrInQa = translated;
    });
    this.translate.get('cat.selasfnl').subscribe((translated: string) => {
      this.intMessages.SelFinalize = translated;
    });
    this.translate.get('cat.selpltpe').subscribe((translated: string) => {
      this.intMessages.SelPlType = translated;
    });
    this.translate.get('cat.caterr').subscribe((translated: string) => {
      this.intMessages.CatError = translated;
    });
    this.translate.get('cat.uncatsucc').subscribe((translated: string) => {
      this.intMessages.uncatsucc = translated;
    });
    this.translate.get('cat.catcnfm').subscribe((translated: string) => {
      this.intMessages.RecCnfirm = translated;
    });
    this.translate.get('cat.RecCatOthCnfm').subscribe((translated: string) => {
      this.intMessages.RecCatOthCnfm = translated;
    });
    this.translate.get('Std.SetUp.Markingprocessclosure').subscribe((translated: string) => {
      this.intMessages.Markingprocessclosure = translated;
    });

    this.translate
      .get('Std.SetUp.Markingprocesspaused')
      .subscribe((translated: string) => {
        this.intMessages.Markingprocesspaused = translated;
      });
    this.translate
      .get('cat.stdscriptcompted')
      .subscribe((translated: string) => {
        this.intMessages.stdscriptcompted = translated;
      });
  }

  @ViewChild(BaseChartDirective) chart!: BaseChartDirective;

  public barChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      y: {
        beginAtZero: true,
      },
    },
    plugins: {
      legend: {
        display: false,
        position: 'bottom',
      },
      datalabels: {
        anchor: 'center',
        align: 'center',
      },
    },
  };
  public barChartType: ChartType = 'bar';
  public barChartPlugins = [DataLabelsPlugin];

  public barChartData: ChartData<'bar'> = {
    labels: [['Categorisation']],
    datasets: [
      { data: [0], label: 'Trial Marked Scripts' },
      { data: [0], label: 'Categorised Scripts' },
    ],
  };

  getQigDetails(selectedqig: QigUserModel) {
    this.Alert.clear();
    if (selectedqig != null && selectedqig.QigId > 0) {
      this.clearfilter();
      this.IsS1Available = selectedqig.IsS1Available;
      this.trialMarkedScript = [];
      this.activeQig = selectedqig;
      this.activatedRoute.params.subscribe(params => {
        let QigId = params['qigid'];
        this.route.navigateByUrl(this.route.url.replace(QigId, this.activeQig.QigId.toString()));
      });
      this.Getqigworkflowtracking(this.activeQig.QigId);
      this.initCategorise();
      this.collapseallscript();
      this.ScriptSearchValue = '';
    }
  }
  Getqigworkflowtracking(QIGID: number) {
    this.qigservice
      .Getqigworkflowtracking(this.activeQig.QigId, AppSettingEntityType.QIG)
      .subscribe((data) => {
        if (data != null && data != undefined) {
          let WorkFlowStatusTracking = data;

          this.IsClosure = WorkFlowStatusTracking.filter((a: { WorkflowStatusCode: WorkflowStatus; ProcessStatus: WorkflowProcessStatus; }) =>
            a.WorkflowStatusCode == WorkflowStatus.Closure &&
            a.ProcessStatus == WorkflowProcessStatus.Closure);

          this.Ispause = WorkFlowStatusTracking.filter((a: { WorkflowStatusCode: WorkflowStatus; ProcessStatus: WorkflowProcessStatus; }) =>
            a.WorkflowStatusCode == WorkflowStatus.Pause &&
            a.ProcessStatus == WorkflowProcessStatus.OnHold);

          this.IsS1Completed = WorkFlowStatusTracking.filter((a: { WorkflowStatusCode: WorkflowStatus; ProcessStatus: WorkflowProcessStatus; }) =>
            a.WorkflowStatusCode == WorkflowStatus.Standardization_1 &&
            a.ProcessStatus == WorkflowProcessStatus.Completed);


          this.s1closure = this.IsS1Completed.length > 0 ? true : false;

          this.qigpause = this.Ispause.length > 0 ? true : false;

          if (this.IsClosure.length > 0) {
            this.Alert.warning(this.intMessages.Markingprocessclosure + '<br>' + 'Remarks : ' + this.IsClosure[0].Remark + '.');
          }

          else if (this.Ispause.length > 0) {
            this.translate
              .get('Std.QuaAsseCrea.Qualifyingpaused')
              .subscribe((translated: string) => {
                this.Alert.warning(translated + '<br>' + 'Remarks : ' + this.Ispause[0].Remark + '.');
              });
          }

          else if (this.IsS1Completed.length > 0) {
            this.translate
              .get('Std.QuaAsseCrea.s1closure')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          }
        }
      });
  }

  private initCategorise() {
    this.translate.get('cat.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });

    this.translate.get('cat.pgedesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    if (this.activeQig != null && this.activeQig.QigId > 0) {
      this.getscriptreponses(this.activeQig.QigId);
      this.getCategorisationScripts(this.activeQig.QigId, this.filterparam);
    }
  }

  BtnQualifyingAssessment() {
    this.route.navigateByUrl('projects/configurations/s2-s3');
  }

  getscriptreponses(qigId: number): void {
    this.stasticsloading = true;
    this.categorisationService.getCategorisationStatistics(qigId).subscribe(
      (data: CategorisationStasticsModel) => {
        this.qigsummary = data;
        this.barChartData.datasets = [
          { data: [0], label: 'Trial Marked Scripts' },
          { data: [0], label: 'Categorised Scripts' },
        ];

        if (data != null && data != undefined) {
          this.barChartData.datasets = [
            {
              data: [data.TrialMarkedScript],
              label: 'Trial Marked Scripts', barPercentage: 0.5
            },
            {
              data: [data.CategorisedScript],
              label: 'Categorised Scripts', barPercentage: 0.5
            },
          ];
        }

        this.chart?.update();

      },
      (err: any) => {
        throw err;
      },
      () => {
        this.stasticsloading = false;
        this.ScriptSearchValue = '';
      }
    );
  }

  getCategorisationScripts(qigId: number, poolTypes: Array<number> = []): void {
    this.scriptsloading = true;
    this.categorisationService
      .getCategorisationScripts(qigId,this.ScriptSearchValue, poolTypes)
      .subscribe(
        (data: CategorisationModel[]) => {
          this.categorisationScripts = data;
          this.FiltercategorisationScripts = data;
          if (this.expandedscript != null && this.expandedscript != undefined) {
            this.getTrialMarkedScript(
              this.activeQig.QigId,
              this.expandedscript
            );
          }
        },
        (err: any) => {
          throw err;
        },
        () => {
          this.scriptsloading = false;
        }
      );
  }
  IsQigPaused: boolean = false;
  getTrialMarkedScript(qigId: number, catModel: CategorisationModel) {
    this.trialMarkedScript = [];
    this.trialmrkloading = true;
    this.categorisationService
      .getTrialMarkedScript(qigId, catModel.ScriptId)
      .subscribe(
        (data: CategorisationTrialMarkModel[]) => {
          this.trialMarkedScript = data;
        },
        (err: any) => {
          throw err;
        },
        () => {
          this.trialmrkloading = false;
          this.ScriptSearchValue = '';
        }
      );
  }

  SetAsDefinitive(
    trialmark: CategorisationTrialMarkModel,
    trialmarks: CategorisationTrialMarkModel[],
    isInQfAsses: boolean,
    pooltype: number
  ) {
    if (!isInQfAsses && pooltype <= 0 && !this.IsQigPaused) {
      trialmarks.forEach((elem) => {
        if (elem.MarkedBy == trialmark.MarkedBy) {
          elem.SelectAsDefinitive = !trialmark.SelectAsDefinitive;
        } else {
          elem.SelectAsDefinitive = false;
        }
      });
      this.ScriptSearchValue = '';
    }
  }

  filterparam: Array<number> = [];
  selectPoolFilter() {
    this.filterparam = [];
    this.pooltypes.forEach((pElement: any) => {
      if (pElement.Selected) {
        this.filterparam.push(pElement.Value);
      }
    });
  }

  applyfilter() {
    //  if (this.filterparam.length > 0) {
    this.collapseallscript();
    this.getCategorisationScripts(this.activeQig.QigId, this.filterparam);
    // }

  }

  clearfilter() {
    this.filterparam = [];
    this.pooltypes.forEach((pElement: any) => {
      pElement.Selected = false;
    });
  }

  private collapseallscript() {
    this.expandedscript = null;
    this.categorisationScripts?.forEach((elem) => {
      elem.PanelOpenState = false;
    });
  }

  trailmarking(script: CategorisationModel, markingdata: IRecommedData) {
    this.categorisationService
      .IsQigInQualifying(this.activeQig.QigId, script.ScriptId)
      .subscribe(
        (data: boolean) => {
          if (data || script.PoolType > 0) {
            markingdata.IsViewMode = true;
          }
          markingdata.QigId = this.activeQig.QigId;
          const dialogRef = this.dialog.open(QuestionAnnotatorComponent, {
            data: markingdata,
            panelClass: 'fullviewpop',
          });
          dialogRef.afterClosed().subscribe((result) => {
            this.initCategorise();
          });
        },
        (err: any) => {
          throw err;
        },
        () => {
          this.scriptsloading = false;
          this.ScriptSearchValue = '';
        }
      );
  }

  categorisationredirect(script: CategorisationModel) {
    this.qigId = this.activeQig.QigId;
    let scriptId = script.ScriptId;

    this.qigservice
      .Getqigworkflowtracking(this.activeQig.QigId, AppSettingEntityType.QIG)
      .subscribe((data) => {
        if (data != null && data != undefined) {
          let WorkFlowStatusTracking = data;
          this.Ispause = WorkFlowStatusTracking.filter((a: { WorkflowStatusCode: WorkflowStatus; ProcessStatus: WorkflowProcessStatus; }) =>
            a.WorkflowStatusCode == WorkflowStatus.Pause &&
            a.ProcessStatus == WorkflowProcessStatus.OnHold);

          this.IsS1Completed = WorkFlowStatusTracking.findIndex(
            (a: {
              WorkflowStatusCode: WorkflowStatus;
              ProcessStatus: WorkflowProcessStatus;
            }) =>
              a.WorkflowStatusCode == WorkflowStatus.Standardization_1 &&
              a.ProcessStatus == WorkflowProcessStatus.Completed
          );

          if (this.IsS1Completed >= 0 && script.PoolType <= 0) {
            this.Alert.clear();
            this.Getqigworkflowtracking(this.activeQig.QigId);
            this.initCategorise();
            this.collapseallscript();

            this.translate
              .get('Std.QuaAsseCrea.s1closure')
              .subscribe((translated: string) => {

                this.Alert.warning(translated);

              });
          }
          else {
            this.route.navigate(['/projects/configurations/categorisation', this.qigId, scriptId]);
          }
        }
      });
    this.ScriptSearchValue = '';
  }

  SearchScript() {
    this.getCategorisationScripts(this.activeQig.QigId,this.filterparam);
  }

  UnrecommandScript(scriptId: number) {

    const confirmDialogue = this.dialog.open(ConfirmationDialogComponent, {
      data: { message: "Do you want to unrecommend this script" }
    });

    confirmDialogue.afterClosed().subscribe(result => {

      if (result) {

        var obj = new UnrecommandedScript();

        obj.Qigid = this.activeQig.QigId;
        obj.ScriptId = scriptId;

        this.reommandservice.UnrecommandedScripts(obj).pipe(first())
          .subscribe({
            next: (data: any) => {
             
              if (data == "SU001") {
                this.Alert.success("Script unrecommended  successfully");
              }
              if (data == "CTGRTN") {
                this.Alert.success("Script is already categorized");
              }
              if(data == "Unrecmended"){
                this.Alert.warning("This Script is already Unrecommended.");
              }
              if (data == "PAUSE") {
              this.Getqigworkflowtracking(this.activeQig.QigId);
              }
              if (data == "S1COMPLETED") {
                this.translate
                .get('Std.QuaAsseCrea.s1closure')
                .subscribe((translated: string) => {
                  this.Alert.warning(translated);
                });
              }

               this.getCategorisationScripts(this.activeQig.QigId, this.filterparam);
               this.getscriptreponses(this.activeQig.QigId);
            },
            error: (err: any) => {
              throw err;
            },
            complete: () => {
              console.log(scriptId);
            },
          });
      }
    });
  }
}
