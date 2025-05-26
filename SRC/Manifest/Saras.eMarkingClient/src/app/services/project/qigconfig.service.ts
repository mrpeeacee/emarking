import { Injectable } from '@angular/core';
import { BaseService } from '../base.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProjectQigModel } from 'src/app/model/project/qigconfig';
import { IUserQig } from 'src/app/model/standardisation/UserQIG';
import { QigQuestionModel } from 'src/app/model/project/qig';

@Injectable({
  providedIn: 'root'
})
export class QigConfigService extends BaseService {

  constructor(private http: HttpClient) {
    super();
  }
  getQigs(): Observable<[IUserQig]> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/standardisation-trailmarkingscript/QIGs`;
    return this.http.get<any>(api, { headers: this.headers });
  }
  getAllViewQigQuestions(qigid: number): Observable<QigQuestionModel[]> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/qig/qig/${qigid}/question`;
    return this.http.get<QigQuestionModel[]>(api, { headers: this.headers });
  }
  CheckIsCBPproject(): Observable<any> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/qig/qig/IsCBPproject`;
    return this.http.get<any>(api, { headers: this.headers });
  }
  GetQigQuestionandMarks(QigId: number): Observable<ProjectQigModel> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/qig/qigquestionsandmarks/${QigId}`;
    return this.http.get<any>(api, { headers: this.headers });
  }

  Getavailablemarkschemes(MaxMark: number, markschemeType?: number): Observable<any> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/qig/availablemarkschemes/${MaxMark}/${markschemeType}`;
    return this.http.get<any>(api, { headers: this.headers });
  }
  TagAvailableMarkScheme(questionlst: QigQuestionModel): Observable<any> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/qig/tagavailablemarkscheme`;
    return this.http.post<any>(api, questionlst, { headers: this.headers });
  }
  GetSetupStatus(): Observable<any> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/qig/setupstatus`;
    return this.http.get<any>(api, { headers: this.headers });
  }
  UpdateMaxMarks(auditQigQuestionModel:QigQuestionModel): Observable<any> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/qig/max-marks`;
    return this.http.post<any>(api,auditQigQuestionModel, { headers: this.headers });
  }
  SaveScoringcomponentLibrary(available_qu:QigQuestionModel): Observable<any> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/qig/SaveScoringComponentLibrary`;
    return this.http.post<any>(api, available_qu, { headers: this.headers });
  }
  GetQIGConfigDetails(QigId: number): Observable<any> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/qig/qig-config-details/${QigId}`;
    return this.http.get<any>(api, { headers: this.headers });
  }
  GetScoringComponentLibrary(MaxMark: number): Observable<any> {
    debugger;
    let api = `${this.endpoint}/api/public/${this.apiVersion}/qig/availableScoringLibrary/${MaxMark}`;
    return this.http.get<any>(api, { headers: this.headers });
  }
}
