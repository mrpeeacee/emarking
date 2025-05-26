import { Component, OnInit, Inject } from '@angular/core';
import { UserprivilegeService } from 'src/app/services/project/privileges.service';
import { CreateEditUser } from 'src/app/model/Global/UserManagement/UserManagementModel';
import {
  MatDialogRef,
  MatDialog,
  MAT_DIALOG_DATA,
} from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'emarking-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
})
export class ProfileComponent implements OnInit {
  objMyprofile!: CreateEditUser;

  constructor(
    public userprivilegeService: UserprivilegeService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public translate: TranslateService,
    public dialogRef: MatDialogRef<ProfileComponent>,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.objMyprofile = this.data.CreateEditUser;
  }

  clickMethod() {
    this.dialogRef.close({ status: 0 });
  }
}
