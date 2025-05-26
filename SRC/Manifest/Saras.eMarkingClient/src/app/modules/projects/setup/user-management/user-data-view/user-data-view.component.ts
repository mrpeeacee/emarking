import { ChangeDetectorRef, Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ProjectUsersModel, Qiguserdataviewmodel, QigUsersViewByIdModel, CopyMarkingTeamCls, SuspendUserDetails } from 'src/app/model/project/setup/user-management';
import { AlertService } from 'src/app/services/common/alert.service';
import { UserManagementService } from 'src/app/services/project/setup/user-management.service';
import { UserCountsComponent } from '../user-counts/user-counts.component';
import { first } from 'rxjs/operators';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { CopymarkingteamComponent } from '../copymarkingteam/copymarkingteam.component';
import { StdSetupService } from 'src/app/services/project/standardisation/std-one/std-setup.service';
import { MoveMarkingTeamComponent } from '../move-marking-team/move-marking-team.component';

@Component({
  selector: 'emarking-user-data-view',
  templateUrl: './user-data-view.component.html',
  styleUrls: ['./user-data-view.component.css']
})
export class UserDataViewComponent implements OnInit {
  @ViewChild(UserCountsComponent) usercountcomponent!: UserCountsComponent;
  @Output() ShowHierarchyClick = new EventEmitter<(boolean | undefined)>();
  @Output() ShowDataClick = new EventEmitter<(boolean | undefined)>();
  @Output() DeleteClick = new EventEmitter<(boolean | undefined)>();
  @ViewChild('closebutton') closebutton: any;
  @ViewChild('close') close: any;
  @ViewChild('matSelect') matSelect: any;
  @ViewChild('closesuspendmodal') closesuspendmodal: any;
  @ViewChild('untapopupclose') closeuntagpopup: any;



  @ViewChild("taguser") taguser: any;
  @ViewChild("reportinguser") reportinguser: any;
  @ViewChild("retagreportingroleid") retagreportinguser: any;


  hideuders: boolean = false;

  SuspendUserDetails!: SuspendUserDetails[];

  RetagUsers!: SuspendUserDetails[];

  ReportingUsers: SuspendUserDetails[] = [];

  untagparam = new SuspendUserDetails();

  n!: SuspendUserDetails[];

  selected: number = 0;

  selected1: any = [];

  selectedValueOption: string = "0";
  selectedValueReporting: string = "0";
  value!: any;

  untagroleid: number = 0;
  retagroleId: number = 0;

  rbvalue = 0;

  reportingname: string = "";

  reportingmngrdisabled = false;
  tempreportingmngrdisabled = false;
  IsQIGVal!: boolean;

  constructor(private dialog: MatDialog, public usermanagementService: UserManagementService,
    public translate: TranslateService, private cdref: ChangeDetectorRef,
    public Alert: AlertService, private route: Router, public stdsetupservice: StdSetupService) { }

  ngOnInit(): void {

    this.translate.get('SetUp.UserManagement.Deletealertmessage').subscribe((translated: string) => {
      this.intMessages.Deletealertmessage = translated;
    });
    this.translate.get('SetUp.UserManagement.Blockalertmessage').subscribe((translated: string) => {
      this.intMessages.Blockalertmessage = translated;
    });
    this.translate.get('SetUp.UserManagement.Unblockalertmessage').subscribe((translated: string) => {
      this.intMessages.Unblockalertmessage = translated;
    });
    this.translate.get('SetUp.UserManagement.getusererror').subscribe((translated: string) => {
      this.intMessages.getusererror = translated;
    });
    this.translate.get('SetUp.UserManagement.userupdate').subscribe((translated: string) => {
      this.intMessages.userupdate = translated;
    });
    this.translate.get('SetUp.UserManagement.userdeleteerror').subscribe((translated: string) => {
      this.intMessages.userdeleteerror = translated;
    });
    this.translate.get('SetUp.UserManagement.userupdatefail').subscribe((translated: string) => {
      this.intMessages.userupdatefail = translated;
    });
    this.translate.get('SetUp.UserManagement.useralredydeleted').subscribe((translated: string) => {
      this.intMessages.useralredydeleted = translated;
    });
    this.translate.get('SetUp.UserManagement.filename').subscribe((translated: string) => {
      this.intMessages.filename = translated;
    });
    this.translate.get('SetUp.UserManagement.login').subscribe((translated: string) => {
      this.intMessages.login = translated;
    });
    this.translate.get('SetUp.UserManagement.reporting').subscribe((translated: string) => {
      this.intMessages.reporting = translated;
    });
    this.translate.get('SetUp.UserManagement.role').subscribe((translated: string) => {
      this.intMessages.role = translated;
    });
    this.translate.get('SetUp.UserManagement.save').subscribe((translated: string) => {
      this.intMessages.save = translated;
    });
    this.translate.get('SetUp.UserManagement.s1error').subscribe((translated: string) => {
      this.intMessages.s1error = translated;
    });
    this.translate.get('SetUp.UserManagement.live').subscribe((translated: string) => {
      this.intMessages.live = translated;
    });
    this.translate.get('SetUp.UserManagement.reportingtoempty').subscribe((translated: string) => {
      this.intMessages.reportingtoempty = translated;
    });
    this.translate.get('SetUp.UserManagement.Suspendalertmessage').subscribe((translated: string) => {
      this.intMessages.Suspendalertmessage = translated;
    });
    this.translate.get('SetUp.UserManagement.Resumealertmessage').subscribe((translated: string) => {
      this.intMessages.Resumealertmessage = translated;
    });

    this.translate.get('SetUp.UserManagement.retaguserconfirmalrt').subscribe((translated: string) => {
      this.intMessages.Retagusercnfrmalrtmsg = translated;
    });
  }

  selectedObject: any;
  textTypes: any;
  status!: string;
  qigusersdataviewlist!: Qiguserdataviewmodel[];
  AvailableUsersValue: string = "";
  ProjectSearchSelectedValue: string = "";
  ProjectsSearchList!: Qiguserdataviewmodel[];
  ProjectUsersSearchedList!: Qiguserdataviewmodel[];
  UsersSearchedList!: Qiguserdataviewmodel[];
  showhide: boolean = true;
  dataviewshowhide: boolean = false;
  databtnstylechange: boolean = true;
  hierachybtnstylechange: boolean = false;
  dataloading: boolean = false;
  formloading: boolean = false;
  rolecode: string = '';
  activeQig: number = 0;
  activeQigName!:string;
  projectusersviewlist!: ProjectUsersModel[];
  Qiguserslist!: QigUsersViewByIdModel;
  hierarchyid!: number;
  selectedval!: any;
  selectedFood1!: string;
  S1started: boolean = false;
  Livemarkingstarted: boolean = false;
  Suspendstatus: boolean = false;
  Suspendprojectuserid!: number;
  SuspendQigid!: number;
  Suspendloading: boolean = false;
  intMessages: any = {
    Deletealertmessage: '',
    Blockalertmessage: '',
    Unblockalertmessage: '',
    getusererror: '',
    userupdate: '',
    userdeleteerror: '',
    userupdatefail: '',
    useralredydeleted: '',
    filename: '',
    login: '',
    reporting: '',
    role: '',
    save: '',
    s1error: '',
    live: '',
    reportingtoempty: '',
    Suspendalertmessage: '',
    Resumealertmessage: '',
    selectanyoneradiobutton: '',
    Retagusercnfrmalrtmsg: ''
  };
  deletestatus: boolean = false;
  Suspendremarks!: string;

  getQigusersdatatview(activeQig: any) {
    this.activeQig = activeQig;
    this.dataloading = true;
    this.databtnstylechange = true;
    this.hierachybtnstylechange = false;
    var objQiguserdata = new Qiguserdataviewmodel();
    objQiguserdata.ProjectUserRoleID = 0; 
    objQiguserdata.QIGId = activeQig;
    this.AvailableUsersValue = "";
    this.ProjectsSearchList = [];
    this.usermanagementService.QigdataorHierarchyview(objQiguserdata).subscribe(data => {
      if (data.length > 0 || data != null || data != undefined) {
        this.dataloading = true;
        this.qigusersdataviewlist = data;
        this.ProjectsSearchList = data;
        this.ProjectUsersSearchedList = data;

        setTimeout(() => {
          this.CheckS1StartedOrLiveMarkingEnabled(this.activeQig);
        }, 100);
      }
    }, (err: any) => {
      throw (err)
    }, () => {
      this.dataloading = false;
    });
  }

  CheckS1StartedOrLiveMarkingEnabled(QigId: number) {
    this.S1started = false;
    this.Livemarkingstarted = false;
    this.usermanagementService.CheckS1StartedOrLiveMarkingEnabled(QigId).pipe(first()).subscribe({
      next: (data: any) => {
        if (data != null || data != undefined) {
          if (data == 'S1Started') {
            this.S1started = true;
            this.Livemarkingstarted = false;
          }
          else if (data == 'LiveMarkingStarted') {
            this.Livemarkingstarted = true;
            this.S1started = false;
          }
        }
      },
      error: (err: any) => {
        throw (err);
      }
    });
  }

  openModal(templateRef: any) {
    this.dialog.open(templateRef, {
      panelClass: 'alert_class',
      width: '450px'
    });
  }

  SearchAvailableUsers() {
    var AvailableUsersValue = this.AvailableUsersValue;
    var that = this.rolecode;
    if (that != '') {
      this.ProjectUsersSearchedList = this.ProjectsSearchList.filter(function (el) {
        return (
          (el.UserName.toLowerCase().includes(AvailableUsersValue.trim().toLowerCase()) ||
            el.LoginName.toLowerCase().includes(AvailableUsersValue.trim().toLowerCase())) &&
          (el.RoleID.toLowerCase().trim() == that.trim())
        )
      });
    }
    else {
      this.ProjectUsersSearchedList = this.ProjectsSearchList.filter(function (el) {
        return (
          el.UserName.toLowerCase().includes(AvailableUsersValue.trim().toLowerCase()) ||
          el.LoginName.toLowerCase().includes(AvailableUsersValue.trim().toLowerCase())
        )
      });
    }
  }

  FilterQigUsers(role: string) {
    this.AvailableUsersValue = "";
    this.rolecode = role;
    this.ProjectUsersSearchedList = this.qigusersdataviewlist.filter(a => a.RoleID.toLowerCase() == role.toLowerCase() || role == '');
  }

  ShowDataComponent(showhide: boolean, IsQIG: boolean = false) {
    this.IsQIGVal = IsQIG;
    this.showhide = showhide;
    this.dataviewshowhide = false;
  }

  ShowHideDataComponent(showhideval: boolean) {
    this.dataviewshowhide = showhideval;
    this.showhide = false;
  }

  ShowHierarchyView(showhideval: boolean) {
    this.databtnstylechange = false;
    this.hierachybtnstylechange = true;
    this.ShowHierarchyClick.emit(showhideval);
  }

  ShowDataView(showhideval: boolean) {
    this.databtnstylechange = true;
    this.hierachybtnstylechange = false;
    this.ShowDataClick.emit(showhideval);
  }

  GotoUploadComp() {
    this.route.navigateByUrl('projects/setup/user-management/' + this.activeQig + '/user-import');
  }

  getQigUsersById(dataviewuserlist: any, mytemplate: any) {
    if (this.S1started || this.Livemarkingstarted) {
      this.openModal(mytemplate);
    }
    else {
      if (this.Qiguserslist != undefined) {
        this.Qiguserslist.ReportingToID = 0;
      }
      this.formloading = true;
      this.hierarchyid = dataviewuserlist.ProjectQIGTeamHierarchyID;
      this.usermanagementService.GetQiguserDataById(this.activeQig, dataviewuserlist.ProjectQIGTeamHierarchyID).pipe(first()).subscribe({
        next: (data: any) => {
          if (data != null && data != undefined) {
            this.Qiguserslist = data;
          }
        },
        error: (err: any) => {
          this.Alert.error(this.intMessages.getusererror);
          throw (err);
        }, complete: () => {
          this.formloading = false;
        }
      });
    }
  }

  UpdateQigUsersById() {
    this.selectedval = this.matSelect.value;
    this.Alert.clear();
    this.dataloading = true;
    this.formloading = true;
    if (this.selectedval == null) {
      this.Alert.clear();
      this.Alert.warning(this.intMessages.reportingtoempty);
      this.dataloading = false;
      this.formloading = false;
      return;
    }
    this.usermanagementService.UpdateQiguserDataById(this.activeQig, this.hierarchyid, this.selectedval).pipe(first()).subscribe({
      next: (data: any) => {
        if (data != null && data != undefined) {
          if (data == "U001") {
            this.Alert.clear();
            this.Alert.success(this.intMessages.userupdate);
            this.getQigusersdatatview(this.activeQig);
            this.closebutton.nativeElement.click();
            return;
          }
          else {
            this.Alert.clear();
            this.Alert.warning(this.intMessages.userupdatefail);
            return;
          }
        }
      },
      error: (err: any) => {
        this.Alert.error(this.intMessages.userdeleteerror);
        throw (err);
      }, complete: () => {
        this.dataloading = false;
        this.formloading = false;
      }
    });

  }

  DeleteUsers(UserRoleId: number, mytemplate: any) {
    if (this.S1started || this.Livemarkingstarted) {
      this.openModal(mytemplate);
    }
    else {
      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: this.intMessages.Deletealertmessage
        },
      });
      confirmDialog.afterClosed().subscribe(result => {
        this.deletestatus = false;
        this.Alert.clear();
        if (result === true) {
          this.Alert.clear();
          this.dataloading = true;
          this.usermanagementService.UserDelete(UserRoleId, this.activeQig)
            .pipe(first())
            .subscribe({
              next: (data: any) => {
                if (data == "S001") {
                  this.deletestatus = true;
                  this.translate
                    .get('SetUp.UserManagement.Userdeletesuccess')
                    .subscribe((translated: string) => {
                      this.Alert.clear();
                      this.Alert.success(translated);
                      this.getQigusersdatatview(this.activeQig);
                      this.callcountcomponent();
                    });
                }
                else {
                  this.translate
                    .get('SetUp.UserManagement.Userdeletefailed')
                    .subscribe((translated: string) => {
                      this.Alert.clear();
                      this.Alert.error(translated);
                    });
                }
              },
              complete: () => {
                this.dataloading = false;
              },
            });
        }
      });
    }
  }

  callcountcomponent() {
    if (this.deletestatus) {
      this.DeleteClick.emit();
    }
  }

  GetSuspendUsers(dataviewuserlist: any) {
    if (dataviewuserlist != null && dataviewuserlist != undefined) {
      this.Suspendremarks = '';
      this.Suspendloading = true;
      this.Suspendprojectuserid = dataviewuserlist.ProjectUserRoleID;
      this.SuspendQigid = this.activeQig;

      this.usermanagementService.GetSuspendUsers(dataviewuserlist.ProjectUserRoleID, 1)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.Suspendstatus = data;
          },
          complete: () => {
            this.Suspendloading = false;
          },
        });
    }
  }

  GetResumeUsers(dataviewuserlist: any) {
    const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        message: this.intMessages.Resumealertmessage
      },
    });
    confirmDialog.afterClosed().subscribe(result => {
      this.Suspendloading = false;
      this.Alert.clear();
      if (result === true) {
        this.Alert.clear();
        this.Suspendloading = true;
        var suspenduser = new SuspendUserDetails();
        suspenduser.QigId = this.activeQig;
        suspenduser.Remarks = this.Suspendremarks;
        suspenduser.ProjectUserRoleID = dataviewuserlist.ProjectUserRoleID;
        suspenduser.Status = false;
        this.usermanagementService.SuspendUsers(suspenduser)
          .pipe(first())
          .subscribe({
            next: (data: any) => {
              if (data == "S001") {
                this.translate
                  .get('SetUp.UserManagement.UserResumedsuccess')
                  .subscribe((translated: string) => {
                    this.Alert.clear();
                    this.Alert.success(translated);
                    this.deletestatus = true;
                    this.callcountcomponent();
                    this.getQigusersdatatview(this.activeQig);
                  });
              }
              else {
                this.translate
                  .get('SetUp.UserManagement.UserResumefailed')
                  .subscribe((translated: string) => {
                    this.Alert.clear();
                    this.Alert.error(translated);
                  });
              }
            },
            complete: () => {
              this.Suspendloading = false;
            },
          });
      } else {
        this.Suspendloading = false;
      }
    });
  }

  SaveSuspendUsers() {
    var suspenduser = new SuspendUserDetails();
    suspenduser.QigId = this.SuspendQigid;
    suspenduser.Remarks = this.Suspendremarks;
    suspenduser.ProjectUserRoleID = this.Suspendprojectuserid;
    suspenduser.Status = true;

    if (this.Suspendstatus && this.Suspendremarks != '') {
      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: this.intMessages.Suspendalertmessage
        },
      });
      confirmDialog.afterClosed().subscribe(result => {
        this.Suspendloading = false;
        this.Alert.clear();
        if (result === true) {
          this.Alert.clear();
          this.Suspendloading = true;
          this.usermanagementService.SuspendUsers(suspenduser)
            .pipe(first())
            .subscribe({
              next: (data: any) => {
                if (data == "S001") {
                  this.translate
                    .get('SetUp.UserManagement.UserSuspendsuccess')
                    .subscribe((translated: string) => {
                      this.Alert.clear();
                      this.Alert.success(translated);
                      this.deletestatus = true;
                      this.callcountcomponent();
                      this.getQigusersdatatview(this.activeQig);
                      this.closesuspendmodal.nativeElement.click();
                    });
                }
                else {
                  this.translate
                    .get('SetUp.UserManagement.UserSuspendfailed')
                    .subscribe((translated: string) => {
                      this.Alert.clear();
                      this.Alert.error(translated);
                    });
                }
              },
              complete: () => {
                this.Suspendloading = false;
              },
            });
        }
        else {
          this.Suspendloading = false;
        }
      });
    }
    else if (this.Suspendremarks == '') {
      this.Suspendloading = false;
      this.translate
        .get('SetUp.UserManagement.reamrksalert')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
    }
    else if (!this.Suspendstatus) {
      this.Alert.clear();
      this.Suspendloading = true;
      if (suspenduser.Remarks != '') {
        this.usermanagementService.SuspendUsers(suspenduser)
          .pipe(first())
          .subscribe({
            next: (data: any) => {
              if (data == "S001") {
                this.translate
                  .get('SetUp.UserManagement.UserSuspendsuccess')
                  .subscribe((translated: string) => {
                    this.Alert.clear();
                    this.Alert.success(translated);
                    this.getQigusersdatatview(this.activeQig);
                    this.deletestatus = true;
                    this.callcountcomponent();
                    this.closesuspendmodal.nativeElement.click();
                  });
              }
              else if (data == "E002") {
                this.Alert.clear();
                this.translate
                  .get('SetUp.UserManagement.actionnull')
                  .subscribe((translated: string) => {
                    this.Alert.clear();
                    this.Alert.warning(translated);
                  });
              }
              else if (data == "E001") {
                this.Suspendloading = false;
                this.translate
                  .get('SetUp.UserManagement.UserSuspendfailed')
                  .subscribe((translated: string) => {
                    this.Alert.clear();
                    this.Alert.error(translated);
                  });
              }
            },
            complete: () => {
              this.Suspendloading = false;
            },
          });
      }
      else {
        this.Suspendloading = false;
        this.translate
          .get('SetUp.UserManagement.reamrksalert')
          .subscribe((translated: string) => {
            this.Alert.clear();
            this.Alert.warning(translated);
          });
      }
    }
  }


  BlockorUnblockUsers(UserRoleId: number, Isactive: boolean, mytemplate: any) {
    if (this.S1started || this.Livemarkingstarted) {
      this.openModal(mytemplate);
    }
    else {
      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: Isactive ? this.intMessages.Blockalertmessage : this.intMessages.Unblockalertmessage
        },
      });
      confirmDialog.afterClosed().subscribe(result => {
        this.deletestatus = false;
        this.Alert.clear();
        if (result === true) {
          this.Alert.clear();
          this.dataloading = true;
          this.usermanagementService.BlockorUnblockUsers(UserRoleId, Isactive, this.activeQig)
            .pipe(first())
            .subscribe({
              next: (data: any) => {
                if (data == "BLK001") {
                  this.translate
                    .get('SetUp.UserManagement.Userblockedsuccess')
                    .subscribe((translated: string) => {
                      this.deletestatus = true;
                      this.Alert.clear();
                      this.Alert.success(translated);
                      this.getQigusersdatatview(this.activeQig);
                      this.callcountcomponent();
                    });
                }
                else if (data == "BLK002") {
                  this.translate
                    .get('SetUp.UserManagement.Userunblockedsuccess')
                    .subscribe((translated: string) => {
                      this.Alert.clear();
                      this.deletestatus = true;
                      this.Alert.success(translated);
                      this.getQigusersdatatview(this.activeQig);
                      this.callcountcomponent();
                    });
                }
                else {
                  this.translate
                    .get('SetUp.UserManagement.Userdeletefailed')
                    .subscribe((translated: string) => {
                      this.Alert.clear();
                      this.Alert.error(translated);
                    });
                }
              },
              complete: () => {
                this.dataloading = false;
              },
            });
        }
      });
    }
  }

  DownloadUser(dataviewuserlist: any) {
    var objQiguserdata = new Qiguserdataviewmodel();
    objQiguserdata.ProjectUserRoleID = dataviewuserlist.ProjectUserRoleID;
    objQiguserdata.UserName = dataviewuserlist.UserName;
    objQiguserdata.QIGId = this.activeQig;
    this.usermanagementService.QigdataorHierarchyview(objQiguserdata).subscribe(data => {
      if (data.length > 0 || data != null || data != undefined) {

        this.UsersSearchedList = data;

        this.download(this.UsersSearchedList);
      }
    }, (err: any) => {
      throw (err)
    }, () => {
      this.dataloading = false;
    });
  }

  download(UsersSearchedList: Qiguserdataviewmodel[]) {
    let fileName = this.intMessages.filename;
    let columnNames = [this.intMessages.login, this.intMessages.role, this.intMessages.reporting];
    let header = columnNames.join(',');

    let csv = header;
    csv += '\r\n';

    UsersSearchedList.forEach(element => {
      var data = [
        {
          "LoginName": element.LoginID,
          "Role": element.RoleID,
          "ReportingTo": element.ReportingToLoginName
        }
      ];

      data.forEach(c => {
        csv += [c["LoginName"], c["Role"], c["ReportingTo"]].join(',');
        csv += '\r\n';
      })

    });

    var blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });

    var link = document.createElement("a");
    if (link.download !== undefined) {
      var url = URL.createObjectURL(blob);
      link.setAttribute("href", url);
      link.setAttribute("download", fileName);
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  }

  CopyMarkingTeam() {
    var cd = new CopyMarkingTeamCls();
    cd.QigId = this.activeQig;

    const confirmDialog = this.dialog.open(CopymarkingteamComponent, {
      data: cd,
    });
    confirmDialog.afterClosed().subscribe(result => {
      if (result) {
        this.dataloading = true;
        this.getQigusersdatatview(this.activeQig);
        this.callcountcomponent();
      }
    });
  }

  MoveMarkingTeam() {
    var cd = new CopyMarkingTeamCls();
    cd.QigId = this.activeQig;

    const confirmDialog = this.dialog.open(MoveMarkingTeamComponent, {
      data: cd,
    });

    confirmDialog.afterClosed().subscribe(result => {
      console.log(result);
    })

  }

  getUsers(dataviewuserlist: any) {
    this.selected = 0;
    this.tempreportingmngrdisabled = false;
    var obj = new SuspendUserDetails();
    obj.ProjectUserRoleID = 0;
    obj.FirstName = "select user";
    this.ReportingUsers.push(obj);
    this.selectedValueOption = "0";
    this.selectedValueReporting = "0";
    this.value = null;
    this.hideuders = false;
    this.untagroleid = dataviewuserlist.ProjectUserRoleID;
    this.reportingname = dataviewuserlist.ReportingTousernamename;
    const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        message: "You are about to untag " + dataviewuserlist.UserName + " from this QIG. On doing so,<br/>" +
          "(i) The reportees of " + dataviewuserlist.UserName + " will have to report to another supervisor <br/> " +
          "(ii) If " + dataviewuserlist.UserName + " has any scripts pending or not started in their work space, they will be returned to the live pool <br/>" +
          "(iii) If " + dataviewuserlist.UserName + " has checked out any scripts in Quality check, the check-out will be undone.<br/>" +
          "(iv) If " + dataviewuserlist.UserName + " has a few re-marked scripts to be reviewed, they will be assigned to the replacement supervisor."

      }
    });

    confirmDialog.afterClosed().subscribe(result => {

      this.Alert.clear();

      if (result === true) {
        this.usermanagementService.Untaguserhaschildusers(this.untagroleid, this.activeQig).pipe(first())
          .subscribe({
            next: (data: any) => {

              if (data) {

                this.GetupperUser();
                document.getElementById("apopup")!.click();

              }
              else {

                this.untagparam.unmapProjectUserId = this.untagroleid;
                this.untagparam.replacementPURId = 0;
                this.untagparam.QigId = this.activeQig;
                this.untagparam.ReportingTo = 0;

                this.UntagUser(this.untagparam);
              }

            },
            error: (error: any) => {
              console.log(error);
            },
            complete: () => {
              console.log(result);
            }
          })
      }

    })
  }

  GetupperUser() {
    this.usermanagementService.GetUpperHierarchyRole(this.untagroleid, this.activeQig)
      .pipe(first()).subscribe({
        next: (data: any) => {
          this.SuspendUserDetails = data;
        },
        error: (error: any) => {
          console.log(error);
        },
        complete: () => {
          this.dataloading = false;
        }
      })
  }
  GetReportingUser(event: any) {
    var roleId = Number(event.value);
    this.usermanagementService.GetReportingToHierarchy(roleId, this.activeQig)
      .pipe(first()).subscribe({
        next: (data: any) => {
          this.n = data;
          if (this.n != null && this.n != undefined) {
            if (this.n.filter(a => a.CurrentReportingTo).length > 0) {
              this.selected = (this.n.filter(a => a.CurrentReportingTo)[0].ProjectUserRoleID);
              this.reportingmngrdisabled = true;
              this.tempreportingmngrdisabled=true;
            }
            else {
              this.selected = 0;
              this.reportingmngrdisabled = false;   
              this.tempreportingmngrdisabled=false;
            }
          }
          else {
            this.selected = 0;
            this.reportingmngrdisabled = false;
            this.tempreportingmngrdisabled=false;
          }
          this.ReportingUsers = [];

          if (this.n != null && this.n != undefined) {
            this.ReportingUsers = data;
          }


          var obj = new SuspendUserDetails();
          obj.ProjectUserRoleID = 0;
          obj.FirstName = "select user";
          obj.roleCode = "";

          this.ReportingUsers.push(obj);
        },
        error: (error: any) => {
          console.log(error);
        },
        complete: () => {
          this.dataloading = false;
        }
      })
  }

  ngAfterContentChecked(): void {
    this.cdref.detectChanges();
  }

  UntagUser(untagparam: any) {

    this.hideuders = false;
    this.selectedValueOption = "0";
    this.selectedValueReporting = "0"

    this.value = null;


    this.usermanagementService.UntagqigUsers(untagparam).pipe(first())
      .subscribe({

        next: (data: any) => {

          if (data == "S001") {

            this.translate
              .get('SetUp.UserManagement.untauseralrtmsg')
              .subscribe((translated: string) => {
                this.Alert.success(translated);
              });
          }

          this.getQigusersdatatview(this.activeQig);

          this.closeuntagpopup.nativeElement.click();

        },
        error: (error: any) => {
          console.log(error);
        },
        complete: () => {
          console.log(untagparam);
        }
      })

  }


  GetRetagUser(dataviewuserlist: any) {
    this.selectedValueOption = "0";
    this.value = null;



    this.retagroleId = dataviewuserlist.ProjectUserRoleID;
    const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        message: this.intMessages.Retagusercnfrmalrtmsg
      }
    });
    confirmDialog.afterClosed().subscribe(result => {
      this.Alert.clear();
      if (result === true) {
        this.usermanagementService.GetReTagUpperHierarchyRole(dataviewuserlist.ProjectUserRoleID, this.activeQig)
          .pipe(first()).subscribe({
            next: (data: any) => {
              this.RetagUsers = data;
            },
            error: (error: any) => {
              console.log(error);
            },
            complete: () => {
              this.dataloading = false;
            }
          })
        document.getElementById("aretagpopup")!.click();
      }
    });
  }


  RetagUser() {
    var reportingTo = Number(this.retagreportinguser.value);
    var rolecode = "AO";
    if(reportingTo==0)
    {
      this.translate
        .get('Please select a user to Retag')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    }
    else
    {
    this.usermanagementService.ReTagQigUser(this.retagroleId, this.activeQig, reportingTo, rolecode).pipe(first())
      .subscribe({
        next: (data: any) => {
          
          if (data == "S001") {

            this.translate
              .get('SetUp.UserManagement.retagsuccessalrtmsg')
              .subscribe((translated: string) => {
                this.Alert.success(translated);
              });

            this.getQigusersdatatview(this.activeQig);
            this.closesuspendmodal.nativeElement.click();
          }    
        },
        error: (error: any) => {
          console.log(error);
        },
        complete: () => {
          console.log(rolecode);
        }
      })
    }
  }

  DefaultUser() {
    this.hideuders = false;
  }

  ReplaceAnotherUser() {
    this.hideuders = true;
  }


  ReplaceWithNewUser() { 
    this.Alert.clear();
    if (this.rbvalue == 1) {

      this.untagparam.unmapProjectUserId = this.untagroleid;
      this.untagparam.replacementPURId = 0;
      this.untagparam.QigId = this.activeQig;
      this.untagparam.ReportingTo = 0;

      this.UntagUser(this.untagparam);

    }
    else if (this.rbvalue == 2 && this.validation(Number(this.taguser.value))) {

      this.untagparam.unmapProjectUserId = this.untagroleid;
      this.untagparam.replacementPURId = Number(this.taguser.value);
      this.untagparam.QigId = this.activeQig;
      if(this.tempreportingmngrdisabled)
      {
        this.untagparam.ReportingTo = Number(this.reportinguser.value);
      }
      else
      {
        this.untagparam.ReportingTo = 0;
      }
      this.UntagUser(this.untagparam);
    }
    else if (this.rbvalue == 0) {

      this.translate
        .get('SetUp.UserManagement.selectanyoneradiobtn')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    }

  }

  private validation(num: number): boolean {
    if (num == 0) {
      this.translate
        .get('SetUp.UserManagement.rplcmntuseralrtmsg')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
      return false;
    }
    return true;
  }
}



