import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RoutesRecognized
} from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { Globalconst } from './model/globalconst';
import { AuthService } from './services/auth/auth.service';
import { CreateEditUser } from './model/Global/UserManagement/UserManagementModel';

@Component({
  selector: 'emarking-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Saras-eMarkingClient';
  isLoggedIn!: boolean;
  showHeader = false;
  objMyprofile!: CreateEditUser;

  constructor(
    public authService: AuthService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private titleService: Title,
    readonly translate: TranslateService,
    private route: ActivatedRoute
  ) {
    translate.addLangs(['en', 'es', 'fr']);
    // Set default language
    translate.setDefaultLang('en');
    translate.use('en');

    window.onstorage = (a: any) => {
      if (a.key == 'event_token') {
        let loggedIn = this.authService.loginStatus;
        let logval = loggedIn.value;
        let reloadpage = true;
        this.route.queryParams.subscribe(params => {
          reloadpage = params.tabs != '1';
          if (logval && reloadpage) {
            this.router.navigateByUrl('/login?tabs=1');
          }
        });
      }
    };
  }
  LoginStatus$!: Observable<boolean>;
  ngOnInit(): void {

    this.isLoggedIn = this.authService.isLoggedIn;

    this.LoginStatus$ = this.authService.isAuthenticated;
    this.router.events.subscribe(data => {
      if (
        data instanceof RoutesRecognized &&
        data != null &&
        data.state != null &&
        data.state.root != null &&
        data.state.root.firstChild != null
      ) {
        this.titleService.setTitle(data.state.root.firstChild.data.title);
        this.titleService.setTitle('eMarking');
      }
      this.setglogalvariable(data);
    });   
  }

  getChild(activatedRoute: ActivatedRoute): any {
    if (this.activatedRoute.firstChild) {
      return this.getChild(this.activatedRoute.firstChild);
    } else {
      return activatedRoute;
    }
  }
  private setglogalvariable(data: any) {
    if (data instanceof NavigationEnd) {
      let curUrlTree = this.router.parseUrl(this.router.url);
      if (
        curUrlTree.root.children.primary.segments[1] != null &&
        curUrlTree.root.children.primary.segments[1] != undefined
      ) {
        var pId = curUrlTree.root.children.primary.segments[1].path;
        if (pId != null && pId != undefined) {
          Globalconst.projectId = parseInt(
            curUrlTree.root.children.primary.segments[1].path
          );
        } else {
          Globalconst.projectId = 0;
        }
      } else {
        Globalconst.projectId = 0;
      }
    }
  }
}
