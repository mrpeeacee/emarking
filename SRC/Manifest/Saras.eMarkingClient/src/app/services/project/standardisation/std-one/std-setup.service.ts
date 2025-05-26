import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/services/base.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class StdSetupService extends BaseService {
  readonly ExamcenterAPIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/s1/setup/exam-centers`;
  readonly KeypersonnelApiUrl = `${this.endpoint}/api/${this.apiVersion}/projects/s1/setup/key-personnels`;
  readonly StdConfigApiUrl = `${this.endpoint}/api/${this.apiVersion}/projects/s1/setup/std-settings`;

  constructor(
    private http: HttpClient, public Alert: AlertService) {
    super();
  }

  UpdateKeyPersonnels(KeyPersonnelsData: any, QigId: number): Observable<any> {
    return this.http.post<any>(this.KeypersonnelApiUrl + `/${QigId}`, KeyPersonnelsData, { headers: this.headers });
  }

  ProjectKps(QigId: number): Observable<any[]> {
    return this.http.get<any>(this.KeypersonnelApiUrl + `/${QigId}`, { headers: this.headers });
  }

  ProjectCenters(QigId: number): Observable<any[]> {
    return this.http.get<any>(this.ExamcenterAPIUrl + `/${QigId}`, { headers: this.headers });
  }

  UpdateProjectCenters(ProjectCenteres: any, QigId: number): Observable<any> {
    return this.http.post<any>(this.ExamcenterAPIUrl + `/${QigId}`, ProjectCenteres, { headers: this.headers });
  }

  GetQIGConfiguration(QigId: number): Observable<any[]> {
    return this.http.get<any>(this.StdConfigApiUrl + `/qig/${QigId}`, { headers: this.headers });
  }

  GetAppsettingGroup(SettingGroupcode: string): Observable<any> {
    return this.http.get<any>(this.StdConfigApiUrl + `/${SettingGroupcode}`, { headers: this.headers });
  }

  GetStdQigSettings(groupcode: string, entitytype: number = 0, entityid: number = 0): Observable<any[]> {
    return this.http.get<any>(this.StdConfigApiUrl + `/${groupcode}/${entitytype}/${entityid}`, { headers: this.headers });
  }

  UpdateProjectConfig(projectconfig: any) {
    return this.http.patch<{ Token: string }>(this.StdConfigApiUrl, projectconfig, { headers: this.headers }).pipe(
      catchError((err) => {
        return throwError(err);
      }), map((result: any) => {
        return result;
      })
    );
  }
}
