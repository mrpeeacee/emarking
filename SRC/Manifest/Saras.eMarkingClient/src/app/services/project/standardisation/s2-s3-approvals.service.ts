import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApprovalModel, AssignAdditionalStdScriptsModel, MarkingPersonal, S2S3Approvals } from 'src/app/model/project/standardisation/s2-s3-approvals';
import { BaseService } from '../../base.service';
import { AlertService } from '../../common/alert.service';

@Injectable({
  providedIn: 'root'
})
export class S2S3ApprovalsService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/standardisation/approvals`;
  readonly QualifyAPIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/standardisation/qualifying`;
  constructor(private http: HttpClient, public Alert: AlertService) { 
    super();
  }

  GetApprovalStatus(qigId: number): Observable<any> {
    return this.http.get<S2S3Approvals>(this.APIUrl + `/${qigId}/status`, { headers: this.headers });
  }
  
  GetMarkingPersonal(qigId: number, projectUserRoleId: number = 0): Observable<any>{
    return this.http.get<MarkingPersonal>(this.APIUrl + `/${qigId}/personals/${projectUserRoleId}`, { headers: this.headers });
  }

  GetAssignAdditionalStdScripts(QigID: number, UserRoleId: number = 0): Observable<any>{
    return this.http.get<any>(this.QualifyAPIUrl + `/s3/${QigID}/${UserRoleId}/assignscripts`, { headers: this.headers });
  }

  AssignAdditionalStdScripts(assignAdditionalStdScriptsModel: AssignAdditionalStdScriptsModel): Observable<any> {
    return this.http.post<AssignAdditionalStdScriptsModel>(this.QualifyAPIUrl + `/s2-s3/assignstdscripts`, assignAdditionalStdScriptsModel, { headers: this.headers });
  }

  scriptApproval(qigId: number, projectUserRoleId: number, approvalModel: ApprovalModel): Observable<any>{
    return this.http.patch<any>(this.APIUrl + `/${qigId}/${projectUserRoleId}`, approvalModel, { headers: this.headers });
  }

}
