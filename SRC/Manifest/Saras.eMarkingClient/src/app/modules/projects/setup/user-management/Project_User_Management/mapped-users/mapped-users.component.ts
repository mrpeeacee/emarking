import {
  Component,
  OnInit,
  ViewChild,
  Output,
  EventEmitter
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { UserManagementService } from 'src/app/services/project/setup/user-management.service';
import { TranslateService } from '@ngx-translate/core';
import { AlertService } from 'src/app/services/common/alert.service';
import {
  GetAllMappedUsersModel,
  MappedUsersCount,
  MappedUsersList,
  SearchFilterModel,
  UnMappedUsersList,
  UnMappedUsersModel,
  UnMapAodetails,
  RolesModel,
  SaveMappedUsersModel,
  ChangeRoleModel,
  CreateEditProjectUserRoleChange,
  SupervisorRoledetails,
  Qigsupervisorroledetails
} from 'src/app/model/project/setup/user-management';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { CommonService } from 'src/app/services/common/common.service';
import { first } from 'rxjs/operators';
import { PerfectScrollbarComponent } from 'ngx-perfect-scrollbar';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { PageEvent, MatPaginator } from '@angular/material/paginator';

@Component({
  selector: 'emarking-mapped-users',
  templateUrl: './mapped-users.component.html',
  styleUrls: ['./mapped-users.component.css']
})
export class MappedUsersComponent implements OnInit {
  template: any;
  sortdownclicked1: boolean = true;
  sortupclicked1: boolean = true;

  constructor(
    public dialog: MatDialog,
    public commonService: CommonService,
    public usermanagementService: UserManagementService,
    public translate: TranslateService,
    public Alert: AlertService
  ) { }

  @ViewChild('paginator') paginator!: MatPaginator;
  @ViewChild('closeComponentUnmapusers') closemodalpopup: any;
  @ViewChild('closeremappopup') closeremappopup: any;
  @ViewChild('perfectScroll') perfectScroll!: PerfectScrollbarComponent;

  // Pagination
  length = 0;
  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 25, 100];
  pageEvent!: PageEvent;
  showPageSizeOptions = true;
  Objsearchfilter: SearchFilterModel = new SearchFilterModel();
  dataloading: boolean = false;
  mappeduserlist!: GetAllMappedUsersModel;
  mappedusercount!: MappedUsersCount;
  AvailableUsersValue: string = "";
  usersList!: MappedUsersList;
  IsmodelShow!: any;
  ProjectsSearchList!: GetAllMappedUsersModel[];
  OnlyAOresult!: GetAllMappedUsersModel[];
  ListOfOnlyAo!: GetAllMappedUsersModel[];
  ProjectUsersSearchedList!: GetAllMappedUsersModel[];
  currentpagesize!: number;
  unmapedusrlist!: UnMappedUsersList;
  unmapedAOusrlist!: UnMappedUsersModel[];
  mappedcount!: number;
  totcountbtnstyle: boolean = true;
  eobtnstyle: boolean = false;
  aobtnstyle: boolean = false;
  cmbtnstyle: boolean = false;
  acmbtnstyle: boolean = false;
  tlbtnstyle: boolean = false;
  atlbtnstyle: boolean = false;
  markerbtnstyle: boolean = false;
  unmappedusercountbtnstyle: boolean = false;
  enableanchortag: boolean = true;
  countsloading: boolean = false;
  UsersValue: string = '';
  AoValue: string = '';
  editselectuser!: GetAllMappedUsersModel;
  minimumdate = new Date();
  tominimumdate = new Date();
  pickfromdate!: any;
  picktodate!: any;
  temppickfromdate!: any;
  temppicktodate!: any;
  SuperadminCount?: any;
  ServiceadminCount?: any;
  AOcount?: any;
  CMcount?: any;
  ACMcount?: any;
  TLcount?: any;
  ATLcount?: any;
  Markerscount?: any;
  UnMappedUserscount?: any;
  EOcount?: any;
  RoleName?: string = '';
  rolecode: string = '';
  UnmappedAO!: string;
  tempRolecode!: string;
  SelectedUsers: any;
  userrolelist!: RolesModel[];
  remappeduserId!: any;
  IsS1Enabled!: boolean;
  IsLiveMarkingEnabled!: boolean;
  currentloginrolecode!: string;
  currentselectedrolecode!: string;
  userslstloading: boolean = false;
  currentselectedorder!: number;
  currentselectedcolumn!: string;
  currentpageindexnumber!: number;
  UnmappedUserSelected: boolean = false;
  filteredRoleMapUser: string = "";
  //for Unmapping AO
  selectedAO!: number;
  selectedrolename!: string;
  selectedusername!:string;
  selectedValue!: number;
  Suspendstatus: boolean = false;
  sortupclicked: boolean = true;
  sortdownclicked: boolean = true;
  @Output() UnmappedClick = new EventEmitter<boolean | undefined>();
  @ViewChild('matSelect') matSelect: any;
  UserDoneActivity: boolean = false;
  UserIsnotdoneActivity: boolean = false;
  @ViewChild('changerolebutton') closebutton: any;
  @ViewChild('mytemplateclose') mytemplateclose: any;
  @ViewChild('qigsupervisorid') qigsupervisorid: any
  changerolemodel: ChangeRoleModel = new ChangeRoleModel();
  filterroledetails?: SupervisorRoledetails[];
  selectedval!: any;
  supervisorselectedval!: any;
  projectuserroleid!: number;
  beforeuserroleorder!: any;
  aftereuserroleorder!: any;
  qigsupervisordetails: Qigsupervisorroledetails[] = [];
  qigid: any;

  intMessages: any = {
    Suspendalertmessage: '',
    Resumealertmessage: '',
    areyousureyouwantounmapusers: ''
  };
  ngOnInit(): void {
    this.Objsearchfilter.PageNo = this.pageIndex + 1;
    this.currentpageindexnumber = this.pageIndex + 1;
    this.Objsearchfilter.PageSize = this.pageSize;
    this.getmappedusers(this.Objsearchfilter);
    this.GetRoles();
    this.UsersValue = '';
    this.AoValue = '';
    this.getunmappedusers(this.Objsearchfilter);

    this.translate
      .get('SetUp.Map.areyousureyouwantounmapusers')
      .subscribe((translated: string) => {
        this.intMessages.areyousureyouwantounmapusers = translated;
      });
    this.translate
      .get('Project User Management')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });

    this.translate
      .get('SetUp.UserManagement.mapuserpagedesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
  }

  GetFilteredRoles(Rolecode: string, selectedroletype: boolean) {
    this.filteredRoleMapUser = Rolecode;
    this.UsersValue = '';
    this.paginator.pageIndex = 0;
    this.Objsearchfilter.PageNo = this.pageIndex + 1;
    if (this.pageEvent != undefined || this.pageEvent != null) {
      this.Objsearchfilter.PageSize = this.pageEvent.pageSize;
    }
    else {
      this.Objsearchfilter.PageSize = this.pageSize;
    }
    this.Objsearchfilter.RoleCode = Rolecode;
    this.currentselectedrolecode = Rolecode;
    this.Objsearchfilter.SearchText = this.UsersValue.trim();
    this.Objsearchfilter.UnmappedUserCountbit = selectedroletype;
    this.getmappedusers(this.Objsearchfilter);
    this.totcountbtnstyle = false;
    this.eobtnstyle = false;
    this.aobtnstyle = false;
    this.cmbtnstyle = false;
    this.acmbtnstyle = false;
    this.tlbtnstyle = false;
    this.atlbtnstyle = false;
    this.markerbtnstyle = false;
    this.unmappedusercountbtnstyle = false;
    if (Rolecode == '' && selectedroletype) {
      this.enableanchortag = true;
      this.unmappedusercountbtnstyle = true;
      this.UnmappedUserSelected = true;
    } else {
      this.UnmappedUserSelected = false;
      this.enableanchortag = true;
      this.totcountbtnstyle = Rolecode === '';
      this.aobtnstyle = Rolecode === 'EO' || Rolecode === 'AO';
      this.cmbtnstyle = Rolecode === 'CM';
      this.acmbtnstyle = Rolecode === 'ACM';
      this.tlbtnstyle = Rolecode === 'TL';
      this.atlbtnstyle = Rolecode === 'ATL';
      this.markerbtnstyle = Rolecode === 'MARKER';
    }
  }

  getmappedusers(Objsearchfilter: SearchFilterModel) {   
    this.userslstloading = true;
    this.ListOfOnlyAo = [];
    this.ProjectUsersSearchedList = [];
    this.usermanagementService.GetMappedUsers(Objsearchfilter).subscribe(
      data => {
        if (data.length > 0 || data != null || data != undefined) {
          this.usersList = data;
          this.currentloginrolecode = data.currentloginrolecode;
          this.OnlyAOresult = data.OnlyAOresult;
          this.ListOfOnlyAo = data.OnlyAOresult;
          this.ProjectsSearchList = data.allmappeduserlist;
          this.ProjectUsersSearchedList = data.allmappeduserlist;
          this.mappedcount = this.usersList.mappedusercount.reduce(
            (prev, next) => prev + next.RoleCounts,
            0
          );
          this.AOcount = this.usersList.mappedusercount.filter(
            z => z.RoleCode == 'Assessment Officer'
          )[0].RoleCounts;
          this.CMcount = this.usersList.mappedusercount.filter(
            z => z.RoleCode == 'Chief Marker'
          )[0].RoleCounts;
          this.ACMcount = this.usersList.mappedusercount.filter(
            z => z.RoleCode == 'Assistant Chief Marker'
          )[0].RoleCounts;
          this.TLcount = this.usersList.mappedusercount.filter(
            z => z.RoleCode == 'Team Lead'
          )[0].RoleCounts;
          this.ATLcount = this.usersList.mappedusercount.filter(
            z => z.RoleCode == 'Assistant Team Lead'
          )[0].RoleCounts;
          this.Markerscount = this.usersList.mappedusercount.filter(
            z => z.RoleCode == 'Marker'
          )[0].RoleCounts;
          this.length = data.FilterUserCount;
          this.UnMappedUserscount = this.usersList.UnMappedUserscount;
        }
      },
      (err: any) => {
        throw err;
      },
      () => {
        this.userslstloading = false;
      }
    );
  }

  SearchScript() {
    this.paginator.pageIndex = 0;
    this.Objsearchfilter.PageNo = this.pageIndex + 1;
    this.Objsearchfilter.PageSize = this.currentpagesize;
    this.Objsearchfilter.RoleCode = this.currentselectedrolecode;
    this.Objsearchfilter.SearchText = this.UsersValue.trim();
    this.getmappedusers(this.Objsearchfilter);
  }

  SearchAOlist() {
    var AvailableUsersValue = this.AvailableUsersValue;

      this.ListOfOnlyAo = this.OnlyAOresult.filter(function (el) {
        return (el.LoginName.toLowerCase().includes(AvailableUsersValue.trim().toLowerCase()))       
      });
    // }
    // else {
    //   this.ListOfOnlyAo = this.OnlyAOresult.filter(function (el) {
    //     return (el.LoginName.toLowerCase().includes(AvailableUsersValue.trim().toLowerCase()) )
    //   });
    // }
  }

  //this is only for AO
  getUnmapProjectUserRoleID(
    UnmapProjectUserRoleID: number,
    RoleName: string,
    mytemplate: any,
    Name:string,
    IsS1Enabled: boolean,
    IsLiveMarkingEnabled: boolean
  ) {
    this.selectedValue = 0;
    this.template = mytemplate;
    this.selectedAO = UnmapProjectUserRoleID;
    this.selectedrolename = RoleName;
    this.selectedusername = Name;
    this.IsS1Enabled = IsS1Enabled;
    this.IsLiveMarkingEnabled = IsLiveMarkingEnabled;

    if (this.IsS1Enabled && !this.IsLiveMarkingEnabled) {
      this.openModal(mytemplate);
    } else if (this.IsLiveMarkingEnabled && !this.IsS1Enabled) {
      this.openModal(mytemplate);
    } else if (this.IsLiveMarkingEnabled && this.IsS1Enabled) {
      this.openModal(mytemplate);
    } else if (!this.IsS1Enabled && !this.IsLiveMarkingEnabled) {
      this.perfectScroll?.directiveRef!.update();
      this.perfectScroll?.directiveRef!.scrollToTop(0, 0);
      this.perfectScroll?.directiveRef!.scrollToLeft(0, 0);
      document.getElementById('ancshowAOlist')!.click();
    }
  }

  openModal(templateRef: any) {
    this.dialog.open(templateRef, {
      panelClass: 'alert_class',
      width: '450px'
    });
  }

  //this is for Marking Personel
  UnMapMarkingpersonelUsers(
    UnmapProjectUserRoleID: number,
    RoleName: string,
    mytemplate: any,
    IsS1Enabled: boolean,
    Name:string,
    IsLiveMarkingEnabled: boolean
  ) {
    this.template = mytemplate;
    this.selectedrolename = RoleName;
    this.selectedusername = Name;
    this.IsS1Enabled = IsS1Enabled;
    this.IsLiveMarkingEnabled = IsLiveMarkingEnabled;
    if (this.IsS1Enabled && !this.IsLiveMarkingEnabled) {
      this.openModal(mytemplate);
    } else if (this.IsLiveMarkingEnabled && !this.IsS1Enabled) {
      this.openModal(mytemplate);
    } else if (this.IsLiveMarkingEnabled && this.IsS1Enabled) {
      this.openModal(mytemplate);
    } else if (!this.IsS1Enabled && !this.IsLiveMarkingEnabled) {
      this.abc(UnmapProjectUserRoleID, mytemplate, 0);
    }
  }

  abc(
    UnmapProjectUserRoleID: number,
    mytemplate: any,
    ReplacementUserID: number
  ) {
    if (!this.Suspendstatus) {
      this.dataloading = true;
      var Unmapaodetail = new UnMapAodetails();
      if (ReplacementUserID == 0) {
        Unmapaodetail.UnmapProjectUserRoleID = UnmapProjectUserRoleID;
        Unmapaodetail.ReplacementUserID = 0;
      } else {
        Unmapaodetail.UnmapProjectUserRoleID = this.selectedAO;
        Unmapaodetail.ReplacementUserID = this.selectedValue;
      }

      if (this.dataloading != null) {
        this.dataloading = true;
        const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
          data: {
            message: this.intMessages.areyousureyouwantounmapusers
          }
        });
        confirmDialog.afterClosed().subscribe(result => {
          this.Alert.clear();
          if (result === true) {
            this.dataloading = true;
            this.usermanagementService
              .UnMapParticularUsers(Unmapaodetail)
              .subscribe({
                next: (data: any) => {
                  if (data == 'S001') {
                    this.translate
                      .get('SetUp.Map.usersunmappedsuccessfully')
                      .subscribe((translated: string) => {
                        this.Alert.clear();
                        this.Alert.success(translated);
                      });

                    this.closemodalpopup.nativeElement.click();
                    //document.getElementById("nav-profile-tab")!.click();
                    this.getmappedusers(this.Objsearchfilter);
                  } else {
                    this.translate
                      .get('SetUp.Map.unmappingofuserfailed')
                      .subscribe((translated: string) => {
                        this.Alert.clear();
                        this.Alert.warning(translated);
                      });
                  }
                },
                complete: () => {
                  this.dataloading = false;
                }
              });
          }
          this.dataloading = false;
        });
      } else {
        this.translate.get('UnmappedFailed').subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
      }
    }
  }

  UnMapParticularAO(mytemplate: any) {
    if (this.selectedValue == 0 || this.selectedValue == undefined) {
      this.translate
        .get('SetUp.Map.pleaselectareportingAO')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
    } else {
      this.template = mytemplate;

      this.dataloading = true;
      var Unmapaodetail = new UnMapAodetails();
      Unmapaodetail.UnmapProjectUserRoleID = this.selectedAO;
      Unmapaodetail.ReplacementUserID = this.selectedValue;

      if (
        !this.Suspendstatus ||
        Unmapaodetail.ReplacementUserID != undefined ||
        Unmapaodetail.ReplacementUserID != 0
      ) {
        if (this.dataloading != null) {
          this.dataloading = true;
          const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
            data: {
              message: this.intMessages.areyousureyouwantounmapusers
            }
          });
          confirmDialog.afterClosed().subscribe(result => {            
            this.Alert.clear();
            if (result === true) {            
              this.dataloading = true;
              this.usermanagementService
                .UnMapParticularUsers(Unmapaodetail)
                .subscribe({
                  next: (data: any) => {
                    if (data == 'S001') {
                      this.translate
                        .get('SetUp.Map.aousersunmappedsuccessfully')
                        .subscribe((translated: string) => {
                          this.Alert.clear();
                          this.Alert.success(translated);
                        });
                      this.closemodalpopup.nativeElement.click();

                      var Objsearchfilter = new SearchFilterModel();
                      Objsearchfilter.PageNo = 0;
                      Objsearchfilter.PageSize = 100;
                      this.dataloading = false;
                      this.GetFilteredRoles(this.filteredRoleMapUser, false);
                      document.getElementById('nav-home-tab')!.click();
                    } else {
                      this.translate
                        .get('SetUp.Map.unmapofuser')
                        .subscribe((translated: string) => {
                          this.Alert.clear();
                          this.Alert.warning(translated);
                        });
                    }
                  },
                  complete: () => {
                    this.dataloading = false;
                  }
                });
            }
            this.dataloading = false;
          });
        } else {
          this.translate
            .get('SetUp.Map.unmapofuser')
            .subscribe((translated: string) => {
              this.Alert.clear();
              this.Alert.warning(translated);
            });
        }
      } else {
        alert('SetUp.Map.plselectao');
      }
    }
  }

  GetRoles() {
    this.usermanagementService.GetRoles().subscribe(data => {
      this.userrolelist = data;
    });
  }

  Clearpopup() {
    debugger
    this.matSelect.value = '';
    this.UsersValue = '';
    this.pickfromdate = '';
    this.picktodate = '';
    this.AvailableUsersValue = '';
    this.ListOfOnlyAo = this.OnlyAOresult;
  }

  Onremapuserclick(remappeduserId: number) {
    this.remappeduserId = remappeduserId;
    document.getElementById('remapuser')!.click();
  }

  fromDatechangeEvent(event: MatDatepickerInputEvent<Date>) {
    if (event != null && event.value != null) this.tominimumdate = event.value;
  }

  SaveReMappedUsers() {
    this.dataloading = true;
    this.tempRolecode = this.matSelect.value;
    let StartDate = new Date(this.pickfromdate);
    this.pickfromdate = new Date(StartDate.getFullYear(), StartDate.getMonth(), StartDate.getDate());
    let EndDate = new Date(this.picktodate);
    this.picktodate = new Date(EndDate.getFullYear(), EndDate.getMonth(), EndDate.getDate());

    this.Alert.clear();

    if (this.tempRolecode == undefined || this.tempRolecode == '') {
      this.Alert.clear();
      this.translate
        .get('SetUp.Map.selectarole')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
      this.dataloading = false;
    } else if (
      this.pickfromdate == '' ||
      this.pickfromdate == 'Invalid Date' ||
      this.pickfromdate == undefined
    ) {
      this.Alert.clear();
      this.translate
        .get('SetUp.Map.selectfrmdate')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
      this.dataloading = false;
      return;
    } else if (
      this.pickfromdate == '' ||
      this.picktodate == 'Invalid Date' ||
      this.picktodate == undefined
    ) {
      this.Alert.clear();
      this.translate
        .get('SetUp.Map.selecttodate')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
      this.dataloading = false;
      return;
    } else if (this.pickfromdate > this.picktodate) {
      this.Alert.clear();
      this.translate
        .get('SetUp.Map.todatecannotbefromdate')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
      this.dataloading = false;
      return;
    } else {
      var mapdetails = new SaveMappedUsersModel();
      mapdetails.Rolecode = this.tempRolecode;
      var Userid = [{ UserID: this.remappeduserId }];
      mapdetails.UserID = Userid;
      mapdetails.Appointmentstartdate = this.pickfromdate;
      mapdetails.Appointmentenddate = this.picktodate;
      this.usermanagementService
        .SaveMapUsers(mapdetails)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data != undefined || data != null) {
              if (data == 'S001') {
                this.translate
                  .get('SetUp.Map.remapsucessmessage')
                  .subscribe((translated: string) => {
                    this.Alert.success(translated);
                  });

                this.closeremappopup.nativeElement.click();
                document.getElementById('totalusers')!.click();
                this.getmappedusers(this.Objsearchfilter);
                this.dataloading = false;
                return;
              } else {
                this.translate
                  .get('SetUp.Map.errormessgewhilemapping')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
              }
            }
          },
          error: (a: any) => {
            this.dataloading = false;
            throw a;
          },
          complete: () => {
            this.dataloading = false;
          }
        });
    }
  }

  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.length = e.length;
    this.Objsearchfilter.PageNo = e.pageIndex + 1;
    this.Objsearchfilter.PageSize = e.pageSize;
    this.currentpagesize = e.pageSize;
    this.Objsearchfilter.SearchText = this.UsersValue.trim();
    this.Objsearchfilter.SortOrder = this.currentselectedorder;
    this.Objsearchfilter.SortField = this.currentselectedcolumn;
    this.getmappedusers(this.Objsearchfilter);
  }

  SortList(order: number, colname: string) {
    this.currentselectedorder = order;
    this.currentselectedcolumn = colname;
    this.sortdownclicked1 = this.sortupclicked1 = this.sortdownclicked = this.sortupclicked = true;
    if (this.pageEvent != undefined || this.pageEvent != null) {
      this.Objsearchfilter.PageSize = this.pageEvent.pageSize;
    }
    else {
      this.Objsearchfilter.PageSize = this.pageSize;
    }
    this.Objsearchfilter.SortOrder = this.currentselectedorder;
    this.Objsearchfilter.SortField = this.currentselectedcolumn;
    if (order != null) {
      if (colname == 'username') {
        this.sortdownclicked = order === 1 ? false : true;
        this.sortupclicked = order === 0 ? false : true;
      }
      else {
        this.sortdownclicked1 = order === 1 ? false : true;
        this.sortupclicked1 = order === 0 ? false : true;
      }

    }
    this.getmappedusers(this.Objsearchfilter);
  }

  getunmappedusers(Objsearchfilter: SearchFilterModel) {
    this.dataloading = true;
    this.Objsearchfilter.PageSize = 0;
    this.AvailableUsersValue = "";
    this.usermanagementService.GetUnMappedUsers(Objsearchfilter).subscribe(
      data => {
        if (data.length > 0 || data != null || data != undefined) {
          this.dataloading = true;
          this.unmapedusrlist = data;
          this.unmapedAOusrlist = this.unmapedusrlist.allunmappeduserlist.filter(
            z => z.RoleCode == 'AO'
          );
          this.dataloading = false;
        }
      },
      (err: any) => {
        this.dataloading = false;
        throw err;
      },
      () => {
        this.dataloading = false;
        this.perfectScroll?.directiveRef!.update();
        this.perfectScroll?.directiveRef!.scrollToTop(0, 0);
        this.perfectScroll?.directiveRef!.scrollToLeft(0, 0);
      }
    );
  }

  CheckACtivityOfUser(projuserroleid: number, rolename: string, mytemplate: any,username:any) {
    this.UserIsnotdoneActivity = false;
    this.UserDoneActivity = false;
    this.changerolemodel = new ChangeRoleModel();
    this.projectuserroleid = projuserroleid;
    this.usermanagementService.CheckActivityOfMP(projuserroleid).subscribe(
      data => {
        if (data != null || data != undefined) {
          this.userslstloading = true;
          this.changerolemodel.currentuserrolecode = rolename;
          this.changerolemodel.currentusername = username;

          if (data.IsActivityExists) {
            this.changerolemodel.IsActivityExists = data.IsActivityExists;
            this.UserDoneActivity = true;
            this.UserIsnotdoneActivity = false;
            this.closebutton.nativeElement.click();
            this.openModal(mytemplate);
          }
          else {
            this.changerolemodel.IsActivityExists = data.IsActivityExists;
            this.changerolemodel.QIGdetails = data.QIGdetails;
            this.filterroledetails = data.SupervisorRoledetails;
            this.beforeuserroleorder = data.Roledetails?.filter((a: { RoleCode: string; }) => a.RoleCode == rolename).map((a: { Order: any; }) => a.Order);
            this.changerolemodel.Roledetails = data.Roledetails.filter((a: { RoleCode: string; }) => a.RoleCode != rolename);
            this.UserIsnotdoneActivity = true;
            this.UserDoneActivity = false;
            this.mytemplateclose.nativeElement.click();
          }
        }
      },
      (err: any) => {
        this.userslstloading = false;
        throw err;
      },
      () => {
        this.userslstloading = false;
      }
    );
  }

  OnChangRole(event: any) {
    this.qigsupervisordetails = [];
    this.selectedval = event.target.value;
    this.aftereuserroleorder = this.changerolemodel.Roledetails?.filter(a => a.RoleCode == this.selectedval).map(a => a.Order);

    if (this.changerolemodel?.QIGdetails != null) {
      this.changerolemodel?.QIGdetails.forEach(element => {
        if (element.SupervisorRoledetails?.length > 0) {
          element.SupervisorRoledetails = element.SupervisorRoledetails?.filter(a => a.Order < this.aftereuserroleorder);
        }
      });
    }
  }

  OnChangSupervisorRole(event: any, qigid: any) {
    this.supervisorselectedval = Number(event.target.value);
    if (this.changerolemodel?.QIGdetails != null) {
      this.changerolemodel?.QIGdetails?.forEach(element => {
        if (element.SupervisorRoledetails?.length > 0) {
          element.SupervisorRoledetails?.filter(a => a.ProjectUserRoleID == this.supervisorselectedval && a.QIGID == qigid).forEach(elem => {
            this.qigsupervisordetails.push({
              ProjectUserRoleID: this.projectuserroleid,
              ProjectQIGID: elem.QIGID,
              ReportingTo: elem.ProjectUserRoleID
            });
          })
        }
      });
    }
  }

  UpdateChangeRoleDetails() {
    this.Alert.clear();
    this.dataloading = true;

    if (this.qigsupervisorid != undefined) {
      var supervisoeval = this.qigsupervisorid.nativeElement.value;
    }
    if ((this.selectedval == null || this.selectedval == undefined) && (this.changerolemodel?.Roledetails != null)) {
      this.Alert.clear();
      this.translate
        .get('SetUp.Map.ChangeRole.warning1')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
      this.dataloading = false;
      return;
    }
    else if ((this.changerolemodel?.QIGdetails != null && supervisoeval == 0) && (this.supervisorselectedval == 0 || this.supervisorselectedval == undefined)) {
      this.Alert.clear();
      this.translate
        .get('SetUp.Map.ChangeRole.warning2')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
      this.dataloading = false;
      return;
    }
    else if (supervisoeval != 0 && (this.supervisorselectedval == 0 || this.supervisorselectedval == undefined || this.qigsupervisordetails?.length == 0)) {
      this.changerolemodel?.QIGdetails?.forEach(ele => {
        if (ele.SupervisorRoledetails.length > 0) {
          ele.SupervisorRoledetails?.filter(a => a.ProjectUserRoleID == ele.ReportingTo && a.QIGID == ele.ProjectQIGID).forEach(elem => {
            this.qigsupervisordetails.push({
              ProjectUserRoleID: this.projectuserroleid,
              ProjectQIGID: elem.QIGID,
              ReportingTo: elem.ProjectUserRoleID
            });
          })
        }
      })
    }

    var userchangerolemodel = new CreateEditProjectUserRoleChange();
    userchangerolemodel.RoleCode = this.selectedval;
    userchangerolemodel.ProjectUserRoleID = this.projectuserroleid;
    userchangerolemodel.ChangeType = this.beforeuserroleorder > this.aftereuserroleorder ? 1 : 2;
    userchangerolemodel.qigsupervisorroledetails = this.qigsupervisordetails;

    this.usermanagementService.CreateEditProjectUserRoleChange(userchangerolemodel).pipe(first()).subscribe({
      next: (data: any) => {
        if (data != null && data != undefined) {
          if (data == 'S001') {
            this.Alert.clear();
            this.translate
              .get('SetUp.Map.ChangeRole.success')
              .subscribe((translated: string) => {
                this.Alert.success(translated);
              });
            setTimeout(() => {
              this.getmappedusers(this.Objsearchfilter);
            }, 100);
            this.closebutton.nativeElement.click();
            return;
          }
          else if (data == 'E001') {
            this.Alert.clear();
            this.translate
              .get('SetUp.Map.ChangeRole.warning3')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
            return;
          }
          else if (data == 'E002') {
            this.Alert.clear();
            this.translate
              .get('SetUp.Map.ChangeRole.warning4')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
            return;
          }
        }
      },
      error: (err: any) => {
        this.translate
          .get('SetUp.Map.ChangeRole.error')
          .subscribe((translated: string) => {
            this.Alert.error(translated);
          });
        throw (err);
      }, complete: () => {
        this.dataloading = false;
        this.qigsupervisordetails = [];
      }
    });
  }
}
