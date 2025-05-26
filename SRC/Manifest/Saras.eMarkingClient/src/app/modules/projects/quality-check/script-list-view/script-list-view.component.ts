import {
  Component,
  Output,
  EventEmitter,
  Input,
  ViewChild,
} from '@angular/core';
import {
  QualityCheckScriptDetails,
  Qualitycheckedbyusers,
  Livepoolscript,
  Scriptsoflivepool,
} from 'src/app/model/project/quality-check/marker-tree-view-model';
import { QualityCheckService } from '../../../../services/project/quality-check/quality-check.service';
import { TranslateService } from '@ngx-translate/core';
import { QigService } from 'src/app/services/project/qig.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { WorkflowStatusTrackingModel } from 'src/app/model/standardisation/Assessment';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import { first } from 'rxjs/operators';
import {
  WorkflowStatus,
  WorkflowProcessStatus,
} from 'src/app/model/common-model';
import { LiveMarkingService } from 'src/app/services/project/live-marking/live-marking.service';
import { QualityCheckComponent } from '../quality-check.component';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'emarking-script-list-view',
  templateUrl: './script-list-view.component.html',
  styleUrls: ['./script-list-view.component.css'],
})
export class ScriptListViewComponent {
  qascriptlist!: QualityCheckScriptDetails[];
  qascriptloading: boolean = true;
  IsQigPause: boolean = false;
  PClosed: boolean = false;
  WorkFlowStatusTracking: WorkflowStatusTrackingModel[] = [];
  intMessages: any = {
    Markingprocesspaused: '',
  };
  isChecked: boolean = false;
  ScriptSearchValue: string = '';
  selected: any[] = [];
  scriptschecked: QualityCheckScriptDetails[] =[];
  scriptslst : any[]=[];
  ReturntoMarker: boolean= false;
  QigID: number = 0;
  activeaccordintab: number = 1;
  activefilterTab: number = 1;
  tabindex = 0;
  activetabfilter = 0;
  CheckoutCount : boolean = false;
  ScriptSearchList!: QualityCheckScriptDetails[];
  typeinfo: number = 1;
  Qualitycheckedbyusers: Qualitycheckedbyusers[] = [];
  Ispageloading: boolean = false;
  scriptsids: Scriptsoflivepool[] = [];
  IsHeaderCheck: boolean = false;
  IsLivePoolEnable: boolean = false;
  userid: number = 0;
  @ViewChild('txtsrch') TxtSearch: any;
  constructor(
    public qaCheckService: QualityCheckService,
    public translate: TranslateService,
    private qigservice: QigService,
    private liveMarkingService: LiveMarkingService,
    public Alert: AlertService,
    public qltychk: QualityCheckComponent,
    private dialog: MatDialog,
  ) {}

  @Output() viewscriptEvent = new EventEmitter<QualityCheckScriptDetails>();
  @Input() rclevel: number = 0;
  sortupclicked: boolean = false;
  sortdownclicked: boolean = false;
  Checkedtypes: any;
  nodedetails: any;

  ngOnInit(): void {
    this.Checkedtypes = [
      {
        Id: 1,
        Text: 'Checked-out',
        Selected: false,
        ischecked: true,
      },
      {
        Id: 2,
        Text: 'Not checked-out',
        Selected: false,
        ischecked: false,
      },
    ];
    this.ScriptSearchValue = '';
  }

  ViewscriptDetails(script: QualityCheckScriptDetails) {
    this.Alert.clear();    
    this.qigservice
      .Getqigworkflowtracking(this.QigID, AppSettingEntityType.QIG)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.WorkFlowStatusTracking = data;

          let swa = this.WorkFlowStatusTracking.filter(
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
                  translated + '<br>' + 'Remarks : ' + projectclosure[0].Remark + '.';
              });
              this.translate
              .get('Quality-Check.ScriptDetailsView.projectclosedalrtmsg')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
            this.ScriptIsinLivePool(
              script.ScriptId,
              script.PhaseStatusTrackingID,
              script
            );

          } else if (swa.length > 0) {
            this.IsQigPause = true;

            this.translate
              .get('Std.SetUp.Markingprocesspaused')
              .subscribe((translated: string) => {
                this.intMessages.Markingprocesspaused =
                  translated +
                  '<br>' +
                  'Remarks : ' +
                  swa[0].Remark +
                  '.';
              });

            this.Alert.warning(
              'This QIG is currently under “Paused state”.' +
                '<br>' +
                'Remarks : ' +
                swa[0].Remark +
                '.'
            );
            this.ScriptIsinLivePool(
              script.ScriptId,
              script.PhaseStatusTrackingID,
              script
            );
          } else {
            this.IsQigPause = false;
            this.PClosed = false;
            this.ScriptIsinLivePool(
              script.ScriptId,
              script.PhaseStatusTrackingID,
              script
            );
          }
        },
        error: (ar: any) => {
          this.qascriptloading = false;
          throw ar;
        },
        complete: () => {
          this.qascriptloading = false;
        },
      });
  }

  getQigTeamScriptListDetails(
    QigId: number,
    usrRoleId: number,
    activeaccordintab: number,
    activefilterTab: number,
    data: any
  ) {  
    this.nodedetails = data;
    this.IsHeaderCheck = false;
    this.tabindex = activeaccordintab;
    this.activetabfilter = activefilterTab;
    this.TxtSearch.nativeElement.value = '';
    this.qascriptloading = true;
    this.QigID = QigId;
    this.userid = usrRoleId;
    this.CheckQigPauseStatus(this.QigID);
    this.clearfilter();
    this.qaCheckService
      .GetTeamStatisticsList(
        QigId,
        usrRoleId,
        activeaccordintab,
        activefilterTab
      )
      .subscribe({
        next: (res: any) => {
          this.qascriptlist = res;
          this.CheckoutCount = this.qascriptlist.some(
            a => a.IsScriptCheckedOut && a.RoleName!='MARKER'
          );
          this.ScriptSearchList = this.qascriptlist;
          if (this.qascriptlist?.length > 0 && this.qascriptlist != undefined) {
            this.sortupclicked = true;
            this.sortdownclicked = false;
          } else {
            this.sortupclicked = false;
            this.sortdownclicked = false;
          }

          if (this.qascriptlist.length > 0) {
            this.IsLivePoolEnable = this.qascriptlist.every(
              (a) => a.IsLivePoolEnable
            );
          } else {
            this.IsLivePoolEnable = false;
          }
        },
        error: (a: any) => {
          this.qascriptloading = false;
          throw a;
        },
        complete: () => {
          this.qascriptloading = false;
        },
      });
  }

  CheckQigPauseStatus(QigID: number) {
    this.Alert.clear();
    this.qigservice
      .Getqigworkflowtracking(QigID, AppSettingEntityType.QIG)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.WorkFlowStatusTracking = data;

          let qw = this.WorkFlowStatusTracking.filter(
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
          } else if (qw.length > 0) {
            this.IsQigPause = true;

            this.translate
              .get('Std.SetUp.Markingprocesspaused')
              .subscribe((translated: string) => {
                this.intMessages.Markingprocesspaused =
                  translated + '<br>' + 'Remarks : ' + qw[0].Remark + '.';
              });

            this.Alert.warning(
              'This QIG is currently under “Paused state”.' +
                '<br>' +
                'Remarks : ' +
                qw[0].Remark +
                '.'
            );
          } else {
            this.IsQigPause = false;
            this.PClosed = false;
          }
        },
        error: (arq: any) => {
          this.qascriptloading = false;
          throw arq;
        },
        complete: () => {
          this.qascriptloading = false;
        },
      });
  }

  SearchScript() {
    var ScriptSearchValue = this.ScriptSearchValue;
    this.qascriptlist = this.ScriptSearchList.filter(function (el) {
      return el.ScriptName.toLowerCase().includes(
        ScriptSearchValue.trim().toLowerCase()
      );
    });
    this.qascriptlist = this.qascriptlist.filter(
      (x) => this.selected.includes(x.ScriptName) || this.selected.length == 0
    );
  }

  SortList(order: number) {
    this.sortupclicked = false;
    this.sortdownclicked = false;

    if (order != null && order == 0) {
      this.sortdownclicked = true;
      this.sortupclicked = false;
      this.qascriptlist.sort(
        (a, b) =>
          new Date(b.ACTIONDATE).getTime() - new Date(a.ACTIONDATE).getTime()
      );
    } else if (order != null && order == 1) {
      this.sortupclicked = true;
      this.sortdownclicked = false;
      this.qascriptlist.sort(
        (a, b) =>
          new Date(a.ACTIONDATE).getTime() - new Date(b.ACTIONDATE).getTime()
      );
    }
  }
  Getcheckedbyuserslist() {
    this.Ispageloading = true;
    this.qaCheckService
      .Getcheckedbyuserslist(this.QigID)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.Qualitycheckedbyusers = data;
        },
        error: (err: any) => {
          throw err;
        },
        complete: () => {
          this.Ispageloading = false;
        },
      });
  }

  Filterdata(event: any) {
    this.selected = [];
    if (this.Checkedtypes.length > 0) {
      var ScriptSearchValue = this.ScriptSearchValue;
      this.Checkedtypes.forEach((element: any) => {
        if (element.Selected) {
          this.selected.push(element.ischecked);
        }
      });

      if (this.selected.length == 0) this.qascriptlist = this.ScriptSearchList;
      else
        this.qascriptlist = this.ScriptSearchList.filter((x) =>
          this.selected.includes(x.IsScriptCheckedOut)
        );
      this.qascriptlist = this.qascriptlist.filter(function (el) {
        return el.ScriptName.toLowerCase().includes(
          ScriptSearchValue.trim().toLowerCase()
        );
      });
    }
  }

  clearfilter() {
    this.selected = [];
    this.Checkedtypes.forEach((pElement: any) => {
      pElement.Selected = false;
    });
  }

  ScriptToBeSubmit(){
    this.ScriptToBeSend(2);
  }

  sendscriptstolivepool() {
     this.ScriptToBeSend(1);
  }

  RevertscripttoPreviousstate(){
    this.ScriptToBeSend(3);
  }

  ScriptToBeSend(num:any){
    this.Ispageloading = true;
    this.qigservice
      .Getqigworkflowtracking(this.QigID, AppSettingEntityType.QIG)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.WorkFlowStatusTracking = data;

          let swa = this.WorkFlowStatusTracking.filter(
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

          } else if (swa.length > 0) {
            this.IsQigPause = true;

            this.translate
              .get('Std.SetUp.Markingprocesspaused')
              .subscribe((translated: string) => {
                this.intMessages.Markingprocesspaused =
                  translated + '<br>' + 'Remarks : ' + swa[0].Remark + '.';
              });

            this.Alert.warning(
              'This QIG is currently under “Paused state”.' +
                '<br>' +
                'Remarks : ' +
                swa[0].Remark +
                '.'
            );
          } else {
            this.IsQigPause = false;
            this.PClosed = false;

            var obj = new Livepoolscript();
            obj.QigId = this.QigID;
            this.scriptsids = [];
            if (this.qascriptlist.length > 0) {
              this.qascriptlist.forEach((element) => {
                if (element.IsChecked) {
                  var sobj = new Scriptsoflivepool();
                  sobj.ScriptId = element.ScriptId;
                  this.scriptsids.push(sobj);
                }

                obj.scriptsids = this.scriptsids;
              });
            } else {
              obj.scriptsids = this.scriptsids;
            }

            if (obj.scriptsids.length > 0) {
              if(num == 1){
                 this.liveMarkingService
                .MoveScriptsToLivePool(obj)
                .pipe(first())
                .subscribe({
                  next: (result: any) => {
                    if (result == 'S001') {
                      this.qltychk.GetQualityCheckTeamSummary(
                        this.QigID,
                        this.userid
                      );
                      this.getQigTeamScriptListDetails(
                        this.QigID,
                        this.userid,
                        this.tabindex,
                        this.activetabfilter,
                        this.nodedetails
                      );

                      this.translate
                      .get('Quality-Check.ScriptListView.scrptsentlivepoolsucc')
                      .subscribe((translated: string) => {
                        this.Alert.success(translated);
                      });
                    }
                  },
                  error: (err: any) => {
                    throw err;
                  },
                  complete: () => {
                    this.Ispageloading = false;
                  },
                });
              }
              if(num == 2){

                var scripts = this.qascriptlist.filter(a => a.IsChecked && a.IsScriptCheckedOut)

                if(scripts.length > 0){
                  this.translate
                  .get('Quality-Check.ScriptListView.selectedchkedscript')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
                  this.Ispageloading = false;
                }
                else{
                  const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
                    data: {
                      message: 'You have selected ' + obj.scriptsids.length + ' script/s to change their status as "Submitted". Are you sure you want to continue? <br/> <br/> This action removes the selected scripts from the RC 1 pool and moves them to the submitted pool. Are you sure you want to continue?',
                    },
                    panelClass: 'confirmationpop',
                  });
                  confirmDialog.afterClosed().subscribe((res: any) => {
               
                    if (res === true) {
                      this.qaCheckService
                  .ScriptToBeSubmit(obj)
                  .pipe(first())
                  .subscribe({
                    next: (result: any) => {
                      if (result == 'S001') {
                        this.qltychk.GetQualityCheckTeamSummary(
                          this.QigID,
                          this.userid
                        );
                        this.getQigTeamScriptListDetails(
                          this.QigID,
                          this.userid,
                          this.tabindex,
                          this.activetabfilter,
                          this.nodedetails
                        );
  
                        this.translate
                        .get('Quality-Check.ScriptListView.rmvrcsuccmsg')
                        .subscribe((translated: string) => {
                          this.Alert.success(translated);
                        });
                      }
                      else{
                        this.qltychk.GetQualityCheckTeamSummary(
                          this.QigID,
                          this.userid
                        );
                        this.getQigTeamScriptListDetails(
                          this.QigID,
                          this.userid,
                          this.tabindex,
                          this.activetabfilter,
                          this.nodedetails
                        );
                        this.Alert.warning(result);
                      }
                    },
                    error: (err: any) => {
                      throw err;
                    },
                    complete: () => {
                      this.Ispageloading = false;
                    },
                  }); 
                     
                    } 
                    else{
                      this.Ispageloading = false;
                    }                 
                  });
                }

              }
              if(num == 3){    
                
                this.scriptschecked = this.qascriptlist.filter(a => a.IsChecked)
                this.scriptslst = this.scriptschecked.filter(a=>!a.IsScriptCheckedOut)
                
                if(this.scriptschecked.length == 1 && this.scriptslst.length == 1)
                {
                  this.translate
                  .get('Quality-Check.ScriptListView.notchecked')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });

                 this.Ispageloading = false;
                } 
                else if(this.scriptschecked.length > 1 && (this.scriptslst.length > 1 || this.scriptslst.length == 1))
                {
                    this.translate
                    .get('Quality-Check.ScriptListView.notcheckedscript')
                    .subscribe((translated: string) => {
                      this.Alert.warning(translated );
                    });
  
                   this.Ispageloading = false;
                  } 

                else {                 
                this.ReturntoMarker = this.scriptschecked.some(a=> a.RoleName== 'MARKER');
                var remarked = this.scriptschecked.some(a=> (a.Phase == 2 || a.Phase == 3 || a.Phase == 4) && a.Scriptstatus==7);
                var remarking =  this.scriptschecked.some(a=>(a.Phase == 2 || a.Phase == 3 || a.Phase == 4)  && a.Scriptstatus==6);
                var ret = this.scriptschecked.some(a=> a.RoleName != 'MARKER');
                 if(this.ReturntoMarker && ret){
                  this.translate
                  .get('Quality-Check.ScriptListView.rtntomrk')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
                  this.Ispageloading = false;
                 }
                
                else if(this.ReturntoMarker){
                  this.translate
                  .get('Quality-Check.ScriptListView.rtntomrkr')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
                  this.Ispageloading = false;
                }

                else if(remarked){
                  this.translate
                  .get('Quality-Check.ScriptListView.resubmitted')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
                  this.Ispageloading = false;
                }

                else if(remarking){
                  this.translate
                  .get('Quality-Check.ScriptListView.remarking')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
                  this.Ispageloading = false;
                }

                else{this.qaCheckService
                  .RevertCheckout(this.scriptschecked)
                  .pipe(first())
                  .subscribe({
                    next: (result: any) => {                     
                      if (result == 'S001') {

                        this.qltychk.GetQualityCheckTeamSummary(
                          this.QigID,
                          this.userid
                        );
                        this.getQigTeamScriptListDetails(
                          this.QigID,
                          this.userid,
                          this.tabindex,
                          this.activetabfilter,
                          this.nodedetails
                        );
  
                        this.translate
                        .get('Quality-Check.ScriptListView.revert')
                        .subscribe((translated: string) => {
                          this.Alert.success(translated);
                        });
                    }
                   },
                  error: (err: any) => {
                    throw err;
                  },
                  complete: () => {
                    this.Ispageloading = false;
                  },
                });
               }
             }
           }
            
            } else {

              this.translate
              .get('Quality-Check.ScriptListView.selectatleastonescriptalrtmsg')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });

              this.Ispageloading = false;
            }
          }
        },
        error: (ar: any) => {
          this.qascriptloading = false;
          throw ar;
        },
        complete: () => {
          this.qascriptloading = false;
        },
      });
  }



  CheckAll(evt: any) {
    if (evt.checked) {
      this.qascriptlist.forEach((element) => {
        element.IsChecked = true;
      });
    } else {
      this.qascriptlist.forEach((element) => {
        element.IsChecked = false;
      });
    }
  }

  UnCheck(evt: any) {
    if (this.qascriptlist.every((a) => a.IsChecked)) {
      this.IsHeaderCheck = true;
    } else {
      this.IsHeaderCheck = false;
    }
  }

  ScriptIsinLivePool(
    ScriptId: number,
    PhaseTrackingId: number,
    script: QualityCheckScriptDetails
  ) {
    this.liveMarkingService
      .CheckScriptIsLivePool(ScriptId, PhaseTrackingId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data == 'SU001') {
            this.viewscriptEvent.emit(script);
          } else {
            this.getQigTeamScriptListDetails(
              this.QigID,
              this.userid,
              this.tabindex,
              this.activetabfilter,
              this.nodedetails
            );
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
          this.qascriptloading = false;
        },
      });
  }
}
