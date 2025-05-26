import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { LiveMarkingOverviewsModel, StandardisationApprovalCountsModel, StandardisationOverviewModel } from 'src/app/model/project/dashbaord/ao-cm-dashboard';
import { BaseService } from '../../base.service';
import { AlertService } from '../../common/alert.service';

@Injectable({
  providedIn: 'root',
})
export class DashboardService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/dashboards/overview`;
  readonly QaAPIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/quality-checks`;
  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }
  GetAllOverView(QigId: number, ProjectUserRoleId: number): Observable<any> {
    let api = `${this.APIUrl}/all/${QigId}/${ProjectUserRoleId}`;
    return this.http.get<any>(api, { headers: this.headers });
  }

  getStandardisationOverview(QigId: number): any {
    let api = `${this.APIUrl}/${QigId}/standardisations`;
    return this.http
      .get<StandardisationOverviewModel>(api, { headers: this.headers })
      .pipe(
        map((res: StandardisationOverviewModel) => {
          return res || {};
        })
      );
  } 
  getStandardisationApprovals(QigId: number): any {
    let api = `${this.APIUrl}/${QigId}/approvals`;
    return this.http
      .get<StandardisationApprovalCountsModel>(api, { headers: this.headers })
      .pipe(
        map((res: StandardisationApprovalCountsModel) => {
          return res || {};
        })
      );
  }

  getLiveMarkingOverviews(QigId: number): any {
    let api = `${this.APIUrl}/${QigId}/live-pool`;
    return this.http
      .get<LiveMarkingOverviewsModel>(api, { headers: this.headers })
      .pipe(
        map((res: LiveMarkingOverviewsModel) => {
          return res || {};
        })
      );
  } 
   
}
