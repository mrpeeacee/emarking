import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators'; 
import { CategorisationModel, CategorisationStasticsModel, CategorisationTrialMarkModel1, CategoriseAsModel } from 'src/app/model/project/standardisation/std-one/categorisaton/categorisation';
import { WorkflowStatusTrackingModel } from 'src/app/model/standardisation/Assessment';
import { BaseService } from 'src/app/services/base.service';
import { AlertService } from 'src/app/services/common/alert.service';

@Injectable({
  providedIn: 'root'
})
export class CategorisationService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/s1/categorisation-pools`;
  readonly QigApiUrl = `${this.endpoint}/api/public/${this.apiVersion}/`;
  constructor(
    private http: HttpClient, public Alert: AlertService) {
    super();
  }

  getCategorisationStatistics(qigId: number): any {
    let api = `${this.APIUrl}/${qigId}/statistics`;
    return this.http.get<CategorisationStasticsModel>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res
      })
    )
  }

  getCategorisationScripts(qigId: number,searchValue:string ,poolTypes: Array<number>): any {
    let api = `${this.APIUrl}/scripts/${qigId}`;
    if (searchValue != "") {
      api += `/${searchValue}`;
     }
    if (poolTypes.length > 0) {
      api += `/filter?poolTypes=${poolTypes.join('&poolTypes=')}`;
    }
    return this.http.get<CategorisationModel[]>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res
      })
    )
  }

  IsQigInQualifying(qigId: number, scriptId: number): any {
    let api = `${this.APIUrl}/qualified/${qigId}/${scriptId}`;
    return this.http.get<boolean>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res
      })
    )
  }

  IsScriptCategorised(qigId: number, scriptId: number): any {
    let api = `${this.APIUrl}/${qigId}/categorised/${scriptId}`;
    return this.http.get<boolean>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res
      })
    )
  }

  getTrialMarkedScript(qigId: number, scriptId: number, UserScriptMarkingRefID?:number): any {
    if(UserScriptMarkingRefID == undefined)
    {
      UserScriptMarkingRefID = 0;
    }
    let api = `${this.APIUrl}/${qigId}/trial-marked-script/${scriptId}/${UserScriptMarkingRefID}`;
    return this.http.get<CategorisationTrialMarkModel1[]>(api, { headers: this.headers }).pipe(
      map((res: CategorisationTrialMarkModel1[]) => {
        return res
      })
    )
  }

  categorise(categoriseModel: CategoriseAsModel) {
    let api = `${this.APIUrl}/categorise`;
    return this.http.patch<string>(api, categoriseModel, { headers: this.headers }).pipe(
      catchError((err) => {
        return throwError(err);
      }), map(result => {
        return result;
      })
    );
  }

  recategorise(categoriseModel: CategoriseAsModel) {
    let api = `${this.APIUrl}/re-categorise`;
    return this.http.patch<string>(api, categoriseModel, { headers: this.headers }).pipe(
      catchError((err) => {
        return throwError(err);
      }), map(result => {
        return result;
      })
    );
  }

  Getqigworkflowtracking(entityid: number, entitytype: any): Observable<any> {
    return this.http.get<WorkflowStatusTrackingModel>(this.QigApiUrl + `qig/workflowtracking/${entityid}/${entitytype}`, { headers: this.headers });
  }

}
