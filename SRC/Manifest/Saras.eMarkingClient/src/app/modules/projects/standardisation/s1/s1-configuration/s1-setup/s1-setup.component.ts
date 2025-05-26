import { Component, OnInit } from '@angular/core';
import { CommonService } from 'src/app/services/common/common.service';
import { StdSetupService } from 'src/app/services/project/standardisation/std-one/std-setup.service';
import { ProjectCenteres, GetAppSettingGroupModel, UpdateProjectConfigModel } from 'src/app/model/project/standardisation/std-one/setup-model';
import { AlertService } from 'src/app/services/common/alert.service';
import { AppSettingEntityType, AppSettingGroup, AppSettingValueType } from 'src/app/model/appsetting/app-setting-model';
import { first } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { QigUserModel } from 'src/app/model/project/qig';
import { QigService } from 'src/app/services/project/qig.service';
import { WorkflowProcessStatus, WorkflowStatus } from 'src/app/model/common-model';

@Component({
  templateUrl: './s1-setup.component.html',
  styleUrls: ['./s1-setup.component.css']
})
export class S1SetupComponent implements OnInit {
  KPList: any = [];
  ATLList: any = [];
  TLList: any = [];
  ACMList: any = [];
  CMList: any = [];
  AOList: any = [];
  isCheckedAO: boolean = true;
  isCheckedCM: boolean = true;
  isCheckedACM: boolean = true;
  isTl: any = false;
  isATl: any = false;
  disableAO: boolean = true;
  disableCM: boolean = true;
  disableACM: boolean = true;
  disableTL: boolean = false;
  disableATL: boolean = false;
  AlrRecmd: string = '';
  ProjectCenterlist!: ProjectCenteres[];
  ProjectCenterSelectedlist!: ProjectCenteres[];
  checked: boolean | undefined;
  AvailableCentersValue: string = "";
  ProjectSearchSelectedValue: string = "";
  ProjectsSearchList!: ProjectCenteres[];
  ProjectsSearchSelectedList!: ProjectCenteres[];
  selected: any[] = [];
  QigConfiguration!: any;
  total: number = 0;
  totscripts_selected: number = 0;
  avltotal: number = 0;
  SelectedTotal: number = 0;
  ViewAvailanleCenters: boolean = true;
  ViewSelectedCenters: boolean = false;
  ProjectConfig: UpdateProjectConfigModel[] = [];
  AppstngList!: GetAppSettingGroupModel;
  max_total: number = 0;
  Ispauseoronholdors1completed: boolean = false;
  IsS1Required: boolean = false;
  setuploading: boolean = false;
  kpsaveloading: boolean = false;
  centerssaveloading: boolean = false;
  configsaveloading: boolean = false;
  activeQig!: QigUserModel;
  noresponsecount: number = 0;
  intMessages: any = {
    Confirmwarning: '',
    Deletewarning: '',
    Nodatawarning: '',
    Errorgetconfigwarning: '',
    Recommpoolcountlesswarning: '',
    Updateconfigsuccesswarning: '',
    Updateconfigerrorwarning: '',
    Updatekpsuccesswarning: '',
    Centerwarning: '',
    Updatecentersuccesswarning: '',
    Markingprocesspaused: '',
    Totallessrecompoolcountwarning: '',
    Selectcenterswarning: '',
    Markingprocessclosure: '',
    Totscriptsnullswarning: '',
    S1comp: ''
  };

  constructor(public stdsetupservice: StdSetupService,
    public commonService: CommonService,
    private dialog: MatDialog,
    public Alert: AlertService,
    readonly translate: TranslateService,public qigservice: QigService) {
  }

  ngOnInit(): void {
    this.translate.get('Std.SetUp.Title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('Std.SetUp.desc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.translate.get('Std.SetUp.AlrRecmd').subscribe((translated: string) => {
      this.AlrRecmd = translated;
    });
    this.translate.get('Std.SetUp.Confirmwarning').subscribe((translated: string) => {
      this.intMessages.Confirmwarning = translated;
    });
    this.translate.get('Std.SetUp.Deletewarning').subscribe((translated: string) => {
      this.intMessages.Deletewarning = translated;
    });
    this.translate.get('Std.SetUp.Nodatawarning').subscribe((translated: string) => {
      this.intMessages.Nodatawarning = translated;
    });
    this.translate.get('Std.SetUp.Errorgetconfigwarning').subscribe((translated: string) => {
      this.intMessages.Errorgetconfigwarning = translated;
    });
    this.translate.get('Std.SetUp.Recommpoolcountlesswarning').subscribe((translated: string) => {
      this.intMessages.Recommpoolcountlesswarning = translated;
    });
    this.translate.get('Std.SetUp.Updateconfigsuccesswarning').subscribe((translated: string) => {
      this.intMessages.Updateconfigsuccesswarning = translated;
    });
    this.translate.get('Std.SetUp.Updateconfigerrorwarning').subscribe((translated: string) => {
      this.intMessages.Updateconfigerrorwarning = translated;
    });
    this.translate.get('Std.SetUp.Updatekpsuccesswarning').subscribe((translated: string) => {
      this.intMessages.Updatekpsuccesswarning = translated;
    });
    this.translate.get('Std.SetUp.Centerwarning').subscribe((translated: string) => {
      this.intMessages.Centerwarning = translated;
    });
    this.translate.get('Std.SetUp.Updatecentersuccesswarning').subscribe((translated: string) => {
      this.intMessages.Updatecentersuccesswarning = translated;
    });
    this.translate.get('Std.SetUp.Markingprocesspaused').subscribe((translated: string) => {
      this.intMessages.Markingprocesspaused = translated;
    });
    this.translate.get('Std.SetUp.Totallessrecompoolcountwarning').subscribe((translated: string) => {
      this.intMessages.Totallessrecompoolcountwarning = translated;
    });
    this.translate.get('Std.SetUp.Selectcenterswarning').subscribe((translated: string) => {
      this.intMessages.Selectcenterswarning = translated;
    });
    this.translate.get('Std.SetUp.Markingprocessclosure').subscribe((translated: string) => {
      this.intMessages.Markingprocessclosure = translated;
    });
    this.translate.get('Std.SetUp.Totscriptsnullswarning').subscribe((translated: string) => {
      this.intMessages.Totscriptsnullswarning = translated;
    });
    this.translate.get('Std.SetUp.S1comp').subscribe((translated: string) => {
      this.intMessages.S1comp = translated;
    });
  }

  getQigDetails(selectedqig: QigUserModel) {
    if (selectedqig != null && selectedqig?.QigId > 0) {
      this.activeQig = selectedqig;
      this.IsS1Required = selectedqig?.IsS1Available;
      setTimeout(() => {
        this.Checkqigstatus();
      }, 1000);      
      this.KeyPersonnels(this.activeQig?.QigId);
      this.ProjectCenters(this.activeQig?.QigId);
      this.GetQIGStandardizationScriptSettings(this.activeQig?.QigId, true);
      this.Appsetting();
      this.CloseFn(this.ProjectCenterSelectedlist);
    }
  }

  Checkqigstatus() {
    this.Ispauseoronholdors1completed = false;
    this.qigservice
      .Getqigworkflowtracking(this.activeQig?.QigId, AppSettingEntityType.QIG).subscribe((data) => {
        let WorkFlowStatusTracking = data;

        var Qigpausedata = WorkFlowStatusTracking.filter((a: { WorkflowStatusCode: WorkflowStatus; ProcessStatus: WorkflowProcessStatus; }) =>
          a.WorkflowStatusCode == WorkflowStatus.Pause &&
          a.ProcessStatus == WorkflowProcessStatus.OnHold);

        var Qigclosuredata = WorkFlowStatusTracking.filter((a: { WorkflowStatusCode: WorkflowStatus; ProcessStatus: WorkflowProcessStatus; }) =>
          a.WorkflowStatusCode == WorkflowStatus.Closure &&
          a.ProcessStatus == WorkflowProcessStatus.Closure);

        var s1closuredata = WorkFlowStatusTracking.filter((a: { WorkflowStatusCode: WorkflowStatus; ProcessStatus: WorkflowProcessStatus; }) =>
          a.WorkflowStatusCode == WorkflowStatus.Standardization_1 &&
          a.ProcessStatus == WorkflowProcessStatus.Completed);

        if (Qigclosuredata.length > 0) {
          this.Ispauseoronholdors1completed = true;
          this.Alert.warning(this.intMessages.Markingprocessclosure + '<br>' + 'Remarks : ' + Qigclosuredata[0].Remark+'.');
        }
        else if (Qigpausedata.length > 0) {
          this.Ispauseoronholdors1completed = true;
          this.Alert.warning(this.intMessages.Markingprocesspaused + '<br>' + 'Remarks : ' + Qigpausedata[0].Remark+'.');
        }
        else if (s1closuredata.length > 0) {
          this.Ispauseoronholdors1completed = true;
          this.translate
            .get('Std.QuaAsseCrea.s1closure')
            .subscribe((translated: string) => {
              this.Alert.clear();
              this.Alert.warning(translated);
            });
        }
      });
  }

  KeyPersonnels(QigId: number) {
    this.setuploading = true;
    this.disableTL = false;
    this.disableATL = false;
    this.stdsetupservice.ProjectKps(QigId).pipe(first()).subscribe({
      next: (data: any) => {
        this.KPList = data;
        this.ATLList = data.filter((li: { RoleCode: string; }) => li.RoleCode == 'ATL');
        this.TLList = data.filter((li: { RoleCode: string; }) => li.RoleCode == 'TL');
        this.ACMList = data.filter((li: { RoleCode: string; }) => li.RoleCode == 'ACM');
        this.CMList = data.filter((li: { RoleCode: string; }) => li.RoleCode == 'CM');
        this.AOList = data.filter((li: { RoleCode: string; }) => li.RoleCode == 'AO');

        if (this.AOList.length == this.AOList?.filter((a: { IsKP: boolean; }) => a.IsKP).length) {
          if (this.AOList.length == 0 && this.AOList?.filter((a: { IsKP: boolean; }) => a.IsKP).length == 0) {
            this.isCheckedAO = false;
          }
          else {
            this.isCheckedAO = true
          }
        }

        if (this.CMList.length == this.CMList?.filter((a: { IsKP: boolean; }) => a.IsKP).length) {
          if (this.CMList.length == 0 && this.CMList?.filter((a: { IsKP: boolean; }) => a.IsKP).length == 0) {
            this.isCheckedCM = false;
          }
          else {
            this.isCheckedCM = true;
          }
        }

        if (this.ACMList.length == this.ACMList?.filter((a: { IsKP: boolean; }) => a.IsKP).length) {
          if (this.ACMList.length == 0 && this.ACMList?.filter((a: { IsKP: boolean; }) => a.IsKP).length == 0) {
            this.isCheckedACM = false;
          }
          else {
            this.isCheckedACM = true;
          }
        }

        if (this.TLList?.filter((a: { IsKP: boolean; }) => !a.IsKP).length > 0) {
          this.isTl = false;
        }
        else {
          this.isTl = true;
        }

        if (this.ATLList?.filter((a: { IsKP: boolean; }) => !a.IsKP).length > 0) {
          this.isATl = false;
        }
        else {
          this.isATl = true;
        }

        if (this.TLList.length == this.TLList?.filter((a: { IsKP: boolean; }) => a.IsKP).length) {
          if (this.TLList.length == 0 && this.TLList?.filter((a: { IsKP: boolean; }) => a.IsKP).length == 0) {
            this.isTl = false;
          }
          this.disableTL = this.TLList?.length == 0 && this.TLList?.filter((a: { IsKP: boolean; }) => a.IsKP).length == 0
          this.disableTL = this.TLList?.length == this.TLList?.filter((a: { IsKpTagged: boolean, IsKpTrialmarkedorcategorised: boolean }) =>
            a.IsKpTagged || a.IsKpTrialmarkedorcategorised).length
        }

        if (this.ATLList.length == this.ATLList?.filter((a: { IsKP: boolean; }) => a.IsKP).length) {
          if (this.ATLList.length == 0 && this.ATLList?.filter((a: { IsKP: boolean; }) => a.IsKP).length == 0) {
            this.isATl = false;
          }
          this.disableATL = this.ATLList.length == 0 && this.ATLList?.filter((a: { IsKP: boolean; }) => a.IsKP).length == 0;
          this.disableATL = this.ATLList.length == this.ATLList?.filter((a: { IsKpTagged: boolean, IsKpTrialmarkedorcategorised: boolean }) =>
            a.IsKpTagged || a.IsKpTrialmarkedorcategorised).length;
        }
      },
      error: (err: any) => {
        throw (err);
      },
      complete: () => {
        this.setuploading = false;
      }
    });
  }

  toggleTL(event: any) {
    this.TLList.forEach(function (element: any) {
      if (!element.IsKpTagged && !element.IsKpTrialmarkedorcategorised)
        element.IsKP = event.checked;
    });
  }

  toggleATL(event: any) {
    this.ATLList.forEach(function (element: any) {
      if (!element.IsKpTagged && !element.IsKpTrialmarkedorcategorised)
        element.IsKP = event.checked;
    });
  }

  CheckHeaderTL(event: any) {
    this.isTl = this.TLList.length == this.TLList?.filter((a: { IsKP: boolean; }) => a.IsKP).length;
  }

  CheckHeaderATL(event: Event) {
    this.isATl = this.ATLList.length == this.ATLList?.filter((a: { IsKP: boolean; }) => a.IsKP).length;
  }

  ProjectCenters(QigId: number) {
    this.setuploading = true;
    this.stdsetupservice.ProjectCenters(QigId).pipe(first()).subscribe({
      next: (data: any) => {
        this.ProjectCenterlist = data;
        this.ProjectCenterSelectedlist = data;
        this.ProjectsSearchList = data;
        this.ProjectsSearchSelectedList = data;
        this.avltotal = this.ProjectCenterlist?.length;
        this.noresponsecount = this.ProjectCenterlist[0]?.noresponsecount;
        var Projectcenterlst = this.ProjectCenterlist
        var Isanyrecommanded = false;
        var filterdLst = this.ProjectCenterlist?.filter(center => center.IsRecommended);
        if (filterdLst != null && filterdLst != undefined && filterdLst.length > 0) {
          Isanyrecommanded = true;
        }

        for (var i = 0; i < this.ProjectCenterlist.length; i++) {
          if (Projectcenterlst[i].IsSelectedForRecommendation) {
            Projectcenterlst[i].checked = Projectcenterlst[i]?.IsSelectedForRecommendation;

            if (Isanyrecommanded) {
              Projectcenterlst[i].IsrecDisabled = true;
            }
            else {
              Projectcenterlst[i].IsrecDisabled = false;
            }
          }
        }

        this.totscripts_selected = 0;
        this.total = 0;
        var projectelst = this.ProjectCenterlist;
        for (var cl = 0; cl < this.ProjectCenterlist.length; cl++) {
          if (projectelst[cl]?.IsSelectedForRecommendation) {
            this.totscripts_selected = this.totscripts_selected + projectelst[cl]?.TotalNoOfScripts;
            this.total = this.total + projectelst[cl]?.TotalNoOfScripts;
            
          }
          else {
            projectelst[cl].IsSelectedForRecommendation = false;
          }
        }

        if (this.totscripts_selected == 0) {
          this.max_total = 1;
        }
        else {
          this.max_total = this.totscripts_selected;
        }
      },
      error: (err: any) => {
        throw (err)
      },
      complete: () => {
        this.setuploading = false;
      }

    });
  }

  onChecked(ProjectCenter: any) {
    ProjectCenter.checked = !ProjectCenter.checked;
    if (ProjectCenter.checked) {
      ProjectCenter.IsSelectedForRecommendation = true;
    }
    else {
      ProjectCenter.IsSelectedForRecommendation = false;
    }

    this.total = 0;
    var projectelst = this.ProjectsSearchSelectedList;

    for (var i = 0; i < this.ProjectsSearchSelectedList?.length; i++) {
      if (projectelst[i]?.IsSelectedForRecommendation) {
        this.total = this.total + projectelst[i]?.TotalNoOfScripts;
      }
      else {
        projectelst[i].IsSelectedForRecommendation = false;
      }
    }
  }

  delete(SelectedProjCenters: any) {

    const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        message: this.intMessages.Confirmwarning
      }
    });

    confirmDialog.afterClosed().subscribe(result => {
      this.Alert.clear();
      if (result === true) {
        SelectedProjCenters.checked = !SelectedProjCenters.checked;
        SelectedProjCenters.IsSelectedForRecommendation = false;
        this.SelectedTotal = this.SelectedTotal - 1;
        this.Alert.clear();
        this.Alert.success(this.intMessages.Deletewarning)
      }

      this.total = 0;
      var projecteselectedlst = this.ProjectsSearchSelectedList;
      for (var sl = 0; sl < this.ProjectsSearchSelectedList?.length; sl++) {
        if (projecteselectedlst[sl]?.IsSelectedForRecommendation) {
          this.total = this.total + projecteselectedlst[sl]?.TotalNoOfScripts;
        }
        else {
          projecteselectedlst[sl].IsSelectedForRecommendation = false;
        }
      }
    });
  }

  SearchAvailableCenters() {
    var AvailableCentersValue = this.AvailableCentersValue;
    this.ProjectCenterlist = this.ProjectsSearchList?.filter(function (el) { return el.CenterName.toLowerCase().includes(AvailableCentersValue.trim().toLowerCase()) });
    this.ProjectCenterlist = this.ProjectCenterlist?.filter(x => this.selected.includes(x.CenterCode) || this.selected.length == 0);
    this.onChecked(this.ProjectCenterlist);
  }

  SearchSelectedCenters() {
    var ProjectSearchSelectedValue = this.ProjectSearchSelectedValue;
    var selectedlist = this.ProjectsSearchSelectedList?.filter(x => x.checked);
    this.ProjectCenterSelectedlist = selectedlist?.filter(function (el) { return el.CenterName.toLowerCase().includes(ProjectSearchSelectedValue.trim().toLowerCase()) });
    this.onChecked(this.ProjectCenterlist);
  }

  ViewSelectedFn() {
    this.SelectedTotal = 0;
    this.ViewAvailanleCenters = false;
    this.ViewSelectedCenters = true;
    var selectedobj = this.ProjectsSearchSelectedList

    for (var i = 0; i < this.ProjectsSearchSelectedList?.length; i++) {
      if (selectedobj[i].checked) {
        this.SelectedTotal = this.SelectedTotal + 1;
      }
    }
    this.ProjectSearchSelectedValue = "";
  }

  CloseFn(SelectedProjCenters: any) {

    SelectedProjCenters?.forEach((_data: any) => {
      if (_data?.checked) {
        _data.IsSelectedForRecommendation = true;
      }
      else {
        _data.IsSelectedForRecommendation = false;
      }
    });

    this.total = 0;
    var projecteselectedlst = this.ProjectsSearchSelectedList;
    for (var k = 0; k < this.ProjectsSearchSelectedList?.length; k++) {
      if (projecteselectedlst[k]?.IsSelectedForRecommendation) {
        this.total = this.total + projecteselectedlst[k]?.TotalNoOfScripts;
      }
      else {
        projecteselectedlst[k].IsSelectedForRecommendation = false;
      }
    }

    this.ViewSelectedCenters = false;
    this.ViewAvailanleCenters = true;
    this.AvailableCentersValue = "";
    this.ProjectCenters(this.activeQig.QigId);
  }

  onKeydownAvailableCenter(event: any) {
    this.AvailableCentersValue = "";
  }

  onKeydownSelectedCenter(event: any) {
    this.ProjectSearchSelectedValue = '';
  }

  GetQIGStandardizationScriptSettings(QigId: number, Ispageload: boolean) {
    this.setuploading = true;
    this.stdsetupservice.GetQIGConfiguration(this.activeQig?.QigId).pipe(first()).subscribe({
      next: (data: any) => {
        this.QigConfiguration = data;
        this.QigConfig();
        this.Alert.clear();
        if (Ispageload) {
          if (this.QigConfiguration?.RecomendationPoolCount < this.QigConfiguration?.script_total) {
            this.Alert.clear();
            this.Alert.warning(this.intMessages.Recommpoolcountlesswarning);
          }
        }
      },
      error: (err: any) => {
        throw (err);
      },
      complete: () => {
        this.setuploading = false;
      }
    });
  }

  QigConfig() {
    this.stdsetupservice.GetStdQigSettings(AppSettingGroup.Standardization1SettingGroupCode, AppSettingEntityType.QIG, this.activeQig?.QigId).pipe(first()).subscribe({
      next: (data: any) => {
        if (data != null && data != undefined) {
          this.ProjectConfig = data;
          this.ProjectConfig?.forEach(element => {
            element.Value = JSON.stringify(element?.Value?.toLowerCase());
            if (element?.Children != null) {
              element?.Children?.forEach(child => {
                child.Value = JSON.stringify(child?.Value?.toLowerCase());
                element.EntityID = this.activeQig?.QigId;
              });
            }
          });
        } else {
          this.Alert.warning(this.intMessages.Nodatawarning);
        }
      },
      error: (err: any) => {
        throw (err);
      },

    });
  }

  Appsetting() {
    this.setuploading = true;
    this.stdsetupservice.GetAppsettingGroup(AppSettingGroup.Standardization1SettingGroupCode).pipe(first()).subscribe({
      next: (data: any) => {
        this.AppstngList = data;
      },
      error: (err: any) => {
        throw (err);
      },
      complete: () => {
        this.setuploading = false;
      }
    });
  }

  SaveKPs(evnt: any) {
    var selectedkps = this.KPList.filter((a: { IsKP: boolean; }) => a.IsKP);
    this.kpsaveloading = true;
    this.stdsetupservice.UpdateKeyPersonnels(selectedkps, this.activeQig?.QigId).pipe(first()).subscribe({
      next: (data: any) => {
        this.Alert.clear();
        if (data == "Alreadyrecommendedortrailmarked") {
          this.Alert.clear();
          this.Alert.warning(this.AlrRecmd);
          this.KeyPersonnels(this.activeQig?.QigId);
        }
        if (data == "UP002") {
          this.Alert.clear();
          this.Alert.success(this.intMessages.Updatekpsuccesswarning);
          this.KeyPersonnels(this.activeQig?.QigId);
        }
      },
      error: (err: any) => {
        throw (err);
      },
      complete: () => {
        this.kpsaveloading = false;
      }
    });
  }

  SaveCenters(evnt: any) {

    var total = this.total;
    var Recompoolcount = this.QigConfiguration?.RecomendationPoolCount;

    this.Alert.clear();
    if (this.ProjectCenterlist.length > 0) {
      if (total == 0) {
        this.Alert.clear();
        this.Alert.warning(this.intMessages.Centerwarning);
        return;
      }
      else if (total < Recompoolcount) {
        this.Alert.clear();
        this.Alert.warning(this.intMessages.Totallessrecompoolcountwarning);
        return;
      }
      else {
        var selectedprojectcenters = this.ProjectCenterSelectedlist.filter((a: { IsSelectedForRecommendation: boolean; }) => a.IsSelectedForRecommendation);
        this.centerssaveloading = true;
        this.stdsetupservice.UpdateProjectCenters(selectedprojectcenters, this.activeQig?.QigId).pipe(first()).subscribe({
          next: (data: any) => {
            this.Alert.clear();
            if (data == "UP001") {
              this.Alert.clear();
              this.Alert.success(this.intMessages.Updatecentersuccesswarning);
              this.ProjectCenters(this.activeQig?.QigId);
              this.GetQIGStandardizationScriptSettings(this.activeQig?.QigId, false);
            }
          },
          error: (err: any) => {
            throw (err);
          },
          complete: () => {
            this.centerssaveloading = false;
          }
        });
      }
    }
  }

  SaveQIGConfig(evnt: any) {
    var total = this.totscripts_selected;
    var settingtot = this.QigConfiguration.script_total;

    this.Alert.clear();

    if ((this.QigConfiguration?.RecomendationPoolCount == null || this.QigConfiguration?.RecomendationPoolCount == 0) && (this.QigConfiguration?.RecomendationPoolCountPerKP == null || this.QigConfiguration?.RecomendationPoolCountPerKP == 0)) {
      this.translate.get('Std.SetUp.Recommpoolorperkpcountnull').subscribe((translated: string) => {
        this.Alert.clear();
        this.Alert.warning(translated);
      });
      return;
    }
    else if (((this.QigConfiguration?.RecomendationPoolCount != null || this.QigConfiguration?.RecomendationPoolCount != 0) && (this.QigConfiguration?.RecomendationPoolCountPerKP == null || this.QigConfiguration?.RecomendationPoolCountPerKP == 0)) || (this.QigConfiguration?.RecomendationPoolCountPerKP == null || this.QigConfiguration?.RecomendationPoolCountPerKP == 0)) {
      this.translate.get('Std.SetUp.Recommpoolperkpcountnull').subscribe((translated: string) => {
        this.Alert.clear();
        this.Alert.warning(translated);
      });
      return;
    }
    else if (((this.QigConfiguration?.RecomendationPoolCount == null || this.QigConfiguration?.RecomendationPoolCount == 0) && (this.QigConfiguration?.RecomendationPoolCountPerKP != null || this.QigConfiguration?.RecomendationPoolCountPerKP != 0)) || (this.QigConfiguration?.RecomendationPoolCount == null || this.QigConfiguration?.RecomendationPoolCount == 0)) {
      this.translate.get('Std.SetUp.Recommpoolcountnull').subscribe((translated: string) => {
        this.Alert.clear();
        this.Alert.warning(translated);
      });
      return;
    }
    else if (total == 0) {
      this.Alert.clear();
      this.Alert.warning(this.intMessages.Totscriptsnullswarning);
      return;
    }
    else if ((total < this.QigConfiguration?.RecomendationPoolCount) || (total < this.QigConfiguration?.RecomendationPoolCountPerKP)) {
      this.Alert.clear();
      this.Alert.warning(this.intMessages.Selectcenterswarning);
      return;
    }
    else if (this.QigConfiguration?.RecomendationPoolCount < settingtot) {
      this.Alert.clear();
      this.Alert.warning(this.intMessages.Recommpoolcountlesswarning);
      return;
    }

    this.ProjectConfig.forEach(element => {
      if (element?.Children != null && element?.Children?.length > 0) {
        element?.Children.forEach(child => {
          this.ProjectConfig.push(child);
        });
      }
    });

    this.ProjectsSearchSelectedList?.forEach(element => {
      if (element?.Children != null && element?.Children?.length > 0 && element?.IsSelectedForRecommendation) {
        element?.Children?.forEach(child => {
          this.ProjectsSearchSelectedList?.push(child);
        });
      }
    });

    this.ProjectConfig = [];
    this.QigConfig();

    var AppstngList = this.AppstngList;
    var appsetmodel = new UpdateProjectConfigModel();
    appsetmodel.AppSettingKeyID = this.QigConfiguration.AppSettingKeyIDPoolCount;
    appsetmodel.EntityID = this.QigConfiguration.QIGID;
    appsetmodel.SettingGroupID = AppstngList.SettingGroupID;
    appsetmodel.EntityType = AppSettingEntityType.QIG;
    appsetmodel.Value = this.QigConfiguration.RecomendationPoolCount;
    appsetmodel.ValueType = AppSettingValueType.Int;
    appsetmodel.AppsettingKey = this.QigConfiguration.RecommendationPoolCountAppSettingKey;
    this.ProjectConfig.push(appsetmodel);

    appsetmodel = new UpdateProjectConfigModel();
    appsetmodel.AppSettingKeyID = this.QigConfiguration.AppSettingKeyIDPoolCountPerKP;
    appsetmodel.EntityID = this.QigConfiguration.QIGID;
    appsetmodel.EntityType = AppSettingEntityType.QIG;
    appsetmodel.SettingGroupID = AppstngList.SettingGroupID;
    appsetmodel.ValueType = AppSettingValueType.Int;
    appsetmodel.Value = this.QigConfiguration.RecomendationPoolCountPerKP;
    appsetmodel.AppsettingKey = this.QigConfiguration.RecommendationCountKPAppSettingKey;
    appsetmodel.ValueType = AppSettingValueType.Int;
    this.ProjectConfig.push(appsetmodel);

    this.configsaveloading = true;

    this.stdsetupservice.UpdateProjectConfig(this.ProjectConfig).pipe(first())
      .subscribe({
        next: (data: any) => {
          this.Alert.clear();

          if (data != null && data != undefined) {
            if (data == 'E001') {
              this.Alert.clear();
              this.Alert.error(this.intMessages.Updateconfigerrorwarning);
              this.GetQIGStandardizationScriptSettings(this.activeQig.QigId, false);
            }
            if (data == 'U001' || data == 'S001' || data == 'N001') {
              this.Alert.clear();
              this.Alert.success(this.intMessages.Updateconfigsuccesswarning);
              this.GetQIGStandardizationScriptSettings(this.activeQig.QigId, false);
            }
          }
        },
        error: (err: any) => {
          throw (err);
        },
        complete: () => {
          this.configsaveloading = false;
        }
      });
  }

  validateNumber(event: any) {
    var invalidChars = ["-", "e", "+", "E", "."];
    if (invalidChars.includes(event.key)) {
      event.preventDefault();
    }
  }

}

