import {
  Component,
  OnInit,
  PipeTransform,
  Pipe,
} from '@angular/core';
import { LiveMarkingService } from 'src/app/services/project/live-marking/live-marking.service';
import { first, takeWhile } from 'rxjs/operators';
import {
  LiveMarkingModel,
  Livescripts,
  ClsLivescript,
} from 'src/app/model/project/live-marking/live-marking-model';
import { IRecommedData } from 'src/app/model/standardisation/recommendation';
import { QuestionAnnotatorComponent } from '../../marking-player/question-annotator/question-annotator.component';
import { MatDialog } from '@angular/material/dialog';
import {
  QigScriptModule,
  WorkflowStatus,
  WorkflowProcessStatus,
} from 'src/app/model/common-model';
import { timer } from 'rxjs';
import { QigService } from 'src/app/services/project/qig.service';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import { WorkflowStatusTrackingModel } from 'src/app/model/standardisation/Assessment';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { AlertService } from 'src/app/services/common/alert.service';

import {
  DateAdapter,
  MAT_DATE_LOCALE,
  MAT_DATE_FORMATS,
} from '@angular/material/core';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { DatePipe } from '@angular/common';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { AuthService } from 'src/app/services/auth/auth.service';
import { NotificationService } from 'src/app/services/common/notification.service';

export const MY_FORMATS = {
  parse: {
    dateInput: 'YYYY-MM-DD', //HH:mm:ss
  },
  display: {
    dateInput: 'DD-MM-yyyy', //HH:mm:ss
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'DD-MM-yyyy HH:mm:ss',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

@Component({
  selector: 'emarking-live-marking',
  templateUrl: './live-marking.component.html',
  styleUrls: ['./live-marking.component.css'],

  providers: [
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE],
    },
    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
    DatePipe,
  ],
})
export class LiveMarkingComponent implements OnInit {
  dateFormGroup!: FormGroup;
  public maxDate = new Date();
  LiveMarkingLoader: boolean = false;
  LiveMarkingModel!: LiveMarkingModel;
  QigId!: number;
  isTrue: boolean = false;
  selected: any[] = [];
  LtScript!: Livescripts[];
  isDownloadDisable: boolean = false;
  isGraceperiod: boolean = false;
  isLiveScript: boolean = false;
  isSubmitted: boolean = false;
  isReallocated: boolean = false;
  Tick: number = 0;
  seconds: any;
  minutes: any;
  timer!: Livescripts[];
  ScriptSearchValue: string = '';
  pickfromdate: Date = new Date();
  picktodate: Date = new Date();
  intMessages: any = {
    Markingprocesspaused: '',
    Alertmessages: '',
  };
  IsQigPause: boolean = false;
  WorkFlowStatusTracking: WorkflowStatusTrackingModel[] = [];
  IsSearched: boolean = false;
  QigName!: string;
  PhaseTrackingId: number = 0;

  constructor(
    private liveMarkingService: LiveMarkingService,
    public dialog: MatDialog,
    private qigservice: QigService,
    public translate: TranslateService,
    public commonService: CommonService,
    public Alert: AlertService,
    private router: ActivatedRoute,
    public datepipe: DatePipe,
    public route: Router,
    private authService: AuthService,
    public notificatonservice: NotificationService
  ) {}
  title!: string;
  arrtimer: any = [];
  serverdatetime: Date = new Date();
  ngOnInit(): void {
    this.notificatonservice.CurrentDateTime$.subscribe((msg) => {
      this.serverdatetime = msg;
      this.maxDate = this.serverdatetime;
    });

    this.QigId = this.router.snapshot.params['QigId'];
    this.QigName = this.router.snapshot.params['qig'];
    var curdate = new Date(this.serverdatetime);

    this.isTrue = true;

    this.textinternationalization();
    this.GetScripts(this.QigId, 1, curdate);

    this.isGraceperiod = false;
    this.isLiveScript = true;
    this.isSubmitted = false;
    this.isReallocated = false;

    this.ScriptSearchValue = '';
  }
  textinternationalization() {
    this.translate
      .get('LiveMarking.LvmrkngPageDesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });

    this.translate
      .get('LiveMarking.Lvmrkng')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
        this.title = translated;
      });
  }

  DownloadScripts() {
    this.CheckQigPause();
  }
  LiveScripts(poolid: number): any {
    this.activepooltab = poolid;
    this.isTrue = true;
    this.isGraceperiod = false;
    this.isLiveScript = true;
    this.isSubmitted = false;
    this.isReallocated = false;
    this.ScriptSearchValue = '';
    var curdate = new Date(this.serverdatetime);
    this.GetScripts(this.QigId, this.activepooltab, curdate);
  }

  GracePeriodScripts(poolid: number): any {
    this.activepooltab = poolid;
    this.isTrue = false;
    this.isGraceperiod = true;
    this.isLiveScript = false;
    this.isSubmitted = false;
    this.isReallocated = false;
    this.ScriptSearchValue = '';
    var curdate = this.serverdatetime;
    this.GetScripts(this.QigId, this.activepooltab, curdate);
  }

  SubmittedScripts(poolid: number): any {
    this.activepooltab = poolid;
    this.pickfromdate = this.serverdatetime;
    this.picktodate = this.serverdatetime;
    this.isTrue = false;
    this.isGraceperiod = false;
    this.isLiveScript = false;
    this.isSubmitted = true;
    this.isReallocated = false;
    this.ScriptSearchValue = '';
    var curdate = this.serverdatetime;
    this.GetScripts(this.QigId, this.activepooltab, curdate);
  }
  ReallocatedScripts(poolid: number): any {
    this.activepooltab = poolid;
    this.isReallocated = true;
    this.isTrue = false;
    this.isGraceperiod = false;
    this.isLiveScript = false;
    this.isSubmitted = false;
    this.ScriptSearchValue = '';
    var curdate = this.serverdatetime;
    this.GetScripts(this.QigId, this.activepooltab, curdate);
  }

  gracetimersubscribs!: any[];
  activepooltab: number = 0;
  GetScripts(QigId: number, pool: number, fDate: any, tDate: any = null) {
    this.LiveMarkingLoader = true;
    this.activepooltab = pool;
    this.textinternationalization();
    this.qigservice
      .Getqigworkflowtracking(this.QigId, AppSettingEntityType.QIG)
      .pipe(first())
      .subscribe({
        next: (wfdata: any) => {
          this.WorkFlowStatusTracking = wfdata;

          let wfstatus = this.WorkFlowStatusTracking.filter(
            (a) =>
              a.WorkflowStatusCode == WorkflowStatus.Pause &&
              a.ProcessStatus == WorkflowProcessStatus.OnHold
          );

          if (wfstatus.length > 0) {
            this.IsQigPause = true;

            this.translate
              .get('LiveMarking.QigPause')
              .subscribe((translated: string) => {
                this.intMessages.Markingprocesspaused =
                  translated + '<br>' + 'Remarks : ' + wfstatus[0].Remark + '.';
              });
            this.Alert.warning(this.intMessages.Markingprocesspaused);
          } else {
            this.IsQigPause = false;
          }

          this.LiveMarkingModel = {} as LiveMarkingModel;
          this.LiveMarkingModel.Livescripts = [{}, {}, {}] as Livescripts[];

          var obj = new ClsLivescript();
          if (tDate != null) {
            obj.QigID = QigId;
            obj.pool = pool;
            obj.FromDate = fDate;
            obj.ToDate = tDate;
          } else {
            obj.QigID = QigId;
            obj.pool = pool;
            obj.FromDate = fDate;
          }
          this.liveMarkingService
            .GetLiveScripts(obj)
            .pipe(first())
            .subscribe({
              next: (livsdata: any) => {
                this.textinternationalization();
                this.LiveMarkingModel = livsdata;
                this.LtScript = this.LiveMarkingModel.Livescripts;
                if (this.LiveMarkingModel.Livescripts != null) {
                  if (obj.pool == 2 || obj.pool == 4) {
                    if (
                      this.gracetimersubscribs != null &&
                      this.gracetimersubscribs != undefined &&
                      this.gracetimersubscribs.length > 0
                    ) {
                      this.gracetimersubscribs.forEach((subs) => {
                        subs.unsubscribe();
                      });
                    }
                    if (
                      this.gracetimersubscribs == null ||
                      this.gracetimersubscribs == undefined
                    ) {
                      this.gracetimersubscribs = [];
                    }
                    this.LiveMarkingModel.Livescripts.forEach((element) => {
                      this.gracetimersubscribs.push(
                        timer(0, 1000)
                          .pipe(takeWhile(() => element.Seconds > -1))
                          .subscribe(() => {
                            --element.Seconds;
                            if (
                              element.Seconds > 0 &&
                              element.Seconds < 1 &&
                              (element.status == 3 || element.status == 7)
                            ) {
                              if (element.status != 7) {
                                this.LiveMarkingModel.SubmittedScript =
                                  this.LiveMarkingModel.SubmittedScript + 1;
                              }
                              this.LiveMarkingModel.GraceperiodScript =
                                this.LiveMarkingModel.GraceperiodScript - 1;
                              if (this.LiveMarkingModel.GraceperiodScript < 1) {
                                this.LiveMarkingModel.GraceperiodScript = 0;
                              }

                              this.Movescripttosubmit(element);
                            }
                          })
                      );
                    });
                  }
                }
              },
              error: (er: any) => {
                throw er;
              },
              complete: () => {
                this.LiveMarkingLoader = false;
              },
            });
        },
        error: (exa: any) => {
          throw exa;
        },
      });
  }

  private Movescripttosubmit(script: Livescripts) {
    let index = this.LiveMarkingModel.Livescripts.findIndex(
      (d) => d.ScriptId === script.ScriptId
    ); //find index in your array
    this.LiveMarkingModel.Livescripts.splice(index, 1);
    if (this.activepooltab == 3) {
      this.GetScripts(this.QigId, 3, this.pickfromdate);
    }
  }

  NavigateToMarkingPlayer(markingdata: IRecommedData) {
    if (!this.LiveMarkingLoader) {
      markingdata.QigId = this.QigId;

      var qigScriptModule = new QigScriptModule();
      qigScriptModule.QigId = this.QigId;
      qigScriptModule.ScriptId = markingdata.ScriptId;

      this.LiveMarkingLoader = true;

      this.PhaseTrackingId = markingdata.PhaseStatusTrackingId;

      this.qigservice
        .Getqigworkflowtracking(qigScriptModule.QigId, AppSettingEntityType.QIG)
        .pipe(first())
        .subscribe({
          next: (qdata: any) => {
            this.WorkFlowStatusTracking = qdata;
            let wfstatust = this.WorkFlowStatusTracking.filter(
              (a) =>
                a.WorkflowStatusCode == WorkflowStatus.Pause &&
                a.ProcessStatus == WorkflowProcessStatus.OnHold
            );

            if (wfstatust.length > 0) {
              this.IsQigPause = true;
              this.translate
                .get('LiveMarking.QigPause')
                .subscribe((translated: string) => {
                  this.intMessages.Markingprocesspaused =
                    translated +
                    '<br>' +
                    'Remarks : ' +
                    wfstatust[0].Remark +
                    '.';
                });
              this.Alert.warning(this.intMessages.Markingprocesspaused);
            } else {
              this.IsQigPause = false;
              if (this.isGraceperiod) {
                this.UpdateScriptStatus(qigScriptModule, true, false);
              }

              this.liveMarkingService
                .CheckScriptIsLivePool(
                  qigScriptModule.ScriptId,
                  this.PhaseTrackingId
                )
                .pipe(first())
                .subscribe({
                  next: (data: any) => {
                    if (data == 'SU001') {
                      const dialogRef = this.dialog.open(
                        QuestionAnnotatorComponent,
                        {
                          data: markingdata,
                          panelClass: 'fullviewpop',
                        }
                      );
                      dialogRef.afterClosed().subscribe((adata) => {
                        this.textinternationalization();
                        this.ScriptSearchValue = '';
                        if (adata.status == 1) {
                          this.LiveMarkingLoader = true;
                          this.liveMarkingService
                            .MoveScriptToGracePeriod(qigScriptModule)
                            .pipe(first())
                            .subscribe({
                              error: (mva: any) => {
                                throw mva;
                              },
                              complete: () => {
                                this.GetScripts(
                                  this.QigId,
                                  this.activepooltab,
                                  this.serverdatetime
                                );
                              },
                            });
                        }
                        if (adata.status == 0 && this.isGraceperiod) {
                          this.UpdateScriptStatus(qigScriptModule, false, true);
                        }
                        if (this.IsSearched) {
                          this.onDataChange();
                        }
                      });
                    } else {
                      this.GetScripts(
                        this.QigId,
                        this.activepooltab,
                        this.serverdatetime
                      );
                      this.translate
                        .get('LiveMarking.scrptisalreadyinlivepool')
                        .subscribe((translated: string) => {
                          this.Alert.warning(translated);
                        });
                    }
                  },
                  error: (err: any) => {
                    throw err;
                  },
                });
            }
          },
          error: (ae: any) => {
            throw ae;
          },
          complete: () => {
            this.LiveMarkingLoader = false;
          },
        });
    }
  }

  UpdateScriptStatus(
    qigScriptModule: QigScriptModule,
    scriptStaus: boolean,
    getScriptscall: boolean = false
  ) {
    this.LiveMarkingLoader = true;
    this.liveMarkingService
      .UpdateScriptMarkingDetails(qigScriptModule, scriptStaus)
      .pipe(first())
      .subscribe({
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.LiveMarkingLoader = false;
          if (getScriptscall) {
            this.GetScripts(this.QigId, 2, this.serverdatetime);
          }
        },
      });
  }

  CheckQigPause() {
    this.isDownloadDisable = true;
    this.LiveMarkingLoader = true;
    this.qigservice
      .Getqigworkflowtracking(this.QigId, AppSettingEntityType.QIG)
      .pipe(first())
      .subscribe({
        next: (wfdata: any) => {
          this.WorkFlowStatusTracking = wfdata;

          let wfstatustp = this.WorkFlowStatusTracking.filter(
            (a) =>
              a.WorkflowStatusCode == WorkflowStatus.Pause &&
              a.ProcessStatus == WorkflowProcessStatus.OnHold
          );

          if (wfstatustp.length > 0) {
            this.IsQigPause = true;
            this.translate
              .get('LiveMarking.QigPause')
              .subscribe((translated: string) => {
                this.intMessages.Markingprocesspaused =
                  translated +
                  '<br>' +
                  'Remarks : ' +
                  wfstatustp[0].Remark +
                  '.';
              });
            this.Alert.warning(this.intMessages.Markingprocesspaused);
          } else {
            if (
              this.LiveMarkingModel.DownloadLimitCount ==
              this.LiveMarkingModel.LivescriptCount
            ) {
              this.translate
                .get('LiveMarking.dwnldlimitsizecrossed')
                .subscribe((translated: string) => {
                  this.Alert.warning(translated);
                });
            } else {
              this.IsQigPause = false;
              this.LiveMarkingLoader = true;
              this.liveMarkingService
                .DownloadScripts(this.QigId)
                .pipe(first())
                .subscribe({
                  next: (ddata: any) => {
                    if (ddata == 'S001') {
                      this.LiveScripts(this.activepooltab);
                      this.translate
                        .get('LiveMarking.DownloadSucc')
                        .subscribe((translated: string) => {
                          this.intMessages.Alertmessages = translated;
                        });
                      this.Alert.success(this.intMessages.Alertmessages);
                    } else if (ddata == 'EXCEEDED') {
                      this.translate
                        .get('LiveMarking.DailyQuotaExced')
                        .subscribe((translated: string) => {
                          this.intMessages.Alertmessages = translated;
                        });
                      this.Alert.warning(this.intMessages.Alertmessages);
                    } else if (ddata == 'NOTYETCOMPLETED') {
                      this.translate
                        .get('LiveMarking.NotYetCompleted')
                        .subscribe((translated: string) => {
                          this.intMessages.Alertmessages = translated;
                        });
                      this.Alert.warning(this.intMessages.Alertmessages);
                    } else if (ddata == 'CLOSED') {
                      this.translate
                        .get('LiveMarking.NoScriptAvailable')
                        .subscribe((translated: string) => {
                          this.intMessages.Alertmessages = translated;
                        });
                      this.Alert.warning(this.intMessages.Alertmessages);
                    } else if (ddata == 'LVMRKNGNOTSTARTED') {
                      this.translate
                        .get('LiveMarking.LvmrkngNotYetStarted')
                        .subscribe((translated: string) => {
                          this.intMessages.Alertmessages = translated;
                        });
                      this.Alert.warning(this.intMessages.Alertmessages);
                    } else if (ddata == 'EXCEEDBATCHSIZE') {
                      this.LiveScripts(this.activepooltab);

                      this.translate
                        .get('LiveMarking.batchsizerched')
                        .subscribe((translated: string) => {
                          this.Alert.warning(translated);
                        });
                    }
                  },
                  error: (ad: any) => {
                    throw ad;
                  },
                  complete: () => {
                    this.LiveMarkingLoader = false;
                    this.isDownloadDisable = false;
                  },
                });
            }
          }
        },
        error: (al: any) => {
          throw al;
        },
        complete: () => {
          this.LiveMarkingLoader = false;
        },
      });
  }

  onDataChange() {
    this.IsSearched = true;
    const _ = moment();
    const Fromdate = moment(this.pickfromdate).add({
      hours: _.hour(),
      minutes: _.minute(),
      seconds: _.second(),
    });

    const Todate = moment(this.picktodate).add({
      hours: _.hour(),
      minutes: _.minute(),
      seconds: _.second(),
    });
    this.orgValueChange(Fromdate, Todate);
  }

  orgValueChange(fDate: any, tDate: any) {
    if (fDate <= tDate) {
      this.GetScripts(this.QigId, 3, fDate, tDate);
    } else {
      this.translate
        .get('LiveMarking.fromdatenotgreaterthantodate')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    }
  }

  SearchScript() {
    var ScriptSearchValue = this.ScriptSearchValue;
    this.LiveMarkingModel.Livescripts = this.LtScript.filter(function (el) {
      return el.ScriptName.toLowerCase().includes(
        ScriptSearchValue.trim().toLowerCase()
      );
    });
    this.LiveMarkingModel.Livescripts =
      this.LiveMarkingModel.Livescripts.filter(
        (x) => this.selected.includes(x.ScriptName) || this.selected.length == 0
      );
  }

  NavigateToDashboard() {
    this.route.navigate([`projects/dashboards/marker`]);
    let curntrole = this.authService.getCurrentRole();
    curntrole.forEach((role) => {
      if (role == 'EO' || role == 'AO' || role == 'ACM' || role == 'CM') {
        this.route.navigate([`/projects/dashboards/ao-cm/`, this.QigId]);
      }
      if (role == 'TL' || role == 'ATL') {
        this.route.navigate([`/projects/dashboards/tl-atl/`, this.QigId]);
      }
      if (role == 'MARKER') {
        this.route.navigate([`/projects/dashboards/marker/`, this.QigId]);
      }
    });
  }

  DownloadedScriptsUserList() {
    this.route.navigateByUrl(
      'projects/live-marking/downloadedscriptuserlist/' +
        this.QigName +
        '/' +
        this.QigId
    );
  }
}
@Pipe({
  name: 'formatTime',
})
export class FormatTimePipe implements PipeTransform {
  transform(value: number): string {
    const minutes: number = Math.floor(value / 60);
    return (
      ('00' + minutes).slice(-2) +
      ':' +
      ('00' + Math.floor(value - minutes * 60)).slice(-2)
    );
  }
}
