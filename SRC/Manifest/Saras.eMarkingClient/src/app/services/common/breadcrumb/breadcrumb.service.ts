import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Data, NavigationEnd, Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { AuthService } from '../../auth/auth.service';
import { Breadcrumb } from './breadcrumb.model';


@Injectable({
  providedIn: 'root'
})
export class BreadcrumbService {

  // Subject emitting the breadcrumb hierarchy
  private readonly _breadcrumbs$ = new BehaviorSubject<Breadcrumb[]>([]);

  // Observable exposing the breadcrumb hierarchy
  readonly breadcrumbs$ = this._breadcrumbs$.asObservable();

  constructor(private router: Router, private authService: AuthService) {
    this.router.events.pipe(
      // Filter the NavigationEnd events as the breadcrumb is updated only when the route reaches its end
      filter((event) => event instanceof NavigationEnd)
    ).subscribe(event => {
      // Construct the breadcrumb hierarchy
      const root = this.router.routerState.snapshot.root;
      const breadcrumbs: Breadcrumb[] = [];
      this.addBreadcrumb(root, [], breadcrumbs);

      // Emit the new hierarchy
      this._breadcrumbs$.next(breadcrumbs);
    });
  }

  private addBreadcrumb(route: ActivatedRouteSnapshot, parentUrl: string[], breadcrumbs: Breadcrumb[]) {

    if (route) {
      // Construct the route URL
      const routeUrl = parentUrl.concat(route.url.map(url => url.path));
      let curntrole = this.authService.getCurrentRole();

      var dashboardpath = '';
      curntrole.forEach((role) => {
        if (
          role == 'EO' ||
          role == 'AO' ||
          role == 'ACM' ||
          role == 'CM' ||
          role == 'SUPERADMIN' ||
          role == 'SERVICEADMIN'
        ) {
          dashboardpath = 'ao-cm';
        }
        if (role == 'TL' || role == 'ATL') {
          dashboardpath = 'tl-atl';
        }
        if (role == 'MARKER') {
          dashboardpath = 'marker';
        }
      });
      // Add an element for the current route part
      if (route.data.breadcrumb) {
        // for (let i = 0; i < route.data.breadcrumb.length; i++) {
        //   const breadcrumb = {
        //     label: route.data.breadcrumb[i].label.replace('PHome', localStorage.getItem("PHome")).replace(':qig', route.params['qig']).replace(':process', route.params['process']),
        //     url: route.data.breadcrumb[i].path.replace(':id', route.params['id']).replace(':process', route.params['process'])
        //     .replace(':qigid', route.params['qigid']).replace(':QigId', route.params['QigId'])
        //     .replace(':userroleid', route.params['userroleid']).replace(':pqid', route.params['pqid'])
        //     .replace('br_dashboard', dashboardpath).replace(':qig',route.params['qig']).replace(':userId',route.params['UserId']).replace(':candidateid',route.params['candidateid'])
        //   }
        //   if (!breadcrumbs.find(o => o.label == route.data.breadcrumb[i].label)) {
        //     breadcrumbs.push(breadcrumb)

        //   }
        // }

        for(let brdcrumb of route.data.breadcrumb){

          const breadcrumb = {
            label: brdcrumb.label.replace('PHome', localStorage.getItem("PHome")).replace(':qig', route.params['qig']).replace(':process', route.params['process']),
            url: brdcrumb.path.replace(':id', route.params['id']).replace(':process', route.params['process'])
            .replace(':qigid', route.params['qigid']).replace(':QigId', route.params['QigId'])
            .replace(':userroleid', route.params['userroleid']).replace(':pqid', route.params['pqid'])
            .replace('br_dashboard', dashboardpath).replace(':qig',route.params['qig']).replace(':userId',route.params['UserId']).replace(':candidateid',route.params['candidateid'])
          };
          if (!breadcrumbs.find(o => o.label == brdcrumb.label)) {
            breadcrumbs.push(breadcrumb);
          }
        }
      }
      if (route != null && route.firstChild != null) {
        // Add another element for the next route part
        this.addBreadcrumb(route.firstChild, routeUrl, breadcrumbs);
      }
    }
  }

  private getLabel(data: Data) {
    // The breadcrumb can be defined as a static string or as a function to construct the breadcrumb element out of the route data
    return typeof data.breadcrumb === 'function' ? data.breadcrumb(data) : data.breadcrumb;
  }

}
