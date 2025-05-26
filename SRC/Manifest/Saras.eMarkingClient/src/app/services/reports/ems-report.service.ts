import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { DownloadOutBoundLogModel, EMS1ReportModel, GetModeOfAssessmentModel, GetOralProjectClosureDetailsModel, ReportsOutboundLogsModel } from 'src/app/model/reports/ems-report';
import { BaseService } from '../base.service';
import { AlertService } from '../common/alert.service';
import { StudentReport, StudentResultReportModel, QuestionCodeModel } from 'src/app/model/reports/studentreports';

@Injectable({
  providedIn: 'root',
})
export class EMSReportService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/reports/ems`;
  readonly BasicAPI = `${this.endpoint}/api/${this.apiVersion}/projects/setup/basic-details`;
  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }

  GetEms1Report(IsText: any, OnlyDelta: any) {
    let api = `${this.APIUrl}/${IsText}/1/${OnlyDelta}`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: EMS1ReportModel[]) => {
        return res || {};
      })
    );
  }

  GetEms2Report(IsText: any, OnlyDelta: any) {
    let api = `${this.APIUrl}/${IsText}/2/${OnlyDelta}`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any[]) => {
        return res || {};
      })
    );
  }

  GetOmsReport(IsText: any, OnlyDelta: number) {
    let api = `${this.APIUrl}/${IsText}/3/${OnlyDelta}`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any[]) => {
        return res || {};
      })
    );
  }

  StudentResultReport(studentResultReportModel: StudentResultReportModel) {
    let api = `${this.APIUrl}/studentreportresult`;
    return this.http.post<StudentReport[]>(api, studentResultReportModel, { headers: this.headers }).pipe(
      map((res: any[]) => {
        return res || {};
      })
    );
  }

  GetQuestionsCodes(qigidval:number) {
    let api = `${this.APIUrl}/QuestionCode/${qigidval}`;
    return this.http.get<QuestionCodeModel[]>(api, { headers: this.headers }).pipe(
      map((res: QuestionCodeModel[]) => {
        return res || {};
      })
    );
  }

  syncEmsReport(isType: number, OnlyDelta: number) {
    let api = `${this.APIUrl}/${isType}/${OnlyDelta}`;
    return this.http.post<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  GetModeOfAssessment() {
    let api = `${this.BasicAPI}/ModeOfAssessment`;
    return this.http.get<GetModeOfAssessmentModel>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  GetReportApiLogs() {
    let api = `${this.APIUrl}/outbound/logs`;
    return this.http.get<ReportsOutboundLogsModel[]>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  GetOralProjectClosureDetails() {
    let api = `${this.APIUrl}/outbound/oralproject`;
    return this.http.get<GetOralProjectClosureDetailsModel>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  DownloadOutboundLogs(correlationid: string, processedon: any){
    let api = `${this.APIUrl}/${correlationid}/downloadoutboundlogs/${processedon}`;
    return this.http.get<DownloadOutBoundLogModel[]>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }
  CheckArhive(){
    let api = `${this.APIUrl}/IsArchive`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res ;
      })
    );
  }
}
