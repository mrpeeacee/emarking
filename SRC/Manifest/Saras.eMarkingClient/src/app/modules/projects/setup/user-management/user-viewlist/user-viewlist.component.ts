import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Projectuserscount, ProjectUsersModel } from 'src/app/model/project/setup/user-management';
import { UserManagementService } from 'src/app/services/project/setup/user-management.service';
import { UserCountsComponent } from '../user-counts/user-counts.component';
import { UserCreationComponent } from '../user-creation/user-creation.component';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { AlertService } from 'src/app/services/common/alert.service';
import { first } from 'rxjs/operators';

@Component({
  selector: 'emarking-user-viewlist',
  templateUrl: './user-viewlist.component.html',
  styleUrls: ['./user-viewlist.component.css']
})
export class UserViewlistComponent implements OnInit {
  @ViewChild(UserCountsComponent) usercountcomponent!: UserCountsComponent;
  @Output() selectUserFilterEvent = new EventEmitter<ProjectUsersModel[] | undefined>();
  @Output() FormResetClick = new EventEmitter<(boolean | undefined)>();
  @Output() createuserclick = new EventEmitter();
  @ViewChild(UserCreationComponent) usercreationcomponent!: UserCreationComponent;

  constructor(public usermanagementService: UserManagementService, public translate: TranslateService,
    private dialog: MatDialog, public Alert: AlertService) { }

  ngOnInit(): void {
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
    this.translate.get('SetUp.UserManagement.UserunblockWarning').subscribe((translated: string) => {
      this.intMessages.UserunblockWarning = translated;
    });
  }

  dataviewlistloading: boolean = false;
  projectusersviewlist!: ProjectUsersModel[];
  projectuserscount!: Projectuserscount;

  UsersValue: string = "";
  ProjectsSearchList!: ProjectUsersModel[];
  ProjectUsersSearchedList!: ProjectUsersModel[];
  showhide: boolean = true;
  rolecode: string = '';
  intMessages: any = {
    filename: '',
    login: '',
    reporting: '',
    role: ''
  };
  IsQIGVal!: boolean;

  getProjectusers() {
    this.UsersValue = "";
    this.ProjectsSearchList = [];
    this.dataviewlistloading = true;
    this.usermanagementService.Projectuserview().subscribe(data => {
      if (data.length > 0 || data != null || data != undefined) {
        this.projectusersviewlist = data;
        this.ProjectsSearchList = data;
        this.ProjectUsersSearchedList = data;
      }
    }, (err: any) => {
      throw (err)
    }, () => {
      this.dataviewlistloading = false;
    });
  }

  SearchAvailableUserslist() {
    var UsersValue = this.UsersValue;
    var that = this.rolecode;
    if (that != '') {
      this.ProjectUsersSearchedList = this.ProjectsSearchList.filter(function (el) {
        return (
          (el.UserName.toLowerCase().includes(UsersValue.trim().toLowerCase()) ||
            el.LoginName.toLowerCase().includes(UsersValue.trim().toLowerCase())) &&
          (el.RoleID.toLowerCase().trim() == that.trim())
        )
      });
    }
    else {
      this.ProjectUsersSearchedList = this.ProjectsSearchList.filter(function (el) {
        return (
          el.UserName.toLowerCase().includes(UsersValue.trim().toLowerCase()) ||
          el.LoginName.toLowerCase().includes(UsersValue.trim().toLowerCase())
        )
      });
    }

  }

  FilterProjectUsers(role: string) {
    this.UsersValue = "";
    this.rolecode = role.toLowerCase();
    this.ProjectUsersSearchedList = this.projectusersviewlist.filter(a => a.RoleID.toLowerCase() == role.toLowerCase() || role == '');
  }

  ShowDataComponent(showhide: boolean, IsQIG: boolean = false) {
    this.IsQIGVal = IsQIG;
    this.showhide = showhide;
  }

  download() {
    let fileName = this.intMessages.filename;
    let columnNames = [this.intMessages.login, this.intMessages.role, this.intMessages.reporting];
    let header = columnNames.join(',');

    let csv = header;
    csv += '\r\n';

    this.projectusersviewlist.forEach(element => {
      var data = [
        {
          "LoginName": element == null ? null : element.LoginName,
          "Role": element == null ? null : element.RoleID,
          "ReportingTo": null
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

  Formreset(val: boolean) {
    this.usercreationcomponent.ResetForm(val);
  }
  addUser() {
    const editorDialog = this.dialog.open(UserCreationComponent, {
      data: {
        disabled: false,
      },
    });
    editorDialog.afterClosed().subscribe((adata) => {
      this.getProjectusers();
      this.createuserclick.emit();
    }
    );
  }
  UnBlocking(UserId: any) {
    if (UserId != null && UserId > 0) {

      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: this.intMessages.UserunblockWarning
        }
      });
      confirmDialog.afterClosed().subscribe(result => {
        this.Alert.clear();
        if (result === true) {
          this.Alert.clear();
          this.usermanagementService.UnBlockUsers(UserId).pipe(first()).subscribe({
            next: (data: any) => {
              if (data == "U001") {
                this.translate
                  .get('SetUp.UserManagement.UserUnblocked')
                  .subscribe((translated: string) => {
                    this.Alert.clear();
                    this.Alert.success(translated);
                    this.getProjectusers();
                  });
              }
              else if (data == "SERROR") {
                this.translate
                  .get('SetUp.UserManagement.userfailed')
                  .subscribe((translated: string) => {
                    this.Alert.clear();
                    this.Alert.warning(translated);
                  });
              }
            },
            complete: () => {
              this.dataviewlistloading = false;
            },

          });
        }
      });
    }
    else {
      this.translate
        .get('SetUp.UserManagement.userfailed')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
    }


  }
}
