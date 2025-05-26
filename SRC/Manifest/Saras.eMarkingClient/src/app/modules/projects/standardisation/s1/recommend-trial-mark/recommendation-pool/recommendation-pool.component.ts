import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CommonService } from 'src/app/services/common/common.service';
import { TranslateService } from '@ngx-translate/core';
import { IRecommedData } from 'src/app/model/standardisation/recommendation';
import { QigService } from 'src/app/services/project/qig.service';
import { AlertService } from 'src/app/services/common/alert.service';
import {
  WorkflowProcessStatus,
  WorkflowStatus,
} from 'src/app/model/common-model';
import { AuthService } from 'src/app/services/auth/auth.service';
import { IRecommedQigScriptData } from 'src/app/model/standardisation/RecommendationQigScript';
import { QigUserModel } from 'src/app/model/project/qig';
import { first } from 'rxjs/operators';
import { RecPoolService } from 'src/app/services/project/standardisation/std-one/recommendation/rec-pool.service';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import { BandingComponent } from '../banding/banding.component';
import { RecommendationService } from 'src/app/services/project/standardisation/std-one/recommendation/recommendation.service';
import { UnrecommandedScript } from 'src/app/model/project/standardisation/std-one/recommendation/recommendation-model';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';

@Component({
  templateUrl: './recommendation-pool.component.html',
  styleUrls: ['./recommendation-pool.component.css'],
})
export class RecommendationPoolComponent implements OnInit {
  @ViewChild('menutrigger') menuclose!: ElementRef;
  RecomQIGs: any = [];
  RecomQIGScripts: any = [];
  RecomQIGScriptsFilter: any = [];
  Scriptslst: any = [];
  Examlst: any = [];
  isLoading: boolean = false;
  errMessage: string = '';
  ScrAlrRecmd!: string;
  AlrRecmdscrpt!: string;
  MyRole!: string[];
  RecomendationCounts: any;
  Ispause: number = 0;
  intMessages: any = {
    Markingprocesspaused: '',
    viewscript: '',
    viewedit: '',
    recscript: '',
    unrecommendscript: '',
    confirmunrcmnd: '',
    confirmunrecmnd: ''
  };
  IsConfigurationCompeleted: boolean = false;
  IsS1Required: boolean = true;
  activeQig!: QigUserModel;
  Isscriptloading: boolean = false;
  Issummaryloading: boolean = false;
  S1completed: any;
  ScriptSearchValue: string = '';
  s1closure: boolean = false;
  qigpause: boolean = false;
  qigpausemessage!: string;
  constructor(
    public dialog: MatDialog,
    public translate: TranslateService,
    public recpoolService: RecPoolService,
    public qigservice: QigService,
    public commonService: CommonService,
    public Alert: AlertService,
    private authservice: AuthService,
    private reommandservice: RecommendationService
  ) { }

  recommendscript(recommend: IRecommedData) {
    if (!this.Isscriptloading) {
      this.Isscriptloading = true;
      this.recpoolService
        .GetRecPoolScript(recommend.QigId, recommend.ScriptId)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            let IsValid = false;
            if (data != null && data != undefined && data.length > 0) {
              var filterdscript = data.filter(
                (scrpt: any) => scrpt.ScriptID === recommend.ScriptId
              );
              if (
                filterdscript != null &&
                filterdscript != undefined &&
                filterdscript.length > 0
              ) {
                let scriptrec = filterdscript[0];
                recommend.IsViewMode =
                  (scriptrec.IsRecommended && !scriptrec.IsRecommendedByMe) ||
                  scriptrec.WorkFlowStatusCode == WorkflowStatus.TrailMarking ||
                  scriptrec.WorkFlowStatusCode ==
                  WorkflowStatus.Categorization ||
                  filterdscript[0]?.ProcessStatus ==
                  WorkflowProcessStatus.OnHold;
                IsValid = true;
                this.openrecmView(recommend);
              }
            }
            if (!IsValid) {
              this.Alert.warning(this.AlrRecmdscrpt);
              this.GetRecPoolStastics(this.activeQig.QigId);
              this.GetRecPoolScript(this.activeQig.QigId);
            }
          },
          error: (a: any) => {
            throw a;
          },
        });
    }
    this.Isscriptloading = false;
  }

  public get WorkflowProcessStatus() {
    return WorkflowProcessStatus;
  }

  openrecmView(recommend: IRecommedData) {
    const dialogRef = this.dialog.open(BandingComponent, {
      data: recommend,
      panelClass: 'fullviewpop',
    });
    dialogRef.afterClosed().subscribe(() => {
      this.GetRecPoolStastics(this.activeQig.QigId);
      this.GetRecPoolScript(this.activeQig.QigId);
      this.ScriptSearchValue = '';
    });
  }

  getrecbtnTitle(recommend: IRecommedQigScriptData) {
    let lbltext = '';
    if (recommend != undefined) {
      if (
        recommend.WorkFlowStatusCode == WorkflowStatus.TrailMarking ||
        recommend.WorkFlowStatusCode == WorkflowStatus.Categorization
      ) {
        lbltext = this.intMessages.viewscript;
      } else {
        lbltext = recommend.IsRecommendedByMe
          ? this.intMessages.viewedit
          : recommend.IsRecommended
            ? this.intMessages.viewscript
            : this.intMessages.recscript;
      }
    }
    return lbltext;
  }

  ngOnInit(): void {
    this.translate
      .get('Std.recommend.viewscript')
      .subscribe((translated: string) => {
        this.intMessages.viewscript = translated;
      });
    this.translate
      .get('Std.recommend.viewedit')
      .subscribe((translated: string) => {
        this.intMessages.viewedit = translated;
      });
    this.translate.get('Std.recommend.rec').subscribe((translated: string) => {
      this.intMessages.recscript = translated;
    });

    this.translate
      .get('Std.recommend.Title')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });

    this.translate.get('Std.recommend.desc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });

    this.translate.get('Std.band.RecAlExt').subscribe((translated: string) => {
      this.AlrRecmdscrpt = translated;
    });
    this.translate.get('Std.band.S1comp').subscribe((translated: string) => {
      this.ScrAlrRecmd = translated;
    });
    this.translate
      .get('Std.SetUp.Markingprocessclosure')
      .subscribe((translated: string) => {
        this.intMessages.Markingprocessclosure = translated;
      });
    this.translate
      .get('Std.SetUp.Markingprocesspaused')
      .subscribe((translated: string) => {
        this.intMessages.Markingprocesspaused = translated;
      });

    this.translate
      .get('Std.recommend.unrecommndscrptcnfrmalrt')
      .subscribe((translated: string) => {
        this.intMessages.confirmunrecmnd = translated;
      });


    this.MyRole = this.authservice.getCurrentRole();
    this.ScriptSearchValue = '';
  }

  getQigDetails(selectedqig: QigUserModel) {
    this.Alert.clear();
    if (selectedqig != null && selectedqig.QigId > 0) {
      this.activeQig = selectedqig;
      this.IsS1Required = selectedqig.IsS1Available;
      this.Checkqigstatus();
      this.GetRecPoolStastics(this.activeQig.QigId);
      setTimeout(() => {
        this.GetRecPoolScript(this.activeQig.QigId);
      }, 1000);
      this.ScriptSearchValue = '';
    }
  }

  isAoCM() {
    let isaut = false;
    this.MyRole = this.authservice.getCurrentRole();
    this.MyRole.forEach((element) => {
      if (element == 'AO' || element == 'CM') {
        isaut = true;
      }
    });
    return isaut;
  }

  isAo() {
    let isao = false;
    this.MyRole = this.authservice.getCurrentRole();
    this.MyRole.forEach((element) => {
      if (element == 'AO') {
        isao = true;
      }
    });
    return isao;
  }

  GetRecPoolScript(QigId: number) {
    this.Isscriptloading = true;
    this.RecomQIGScripts = [];
    this.Scriptslst = [];
    this.IsConfigurationCompeleted = false;
    this.recpoolService
      .GetRecPoolScript(QigId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.RecomQIGScripts = data;
          this.RecomQIGScriptsFilter = data;
          this.Scriptslst = data;

          // Remove duplicates
          var result: any[] = [];

          data.forEach((item: { CenterID: any; }, index: any) => {
            if (result.findIndex(i => i.CenterID == item.CenterID) === -1) {
              result.push(item)
            }
          });
          this.Examlst = result;
          //

          if (
            this.RecomQIGScripts.length >= 0 &&
            this.RecomendationCounts?.TotalTargetRecomendations == 0
          ) {
            if (this.IsS1Required) {
              this.IsConfigurationCompeleted = true;
            }
          }
        },
        error: (a: any) => {
          this.Isscriptloading = false;
          throw a;
        },
        complete: () => {
          this.Isscriptloading = false;
        },
      });
  }

  GetRecPoolStastics(QigId: number) {
    this.Issummaryloading = true;
    this.recpoolService
      .GetRecPoolStastics(QigId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.RecomendationCounts = data;
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.Issummaryloading = false;
        },
      });
  }

  Checkqigstatus() {
    this.qigservice
      .Getqigworkflowtracking(this.activeQig.QigId, AppSettingEntityType.QIG)
      .subscribe((data) => {
        let WorkFlowStatusTracking = data;

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
            a.WorkflowStatusCode == WorkflowStatus.Standardization_1 &&
            a.ProcessStatus == WorkflowProcessStatus.Completed
        );

        this.s1closure = s1closuredata.length > 0 ? true : false;

        this.qigpause = Qigpausedata.length > 0 ? true : false;

        if (Qigclosuredata.length > 0) {
          this.Alert.warning(
            this.intMessages.Markingprocessclosure +
            '<br>' +
            'Remarks : ' +
            Qigclosuredata[0].Remark +
            '.'
          );
        } else if (Qigpausedata.length > 0) {
          this.qigpausemessage =
            this.intMessages.Markingprocesspaused +
            '<br>' +
            'Remarks : ' +
            Qigpausedata[0].Remark +
            '.';
          this.Alert.warning(
            this.intMessages.Markingprocesspaused +
            '<br>' +
            'Remarks : ' +
            Qigpausedata[0].Remark +
            '.'
          );
        } else if (s1closuredata.length > 0) {
          this.translate
            .get('Std.QuaAsseCrea.s1closure')
            .subscribe((translated: string) => {
              this.Alert.clear();
              this.Alert.warning(translated);
            });
        }
      });
  }

  SearchScript() {
    var ScriptSearchValue = this.ScriptSearchValue;

    var filtered = this.RecomQIGScriptsFilter.filter((x: { IscenterSelected: any; }) => x.IscenterSelected);
    if (filtered.length == 0) {
      this.RecomQIGScripts = this.RecomQIGScriptsFilter.filter(function (el: {
        ScriptName: string;
      }) {
        return el.ScriptName.toLowerCase().includes(
          ScriptSearchValue.trim().toLowerCase()
        );
      });
    }
    else {
      this.RecomQIGScripts = filtered.filter(function (el: {
        ScriptName: string;
      }) {
        return el.ScriptName.toLowerCase().includes(
          ScriptSearchValue.trim().toLowerCase()
        );
      });
    }
  }

  UnrecommendationScript(scriptId: number) {
    const confirmDialogue = this.dialog.open(ConfirmationDialogComponent, {
      data: { message: this.intMessages.confirmunrecmnd },
    });
    confirmDialogue.afterClosed().subscribe((result) => {
      if (result) {
        var obj = new UnrecommandedScript();
        obj.Qigid = this.activeQig.QigId;
        obj.ScriptId = scriptId;
        this.isLoading = true;

        this.reommandservice
          .UnrecommandedScripts(obj)
          .pipe(first())
          .subscribe({
            next: (data: any) => {
              if (data == 'SU001') {
                this.translate
                  .get('Std.recommend.saveunrcmnded')
                  .subscribe((translated: string) => {
                    this.intMessages.saveunrcmnded = translated;
                  });

                this.Alert.success(this.intMessages.saveunrcmnded);
              }
              if (data == 'CTGRTN') {
                this.translate
                  .get('Std.recommend.scrptcategorized')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
              }
              if (data == "Unrecmended") {
                this.translate
                  .get('Std.recommend.scrptalrdyunrecmnded')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
              }
              if (data == 'TRMARKG') {

                this.translate
                  .get('Std.recommend.scrpttrialmrked')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
              }
              if (data == 'PAUSE') {
                this.Checkqigstatus();
              }
              if (data == 'S1COMPLETED') {
                this.translate
                  .get('Std.QuaAsseCrea.s1closure')
                  .subscribe((translated: string) => {
                    this.Alert.clear();
                    this.Alert.warning(translated);
                  });
              }

              this.GetRecPoolStastics(this.activeQig.QigId);
              this.GetRecPoolScript(this.activeQig.QigId);
            },
            error: (err: any) => {
              throw err;
            },
            complete: () => {
              this.isLoading = false;
            },
          });
      }
    });
  }

  IsCheckedExamcenter(event: any, el: any) {
    if (event.checked) {
      this.RecomQIGScriptsFilter.filter((a: { CenterID: any }) => a.CenterID == el.CenterID).forEach((element: { IscenterSelected: any; }) => {
        element.IscenterSelected = el.IscenterSelected;
      });
    }
    else {
      this.RecomQIGScriptsFilter.filter((a: { CenterID: any }) => a.CenterID == el.CenterID).forEach((element: { IscenterSelected: any; }) => {
        element.IscenterSelected = false;
      });
    }
  }

  Filter() {
    this.RecomQIGScripts = this.RecomQIGScriptsFilter.filter((x: { IscenterSelected: any; }) => x.IscenterSelected);
    if (this.RecomQIGScripts.length == 0) {
      this.RecomQIGScripts = this.Scriptslst;
    }
    this.RecomQIGScripts = this.RecomQIGScripts.filter((x: { ScriptName:string; }) => x.ScriptName.toLowerCase().includes(
      this.ScriptSearchValue.trim().toLowerCase()
    ));
  }

  ClearFilter() {
    this.RecomQIGScriptsFilter.filter((a: { IscenterSelected: any }) => a.IscenterSelected).forEach((element: { IscenterSelected: any; }) => {
      element.IscenterSelected = false;
    });

    this.RecomQIGScripts = this.RecomQIGScriptsFilter.filter((x: { ScriptName: string; }) => x.ScriptName.toLowerCase().includes(
      this.ScriptSearchValue.trim().toLowerCase()
    ));
  }
}
