import { Injectable } from '@angular/core';
import { BaseService } from '../base.service';
import { HttpClient } from '@angular/common/http';
import { AlertService } from '../common/alert.service';
import { Observable } from 'rxjs';
import { SearchFilterModel } from 'src/app/model/Global/UserManagement/UserManagementModel';
import { ClsMailSent, Mailsentdetails, SyncMetaDataModel } from 'src/app/model/admin-tools/admin-tools-model';

@Injectable({
  providedIn: 'root',
})
export class AdminToolsService extends BaseService {
  readonly BasicAPI = `${this.endpoint}/api/${this.apiVersion}/admin-tools`;
  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }

  getAllProject(): Observable<any[]> {
    let api = `${this.BasicAPI}`;
    return this.http.get<any>(api, { headers: this.headers });
  }

  getLiveMarkingProgress(projectId: number): Observable<any[]> {
    let api = `${this.BasicAPI}/${projectId}`;
    return this.http.get<any>(api, { headers: this.headers });
  }

  getQualityCheckDetails(
    projectId: number,
    selectedrpt: number
  ): Observable<any[]> {
    let api = `${this.BasicAPI}/${projectId}/${selectedrpt}`;
    return this.http.get<any>(api, { headers: this.headers });
  }

  getCandidateScriptDetails(projectId: number, searchFilterModel: SearchFilterModel, IsDownload: boolean = false): Observable<any[]> {
    let api = `${this.BasicAPI}/${projectId}/candidatescript/${IsDownload}`;
    return this.http.post<any>(api, searchFilterModel, { headers: this.headers });
  }

  getFrequencyDistributionReport(projectId: number, searchFilterModel: SearchFilterModel, queType: any = 0, IsDownload: boolean = false): Observable<any[]> {
    let api = `${this.BasicAPI}/${projectId}/frequencydistribution/${queType}/${IsDownload}`;
    return this.http.post<any>(api, searchFilterModel, { headers: this.headers });
  }

  getFIDIReportDetails(projectId: number, searchFilterModel: SearchFilterModel,syncMetaData:boolean,IsDownload: boolean = false): Observable<any[]> {
    let api = `${this.BasicAPI}/${projectId}/fidireport/${IsDownload}/${syncMetaData}`;
    return this.http.post<any>(api, searchFilterModel, { headers: this.headers });
  }

  getProjectStatus(projectId: number): Observable<any> {
    let api = `${this.BasicAPI}/${projectId}`;
    return this.http.post<any>(api, projectId, { headers: this.headers });
  }

  getAllAnswerKeysReport(projectId: number, searchFilterModel: SearchFilterModel, IsDownload: boolean = false): Observable<any[]> {
    
    let api = `${this.BasicAPI}/${projectId}/answerkeyreport/${IsDownload}`;
    return this.http.post<any>(api, searchFilterModel, { headers: this.headers });
  }

  MailSentDetails(clsMailSent: ClsMailSent): Observable<any[]> {
    let api = `${this.BasicAPI}/MailSentDetails`;
    return this.http.post<Mailsentdetails[]>(api, clsMailSent, { headers: this.headers });

  }

  ExportMailSentDetails(): Observable<any> {

    const requestOptions: Object = {
      headers:  this.headers,
      responseType: 'blob'
    }
    let api = `${this.BasicAPI}/Export-mail-sent-report`;
    return this.http.get<any>(api, requestOptions);
  }

  
  GetAnswerKeyCompelteReport(projectId: number): Observable<any> { 
    const requestOptions: Object = {
      headers:  this.headers,
      responseType: 'blob'
    } 
    return this.http.get<any>(this.BasicAPI + `/answerkeycompletereport/${projectId}`, requestOptions);
  }
  GetMarkerPerformanceReport(projectId: number,searchFilterModel: SearchFilterModel,IsDownload: boolean = false): Observable<any[]> {
    let api = `${this.BasicAPI}/marker-performance-analysis/${projectId}/${IsDownload}`;
    return this.http.post<any>(api, searchFilterModel,{ headers: this.headers });

  }

  SyncMetaData(SyncMetaData:SyncMetaDataModel[]): Observable<SyncMetaDataModel[]>
  {
    let api = `${this.BasicAPI}/SyncMetaData`;
    return this.http.post<any>(api, SyncMetaData,{ headers: this.headers });
  }
}
