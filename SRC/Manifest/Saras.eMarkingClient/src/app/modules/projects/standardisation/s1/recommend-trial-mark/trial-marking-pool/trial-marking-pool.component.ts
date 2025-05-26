import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ChartConfiguration, ChartData, ChartType } from 'chart.js';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { QualifyingAssessmentService } from 'src/app/services/standardisation/qualifying-assessment.service';
import { IRecommedData } from 'src/app/model/standardisation/recommendation';
import { MatDialog } from '@angular/material/dialog';
import { QigUserModel } from 'src/app/model/project/qig';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import {
  WorkflowProcessStatus,
  WorkflowStatus,
} from 'src/app/model/common-model';
import { BaseChartDirective } from 'ng2-charts';
import DatalabelsPlugin from 'chartjs-plugin-datalabels';
import { TrialmarkingPoolService } from 'src/app/services/project/standardisation/std-one/trial-marking/trial-marking-pool.service';
import { QigService } from 'src/app/services/project/qig.service';
import { QuestionAnnotatorComponent } from 'src/app/modules/projects/marking-player/question-annotator/question-annotator.component';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { UnrecommandedScript } from 'src/app/model/project/standardisation/std-one/recommendation/recommendation-model';
import { RecommendationService } from 'src/app/services/project/standardisation/std-one/recommendation/recommendation.service';
import { first } from 'rxjs/operators';
import { TrailmarkingModel } from 'src/app/model/project/standardisation/std-one/trial-marking/trial-marking-pool-model';

@Component({
  templateUrl: './trial-marking-pool.component.html',
  styleUrls: ['./trial-marking-pool.component.css'],
})
export class TrialMarkingPoolComponent implements OnInit {
  IsS1Completed: any;

  constructor(
    public translate: TranslateService,
    public trialmarkingpoolservice: TrialmarkingPoolService,
    public qigservice: QigService,
    public qualifyingassessmentservice: QualifyingAssessmentService,
    public commonService: CommonService,
    public Alert: AlertService,
    public dialog: MatDialog,
    private reommandservice: RecommendationService
  ) {}

  @ViewChildren(BaseChartDirective) charts!: QueryList<BaseChartDirective>;
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
  public barChartPlugins = [DatalabelsPlugin];

  public barChartData: ChartData<'bar'> = {
    labels: [['Trial Marking']],
    datasets: [
      { data: [0], label: 'Recommended Scripts' },
      { data: [0], label: 'Trial Marked Scripts' },
    ],
  };

  public pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'bottom',
        onClick: () => {
          return false;
        },
      },
    },
  };
  public pieChartData: ChartData<'pie', number[], string | string[]> = {
    labels: [
      [' Standardised Scripts'],
      [' Add. Standardised Scripts'],
      [' Benchmarked Scripts'],
    ],
    datasets: [
      {
        data: [0, 0, 0],
      },
    ],
  };
  public pieChartType: ChartType = 'pie';
  public pieChartPlugins = [DatalabelsPlugin];

  Qigs: any = [];
  QigScriptsData: any;
  BandInfos: any = [];
  selected: any[] = [];
  SelectedQigId!: number;
  activeQig!: QigUserModel;
  noQig!: boolean;
  errMessage: string = '';
  FilterValue: number = 0;
  IsS1Available: boolean = true;
  Ispause: number = -1;
  IsClosure: number = -1;
  S1completed: number = -1;
  Markingprocesspaused: string = '';
  Markingprocessclosure: string = '';
  S1cmpltdAlert: string = '';
  qigloading: boolean = true;
  countsloading: boolean = true;
  ScriptSearchValue: string = '';
  FilterQigScriptsData: any;
  s1closure: boolean = false;
  qigpause: boolean = false;

  qigpausemessage!: string;

  ngOnInit(): void {
    this.setInternationalisation();
    this.translate
      .get('Std.SetUp.Markingprocesspaused')
      .subscribe((translated: string) => {
        this.Markingprocesspaused = translated;
      });
    this.translate
      .get('Std.QuaAsseCrea.s1closure')
      .subscribe((translated: string) => {
        this.S1cmpltdAlert = translated;
      });
    this.translate
      .get('Std.SetUp.Markingprocessclosure')
      .subscribe((translated: string) => {
        this.Markingprocessclosure = translated;
      });
    this.ScriptSearchValue = '';
  }

  setInternationalisation() {
    this.translate
      .get('Std.TrialMark.Title')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });
    this.translate
      .get('Std.TrialMark.PageDesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
  }

  getQigDetails(selectedqig: QigUserModel, filtervalue: number) {
    this.qigloading = false;
    this.Ispause = -1;
    this.IsClosure = -1;
    if (selectedqig != null && selectedqig.QigId > 0) {
      this.activeQig = selectedqig;
      this.IsS1Available = selectedqig.IsS1Available;

      this.FilterValue = filtervalue;
      var SelectedFilter;
      if (
        this.selected.find((a) => a == 2) &&
        this.selected.find((a) => a == 1)
      ) {
        SelectedFilter = '0';
      } else {
        SelectedFilter = this.selected.length == 0 ? '0' : this.selected.join();
      }
      this.countsloading = true;
      this.trialmarkingpoolservice
        .getQIGScriptForTrialMark(this.activeQig.QigId, SelectedFilter,this.ScriptSearchValue)
        .subscribe((data) => {
          if (data != null) {
            this.QigScriptsData = data;
            this.FilterQigScriptsData = data.TrialMarkedScripts;

            if (
              this.QigScriptsData.NoOfRecommendedScripts == 0 &&
              this.QigScriptsData.NoOfTrialMarkedScripts == 0
            ) {
              this.barChartData.datasets = [
                { data: [0], label: 'Recommended Scripts' },
                { data: [0], label: 'Trial Marked Scripts' },
              ];
            } else {
              this.barChartData.datasets = [
                {
                  data: [this.QigScriptsData.NoOfRecommendedScripts],
                  label: 'Recommended Scripts',
                  barPercentage: 0.4,
                },
                {
                  data: [this.QigScriptsData.NoOfTrialMarkedScripts],
                  label: 'Trial Marked Scripts',
                  barPercentage: 0.4,
                },
              ];
            }
            this.pieChartData.datasets = [
              {
                data: [
                  this.QigScriptsData.StandardizationScriptsCount,
                  this.QigScriptsData.AdditionalStdScriptsCount,
                  this.QigScriptsData.BenchmarkScriptsCount,
                ],
              },
            ];
 

            this.qigservice
              .Getqigworkflowtracking(
                this.activeQig.QigId,
                AppSettingEntityType.QIG
              )
              .subscribe((res) => {
                this.countsloading = false;
                this.charts?.forEach((a) => {
                  a.update();
                });
                let WorkFlowStatusTracking = res;
                if (WorkFlowStatusTracking != null) {
                  var Qigpausedata = WorkFlowStatusTracking.filter(
                    (a: {
                      WorkflowStatusCode: WorkflowStatus;
                      ProcessStatus: WorkflowProcessStatus;
                    }) =>
                      a.WorkflowStatusCode == WorkflowStatus.Pause &&
                      a.ProcessStatus == WorkflowProcessStatus.OnHold
                  );

                  var Qigclosuredata = WorkFlowStatusTracking.filter(
                    (a: {
                      WorkflowStatusCode: WorkflowStatus;
                      ProcessStatus: WorkflowProcessStatus;
                    }) =>
                      a.WorkflowStatusCode == WorkflowStatus.Closure &&
                      a.ProcessStatus == WorkflowProcessStatus.Closure
                  );

                  var s1closuredata = WorkFlowStatusTracking.filter(
                    (a: {
                      WorkflowStatusCode: WorkflowStatus;
                      ProcessStatus: WorkflowProcessStatus;
                    }) =>
                      a.WorkflowStatusCode ==
                        WorkflowStatus.Standardization_1 &&
                      a.ProcessStatus == WorkflowProcessStatus.Completed
                  );

                  this.s1closure = s1closuredata.length > 0 ? true : false;

                  this.qigpause = Qigpausedata.length > 0 ? true : false;

                  this.IsClosure = WorkFlowStatusTracking.findIndex(
                    (a: {
                      WorkflowStatusCode: WorkflowStatus;
                      ProcessStatus: WorkflowProcessStatus;
                    }) =>
                      a.WorkflowStatusCode == WorkflowStatus.Closure &&
                      a.ProcessStatus == WorkflowProcessStatus.Closure
                  );
                  this.Ispause = WorkFlowStatusTracking.findIndex(
                    (a: {
                      WorkflowStatusCode: WorkflowStatus;
                      ProcessStatus: WorkflowProcessStatus;
                    }) =>
                      a.WorkflowStatusCode == WorkflowStatus.Pause &&
                      a.ProcessStatus == WorkflowProcessStatus.OnHold
                  );
                }
                this.IsS1Completed = WorkFlowStatusTracking.findIndex(
                  (a: {
                    WorkflowStatusCode: WorkflowStatus;
                    ProcessStatus: WorkflowProcessStatus;
                  }) =>
                    a.WorkflowStatusCode == WorkflowStatus.Standardization_1 &&
                    a.ProcessStatus == WorkflowProcessStatus.Completed
                );

                if (this.IsClosure >= 0) {
                  this.Alert.clear();
                  this.Alert.warning(
                    this.Markingprocessclosure +
                      '<br>' +
                      'Remarks : ' +
                      Qigclosuredata[0].Remark +
                      '.'
                  );
                } else if (this.Ispause >= 0) {
                  this.qigpausemessage =
                    this.Markingprocesspaused +
                    '<br>' +
                    'Remarks : ' +
                    Qigpausedata[0].Remark +
                    '.';
                  this.Alert.clear();
                  this.Alert.warning(
                    this.Markingprocesspaused +
                      '<br>' +
                      'Remark : ' +
                      Qigpausedata[0].Remark +
                      '.'
                  );
                } else if (this.IsS1Completed >= 0) {
                  this.translate
                    .get('Std.QuaAsseCrea.s1closure')
                    .subscribe((translated: string) => {
                      this.Alert.clear();
                      this.Alert.warning(translated);
                    });
                }
              });
          }
          this.setInternationalisation();
        });
    }
  }

  ViewBandInfo(ScriptId: number) {
    this.trialmarkingpoolservice
      .getScriptQuestionBandInformation(ScriptId)
      .subscribe((data) => {
        this.BandInfos = data;
      });
  }

  SetfilterValue(event: any, optionvalue: any) {
    if (event.checked) {
      this.selected.push(optionvalue);
    } else {
      this.selected = this.selected.filter((item) => item !== optionvalue);
    }
  }
  filterTrialMarkScripts() {
    this.getQigDetails(this.activeQig, 1);
  }
  playerOpening: boolean = false;
  UpdateTrialMarkWorkFlowStatus(TrialMarkedScripts: TrailmarkingModel) {
    if (!this.playerOpening) {
      var markingdata: IRecommedData = {
        ProjectId: 0,
        ScriptId: TrialMarkedScripts.ScriptId,
        ScriptName: '',
        QigId: 0,
        QigName: '',
        IsViewMode: false,
        Status: 0,
        RcType: 0,
        PhaseStatusTrackingId: 0,
      };
      this.playerOpening = true;
      this.qigservice
        .Getqigworkflowtracking(this.activeQig.QigId, AppSettingEntityType.QIG)
        .subscribe({
          next: (data: any) => {
            this.playerOpening = true;
            let WorkFlowStatusTracking = data;
            if (WorkFlowStatusTracking != null) {
              var Qigpausedata = WorkFlowStatusTracking.filter(
                (a: {
                  WorkflowStatusCode: WorkflowStatus;
                  ProcessStatus: WorkflowProcessStatus;
                }) =>
                  a.WorkflowStatusCode == WorkflowStatus.Pause &&
                  a.ProcessStatus == WorkflowProcessStatus.OnHold
              );

              var Qigclosuredata = WorkFlowStatusTracking.filter(
                (a: {
                  WorkflowStatusCode: WorkflowStatus;
                  ProcessStatus: WorkflowProcessStatus;
                }) =>
                  a.WorkflowStatusCode == WorkflowStatus.Closure &&
                  a.ProcessStatus == WorkflowProcessStatus.Closure
              );

              this.IsClosure = WorkFlowStatusTracking.findIndex(
                (a: {
                  WorkflowStatusCode: WorkflowStatus;
                  ProcessStatus: WorkflowProcessStatus;
                }) =>
                  a.WorkflowStatusCode == WorkflowStatus.Closure &&
                  a.ProcessStatus == WorkflowProcessStatus.Closure
              );
              this.Ispause = WorkFlowStatusTracking.findIndex(
                (a: {
                  WorkflowStatusCode: WorkflowStatus;
                  ProcessStatus: WorkflowProcessStatus;
                }) =>
                  a.WorkflowStatusCode == WorkflowStatus.Pause &&
                  a.ProcessStatus == WorkflowProcessStatus.OnHold
              );
              this.S1completed = WorkFlowStatusTracking.findIndex(
                (a: {
                  WorkflowStatusCode: WorkflowStatus;
                  ProcessStatus: WorkflowProcessStatus;
                }) =>
                  a.WorkflowStatusCode == WorkflowStatus.Standardization_1 &&
                  a.ProcessStatus == WorkflowProcessStatus.Completed
              );
            }
            if (this.IsClosure >= 0) {
              this.playerOpening = false;
              this.Alert.warning(
                this.Markingprocessclosure +
                  '<br>' +
                  'Remarks : ' +
                  Qigclosuredata[0].Remark +
                  '.'
              );
            } else if (this.Ispause >= 0) {
              this.playerOpening = false;
              this.Alert.warning(
                this.Markingprocesspaused +
                  '<br>' +
                  'Remarks : ' +
                  Qigpausedata[0].Remark +
                  '.'
              );
            } else if (this.S1completed >= 0) {
              var SelectedFilter;
              if (
                this.selected.find((a) => a == 2) &&
                this.selected.find((a) => a == 1)
              ) {
                SelectedFilter = '0';
              } else {
                SelectedFilter =
                  this.selected.length == 0 ? '0' : this.selected.join();
              }
              var filtervalue = 0;
              this.playerOpening = true;
              this.trialmarkingpoolservice
                .getQIGScriptForTrialMark(this.activeQig.QigId, SelectedFilter,this.ScriptSearchValue)
                .subscribe({
                  next: (res: any) => {
                    this.playerOpening = true;
                    if (res != null) {
                      this.Alert.clear();
                      this.QigScriptsData = res;
                      if (
                        this.QigScriptsData.NoOfRecommendedScripts == 0 &&
                        this.QigScriptsData.NoOfTrialMarkedScripts == 0
                      ) {
                        this.barChartData.datasets = [
                          { data: [0], label: 'Recommended Scripts' },
                          { data: [0], label: 'Trial Marked Scripts' },
                        ];
                      } else {
                        this.barChartData.datasets = [
                          {
                            data: [this.QigScriptsData.NoOfRecommendedScripts],
                            label: 'Recommended Scripts',
                            barPercentage: 0.4,
                          },
                          {
                            data: [this.QigScriptsData.NoOfTrialMarkedScripts],
                            label: 'Trial Marked Scripts',
                            barPercentage: 0.4,
                          },
                        ];
                      }
                      this.pieChartData.datasets = [
                        {
                          data: [
                            this.QigScriptsData.StandardizationScriptsCount,
                            this.QigScriptsData.AdditionalStdScriptsCount,
                            this.QigScriptsData.BenchmarkScriptsCount,
                          ],
                        },
                      ];
                      this.charts?.forEach((a) => {
                        a.update();
                      });
                    }
                  },
                  error: (err: any) => {
                    throw err;
                  },
                  complete: () => {
                    this.playerOpening = false;
                    this.ScriptSearchValue = '';
                  },
                });
              this.Alert.warning(this.S1cmpltdAlert);
            } else {
              var filterddata = this.QigScriptsData.TrialMarkedScripts.filter(
                (li: { IsCategorized: boolean; ScriptId: number }) =>
                  li.IsCategorized && li.ScriptId == TrialMarkedScripts.ScriptId
              );
              this.playerOpening = true;
              this.trialmarkingpoolservice
                .UpdateTrialMarkWorkFlowStatus(TrialMarkedScripts)
                .subscribe({
                  next: (resp: any) => { 
                    this.playerOpening = true;
                    if (resp == 'P001') {
                      this.Alert.clear();
                      if (filterddata.length > 0 || this.Ispause >= 0) {
                        this.Alert.clear();
                        this.Alert.warning(
                          'This script is already categorized you cannot view the script.'
                        );
                        this.playerOpening = false;
                      } else {
                        this.RedirectToTrialMarking(markingdata);
                      }
                    } else if (resp == 'Unrecmended') {
                      this.Alert.warning(
                        'This script is already unrecommended'
                      );
                      this.getQigDetails(this.activeQig, this.FilterValue);
                    } else if (resp == 'CTGRTN') {
                      this.Alert.warning('This script is already categorized');
                      this.getQigDetails(this.activeQig, this.FilterValue);
                    }
                  },
                  error: (err: any) => {
                    throw err;
                  },
                  complete: () => {
                    this.playerOpening = false;
                    this.ScriptSearchValue = '';
                  },
                });
            }
          },
          error: (err: any) => {
            this.playerOpening = false;
            throw err;
          },
        });
    }
  }
  RedirectToTrialMarking(markingdata: IRecommedData) {
    markingdata.QigId = this.activeQig.QigId;
    const dialogRef = this.dialog.open(QuestionAnnotatorComponent, {
      data: markingdata,
      panelClass: 'fullviewpop',
    });
    dialogRef.afterClosed().subscribe(() => {
      this.setInternationalisation();
      this.getQigDetails(this.activeQig, this.FilterValue);
    });
    this.playerOpening = false;
    this.ScriptSearchValue = '';
  }

  SearchScript() {
    this.getQigDetails(this.activeQig, 1);
  }

  UnrecommandScript(scriptId: number) {
    const confirmDialogue = this.dialog.open(ConfirmationDialogComponent, {
      data: { message: 'Do you want to unrecommend this script' },
    });

    confirmDialogue.afterClosed().subscribe((result) => {
      if (result) {
        var obj = new UnrecommandedScript();

        obj.Qigid = this.activeQig.QigId;
        obj.ScriptId = scriptId;

        this.reommandservice
          .UnrecommandedScripts(obj)
          .pipe(first())
          .subscribe({
            next: (data: any) => {
              if (data == 'SU001') {
                this.Alert.success('Script unrecommended  successfully');
              } else if (data == 'TRMARKG') {
                this.Alert.warning('Script is already trialmarked');
              } else if (data == 'CTGRTN') {
                this.Alert.warning('Script is already categorized');
              } else if (data == 'PAUSE') {
                this.getQigDetails(this.activeQig, this.FilterValue);
              } else if (data == 'S1COMPLETED') {
                this.translate
                  .get('Std.QuaAsseCrea.s1closure')
                  .subscribe((translated: string) => {
                    this.Alert.clear();
                    this.Alert.warning(translated);
                  });
              }
              if(data == "Unrecmended"){
                this.Alert.warning("This Script is already Unrecommended.");
              }
              this.getQigDetails(this.activeQig, this.FilterValue);
            },
            error: (err: any) => {
              throw err;
            },
          });
      }
    });
  }
}
