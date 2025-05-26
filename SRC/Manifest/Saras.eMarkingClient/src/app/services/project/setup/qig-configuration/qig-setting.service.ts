import { BaseService } from 'src/app/services/base.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IQigConfig } from 'src/app/model/project/setup/qig-configuration/qig-setting-model';
@Injectable({
    providedIn: 'root'
  })
  export class QigSettingService extends BaseService {
  
    constructor(private http:HttpClient) {
      super();
    }
    readonly apiurl = `${this.endpoint}/api/${this.apiVersion}/projects/setup/qig-config/qig-settings`;
    getQigConfigSettings(QigId:number):Observable<IQigConfig>{
        
        return this.http.get<any>(this.apiurl + `/${QigId}`,{headers:this.headers});
      }

    saveQigConfigSettings(QigId:number,objIQigConfig:IQigConfig):Observable<string>{
   
    return this.http.post<any>(this.apiurl + `/${QigId}`,objIQigConfig,{headers:this.headers});
    }
    SaveMarkingTypeQigConfigSettings(QigId:number,objIQigConfig:IQigConfig):Observable<string>{
   
      return this.http.post<any>(this.apiurl + `/marking-type/${QigId}`,objIQigConfig,{headers:this.headers});
      }
    Getavailablemarkschemes(maxmarks:number):Observable<IQigConfig>{
        
      return this.http.get<any>(this.apiurl + `/${maxmarks}`,{headers:this.headers});
    }
    SaveQigConfigLiveMarkSettings(QigId:number,objIQigConfig:IQigConfig):Observable<string>{
   
      return this.http.post<any>(this.apiurl + `/live-marking/${QigId}`,objIQigConfig,{headers:this.headers});
      }
      CheckLiveMarkingorTrialMarkingStarted(QigId:number):Observable<IQigConfig>{
        
        return this.http.get<any>(this.apiurl + `/${QigId}/LiveMarkedorTrialMarked`,{headers:this.headers});
      }
}