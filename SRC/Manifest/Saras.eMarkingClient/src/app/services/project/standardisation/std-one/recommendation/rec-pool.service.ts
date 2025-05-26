import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IRecommedQigData, IRecommedQigScriptData } from 'src/app/model/standardisation/RecommendationQigScript';
import { BaseService } from 'src/app/services/base.service';
import { AlertService } from 'src/app/services/common/alert.service';


@Injectable({
  providedIn: 'root'
})
export class RecPoolService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/public/${this.apiVersion}/`;

  constructor(
    private http: HttpClient, public Alert: AlertService) {
    super();
  }

  GetRecPoolScript(QIGID: number, ScriptId: number = 0): Observable<IRecommedQigScriptData[]> {
    let api = `${this.endpoint}/api/${this.apiVersion}/projects/standardisation/s1/recommendation/rec-pools/scripts/${QIGID}/${ScriptId}`;
    return this.http.get<any>(api, { headers: this.headers });
  }

  GetRecPoolStastics(QigId:number): Observable<IRecommedQigData> {
    let api = `${this.endpoint}/api/${this.apiVersion}/projects/standardisation/s1/recommendation/rec-pools/statistics/${QigId}`;
    return this.http.get<any>(api, { headers: this.headers });
  }
}
