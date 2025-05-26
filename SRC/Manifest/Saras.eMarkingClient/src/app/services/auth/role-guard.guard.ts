import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { Userrole } from 'src/app/modules/auth/userrole';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {

  constructor(private _auth: AuthService,
    private _router: Router) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    _state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    let expectedRole = route.data.expectedRole
    if (this.containsAny(expectedRole, Userrole.All)) {
      expectedRole = [Userrole.EO, Userrole.AO, Userrole.TL, Userrole.All, Userrole.CM, Userrole.ACM, Userrole.ATL, Userrole.MARKER, Userrole.KP,Userrole.SERVICEADMIN,Userrole.SUPERADMIN,Userrole.EM];
    }
    if (this._auth.isLoggedIn && this.containsAny(expectedRole, this._auth.getCurrentRole())) {
      return true;
    } else {
      this._auth.doLogout(false);
      this._router.navigate(['/error/401']);
      return false;
    }
  }

  containsAny(source: any[], target: string | any[]) {

    return source.some((_val) => target.indexOf(_val) !== -1);

  }
}
