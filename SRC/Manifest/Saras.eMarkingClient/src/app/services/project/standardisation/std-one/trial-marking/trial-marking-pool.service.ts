import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IScriptBandInfo, ITrialMarkQIG } from 'src/app/model/standardisation/UserQIG';
import { BaseService } from 'src/app/services/base.service';
import { TrailmarkingModel } from 'src/app/model/project/standardisation/std-one/trial-marking/trial-marking-pool-model';

@Injectable({
  providedIn: 'root'
})

export class TrialmarkingPoolService extends BaseService {
  readonly ApiUrl = `${this.endpoint}/api/${this.apiVersion}/projects/s1/trial-marking-pools`;

  constructor(
    private http: HttpClient) {
    super();
  }

  getQIGScriptForTrialMark(qigid: number, filter: string,searchValue:string): Observable<ITrialMarkQIG> {
    let api = `${this.ApiUrl}/script/${qigid}/${filter}/${searchValue}`;
    return this.http.get<any>(api, { headers: this.headers });
  }
  getScriptQuestionBandInformation(ScriptId: number): Observable<[IScriptBandInfo]> {
    let api = `${this.ApiUrl}/script/bands/${ScriptId}`;
    return this.http.get<any>(api, { headers: this.headers });
  }
  ////UpdateTrialMarkWorkFlowStatus1(ScriptId: number): Observable<string> {
  ////  let api = `${this.ApiUrl}/workflow-status/${ScriptId}`;
  ////  return this.http.post<any>(api, { headers: this.headers });
  ////}

  UpdateTrialMarkWorkFlowStatus(TrialMarkedScripts: TrailmarkingModel): Observable<any> {
    let APIUrl =  `${this.endpoint}/api/${this.apiVersion}/projects/s1/trial-marking-pools`;
      return this.http.post<any>(APIUrl + `/trailmark`,TrialMarkedScripts, { headers: this.headers });
  }
}
