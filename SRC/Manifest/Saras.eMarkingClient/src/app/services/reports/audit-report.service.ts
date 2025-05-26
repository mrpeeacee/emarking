import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseService } from '../base.service';
import { AlertService } from '../common/alert.service';
import { AuditReportRequestModel } from '../../model/reports/audit-report';
import { map } from 'rxjs/operators';


@Injectable({
  providedIn: 'root',
})
export class AuditReportService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/reports/audit-report/Auditreportlog`;

  constructor(
    private http: HttpClient, public Alert: AlertService) {
    super();
  }



  GetAuditReport(objaudit: AuditReportRequestModel) { 
    let api = `${this.endpoint}/api/${this.apiVersion}/reports/audit-report/log`;

    return this.http.post<any>(api, objaudit,{ headers: this.headers }).pipe(
      map((res: any) => {
        return res;
      })
    );
  }

  GetAppModules() {  
      let api = `${this.endpoint}/api/${this.apiVersion}/reports/audit-report/app-modules`;
      return this.http.post<any>(api,{ headers: this.headers }).pipe(
        map((res: any) => {
          return res;
        })
      );
    }
}



// GetAuditReport(objaudit: AuditReportRequestModel) { 
//   let api = `${this.endpoint}/api/${this.apiVersion}/reports/audit-report/log`;

//   return this.http.post<any>(api, objaudit,{ headers: this.headers }).pipe(
//     map((res: any) => {
//       return res;
//     })
//   );
// }

// GetAppModules() {  
//   let api = `${this.endpoint}/api/${this.apiVersion}/reports/audit-report/app-modules`;
//   return this.http.post<any>(api,{ headers: this.headers }).pipe(
//     map((res: any) => {
//       return res;
//     })
//   );
// }
// }

