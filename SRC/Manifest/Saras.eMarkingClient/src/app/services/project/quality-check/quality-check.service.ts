import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseService } from '../../base.service';
import { AlertService } from '../../common/alert.service';
import { MarkerTreeView, QualityCheckSummary, QualityCheckInitialScriptModel, LivemarkingApproveModel, QualityCheckScriptDetails, TrialmarkingScriptDetails, Qualitycheckedbyusers, Livepoolscript } from '../../../model/project/quality-check/marker-tree-view-model';
import { map, Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class QualityCheckService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/quality-checks`;

  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }

  //Service for Quality-check
  GetQIGProjectUserReportees(QigID: number): Observable<any> {
    return this.http.get<MarkerTreeView>(this.APIUrl + `/${QigID}/repotees`, { headers: this.headers });
  }

  GetQualityCheckSummary(QigId: number, ProjectUserRoleId: number) {
    return this.http.get<QualityCheckSummary>(this.APIUrl + `/${QigId}/summary/${ProjectUserRoleId}`, { headers: this.headers });
  }
  GetLiveMarkingScriptCountDetails(QigId: number, ProjectUserRoleId: number, pool: number) {
    return this.http.get<any>(this.APIUrl + `/${QigId}/repotees/scripts/${pool}/${ProjectUserRoleId}`, { headers: this.headers });
  }


  GetTeamStatistics(QigId: number, ProjectUserRoleId: number,accordintabs: number,activefilterTab: number) {
    return this.http.get<QualityCheckSummary>(this.APIUrl + `/${QigId}/team-statistics/${accordintabs}/${activefilterTab}/${ProjectUserRoleId}`, { headers: this.headers });
  }

  GetTeamStatisticsList(QigId: number, ProjectUserRoleId: number, accordintabs: number,activefilterTab: number) {
    return this.http.get<QualityCheckScriptDetails>(this.APIUrl + `/${QigId}/team-statistics-list/${accordintabs}/${activefilterTab}/${ProjectUserRoleId}`, { headers: this.headers });
  }


  GetScriptInDetails(QigId: number, ScriptId: number) {
    return this.http.get<QualityCheckInitialScriptModel>(this.APIUrl + `/${QigId}/repotees/${ScriptId}/details`, { headers: this.headers });
  }

  LiveMarkingScriptApprovalStatus(livemarkingapproveModel: LivemarkingApproveModel) {
    return this.http.post(this.APIUrl + `/status`, livemarkingapproveModel, { headers: this.headers });
  }
  IsEligibleForLiveMarking(QigId: number) {
    return this.http.get<boolean>(this.APIUrl + `/${QigId}/is-eligiblefor-live-marking`, { headers: this.headers });
  }

  CheckedOutScript(livemarkingapproveModel: LivemarkingApproveModel) {
    return this.http.post(this.APIUrl + `/Checkout`, livemarkingapproveModel, { headers: this.headers });
  }

  AddMarkingRecord(trialmarkingScriptDetails: TrialmarkingScriptDetails) {
    return this.http.post(this.APIUrl + `/InsertMarkingRecord`, trialmarkingScriptDetails, { headers: this.headers });
  }

  Getcheckedbyuserslist(QigId: number) {
    return this.http.get<Qualitycheckedbyusers[]>(this.APIUrl + `/CheckedByUserList/${QigId}`, { headers: this.headers });
  }

  UserStatus(ProjectUserRoleId:number, QigId: number) {
    return this.http.get<string>(this.APIUrl + `/UserStatus/${ProjectUserRoleId}/${QigId}`, { headers: this.headers });
  }

  ScriptToBeSubmit(Livepoolcript:Livepoolscript){
    return this.http.post<any>(this.APIUrl + `/ScriptToBeSubmit`,Livepoolcript, {headers: this.headers})
    .pipe(
      map((res:any) =>{
        return res;
      })
    );
   }

   RevertCheckout(scriptsCheckedout : QualityCheckScriptDetails[]){
    return this.http.post<any>(this.APIUrl + `/RevertCheckout`, scriptsCheckedout, {headers: this.headers})
    .pipe(
      map((res:any) =>{
        return res;
      })
    );
   }

}
