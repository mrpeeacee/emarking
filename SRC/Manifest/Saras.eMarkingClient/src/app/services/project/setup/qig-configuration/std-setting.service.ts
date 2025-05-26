import { BaseService } from 'src/app/services/base.service';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { QIGStandardizationScriptSettings } from 'src/app/model/project/setup/qig-configuration/std-setting-model';

@Injectable({
    providedIn: 'root'
  })
  export class StdSettingService extends BaseService {
  
    constructor(private http:HttpClient) {
      super();
    }
    readonly apiurl = `${this.endpoint}/api/${this.apiVersion}/projects/setup/qig-config/standardisation-settings`;
    GetQigStdSettingsandPracticeMandatory(QigId:number):Observable<QIGStandardizationScriptSettings>{
          return this.http.get<any>(this.apiurl + `/${QigId}` ,{headers:this.headers});
      }

    UpdateQigStdSettingsandPracticeMandatory(qiglst: QIGStandardizationScriptSettings): Observable<any> {
       return this.http.post<any>(this.apiurl, qiglst, { headers: this.headers });
    }
}