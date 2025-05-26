import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/services/base.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IQigSummary } from 'src/app/model/project/setup/qig-configuration/qig-summery-model';


@Injectable({
  providedIn: 'root'
})
export class QigSummaryService extends BaseService {

  constructor(private http:HttpClient) {
    super();
   }

   readonly apiurl = `${this.endpoint}/api/${this.apiVersion}/projects/setup/qig-config/qig-summary`;

   saveQigSummary(QigId:number, qigSummary:IQigSummary):Observable<string>{
    return this.http.post<any>(this.apiurl + `/${QigId}`,qigSummary,{headers:this.headers});
    }

    GetQigSummary(QigId:number):Observable<string>{
      return this.http.get<any>(this.apiurl + `/${QigId}`,{headers:this.headers});
      }
}
