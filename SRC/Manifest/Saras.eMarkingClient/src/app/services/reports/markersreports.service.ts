import { Injectable } from '@angular/core';
import { BaseService } from '../base.service';
import { HttpClient } from '@angular/common/http';
import { MarkerDetails, MarkerPerformanceStatistics, MarkerPerformance, SchoolDetails } from 'src/app/model/reports/markersreports';
import { map } from 'rxjs/operators';
import { IUserQig } from 'src/app/model/standardisation/UserQIG';


@Injectable({
  providedIn: 'root'
})

export class MarkerReportServices extends BaseService {


  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/reports/markers-performance/`;
  constructor(private httpclient: HttpClient) {
    super();
  }


  GetSchoolDetails(projectId: number) {
    return this.httpclient.get<any>(this.APIUrl + `SchoolList/${projectId}`, { headers: this.headers }).pipe(
      map((res: SchoolDetails[]) => {
        return res;
      }));
  }

  MarkerDetailsListReport(markerdetails: MarkerDetails) {
    return this.httpclient.post<any>(this.APIUrl + `MarkerDetailsList`, markerdetails, { headers: this.headers }).pipe(
      map((res: MarkerPerformance[]) => {
        return res;
      }));
  }

  MarkerSchoolCountReport(markerdetails: MarkerDetails) {
    return this.httpclient.post<any>(this.APIUrl + `MarkerSchoolReport`, markerdetails, { headers: this.headers }).pipe(
      map((res: MarkerPerformanceStatistics) => {
        return res;
      }));
  }

  getQigs(ProjectId:number){
    return this.httpclient.get<any>(this.APIUrl + `Qigs/${ProjectId}`,  { headers: this.headers }).pipe(
      map((res: IUserQig[]) => {
        return res;
      }));
  }

}