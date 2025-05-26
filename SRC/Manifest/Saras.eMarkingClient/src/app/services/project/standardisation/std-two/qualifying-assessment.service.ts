import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { PracticeQualifyingEnable } from "src/app/model/project/standardisation/std-two/qualifying-assessment-model";
import { BaseService } from "src/app/services/base.service";
import { AlertService } from "src/app/services/common/alert.service";

@Injectable({
  providedIn: 'root',
})
export class QualifyingAssessmentService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/standardisation/qualifying`;
  readonly QigApiUrl = `${this.endpoint}/api/public/${this.apiVersion}/`;
  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }

  QualifyingAssessmentUpdateSummery(QigID: number): Observable<any> {
    return this.http.patch<PracticeQualifyingEnable>(this.APIUrl + `/${QigID}/summary`, { headers: this.headers });
  }

  GetS2StandardisationScript(qigId: number, userRoleId: number = 0, stdOradd: boolean = false): Observable<any> {
    return this.http.get<any>(this.APIUrl + `/s2/${qigId}/scripts/${userRoleId}/${stdOradd}`, { headers: this.headers });
  }

  GetS3StandardisationScript(qigId: number,stdOradd:boolean): Observable<any> {
    return this.http.get<any>(this.APIUrl + `/s3/${qigId}/${stdOradd}/scripts`, { headers: this.headers });
  }
}