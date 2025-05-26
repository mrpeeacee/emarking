import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ProjectClosureModel } from 'src/app/model/project/setup/project-closure-model';
import { BaseService } from '../../base.service';
import { AlertService } from '../../common/alert.service';

@Injectable({
  providedIn: 'root',
})
export class ProjectClosureService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/setup/projectclosure`;
  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }

  GetProjectClosure(): any {
    let api = `${this.APIUrl}`;
    return this.http
      .get<ProjectClosureModel[]>(api, { headers: this.headers })
      .pipe(
        map((res: any) => {
          return res || {};
        })
      );
  }

  UpdateProjectClosure(projectclosure: ProjectClosureModel): Observable<any> {
    return this.http.patch<any>(this.APIUrl + `/closure`, projectclosure, {
      headers: this.headers,
    });
  }

  UpdateProjectReopen(projectclosure: ProjectClosureModel): Observable<any> {
    return this.http.patch<any>(this.APIUrl + `/reopen`, projectclosure, {
      headers: this.headers,
    });
  }

  CheckDiscrepancy(): any {
    let api = `${this.APIUrl}/discrepancy`;
    return this.http
      .get<ProjectClosureModel[]>(api, { headers: this.headers })
      .pipe(
        map((res: any) => {
          return res || {};
        })
      );
  }

  ClearPendingScripts(qigId: number): Observable<any> {
    return this.http.patch<any>(
      this.APIUrl + `/${qigId}/clear-pending-scripts`,
      {
        headers: this.headers,
      }
    );
  }
}
