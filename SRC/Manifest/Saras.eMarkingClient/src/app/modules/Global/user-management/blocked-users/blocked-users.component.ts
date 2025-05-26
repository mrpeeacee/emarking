import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { GlobalUserManagementService } from 'src/app/services/Global/UserManagement/UserManagementService';
import { GetAllUsersModel } from '../../../../model/Global/UserManagement/UserManagementModel';

@Component({
  selector: 'emarking-blocked-users',
  templateUrl: './blocked-users.component.html',
  styleUrls: ['./blocked-users.component.css']
})
export class BlockedUsersComponent implements OnInit {
  blockUsersList!: GetAllUsersModel[];
  searchblockeduserslist!: GetAllUsersModel[];
  unblockUsersList: any;
  selectAll: boolean = false;
  unBlockUsers: any = [];
  selectedUserIds: any = [];
  users: any;
  SelectAllUser: boolean = false;
  selectAll1: number = 0;
  blockUsersList1: boolean = false;
  selectAllUsers: any;
  blockUsersloading: boolean = false;
  selectedusers: any;
  avaliablesearchusers: string = '';

  constructor(
    public usermanagementService: GlobalUserManagementService,
    public translate: TranslateService,
    public commonService: CommonService,
    public Alert: AlertService,
    public dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.translate
      .get('usermanage.blocktitle')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });
    this.translate
      .get('usermanage.blockdesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
    this.GetBlockedUsers();
  }

  GetBlockedUsers() {
    this.blockUsersloading = true;
    this.usermanagementService.GetBlockedUsers().subscribe({
      next: (result: any) => {
        if (result.length != null && result != undefined) {
          this.blockUsersList = result;
          this.searchblockeduserslist = result;
        }
      },
      complete: () => {
        this.blockUsersloading = false;
      }
    });
  }

  SelectAllUsers() {
    this.blockUsersList.forEach(
      (user: { selected: boolean }) => (user.selected = this.selectAll)
    );
  }

  SelectSingleUsers() {
    this.selectAll =
      this.blockUsersList.length ==
      this.blockUsersList.filter((a: { selected: boolean }) => a.selected)
        .length
        ? true
        : false;
  }

  unBlockSubmit() {
    this.Alert.clear();
    this.selectedusers = this.blockUsersList.filter(
      (a: { selected: any }) => a.selected
    );
    if (this.selectedusers.length == 0) {
      this.translate
        .get('usermanage.SelectUsers')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    } else {
      var strmessage = '';
      this.translate
        .get('usermanage.confmessage')
        .subscribe((translated: string) => {
          strmessage = translated;
        });
      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: strmessage
        }
      });

      confirmDialog.afterClosed().subscribe(results => {
        if (results) {
          this.blockUsersloading = true;
          var blockeduserlist = this.blockUsersList.filter(
            (a: { selected: boolean }) => a.selected
          );
          this.usermanagementService
            .unBlockSubmit(blockeduserlist)
            .subscribe(result => {
              if (result == 'S002') {
                this.avaliablesearchusers = '';
                this.translate
                  .get('usermanage.UnblockedSuccess')
                  .subscribe((translated: string) => {
                    this.Alert.success(translated);
                  });
                this.GetBlockedUsers();
                this.selectAll = false;
              } 
              else {
                this.translate
                  .get('usermanage.UnblockedFailed')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
              }
            });
        } else {
          this.blockUsersloading = false;
          this.blockUsersList.forEach(
            (user: { selected: boolean }) => (user.selected = true)
          );
          this.blockUsersList.forEach(() => {
            this.blockUsersList
              .filter((a: { selected: any }) => a.selected)
              .forEach((item: { selected: boolean }) => {
                item.selected = false;
              });
          });
        }
      });
    }
  }

  SearchBlockedUsers() {
    var avaliablesearchusers = this.avaliablesearchusers;
    this.blockUsersList = this.searchblockeduserslist.filter(function(el) {
      return el.LoginName.toLowerCase().includes(
        avaliablesearchusers.trim().toLowerCase()
      );
    });
  }
}
