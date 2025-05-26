import {
  animate,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';
import { Component, OnInit, HostListener } from '@angular/core';
import { Observable, first } from 'node_modules/rxjs';
import { AuthService } from 'src/app/services/auth/auth.service';
import { CommonService } from 'src/app/services/common/common.service';
import { NotificationService } from 'src/app/services/common/notification.service';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { UserprivilegeService } from 'src/app/services/project/privileges.service';
import { BrandingModel } from 'src/app/model/globalconst';
import { CreateEditUser } from 'src/app/model/Global/UserManagement/UserManagementModel';
import { MatDialog } from '@angular/material/dialog';
import { ProfileComponent } from '../../projects/dashboard/dashboard-module/profile/profile.component';
import { GlobalUserManagementService } from 'src/app/services/Global/UserManagement/UserManagementService';
import { ChangePasswordComponent } from '../../auth/change-password/change-password.component';

@Component({
  selector: 'emarking-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  animations: [
    trigger('sideMenu', [
      state(
        'closed',
        style({
          left: '-400px',
        })
      ),
      state(
        'open',
        style({
          left: '0px',
        })
      ),
      transition('open => closed', animate('400ms ease-out')),
      transition('closed => open', animate('300ms ease-out')),
    ]),
  ],
})
export class HeaderComponent implements OnInit {
  // close the leftsidemenu when click outside

  @HostListener('document:click', ['$event'])
  ondocumentClick(event: MouseEvent) {
    this.leftmenuopen(2, event);
    this.showPopup(2, event);
  }

  public show: boolean = false;

  constructor(
    public authService: AuthService,
    public commonService: CommonService,
    public router: Router,
    public notificatonservice: NotificationService,
    public userprivilegeService: UserprivilegeService,
    public translate: TranslateService,
    private dialog: MatDialog,
    public globalUserManagementService: GlobalUserManagementService
  ) {}

  LoginStatus$!: Observable<boolean>;
  HeaderName$!: Observable<string>;
  InfoText$!: Observable<string>;
  state = 'closed';
  toggletopmenu: boolean = false;
  isShown = false;
  isHidedashabord = true;
  isSetupProject = true;
  currentPage: string = '';

  currentrouting: any;
  menuItems: any = [];
  currentIndex!: null;
  indexOfelement: number = -1;
  activemenus: any = [];
  Privilegeslst: any = [];
  
  Myprofile!: CreateEditUser;
  receivedData: any;

  brand!: BrandingModel;
  ngOnInit(): void {
    this.LoginStatus$ = this.authService.isAuthenticated;
    this.HeaderName$ = this.commonService.GetPageHeader;
    this.InfoText$ = this.commonService.GetPageDescription;
    this.notificatonservice.brand$.subscribe((msg) => (this.brand = msg));
    this.activemenus = [];
    this.activemenus.push({
      ID: 0,
      Name: 'Dashboard',
      RoleCode: 'AO',
      ParentID: 0,
      Url: './projects',
      PrivilegeCode: 'DASHBOARD',
    });
    if (this.authService.isAuthenticated) {
      this.getMyprofileDetails();
    }
    this.globalUserManagementService.data$.subscribe(data => {
      if (data != null) {
        // Perform actions with the received data
        this.Myprofile = data;
      }
      
    });
  }

  //User Profile icon
  showPopup(id: number, event: any) {
    if (id == 1) {
      this.show = !this.show;
      this.leftmenuopen(2, event);
      event.stopPropagation();
    } else {
      this.show = false;
    }
  }

  assignTeam() {
    this.show = !this.show;
  }

  leftmenuopen(id: number, event: any) {
    // checking to close the leftsidemenu when click outside
    if (id == 1) {
      this.showPopup(2, event);
      if (this.state == 'closed') {
        this.state = 'open';
        if (this.state == 'open') {
          let body = document.getElementsByTagName('body')[0];
          body.classList.add('body-landing');
        }
        if (this.currentPage != this.router.url.split('/').pop()) {
          this.GetUserPrivileges();
        }
        this.toggletopmenu = !this.toggletopmenu;
        event.stopPropagation();
      }
    } else {
      this.state = 'closed';
      if (this.state == 'closed') {
        let body = document.getElementsByTagName('body')[0];
        body.classList.remove('body-landing');
      }
      this.toggletopmenu = false;
    }
  }

  leftmenuclose(url: string) {
    if(url=="/projects"){
      this.getMyprofileDetailsProject(0);
    }
    this.state = 'closed';
    this.toggletopmenu = !this.toggletopmenu;
    if (url != undefined && url != null) {
      this.currentPage = url.split('/').pop() || '';
    }
  }

  onLogout(): void {
    this.authService.doLogout();
    this.show = false;
  }

  BasicDetails() {
    this.router.navigateByUrl('/project/basic-details');
  }

  TeamReporting() {
    this.router.navigateByUrl('/project/team-reporting');
  }

  TeamStructure() {
    this.router.navigateByUrl('/project/team-structure');
  }

  TeamStructure2() {
    this.router.navigateByUrl('/project/Team-Structure-step2');
  }

  GetMenuItems(menus: any) {
    var mapmenu: any = {};
    menus.forEach((result: any) => {
      var obj = result;
      if (!(obj.ID in mapmenu)) {
        mapmenu[obj.ID] = obj;
        mapmenu[obj.ID].Url =  this.MenuList(mapmenu, obj);
        mapmenu[obj.ID].children = [];
      }

      if (typeof mapmenu[obj.ID].Name == 'undefined') {
        mapmenu[obj.ID].Url =  this.MenuList(mapmenu, obj);

        mapmenu[obj.ID].Id = obj.ID;
        mapmenu[obj.ID].Name = obj.Name;
    
        mapmenu[obj.ID].Parent = obj.ParentID;
        mapmenu[obj.ID].children = [];
      }

      var parent = obj.ParentID || '-';
      if (!(parent in mapmenu)) {
        mapmenu[parent] = {};
        mapmenu[parent].children = [];
      }

      mapmenu[parent].children.push(mapmenu[obj.ID]);
    });

    this.activemenus = JSON.parse(JSON.stringify(mapmenu['-'].children));
  }


  MenuList(mapmenu:any, obj:any){
    if (
      mapmenu[obj.ID].RoleCode == 'TL' ||
      mapmenu[obj.ID].RoleCode == 'ATL'
    ) {
      return mapmenu[obj.ID].Url = obj.Url.replace(':process', 's2');
    } else if (mapmenu[obj.ID].RoleCode == 'MARKER') {
      return mapmenu[obj.ID].Url = obj.Url.replace(':process', 's3');
    } else {
      return mapmenu[obj.ID].Url = obj.Url.replace(':process', 's1');
    }

  }




  openSubmenu(index: number) {
    this.indexOfelement = index;
  }

  GetUserPrivileges() {
    this.activemenus = [];
    this.userprivilegeService.GetUserPrivileges().subscribe((data) => {
      this.activemenus = data;
      this.GetMenuItems(this.activemenus);
    });
  }

  GetMyprofile() {
    const editorDialog = this.dialog.open(ProfileComponent, {
      data: {
        CreateEditUser: this.Myprofile,
        isSaveClicked: 0,
      },
      panelClass: 'fullviewpop',
    });
    editorDialog.afterClosed().subscribe((result) => {
      if (result?.data == 1) {
        //Intentionally empty
      }
    });
  }

  getMyprofileDetails() {
    this.globalUserManagementService
      .getMyprofileDetails()
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.Myprofile = data;
        },
        error: (err: any) => {
          throw err;
        },
      });
  }

  BtnChangePassword() {
    const editorDialog = this.dialog.open(ChangePasswordComponent, {
      data: {
        CreateEditUser: this.Myprofile,
        isSaveClicked: 0,
      },
      panelClass: 'fullviewpop',
    });
    editorDialog.afterClosed().subscribe((result) => {
      if (result?.data == 1) {
        //Intentionally empty
      }
    });
  }

  getMyprofileDetailsProject(evnet:any) {
    this.globalUserManagementService
      .getMyprofileDetailsProject(evnet)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.Myprofile = data;
          this.sendDataToHeader();
        },
        error: (err: any) => {
          throw err;
        }
      });
  }

  sendDataToHeader() {
    const data = this.Myprofile;
    this.globalUserManagementService.updateData(data);
  }

}
