import { Component, OnInit, ViewChild  } from '@angular/core';
import { UserManagementService } from 'src/app/services/project/setup/user-management.service';
import { TranslateService } from '@ngx-translate/core';
import {
  UnMappedUsersModel,
  RolesModel,
  SaveMappedUsersModel,
  UnMappedUsersList,
  SearchFilterModel
} from 'src/app/model/project/setup/user-management';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { AlertService } from 'src/app/services/common/alert.service';
import * as moment from 'moment';
import { CommonService } from 'src/app/services/common/common.service';
import { Router } from '@angular/router';
import { PageEvent, MatPaginator } from '@angular/material/paginator';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { NotificationService } from 'src/app/services/common/notification.service';
 

@Component({
  selector: 'emarking-un-mapped-users',
  templateUrl: './un-mapped-users.component.html',
  styleUrls: ['./un-mapped-users.component.css']
})
export class UnMappedUsersComponent implements OnInit {
  serverDate!: Date;
  tempminDate: any;
  constructor(
    public dialog: MatDialog,
    public commonService: CommonService,
    public usermanagementService: UserManagementService,
    public translate: TranslateService,
    private route: Router, 
    public Alert: AlertService,
    public router: Router,
    public notificatonservice: NotificationService
  ) {}
  
  private serverTime = new Date();
  // Pagination
  length = 0;
  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 25, 100];
  showPageSizeOptions = true;
  pageEvent!: PageEvent;
  Objsearchfilter: SearchFilterModel = new SearchFilterModel();
  AvailableUsersValue: string = "";

  unmapedusrlist!: UnMappedUsersList;
  listofunmappeduser!:  UnMappedUsersModel[];
  
  unmappedcount: any;
  Ispageloading: boolean = false;
  dataloading: boolean = false;
  UserSearchList!: UnMappedUsersModel[];


  UsersValue: string = '';
  selected: any[] = [];
  userrolelist!: RolesModel[];
  rolecode: string = '';
  ObjsaveMappedUsers = new SaveMappedUsersModel();
  SelecteduserCount: number = 0;
  minimumdate = new Date();
  tominimumdate = new Date();
  pickfromdate!: any;
  picktodate!: any;
  temppickfromdate!: any;
  temppicktodate!: any;
  Finalselectedlist: any;
  selectedfilterlist: any = 0;
  selectedcount!: any;
  showpopup: boolean = false;
  checkedlst!: number;
  tempRolecode!: string;
  unmappedcompclicked: boolean = false;
  SelectedUsers: any[] = [];
  IsAOExist!: boolean;
  currentpagesize!: number;
  sortdownclicked1: boolean=true;
  sortupclicked1: boolean=true;
  sortupclicked: boolean = true;
  sortdownclicked: boolean = true;
  currentselectedorder!: number;
  currentselectedcolumn!: string;

  intMessages: any = {
    areyousurewanttomapao: ''
  }; 
  
  @ViewChild('matSelect') matSelect: any;
  @ViewChild('closebutton') closebutton: any;
  @ViewChild('paginator') unmappedpaginator!: MatPaginator ;

  ngOnInit(): void {
    this.Objsearchfilter.PageNo = this.pageIndex + 1;
    this.Objsearchfilter.PageSize = this.pageSize;
    this.GetUnMappedUsers(this.Objsearchfilter);
    this.GetRoles();
    this.SelectedUsers = [];
    
    this.translate
      .get('SetUp.Map.areyousurewanttomapao')
      .subscribe((translated: string) => {
        this.intMessages.areyousurewanttomapao = translated;
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
    this.getServerTimeFromService();
  }

  GetRoles() {
    this.usermanagementService.GetRoles().subscribe(data => {
      this.userrolelist = data;
    });
  }

  CallUnMappedComp(unmappedclicked: boolean) {
    this.unmappedpaginator!.pageSize = 10;
    this.unmappedpaginator!.pageIndex = 0;
    this.unmappedcompclicked = unmappedclicked;
    this.UsersValue = '';
    this.ngOnInit();
  }

  GetUnMappedUsers(Objsearchfilter: SearchFilterModel) {  
    this.dataloading = true;
    this.usermanagementService.GetUnMappedUsers(Objsearchfilter).subscribe(
      data => {
        if (data.length > 0 || data != null || data != undefined) {
 
          this.unmapedusrlist = data;
          this.UserSearchList = data.allunmappeduserlist;
          this.listofunmappeduser = data.allunmappeduserlist;
          this.unmappedcount = this.unmapedusrlist.unmappedusercount?.RoleCounts;
          this.length = this.unmapedusrlist.unmappedusercount?.FilterUsersCount;
          this.IsAOExist = this.unmapedusrlist.AoCount;
          this.dataloading = false;
          this.unmapedusrlist.allunmappeduserlist?.forEach(element => {
            element.Isselected = this.SelectedUsers.some(
              y => y.UserId == element.UserId
            );
          });
        }          
        },
      (err: any) => {
        this.dataloading = false;
        throw err;
      },
      () => {
        this.dataloading = false;
      }
    );
  }

  
  handlePageEvent(e: PageEvent) {
    this.length = this.listofunmappeduser.length;
    this.pageEvent = e;
    this.currentpagesize = e.pageSize;
    this.Objsearchfilter.PageNo = e.pageIndex + 1;
    this.Objsearchfilter.PageSize = e.pageSize;
    this.Objsearchfilter.SearchText = this.AvailableUsersValue.trim();
    this.GetUnMappedUsers(this.Objsearchfilter);
  }
  
  SearchScript() {
    this.unmappedpaginator.pageIndex = 0;
    this.Objsearchfilter.PageNo = this.pageIndex + 1;
    if(this.pageEvent!=undefined || this.pageEvent!=null)
    {
      this.Objsearchfilter.PageSize = this.pageEvent.pageSize;
    }
    else{
      this.Objsearchfilter.PageSize =this.pageSize;
    }
    this.Objsearchfilter.SearchText = this.AvailableUsersValue.trim();
    this.GetUnMappedUsers(this.Objsearchfilter);
  }

  getServerTimeFromService() {
    this.tempminDate = this.notificatonservice;
    this.minimumdate=this.tempminDate.serverTime; 
  }
  Clearpopup() {
    this.matSelect.value = '';
    this.UsersValue = '';
    this.pickfromdate = '';
    this.picktodate = '';
  }

  Clear() {
    this.UsersValue = '';
    this.pageEvent = new PageEvent();
    this.pageEvent.pageIndex = 0;
    this.pageIndex = 0;
    this.Objsearchfilter.PageSize = 10;
    this.Objsearchfilter.PageNo = 1;
    this.GetUnMappedUsers(this.Objsearchfilter);
    this.tempRolecode = '';
    this.matSelect.value = '';
    this.pickfromdate = '';
    this.picktodate = '';
    (document.getElementById('FromDate') as HTMLInputElement).value = '';
    (document.getElementById('ToDate') as HTMLInputElement).value = '';
    this.Finalselectedlist = this.unmapedusrlist.allunmappeduserlist.forEach(
      element => {
        element.Isselected = false;
      }
    );
  }


  dateClass() {

      var startDate = new Date(
        new Date(this.pickfromdate).getFullYear(),
        new Date(this.pickfromdate).getMonth(),
        new Date(this.pickfromdate).getDate()
      );
      var endDate = new Date(
        new Date(this.picktodate).getFullYear(),
        new Date(this.picktodate).getMonth(),
        new Date(this.picktodate).getDate()
      );
 
    }
  
  onDataChange() {
    this.dataloading = false;
    this.Alert.clear();
    this.Finalselectedlist = this.unmapedusrlist.allunmappeduserlist.filter(
      x => x.Isselected
    );
    const _ = moment();
    this.checkedlst = 0;
  
    let stdate = new Date(this.pickfromdate);
    let enddate = new Date(this.picktodate); 
    this.temppickfromdate = stdate.toLocaleString();
    this.temppicktodate = enddate.toLocaleString();
    this.orgValueChange(this.temppickfromdate, this.temppicktodate);
  }

  orgValueChange(fDate: Date, tDate: Date) {
    this.SaveMappedUsers(fDate, tDate);
  }

  fromDatechangeEvent(event: MatDatepickerInputEvent<Date>) {
    if (event != null && event.value != null) this.tominimumdate = event.value;
  }


  SaveMappedUsers(fDate: Date, tDate: Date) {
    this.dataloading = true;
    this.tempRolecode = this.matSelect.value;
    if (this.tempRolecode == undefined || this.tempRolecode == '') {
      this.translate
      .get('SetUp.Map.selectarole')
      .subscribe((translated: string) => {
        this.Alert.warning(translated);
      });
      this.dataloading = false;     
    } else if (
      this.pickfromdate == '' ||
      this.pickfromdate == undefined ||
      this.pickfromdate == 'Invalid Date'
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
      this.picktodate == '' ||
      this.picktodate == 'Invalid Date' ||
      this.picktodate == undefined
    ) {
      this.translate
        .get('SetUp.Map.selecttodate')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        }); 
      this.dataloading = false;
      return;
    } else if (this.pickfromdate > this.picktodate) {
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
      mapdetails.UserID = this.SelectedUsers;
      mapdetails.Appointmentstartdate = fDate;
      mapdetails.Appointmentenddate = tDate;
      this.usermanagementService.SaveMapUsers(mapdetails).subscribe({
        next: (data: any) => {
          if (data != undefined || data != null) {
            if (data == 'S001') {
              this.closebutton.nativeElement.click();
              this.route.navigateByUrl(
                '/projects/setup/user-management/MappedUsers'
              ); 
              this.translate
                .get('SetUp.Map.usersmappedsuccefly')
                .subscribe((translated: string) => {
                  this.Alert.success(translated);
                }); 
              this.checkedlst = 0;
              this.Objsearchfilter.PageNo = this.pageIndex + 1;
              this.Objsearchfilter.PageSize = this.pageSize;
              this.GetUnMappedUsers(this.Objsearchfilter);
              this.Clear();
              this.dataloading = false;
              return;
            } else {
              this.translate
                .get('SetUp.Map.errwhilemappingusers')
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

  MapAO(userid: number, RoleName: string) {
    const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        message: this.intMessages.areyousurewanttomapao
      }
    });
    confirmDialog.afterClosed().subscribe(result => {
      this.Alert.clear();
      this.Ispageloading = true;
      var mapdetails = new SaveMappedUsersModel();
      var Userid = [{ UserID: userid }];
      mapdetails.UserID = Userid;
      mapdetails.Rolecode = RoleName;
      if (result === true) {
        this.usermanagementService.MapAO(mapdetails).subscribe({
          next: (data: any) => {
            if (data == 'AOEXT001') {
              this.translate
                .get('SetUp.Map.aoalreadymappedtotheproject')
                .subscribe((translated: string) => {
                  this.Alert.warning(translated);
                });  
            } else if (data == 'S001') {
              this.Objsearchfilter.PageNo = this.pageIndex + 1;
              this.Objsearchfilter.PageSize = this.pageSize;
              this.GetUnMappedUsers(this.Objsearchfilter);
              this.route.navigateByUrl(
                '/projects/setup/user-management/MappedUsers'
              ); 
              document.getElementById('nav-home-tab')!.click();
              this.Clear();
              this.dataloading = false;
              this.translate
                .get('SetUp.Map.aomappedsuccfly')
                .subscribe((translated: string) => {
                  this.Alert.success(translated);
                }); 
              return;
            }
          },
          complete: () => {
            this.Ispageloading = false;
          }
        });
      }
    });
  }
  Oncheckedbutton() {   
    if (!this.IsAOExist) {
      this.translate
        .get('SetUp.Map.aoisnotmappedtotheproject')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    } else if (
      this.SelectedUsers
        .length <= 0
    ) {
      this.translate
        .get('SetUp.Map.pleaselectatlestoneuser')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
      this.showpopup = false;
    } else {
      this.selectedcount = this.SelectedUsers.length;
      document.getElementById('btnMap')!.click();
    }
  }


 

  

  storecheckedvalues(selectedusersdata: any, event: any) {
    if (event.checked) {
      this.SelectedUsers.push(selectedusersdata);
    } else {
      this.SelectedUsers = this.SelectedUsers.filter(
        z => z.UserId !== selectedusersdata.UserId
      );
    }
  }

  SortList(order: number, colname: string) {
    this.currentselectedorder = order;
    this.currentselectedcolumn = colname;
    this.sortdownclicked1 = this.sortupclicked1 = this.sortdownclicked = this.sortupclicked = true;
    if(this.pageEvent!=undefined || this.pageEvent!=null)
    {
      this.Objsearchfilter.PageSize = this.pageEvent.pageSize;
    }
    else{
      this.Objsearchfilter.PageSize =this.pageSize;
    }
    this.Objsearchfilter.SortOrder = this.currentselectedorder;
    this.Objsearchfilter.SortField = this.currentselectedcolumn;
if(order!=null)
{
    if(colname=='username' )
    {      
      this.sortdownclicked = order === 1 ? false : true;
      this.sortupclicked = order === 0 ? false : true;
    }
    else {
      this.sortdownclicked1 = order === 1? false : true;
      this.sortupclicked1 = order === 0? false : true;
    }
  
  }
    this.GetUnMappedUsers(this.Objsearchfilter);
  }
}
