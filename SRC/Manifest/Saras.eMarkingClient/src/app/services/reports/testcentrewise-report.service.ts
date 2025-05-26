import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from '../base.service';
import { AlertService } from '../common/alert.service';


@Injectable({
  providedIn: 'root'
})
export class TestcentrewiseReportService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/public/${this.apiVersion}/reports/testcentrewiseresponse`;
  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }
  GetUserResponse(_scheduleid:any, _testcentrid:any): Observable<any> {
    return this.http.get<any>(`${this.endpoint}/api/${this.apiVersion}/projects/reports/userresponse/${_scheduleid}/${_testcentrid}/false`, { headers: this.headers });
  }
  ProjectCenters(): Observable<any> {
    return this.http.get<any>(this.APIUrl +"/projectcenters" , { headers: this.headers });
  }
  getquestiondetails(questionid:any): Observable<any> {
    
    return this.http.get<any>(this.APIUrl  +`/getquestiondetails/${questionid}`, { headers: this.headers });
  }
}
