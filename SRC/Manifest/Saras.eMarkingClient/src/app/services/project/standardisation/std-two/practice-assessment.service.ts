import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from 'src/app/services/base.service';
import { AlertService } from 'src/app/services/common/alert.service';
import {
  PracticeQuestionDetails,
  StandardisationPracticeAssessmentModel,
} from 'src/app/model/project/standardisation/std-two/practice-assessment-model';
import { WorkflowStatusTrackingModel } from 'src/app/model/standardisation/Assessment';

@Injectable({
  providedIn: 'root',
})
export class PracticeBenchmarkService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/standardisation/practice`;
  readonly QigApiUrl = `${this.endpoint}/api/public/${this.apiVersion}/`;

  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }

  GetS2PracticeScript(qigId: number): Observable<any> {
    return this.http.get<StandardisationPracticeAssessmentModel>(this.APIUrl + `/s2/script/${qigId}`, { headers: this.headers });
  }
  
  S2PracticeQuestionDetails(qigId: number, scriptId: number, iscompleted: boolean, userRoleId: number = 0): Observable<any> {
    return this.http.get<PracticeQuestionDetails>(this.APIUrl + `/s2/${qigId}/${scriptId}/questions/${iscompleted}/${userRoleId}`, { headers: this.headers });
  }

  GetS3PracticeScript(qigId: number): Observable<any> {
    return this.http.get<StandardisationPracticeAssessmentModel>(this.APIUrl + `/s3/script/${qigId}`, { headers: this.headers });
  }

  S3PracticeQuestionDetails(qigId: number, scriptId: number, iscompleted: boolean): Observable<any> {
    return this.http.get<PracticeQuestionDetails>(this.APIUrl + `/s3/${qigId}/${scriptId}/questions/${iscompleted}`, { headers: this.headers });
  }

  Getqigworkflowtracking(entityid: number, entitytype: any): Observable<any> {
    return this.http.get<WorkflowStatusTrackingModel>(this.QigApiUrl + `qig/workflowtracking/${entityid}/${entitytype}`, { headers: this.headers });
  }
}
