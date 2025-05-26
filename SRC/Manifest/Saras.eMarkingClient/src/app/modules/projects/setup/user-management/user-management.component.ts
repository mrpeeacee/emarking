import { Component, OnInit, ViewChild } from '@angular/core';
import { QigUserModel } from 'src/app/model/project/qig';
import { UserDataViewComponent } from './user-data-view/user-data-view.component';
import { UserCountsComponent } from './user-counts/user-counts.component';
import { UserViewlistComponent } from './user-viewlist/user-viewlist.component';
import { UserHierarchicalViewComponent } from './user-hierarchical-view/user-hierarchical-view.component';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { Router } from '@angular/router';

@Component({
  selector: 'emarking-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css'],
})
export class UserManagementComponent implements OnInit {
  @ViewChild(UserCountsComponent) usercountcomponent!: UserCountsComponent;
  @ViewChild(UserDataViewComponent)
  userdataviewcomponent!: UserDataViewComponent;
  @ViewChild(UserViewlistComponent)
  userviewlistcomponent!: UserViewlistComponent;
  @ViewChild(UserHierarchicalViewComponent)
  userhierarchycomponent!: UserHierarchicalViewComponent;
  showProjectdata: boolean = true;
  activeQig?: QigUserModel;
  QigIdval: number = -1;
  countshighlight: boolean = false;
  role!: string;
  IsQIG: boolean = false;

  constructor(
    public translate: TranslateService,
    public commonService: CommonService,
    private router: Router
  ) {}
  ngOnInit(): void {
    this.translate
      .get('SetUp.UserManagement.title')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });
    this.translate
      .get('SetUp.UserManagement.description')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
  }

  getQigDetails(selectedqig: QigUserModel) {
    this.activeQig = selectedqig;
    this.QigIdval = this.activeQig.QigId;
    this.IsQIG = this.QigIdval > 0 ? true : false;
    if (selectedqig != null) {
      if (selectedqig.QigId > 0) {
        this.usercountcomponent.getProjectuserscount(
          this.activeQig.QigId,
          this.countshighlight,
          this.IsQIG
        );
        this.userdataviewcomponent.getQigusersdatatview(this.activeQig.QigId);

        this.userdataviewcomponent.ShowDataComponent(true, this.IsQIG);
        this.userviewlistcomponent.ShowDataComponent(false, this.IsQIG);
        this.ShowHierarchyView(false);
        this.router.navigateByUrl(
          'projects/setup/user-management/' + this.activeQig.QigId
        );
      } else {
        this.usercountcomponent.getProjectuserscount(
          this.activeQig.QigId,
          this.countshighlight,
          this.IsQIG
        );
        this.userviewlistcomponent.getProjectusers();
        this.userviewlistcomponent.ShowDataComponent(true, this.IsQIG);
        this.userdataviewcomponent.ShowDataComponent(false, this.IsQIG);
        this.ShowHierarchyView(false);
        this.router.navigateByUrl(
          'projects/setup/user-management/' + this.activeQig.QigId
        );
      }
    }
  }

  FilterUserslist(role: string) {
    if (this.countshighlight) {
      this.usercountcomponent.FilterUserslist(role, this.countshighlight);
    } else {
      if (this.QigIdval > 0) {
        this.userdataviewcomponent.FilterQigUsers(role);
      } else {
        this.userviewlistcomponent.FilterProjectUsers(role);
      }
    }
  }

  ShowHierarchyView(showhideval: boolean) {
    this.IsQIG = this.QigIdval > 0 ? true : false;
    this.countshighlight = false;
    this.usercountcomponent.getProjectuserscount(
      this.QigIdval,
      this.countshighlight,
      this.IsQIG
    );
    this.userhierarchycomponent.ShowHierarchyComponent(showhideval, this.IsQIG);
    if (showhideval) {
      this.countshighlight = true;
      this.usercountcomponent.getProjectuserscount(
        this.QigIdval,
        this.countshighlight,
        this.IsQIG
      );
      this.userdataviewcomponent.ShowHideDataComponent(showhideval);
      this.userhierarchycomponent.getQigusersheirachytview(this.QigIdval);
    }
  }

  ShowDataView(showhideval: boolean) {
    this.IsQIG = this.QigIdval > 0 ? true : false;
    this.countshighlight = false;
    this.usercountcomponent.getProjectuserscount(
      this.QigIdval,
      this.countshighlight,
      this.IsQIG
    );
    this.userdataviewcomponent.getQigusersdatatview(this.QigIdval);
    this.userdataviewcomponent.ShowDataComponent(showhideval, this.IsQIG);
    if (showhideval) {
      this.countshighlight = false;
      this.userhierarchycomponent.ShowHideHierarchyComponent();
    }
  }

  CallCounter() {
    this.IsQIG = this.QigIdval > 0 ? true : false;
    this.usercountcomponent.getProjectuserscount(
      this.QigIdval,
      this.countshighlight,
      this.IsQIG
    );
  }
}
