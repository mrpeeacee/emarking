import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PracticeQualifyingEnable } from 'src/app/model/standardisation/Assessment';
import { BaseService } from 'src/app/services/base.service';
import { AlertService } from 'src/app/services/common/alert.service';

@Injectable({
    providedIn: 'root'
})

export class StdAssessmentService extends BaseService {
    readonly APIUrl = `${this.endpoint}/api/public/${this.apiVersion}/`;
    readonly QaApiUrl = `${this.endpoint}/api/${this.apiVersion}/projects/standardisation/assessments`;

    constructor(private http: HttpClient, public Alert: AlertService) {
        super();
    }

    IsS2PracticeQualifyingEnable(qigId: number): Observable<any> {
        return this.http.get<PracticeQualifyingEnable>(this.QaApiUrl + `/s2/is-enable/${qigId}`, { headers: this.headers });
    }

    IsS3PracticeQualifyingEnable(qigId: number): Observable<any> {
        return this.http.get<PracticeQualifyingEnable>(this.QaApiUrl + `/s3/is-enable/${qigId}`, { headers: this.headers });
    }

    S2AssessmentStatus(QigID: number): Observable<any> {
        return this.http.get<PracticeQualifyingEnable>(this.QaApiUrl + `/s2/status/${QigID}`, { headers: this.headers });
    }

    S3AssessmentStatus(QigID: number): Observable<any> {
        return this.http.get<PracticeQualifyingEnable>(this.QaApiUrl + `/s3/status/${QigID}`, { headers: this.headers });
    } 

    AssessmentStatus(QigID: number, ProjectUserRoleId: number): Observable<any> {
        return this.http.get<PracticeQualifyingEnable>(this.QaApiUrl + `/s3/status/assessmentstatus/${QigID}/${ProjectUserRoleId}`, { headers: this.headers });
    } 
}
