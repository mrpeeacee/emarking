import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuTrigger } from '@angular/material/menu';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSelect } from '@angular/material/select';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { SearchFilterModel, TotalUserWithdrawModel, UserWithdrawModel } from 'src/app/model/project/setup/user-management';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { UserManagementService } from 'src/app/services/project/setup/user-management.service';

@Component({
  selector: 'emarking-user-withdraw',
  templateUrl: './user-withdraw.component.html',
  styleUrls: ['./user-withdraw.component.css']
})
export class UserWithdrawComponent implements OnInit {
  WithdrawUsers!: TotalUserWithdrawModel;
  WithdrawUsersList!: any;
  selectAll: boolean = false;
  UsersValue: string = "";
  userslstloading: boolean = false;
  CheckedCandidateStatustypes: any;
  selectallCheckBox: boolean = false;
  userstatus: string = "";
  SelectedUsers: any[] = [];
  searchCandidateUserForm!: FormGroup;


  // Pagination
  @ViewChild('paginator') paginator!: MatPaginator ;
  @ViewChild(MatMenuTrigger) menuTrigger!: MatMenuTrigger;
  @ViewChild('matSelectUsers') matSelectUsers!: MatSelect;
  @ViewChild('matSelectUsers1') matSelectUsers1!: MatSelect;
  length = 0;
  pageSize = 10;
  pageIndex = 0;
  pageNo = 0;
  pageSizeOptions = [5, 10, 25, 100];
  showPageSizeOptions = true;
  pageEvent!: PageEvent;
  objSearch: SearchFilterModel = new SearchFilterModel();
  


  constructor(public usermanagementService: UserManagementService, public translate: TranslateService, private fb: FormBuilder,
    private dialog: MatDialog, public Alert: AlertService, private route: Router,
    public commonService: CommonService) {

  }

  ngOnInit(): void {
    this.translate.get('SetUp.UserManagement.UserWithdrawPageDescription').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.translate.get('SetUp.UserManagement.CandidateWithdrawPage').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.objSearch.PageNo = 1;
    this.objSearch.PageSize = this.pageSize;
    this.GetUserWithdraw();
    this.SelectedUsers = [];

    this.CheckedCandidateStatustypes = [
      {
        Id: 1,
        Text: '',
        Selected: false,
        ischecked: true,

      },
      {
        Id: 2,
        Text: '',
        Selected: false,
        ischecked: true,
      },
      {
        Id: 3,
        Text: '',
        Selected: false,
        ischecked: true
      },
      {
        Id: 4,
        Text: '',
        Selected: false,
        ischecked: true,
      }
    ];
    this.CheckedCandidateStatustypes.forEach((element: any) => {
      if (element.Id == 1) {
        this.translate
          .get('SetUp.UserManagement.Withdrawn')
          .subscribe((translated: string) => {
            element.Text = translated
          });
      }
      if (element.Id == 2) {
        this.translate
          .get('SetUp.UserManagement.NotWithdrawn')
          .subscribe((translated: string) => {
            element.Text = translated
          });
      }
      if (element.Id == 3) {
        this.translate
          .get('SetUp.UserManagement.Present')
          .subscribe((translated: string) => {
            element.Text = translated
          });
      }
      if (element.Id == 4) {
        this.translate
          .get('SetUp.UserManagement.Absent')
          .subscribe((translated: string) => {
            element.Text = translated
          });
      }
    });

    this.searchCandidateUserForm = this.fb.group({
      fcandidatestatus: new FormControl('')
    });
  }

  GetUserWithdraw() {
    
    this.userslstloading = true;
    this.usermanagementService.GetUserWithdraw(this.objSearch).subscribe(data => {
      this.WithdrawUsers = data;
      this.WithdrawUsersList = this.WithdrawUsers.UserWithdraw;

      if (this.WithdrawUsers.UserWithdraw != null && this.WithdrawUsers.UserWithdraw.length > 0) {
        this.length = this.WithdrawUsers.UserWithdraw[0]!.RowCount;
        var CountWithDrawuser = this.WithdrawUsers.UserWithdraw.filter((user) => user.IsWithDrawn);
        this.selectallCheckBox = CountWithDrawuser.length == this.WithdrawUsersList.length ? true : false;
      }
      else {
        this.length = 0;
        this.selectallCheckBox = true;
      }

      this.WithdrawUsersList.forEach((element: { selected: boolean; ID: any; }) => {
        element.selected = this.SelectedUsers.find(y => y.ID == element.ID);
      });
      this.ValidateCheckbox();
      
    }, (err: any) => {
      throw (err)
    }, () => {
      this.userslstloading = false;
    });

  }

  SearchAvailableUserslist() {
    this.selectallCheckBox = false;
    this.selectAll = false;
    this.paginator.pageIndex = 0;
    this.objSearch.SearchText = this.UsersValue.trim();
    this.objSearch.PageNo = 1;
    this.validationForSelectAllCheckBox();
    this.GetUserWithdraw();
  }

  SelectAllUsers(event: any) {
    this.WithdrawUsersList.forEach((user: any) => {
      if (!user.IsWithDrawn) {
        user.selected = this.selectAll
      }
      if (event.checked && !user.IsWithDrawn) {
        this.SelectedUsers.push(user);
      }
      else {
        
        var indexOfelement = this.SelectedUsers.findIndex(a => a.ID == user.ID)
        this.SelectedUsers.forEach(element => {
          if(indexOfelement>=0)
         {
        this.SelectedUsers.splice(indexOfelement, 1);
        }
        });
        
       
      }
    });

  }

  storecheckedvalues(selectedusersdata: any, event: any) {
    if (event.checked) {
      this.SelectedUsers.push(selectedusersdata);
    }
    else {
      this.SelectedUsers = this.SelectedUsers.filter(z => z.ID !== selectedusersdata.ID);
      
    }
    this.ValidateCheckbox();
  }

  ValidateCheckbox() {
    if ((this.WithdrawUsersList.filter((a: { selected: any; }) => a.selected).length == this.WithdrawUsersList.filter((a: { IsWithDrawn: boolean; }) => !a.IsWithDrawn).length)) {
      this.selectAll = true;
    }
    else {
      this.selectAll = false;
    }
  }

  validationForSelectAllCheckBox() {
    if (this.objSearch.Status == "1" || this.objSearch.Status == "1,3" ||
      this.objSearch.Status == "1,4" || this.objSearch.Status == "1,3,4") {
      this.selectallCheckBox = true;
    }
  }

  UserWithdraw() {
    this.Alert.clear();
    if (this.SelectedUsers.length==0) {
      this.translate
        .get('SetUp.UserManagement.SelectUser')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
    }
    else {
      var strConfirmmessage = "";
      this.translate
        .get('SetUp.UserManagement.confmessage')
        .subscribe((translated: string) => {
          strConfirmmessage = translated;
        });

      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: strConfirmmessage
        }
      });

      confirmDialog.afterClosed().subscribe(results => {
        if (results) {
          this.userslstloading = true;
          this.usermanagementService.UserWithdraw(this.SelectedUsers).subscribe({
            next: (result: any) => {
              if (result == "Success") {
                this.translate
                  .get('SetUp.UserManagement.WithdrawSuccess')
                  .subscribe((translated: string) => {
                    this.Alert.clear();
                    this.Alert.success(translated);
                  });
                this.UsersValue = "";
                this.SelectedUsers=[];
                this.GetUserWithdraw();
              }
              else {
                this.translate
                  .get('SetUp.UserManagement.Withdrawfailed')
                  .subscribe((translated: string) => {
                    this.Alert.clear();
                    this.Alert.warning(translated);
                  });
              }
            },
            complete: () => {
              this.userslstloading = false;
            }
          });
        }
      });
    }
  }

  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.length = e.length;
    this.objSearch.PageNo = e.pageIndex + 1;
    this.objSearch.PageSize = e.pageSize;
    this.GetUserWithdraw();
  }

  Clear() {
    this.selectallCheckBox = false;
    this.searchCandidateUserForm.controls.fcandidatestatus.patchValue([]);
    this.objSearch.Status = "";
    this.objSearch.SearchText = "";
    this.paginator.pageIndex = 0;
    this.objSearch.SearchText = this.UsersValue;
    this.objSearch.PageNo = 1;
    this.GetUserWithdraw();
    this.menuTrigger.closeMenu();
  }

  Filter() {
    this.selectallCheckBox = false;
    this.userstatus = "";
    this.CheckedCandidateStatustypes.filter((x: any) => x.IsWithDrawn).map((a: any) => a.Id).forEach((element: any) => {
      this.userstatus += element + ","
    });
    this.userstatus = this.userstatus.slice(0, -1);
    this.objSearch.Status = this.userstatus;
    this.paginator.pageIndex = 0;
    this.objSearch.PageNo = 1;
    this.validationForSelectAllCheckBox();
    this.GetUserWithdraw();
    this.menuTrigger.closeMenu();
  }

  IscheckedCanditateStatus(event: any, uw: UserWithdrawModel) {
    uw.IsWithDrawn = event.source.selected;
    this.matSelectUsers.close();
    this.matSelectUsers1.close();
  }
  
  AllUserRespones(){
    this.route.navigateByUrl('reports/all-user-response/withdraw');
  }

}
