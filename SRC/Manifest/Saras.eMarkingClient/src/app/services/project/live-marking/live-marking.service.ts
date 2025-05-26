import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../base.service'; 
import { HttpClient } from '@angular/common/http';
import { QigScriptModule } from 'src/app/model/common-model';
import { ClsLivescript } from 'src/app/model/project/live-marking/live-marking-model';
import { Qualitycheckedbyusers, Livepoolscript } from 'src/app/model/project/quality-check/marker-tree-view-model';

@Injectable({
  providedIn: 'root'
})
export class LiveMarkingService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/live-markings/`;
  constructor(private httpclient: HttpClient) {
    super();
   }

   DownloadScripts(QigId:number){
    return this.httpclient.post<any>(this.APIUrl + `Download/${QigId}` , { headers: this.headers }).pipe(
      map((res: any) => {
        return res;
      })
    );
   }

   GetLiveScripts1(QigId:number,pool:number,date: string){
    return this.httpclient.get<any>(this.APIUrl + `LiveScripts/${QigId}/script/${pool}/${date}` , { headers: this.headers }).pipe(
      map((res: any) => {
        return res;
      })
    );
   }

   GetLiveScripts(clsLivescript:ClsLivescript){
 
    return this.httpclient.post<any>(this.APIUrl + `LiveScripts`, clsLivescript,{ headers: this.headers}).pipe(
      map((res: any) => {
        return res;
      })
    );
   }

   MoveScriptToGracePeriod(qigScriptModule:QigScriptModule){
    return this.httpclient.post<any>(this.APIUrl + `GracePeriod/script` , qigScriptModule, { headers: this.headers }).pipe(
      map((res: any) => {
        return res;
      })
    );
   }

   UpdateScriptMarkingDetails(qigScriptModule:QigScriptModule, scriptStatus:boolean){
    return this.httpclient.post<any>(this.APIUrl + `UpdateScriptStatus/script/${scriptStatus}`, qigScriptModule, { headers: this.headers }).pipe(
      map((res: any) => {
        return res;
      })
    );
   }

   GetDownloadedScriptUserList(QigId:number){
    return this.httpclient.get<Qualitycheckedbyusers[]>(this.APIUrl + `Downloadedscriptuserlist/${QigId}` , { headers: this.headers })
    .pipe(
      map((res: Qualitycheckedbyusers[]) => {
        return res;
      })
    );
   }

   MoveScriptsToLivePool(Livepoolcript:Livepoolscript){

    return this.httpclient.post<any>(this.APIUrl + `MoveScriptsToLivePool`,Livepoolcript, {headers: this.headers})
    .pipe(
      map((res:any) =>{
        return res;
      })
    );

   }


  CheckScriptIsLivePool(ScriptId: number, PhaseTrackingId: number) {
    return this.httpclient.get<any>(this.APIUrl + `ScriptIsLivePool/${ScriptId}/${PhaseTrackingId}`, { headers: this.headers }).pipe(
      map((res: any) => {
        return res;
      })
    );
  }

}
