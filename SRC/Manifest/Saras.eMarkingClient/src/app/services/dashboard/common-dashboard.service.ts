import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { map } from 'rxjs/internal/operators/map';
import { AllExamYear, ProjectStatistics } from 'src/app/model/dashboard/dashboard';
import { BaseService } from '../base.service';
import { AlertService } from '../common/alert.service';

@Injectable({
  providedIn: 'root',
})
export class CommonDashboardService extends BaseService {
  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }

  getProjectStatistics(): any {
    let api = `${this.endpoint}/api/${this.apiVersion}/dashboard/statistics`;
    return this.http
      .get<ProjectStatistics>(api, { headers: this.headers })
      .pipe(
        map((res: ProjectStatistics) => {
          return res || {};
        })
      );
  }

  getProjectsLists(year: number = 0): Observable<any[]> {
    let api = `${this.endpoint}/api/${this.apiVersion}/dashboard/projects/${year}`;
    return this.http.get<any>(api, { headers: this.headers });
  }  

  getExamYear(): Observable<any[]>{
    let api = `${this.endpoint}/api/${this.apiVersion}/dashboard/examyear`;
    return this.http.get<AllExamYear[]>(api, { headers: this.headers });
  }

 


  getProjectStatus(ProjectId:any): Observable<any> {
   
    let api=`${this.endpoint}/api/${this.apiVersion}/dashboard/ProjectStatus/${ProjectId}`
    return this.http.get<any>(api,{headers:this.headers}).pipe(
      catchError(err => {
        return throwError(err);
      }),
      map(result => {
        return result;
      })
    );
  }

}
