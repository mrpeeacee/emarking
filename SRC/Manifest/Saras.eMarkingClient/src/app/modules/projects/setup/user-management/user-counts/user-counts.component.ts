import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Projectuserscount, Qiguserdataviewmodel } from 'src/app/model/project/setup/user-management';
import { UserManagementService } from 'src/app/services/project/setup/user-management.service';
import { UserDataViewComponent } from '../user-data-view/user-data-view.component';
import { UserViewlistComponent } from '../user-viewlist/user-viewlist.component';

@Component({
  selector: 'emarking-user-counts',
  templateUrl: './user-counts.component.html',
  styleUrls: ['./user-counts.component.css']
})
export class UserCountsComponent {

  @ViewChild(UserDataViewComponent) userdataviewcomponent!: UserDataViewComponent;
  @ViewChild(UserDataViewComponent) userviewlistcomponent!: UserViewlistComponent;
  @Output() FilterUsersClick = new EventEmitter<(string | undefined)>();
  
  constructor(public usermanagementService: UserManagementService,
    public translate: TranslateService) { }


  countsloading: boolean = false;
  projectuserscount!: Projectuserscount;
  ProjectUsersSearchedList!: Qiguserdataviewmodel[];
  totcountbtnstyle: boolean = false;
  aobtnstyle: boolean = false;
  cmbtnstyle: boolean = false;
  acmbtnstyle: boolean = false;
  tlbtnstyle: boolean = false;
  atlbtnstyle: boolean = false;
  markerbtnstyle: boolean = false;
  enableanchortag: boolean = true;
  IsQIGVal!: boolean; 

  getProjectuserscount(QigId: number, counthighlight: boolean, IsQIG: boolean = false) {
    this.IsQIGVal = IsQIG;
    this.enableanchortag = true;
    if (counthighlight) {
      this.totcountbtnstyle = false;
    }
    else {
      this.totcountbtnstyle = true;
    }
    this.aobtnstyle = false;
    this.cmbtnstyle = false;
    this.acmbtnstyle = false;
    this.tlbtnstyle = false;
    this.atlbtnstyle = false;
    this.markerbtnstyle = false;
    this.countsloading = true;
    this.usermanagementService.Userscount(QigId).subscribe(data => {
      this.projectuserscount = data;
    }, (err: any) => {
      throw (err)
    }, () => {
      this.countsloading = false;
    });
  }

  FilterUserslist(role: string, aenable: boolean) {
    this.totcountbtnstyle = false; this.aobtnstyle = false; this.cmbtnstyle = false; this.acmbtnstyle = false;
    this.tlbtnstyle = false; this.atlbtnstyle = false; this.markerbtnstyle = false;
    if (aenable) {
      this.enableanchortag = false;
    }
    else {
      this.enableanchortag = true;
      if (role == '') {
        this.totcountbtnstyle = true;
      }
      if (role == 'ao') {
        this.aobtnstyle = true;
      }
      if (role == 'cm') {
        this.cmbtnstyle = true;
      }
      if (role == 'acm') {
        this.acmbtnstyle = true;
      }
      if (role == 'tl') {
        this.tlbtnstyle = true;
      }
      if (role == 'atl') {
        this.atlbtnstyle = true;
      }
      if (role == 'marker') {
        this.markerbtnstyle = true;
      }
      this.FilterUsersClick.emit(role);
    }
  }

}
