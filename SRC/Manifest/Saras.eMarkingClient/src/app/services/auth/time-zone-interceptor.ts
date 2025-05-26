import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable()
export class TimeZoneInterceptor implements HttpInterceptor {
  ssnid: string = "";
  public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    var stid = sessionStorage.getItem('xHeader');
    if (stid != null) {
      this.ssnid = stid;
    }
    const modifiedReq = req.clone({
      headers: req.headers
        .append('Local-Time-Zone', Intl.DateTimeFormat().resolvedOptions().timeZone)
        .append('Local-Time-Offset', (new Date().getTimezoneOffset() * -1).toString())
        .append('x-header', this.ssnid)
    });
    return next.handle(modifiedReq);
  }
}
