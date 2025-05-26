import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { QigModel, QigQuestionModel, QigUserModel } from 'src/app/model/project/qig';
import { WorkflowStatusTrackingModel } from 'src/app/model/standardisation/Assessment';
import { BaseService } from '../base.service';
import { AlertService } from '../common/alert.service';

@Injectable({
  providedIn: 'root'
})
export class QigService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/public/${this.apiVersion}/projectsetup`;
  readonly QigApiUrl = `${this.endpoint}/api/public/${this.apiVersion}/`;

  constructor(
    private http: HttpClient, public Alert: AlertService) {
    super();
  }

  getAllQigs(): Observable<QigModel[]> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/qig`;
    return this.http.get<QigModel[]>(api, { headers: this.headers });
  }

  patchQigSetting(qig: QigModel) {
    return this.http.patch<boolean>(`${this.endpoint}/api/public/${this.apiVersion}/qig`, qig, { headers: this.headers }).pipe(
      catchError((err) => {
        return throwError(err);
      }), map(result => {
        return result;
      })
    );
  }

  getAllQigQuestions(qigid: number): Observable<QigQuestionModel[]> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/qig/${qigid}/question`;
    return this.http.get<QigQuestionModel[]>(api, { headers: this.headers });
  }

  getQigs(OnlyIsKpTab?: boolean): Observable<QigUserModel[]> {
    let api = `${this.endpoint}/api/${this.apiVersion}/project/qig-tab`;
    if (OnlyIsKpTab != undefined && OnlyIsKpTab != null) {
      api = api + `/${OnlyIsKpTab}`;
    }
    return this.http.get<QigUserModel[]>(api, { headers: this.headers });
  }

  Getqigworkflowtracking(entityid: number, entitytype: any): Observable<any> {
    return this.http.get<WorkflowStatusTrackingModel>(this.QigApiUrl + `qig/workflowtracking/${entityid}/${entitytype}`, { headers: this.headers });
  }
  
}
