import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ScriptQuestionModel } from 'src/app/model/project/scriptquestion';
import { UserScriptResponseModel, ViewScriptModel} from 'src/app/model/project/trialmarking';
import { BaseService } from '../base.service';
import { AlertService } from '../common/alert.service';
import { QuestionUserResponseMarkingDetailsmodel, UserScriptMarking, MarkingScriptTimeTracking } from '../../model/project/trialmarking';
import { AuthService } from 'src/app/services/auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class TrialmarkingService extends BaseService {

  constructor(
    private http: HttpClient, public Alert: AlertService, public authService: AuthService) {
    super();
  } 
  Getannoatationdetails(qigid:number): Observable<any> 
  {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/standardisation-trailmarking/Getannoatationdetails/${qigid}`;
    return this.http.get<any>(api, { headers: this.headers });
  }
 
  GetquestionResponeText(ScriptId: number, ProjectQuestionId: number): Observable<ScriptQuestionModel> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/standardisation-trailmarking/GetScriptQuestionResponse/${ScriptId}/${ProjectQuestionId}/false`;
    return this.http.get<any>(api, { headers: this.headers });
  }
  Markingsubmit(scriptId: number, workflowstatusid?:any, qigid?:number): Observable<any> {
   
    if(workflowstatusid == null)
    {
      workflowstatusid = 0;
    }
    let api = `${this.endpoint}/api/public/${this.apiVersion}/standardisation-trailmarking/MarkingSubmit/${scriptId}/${workflowstatusid}/${qigid}`;
    return this.http.post<any>(api, { headers: this.headers });
  }

  UserscriptMarking(ques: UserScriptMarking): Observable<UserScriptResponseModel[]> {

    let api = `${this.endpoint}/api/public/${this.apiVersion}/standardisation-trailmarking/userscriptmarking`;
    return this.http.post<any>(api, JSON.stringify(ques), { headers: this.headers });
  }
  ResponseMarking(ques: QuestionUserResponseMarkingDetailsmodel[], QigId:number,IsAutoSave:boolean): Observable<any> {
   let api = `${this.endpoint}/api/public/${this.apiVersion}/standardisation-trailmarking/responsemarking/${QigId}/${IsAutoSave}`;
    return this.http.post<any>(api, JSON.stringify(ques), { headers: this.headers });
  }
  Validateannotations(qigid:number , EntityType:any): Observable<any> 
  {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/standardisation-trailmarking/validateannotation/${qigid}/${EntityType}`;
    return this.http.get<any>(api, { headers: this.headers });
  }
  Getcatagarizedands1configureddetails(qigid:number , scriptid:number, workflowid:number): Observable<any> 
  {
    if (workflowid == null) {
      workflowid = 0;
    }
    let api = `${this.endpoint}/api/public/${this.apiVersion}/standardisation-trailmarking/Getcatagarizedands1configureddetails/${qigid}/${scriptid}/${workflowid}`;
    return this.http.get<any>(api, { headers: this.headers });
  }

  ResponseMarkingDetails(Scriptid: number, ProjectQuestionResponseID: number, Markedby?: any, workflowstatusid?:any, userscriptmarkingrefid?:number): Observable<any> {
    if (Markedby == null) {
      Markedby = 0;
    }
    if (workflowstatusid == null) {
      workflowstatusid = 0;
    }
    let api = `${this.endpoint}/api/public/${this.apiVersion}/standardisation-trailmarking/responsemarkingdetails/${Scriptid}/${ProjectQuestionResponseID}/${Markedby}/${workflowstatusid}/ ${userscriptmarkingrefid}`;
    return this.http.get<any>(api, { headers: this.headers });

  }
  GetUserScheduleDetails(userid : number): Observable<any> {
    return this.http.get<any>(`${this.endpoint}/api/${this.apiVersion}/projects/reports/solution/${userid}`, { headers: this.headers });
}
MarkingScriptTimeTracking(markingscriptTimeTracking:MarkingScriptTimeTracking ): Observable<any> {
  let api = `${this.endpoint}/api/public/${this.apiVersion}/standardisation-trailmarking/MarkingScriptTimeTracking`;
  return this.http.post<any>(api, JSON.stringify(markingscriptTimeTracking), { headers: this.headers });

}
  ViewDownloadMarkScheme(projectquestionid: number, markschemeid: number) {
    if (!markschemeid)
      markschemeid = 0;
    let api = `${this.endpoint}/api/public/${this.apiVersion}/standardisation-trailmarking/Viewanddownloadmarkscheme/${projectquestionid}/${markschemeid}`;
    return this.http.get<any>(api, { headers: this.headers });

  }

  ViewScript(objViewScript :ViewScriptModel) {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/standardisation-trailmarking/view-script/objViewScript`;
    return this.http.post<any>(api, objViewScript, { headers: this.headers });
  }
}
