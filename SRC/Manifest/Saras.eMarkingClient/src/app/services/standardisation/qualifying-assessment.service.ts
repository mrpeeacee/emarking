import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseService } from '../base.service';
import { AlertService } from '../common/alert.service';
import { Observable } from 'rxjs';
import { PracticeQualifyingEnable } from 'src/app/model/standardisation/Assessment'; 

@Injectable({
  providedIn: 'root'
})

export class QualifyingAssessmentService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/public/${this.apiVersion}/`;

  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }
  ScriptCategorizationPool(QIGId: number, QualifiedAssessmentID: number): Observable<any[]> {
    return this.http.get<any>(this.APIUrl + `ScriptCategorizationPool/${QIGId}/${QualifiedAssessmentID}`, { headers: this.headers });
  }



  ScriptdetailsInsertforQualifyAssessment(qulifyingassessmentlst: any, QualifiedassessmentID: number): Observable<any> {
    return this.http.post<any>(this.APIUrl + `ScriptdetailsInsertforQualifyAssessment/${QualifiedassessmentID}`, qulifyingassessmentlst, { headers: this.headers });
  }


  GetS2StandardisationScript(qigId: number): Observable<any> {
    return this.http.get<any>(this.APIUrl + `S2StandardisationScript/${qigId}`, { headers: this.headers });
  }
  GetS3StandardisationScript(qigId: number): Observable<any> {
    return this.http.get<any>(this.APIUrl + `S3StandardisationScript/${qigId}`, { headers: this.headers });
  }


  QualifyingAssessmentUpdateSummery(QigID: number): Observable<any> {
    return this.http.get<PracticeQualifyingEnable>(this.APIUrl + `QualifyingAssessmentUpdateSummery/${QigID}`, { headers: this.headers });
  }

}
