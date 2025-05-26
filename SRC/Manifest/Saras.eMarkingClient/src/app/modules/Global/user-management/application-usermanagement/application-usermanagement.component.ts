import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { GetAllApplicationUsersModel, RoleDetails, RoleSchooldetails, SchoolDetails, SearchFilterModel } from 'src/app/model/Global/UserManagement/UserManagementModel';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { GlobalUserManagementService } from 'src/app/services/Global/UserManagement/UserManagementService';
import { CreateUserComponent } from '../create-user/create-user.component';
import { first } from 'rxjs';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';




@Component({
  selector: 'emarking-application-usermanagement',
  templateUrl: './application-usermanagement.component.html',
  styleUrls: ['./application-usermanagement.component.css']
})
export class ApplicationUsermanagementComponent implements OnInit {
  @ViewChild('paginator') paginator!: MatPaginator;
  @ViewChild('menutrigger') menuclose!: ElementRef;
  allappltnuserslst!: GetAllApplicationUsersModel;
  userslstloading: boolean = false;
  Changestatusloading: boolean = false;
  ScriptSearchValue: string = '';
  statuslst: any;
  ChangeStatus: any;
  ChangeStatusValue: any;
  type: number = 0;
  RemoveEnable: boolean = false;
  strngMessage: string = '';
  strngButonMessage: any;
  isCicked = false;
  createusercomp!: CreateUserComponent;

  roleschooldetails!: RoleSchooldetails;

  searchUserForm!: FormGroup;

  schoolcodes: string = "";
  rolecodes: string = "";
  userstatus: string = "";

  // Pagination
  length = 0;
  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 25, 100];
  showPageSizeOptions = true;
  pageEvent!: PageEvent;
  objSearch: SearchFilterModel = new SearchFilterModel();

  sortupclicked: boolean = true;
  sortdownclicked: boolean = true;

  constructor(public globalusermanagementservice: GlobalUserManagementService, public translate: TranslateService, private fb: FormBuilder,
    private dialog: MatDialog, public Alert: AlertService, private route: Router, public commonService: CommonService) { }

  ngOnInit(): void {

    this.translate.get('usermanage.Acmtitle').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('usermanage.Acmdesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });


    this.objSearch.PageNo = 1;
    this.objSearch.PageSize = this.pageSize;
    this.GetAllusers(0);
    this.GetApplicationLevelUserRoles();

    this.statuslst = [
      {
        Id: 1,
        Text: "",
        Selected: false,
        ischecked: true
      },
      {
        Id: 2,
        Text: "",
        Selected: false,
        ischecked: true
      },
      {
        Id: 3,
        Text: "",
        Selected: false,
        ischecked: true
      },
      {
        Id: 4,
        Text: "",
        Selected: false,
        ischecked: true
      }

    ]
    this.statuslst.forEach((element: any) => {
      if (element.Id == 1) {
        this.translate
          .get('usermanage.active')
          .subscribe((translated: string) => {
            element.Text = translated
          });
      }
      if (element.Id == 2) {
        this.translate
          .get('usermanage.inactive')
          .subscribe((translated: string) => {
            element.Text = translated
          });
      }
      if (element.Id == 3) {
        this.translate
          .get('usermanage.blocked')
          .subscribe((translated: string) => {
            element.Text = translated
          });
      }
      if (element.Id == 4) {
        this.translate
          .get('usermanage.disabled')
          .subscribe((translated: string) => {
            element.Text = translated
          });
      }
    });
    this.ChangeStatus =
    {
      Active: 1,
      InActive: 2,
      Disable: 3,
      Enable: 4,
      Remove: 5
    }
    this.searchUserForm = this.fb.group({
      frole: new FormControl(''),
      fuserstatus: new FormControl(''),
      fschool: new FormControl('')
    });
  }

  GetAllusers(navigate:number) {  
    this.userslstloading = true;
    this.objSearch.navigate=navigate;
    this.globalusermanagementservice.GetAllUsers(this.objSearch).subscribe({
      next: (data: GetAllApplicationUsersModel) => {       
        this.allappltnuserslst = data;
        if (data != null && data.getAllUsersModel.length > 0) {
          this.length = this.allappltnuserslst.getAllUsersModel[0]!.ROWCOUNT;
        }
        else {
          this.length = 0;
        }

      },
      complete: () => {
        this.userslstloading = false;
      },
    });
  }

  SearchScript() {
    this.paginator.pageIndex = 0;
    this.objSearch.SearchText = this.ScriptSearchValue.trim();
    this.objSearch.PageNo = 1;
    this.GetAllusers(1);

  }

  GotoCreateuser() {
    this.route.navigateByUrl('userManagement/CreateUser');
  }

  GotoBlockeduser() {
    this.route.navigate(['userManagement/blocked-users']);
  }

  GotoImportuser() {
    this.route.navigate(['userManagement/ImportUser']);
  }

  Edituser(UserId: number) {
    this.route.navigateByUrl('userManagement/CreateUser/' + UserId);
  }

  ResetPwd(UserId: any) {
    var ConfirmMessage = "";
    this.translate
      .get('shared.confirmMessageOfResetPassword')
      .subscribe((translated: string) => {
        ConfirmMessage = translated
      });
    const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        message: ConfirmMessage
      }
    });
    confirmDialog.afterClosed().subscribe(result => {
      this.Alert.clear();
      if (result === true) {
        this.userslstloading = true;
        this.ScriptSearchValue = '';
        this.globalusermanagementservice.Resetpwd(UserId).subscribe({
          next: (data: any) => {
            if (data != null || data != undefined) {
              if (data.Status == "SUCC001" && data.IsMailSent) {
                this.translate
                  .get('usermanage.ResetSuccessful')
                  .subscribe((translated: string) => {
                    this.Alert.success(translated);
                  });
                this.GetAllusers(1);
              }
              else if (data.Status == "SUCC001" && !data.IsMailSent) {
                this.translate
                  .get('usermanage.ResetSuccessfulErrorSendEmail')
                  .subscribe((translated: string) => {
                    this.Alert.success(translated);
                  });
                this.GetAllusers(1);
              }
              else if (data.Status == "ERR01") {
                this.translate
                  .get('usermanage.ResetError')
                  .subscribe((translated: string) => {
                    this.Alert.error(translated);
                  });
                this.userslstloading = false;
              }
            }
          },
          complete: () => {
            this.userslstloading = false;
          },
        });

      }
    });
  }

  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.length = e.length;
    this.objSearch.PageNo = e.pageIndex + 1;
    this.objSearch.PageSize = e.pageSize;
    this.GetAllusers(1);
  }

  StatusOfRemove(UserId: number) {
    this.Changestatusloading = true;
    this.globalusermanagementservice.StatusOfRemove(UserId).subscribe({
      next: (result: any) => {
        if (result == "MAP001")
          this.RemoveEnable = false;
        else
          this.RemoveEnable = true;
      },
      complete: () => {
        this.Changestatusloading = false;
      },
    });
  }

  
  ScriptExists(UserId: number, TypeId: number, Changestatus: any, isactive: boolean, isDisable: boolean, loginName:string) {
    this.Changestatusloading = true;
    this.globalusermanagementservice.ScriptExists(UserId, TypeId).subscribe({
      next: (result: any) => {
        if (result == "SCRIPTEXIST") {
          if (TypeId == 2)
            this.translate
              .get('usermanage.SCRIPTEXISTINACTIVE')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          if (TypeId == 3) {
            this.translate
              .get('usermanage.SCRIPTEXISTDISABLE')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          }
        }
        else {
          this.ConfirmationForChangestatus(UserId, TypeId, Changestatus, isactive, isDisable, loginName)
        }
      },
      complete: () => {
        this.Changestatusloading = false;
      },
    });
  }


  ConfirmationForChangestatus(UserId: number, TypeId: number, Changestatus: any, isactive: boolean, isDisable: boolean, loginName: string) {
    switch (Changestatus) {
      case 'Active':
        Changestatus = 'usermanage.active';
        break;
      case 'Inactive':
        Changestatus = 'usermanage.inactive';
        break;
      case 'Enable':
        Changestatus = 'usermanage.enable';
        break;
      case 'Disable':
        Changestatus = 'usermanage.disable';
        break;
      case 'remove':
        Changestatus = 'usermanage.remove';
        break;
      default:
        break;
    }
    this.translate
      .get(Changestatus)
      .subscribe((translated: string) => {
        Changestatus = translated
      });

    var ConfirmMessage = "";
    this.translate
      .get('shared.confirmMessage')
      .subscribe((translated: string) => {
        ConfirmMessage = translated
      });
    var thisUsermessage = "";
    this.translate
      .get('usermanage.thisUser')
      .subscribe((translated: string) => {
        thisUsermessage = translated
      });
    var permanently = "";
    this.translate
      .get('usermanage.permanently')
      .subscribe((translated: string) => {
        permanently = translated
      });


    this.strngMessage = ConfirmMessage + Changestatus + thisUsermessage + "?"
    if (TypeId == 5) {
      this.strngMessage = ConfirmMessage + Changestatus + thisUsermessage + permanently + "?"
    }
    if (!isactive && !isDisable) {
      if (TypeId == 3) {
        this.translate
          .get('shared.confirmMessages')
          .subscribe((translated: string) => {
            ConfirmMessage = translated
          });
        this.strngMessage = ConfirmMessage + Changestatus + thisUsermessage + "?"
      }
    }
    else if (!isactive && isDisable) {
      this.translate
        .get('usermanage.enableAndActive')
        .subscribe((translated: string) => {
          ConfirmMessage = translated
        });
      this.strngButonMessage = ConfirmMessage
      this.isCicked = true;
    }
    this.ChangeStatusUsers(UserId, this.strngMessage, this.isCicked, this.strngButonMessage, TypeId, loginName)
  }


  ChangeStatusUsers(UserId: number, strngMessage: string, clicked: boolean, strngButonMessage: string, typeId: number, loginName:string) {
    debugger
    this.Alert.clear();
    this.type = typeId;

    const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        message: strngMessage,
        messageButton: strngButonMessage,
        hasEnable: clicked
      }

    });
    confirmDialog.afterClosed().subscribe(result => {
      this.isCicked = false;
      this.Alert.clear();
      if (confirmDialog.componentInstance.isChecked) {
        this.type = 6;
      }
      if (result) {
        this.userslstloading = true;
        this.globalusermanagementservice.ChangeStatusUsers(UserId, this.type, loginName).subscribe({
          next: (data: any) => {
            switch (data.Status) {
              case "A001":
                this.translate
                  .get('usermanage.ACTIVESUCCESS')
                  .subscribe((translated: string) => {
                    this.Alert.success(translated);
                  });
                break;
              case "IA001":
                this.translate
                  .get('usermanage.INACTIVESUCCESS')
                  .subscribe((translated: string) => {
                    this.Alert.success(translated);
                  });
                break;
              case "D001":
                this.translate
                  .get('usermanage.DISABLESUCCESS')
                  .subscribe((translated: string) => {
                    this.Alert.success(translated);
                  });
                break;
              case "EN001":
                this.translate
                  .get('usermanage.ENABLESUCCESS')
                  .subscribe((translated: string) => {
                    this.Alert.success(translated);
                  });
                break;
              case "S001":
                this.translate
                  .get('usermanage.REMOVESUCCESS')
                  .subscribe((translated: string) => {
                    this.Alert.success(translated);
                  });
                break;
              case "EA001":
                this.translate
                  .get('usermanage.enableActiveSuccess')
                  .subscribe((translated: string) => {
                    this.Alert.success(translated);
                  });
                break;
              case "E002":
                this.translate
                  .get('usermanage.USERMapped')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
                break;
              case "E001":
              case "EM002":
                this.translate
                  .get('usermanage.SCRIPTLivepool')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
                break;
              default:
                this.translate
                  .get('usermanage.Failed')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
                break;
            }
            this.GetAllusers(1);
          },
          complete: () => {
            this.userslstloading = false;
          },
        });
      }
    });
  }



  ExcelReport() {
    const date = new Date();
    const month = date.toLocaleString('default', { month: 'short' });
    var year = date.getFullYear();
    var day = date.getDay();

    this.globalusermanagementservice.GetUserdataCompelteReport().pipe(first())
      .subscribe({
        next: (data: any) => {
          this.Alert.clear();
          var fileName = "UsersDataReport " + day + "-" + month + "-" + year;
          const blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
          var link = document.createElement('a');
          if (link.download !== undefined) {
            var url = URL.createObjectURL(blob);
            link.setAttribute('href', url);
            link.setAttribute('download', fileName);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            this.translate
              .get('usermanage.ExportSuccess')
              .subscribe((translated: string) => {
                this.Alert.success(translated);
              });
          }


        },
        error: (err: any) => {
          throw err;
        },
        complete: () => {
          this.userslstloading = false;
        }

      });

  }



  GetApplicationLevelUserRoles() {

    this.globalusermanagementservice.GetApplicationLevelUserRoles().pipe(first())
      .subscribe({
        next: (data: any) => {

          this.roleschooldetails = data;



        },
        error: (err: any) => {
          throw err;
        },
        complete: () => {
          this.userslstloading = false;
        }
      });

  }

  Filter() {

    this.schoolcodes = "";
    this.rolecodes = "";
    this.userstatus = "";

    this.roleschooldetails.schools.filter(x => x.IsSchoolSelected).map(a => a.SchoolCode).forEach(element => {
      this.schoolcodes += element + ",";
    });

    this.schoolcodes = this.schoolcodes.slice(0, -1);

    this.roleschooldetails.roles.filter(x => x.IsRoleSelected).map(a => a.RoleCode).forEach(element => {
      this.rolecodes += element + ","
    });
    this.rolecodes = this.rolecodes.slice(0, -1);


    this.statuslst.filter((x: any) => x.selected).map((a: any) => a.Id).forEach((element: any) => {
      this.userstatus += element + ","
    });
    this.userstatus = this.userstatus.slice(0, -1);


    this.objSearch.RoleCode = this.rolecodes;
    this.objSearch.SchoolCode = this.schoolcodes;
    this.objSearch.Status = this.userstatus;
    this.menuclose.nativeElement.click()
    this.GetAllusers(1);

  }

  IscheckedSchool(event: any, el: SchoolDetails) {
    el.IsSchoolSelected = event.source.selected;
  }

  IsCheckedRole(event: any, el: RoleDetails) {
    el.IsRoleSelected = event.source.selected;
  }

  IscheckedStatus(event: any, el: any) {
    el.selected = event.source.selected;
  }

  Clear() {


    this.searchUserForm.controls.frole.patchValue([]);
    this.searchUserForm.controls.fuserstatus.patchValue([]);
    this.searchUserForm.controls.fschool.patchValue([]);
    this.objSearch.RoleCode = "";
    this.objSearch.SchoolCode = "";
    this.objSearch.Status = "";
    this.GetAllusers(1);

  }

  close(){
    this.menuclose.nativeElement.click();
  }

  SortList(order: number) {

    this.sortupclicked = false;
    this.sortdownclicked = false;

    if (order != null && order == 0) {
      this.sortdownclicked = true;
      this.sortupclicked = false;

    }
    else if (order != null && order == 1) {
      this.sortupclicked = true;
      this.sortdownclicked = false;
    }

    this.objSearch.SortOrder = order;
    this.objSearch.SortField = "username";

    this.GetAllusers(1)

  }
}
