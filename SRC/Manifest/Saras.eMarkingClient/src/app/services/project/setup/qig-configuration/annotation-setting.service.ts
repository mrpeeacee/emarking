import { BaseService } from 'src/app/services/base.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AnnotationSettings, QigAnnotationDetails } from 'src/app/model/project/setup/qig-configuration/annotation-setting-model';

@Injectable({
    providedIn: 'root'
  })


  export class AnnotationSettingsService extends BaseService{
    constructor(private http:HttpClient) {
        super();
      }
      readonly apiurl = `${this.endpoint}/api/${this.apiVersion}/projects/setup/qig-config/annotation-settings`;
        
      GetQigAnnotationSetting(QigId:number):Observable<AnnotationSettings>{
        return this.http.get<any>(this.apiurl + `/${QigId}`,{headers:this.headers});
      }

      UpdateQigAnnotationSetting(QigId :number,Annotationlst: AnnotationSettings): Observable<any> {
        return this.http.post<any>(this.apiurl + `/${QigId}`, Annotationlst,{ headers: this.headers });
      }

      GetQigAnnotationTools(QigId:number):Observable<QigAnnotationDetails>{
        return this.http.get<any>(this.apiurl + `/GetAnnotationForQIG/${QigId}`,{headers:this.headers});
      }

      SaveAnnotationForQIG(QigId:number, QigAnnotationsDetails:QigAnnotationDetails):Observable<QigAnnotationDetails>{
        return this.http.post<any>(this.apiurl + `/SaveAnnotationForQIG/${QigId}`, QigAnnotationsDetails,{headers:this.headers});
      }
  }