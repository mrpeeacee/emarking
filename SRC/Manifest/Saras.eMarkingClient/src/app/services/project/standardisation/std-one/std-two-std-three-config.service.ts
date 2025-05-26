import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { WorkflowStatusTrackingModel } from 'src/app/model/standardisation/Assessment';
import { QulAssessmentModel } from 'src/app/model/standardisation/QualifyingAssessment';
import { BaseService } from 'src/app/services/base.service';
import { AlertService } from 'src/app/services/common/alert.service';
@Injectable({ providedIn: 'root' })

export class S2S3ConfigService extends BaseService {
    readonly ApiUrl = `${this.endpoint}/api/public/${this.apiVersion}/`;
    readonly QaApiUrl = `${this.endpoint}/api/${this.apiVersion}/projects/s1/s2-s3-configurations/`;

    constructor(private http: HttpClient, public Alert: AlertService) {
        super();
    }

    Getqigworkflowtracking(entityid: number, entitytype: any): Observable<any> {
        return this.http.get<WorkflowStatusTrackingModel>(this.ApiUrl + `qig/workflowtracking/${entityid}/${entitytype}`, { headers: this.headers });
    }

    QualifyingAssessmentupdate( qulassessmentModel: QulAssessmentModel): Observable<any> {
        return this.http.patch<any>(this.QaApiUrl + `assessment`, qulassessmentModel, { headers: this.headers });
    }

    QualifyingAssessmentInsert(QIGId: number, qulassessmentModel: QulAssessmentModel): Observable<any> {
        return this.http.post<any>(this.QaApiUrl + `${QIGId}/assessment`, qulassessmentModel, { headers: this.headers });
    }

    GetQualifyScriptdetails(QIGId: number) {
        return this.http.get<any>(this.QaApiUrl + `${QIGId}/scripts`, { headers: this.headers })
            .pipe(
                map((res: any) => {
                    return res;
                })
            );
    }

    S1CompletedRemarks(EntityID: number, EntityType: number, WorkflowStatusCode: string): Observable<any[]> {
        return this.http.get<any>(this.QaApiUrl + `S1-remark/${EntityID}/${EntityType}/${WorkflowStatusCode}`, { headers: this.headers });
    }


    ProjectWorkflowStatusTracking(WorkflowCode: string, S1CompletedModel: any): Observable<any> {
        return this.http.post<any>(this.QaApiUrl + `workflow-status-tracking/${WorkflowCode}`, S1CompletedModel, { headers: this.headers });
    }

}
