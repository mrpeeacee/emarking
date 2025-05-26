import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../base.service';
import { AlertService } from '../../common/alert.service';
import {
  ProjectLevelConfigModel,
} from 'src/app/model/project/setup/project-level-config-model';
import { Observable } from 'rxjs';
import { QigModel } from 'src/app/model/project/qig';

@Injectable({
  providedIn: 'root',
})
export class ProjectLevelConfigService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/setup/project-level-configurations`;
  readonly rcAPIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/setup/qig-config/rc-settings`;
  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }

  getProjectLevelConfig(): any {
    let api = `${this.APIUrl}`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  updateProjectLevelConfig(projectLeveConfigModel: ProjectLevelConfigModel[]) {
    let api = `${this.APIUrl}`;
    return this.http
      .post<any>(api, projectLeveConfigModel, { headers: this.headers })
      .pipe(
        map((res: any) => {
          return res || {};
        })
      );
  }

  updateProjectRandomCheck() {
    let api = `${this.rcAPIUrl}/project`;
    return this.http.patch<any>(api, { headers: this.headers }).pipe(
      map((result) => {
        return result || {};
      })
    );
  }

  updateProjectStatus() {
    let api = `${this.APIUrl}`;
    return this.http.patch<any>(api, { headers: this.headers }).pipe(
      map((result) => {
        return result || {};
      })
    );
  }


  GetRandomcheckQIGs(
    QigId: number,
    isProjectLevel: boolean = false
  ): Observable<QigModel> {
    return this.http.get<any>(this.rcAPIUrl + `/projectrssetting`, {
      headers: this.headers,
    });
  }
}
