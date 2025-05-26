import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { BaseService } from 'src/app/services/base.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { IBandingScriptResponse, UnrecommandedScript } from 'src/app/model/project/standardisation/std-one/recommendation/recommendation-model';


@Injectable({
  providedIn: 'root',
})
export class RecommendationService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/public/${this.apiVersion}/`;
  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }

  getScriptQuestions(scriptid: number, QigId: number): any {
    let api = `${this.endpoint}/api/${this.apiVersion}/projects/s1/recommendations/scriptquestions/${scriptid}/${QigId}`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  getScriptQuestionResponse(
    scriptid: number,
    projectpuestionid: number,
    isdefault: boolean
  ): any {
    let api = `${this.endpoint}/api/${this.apiVersion}/projects/s1/recommendations/questionresponse/${scriptid}/${projectpuestionid}/${isdefault}`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }
  recommend(
    scriptid: number,
    scripresponses: IBandingScriptResponse[],
    qigid: number
  ) {
    let api = `${this.endpoint}/api/${this.apiVersion}/projects/s1/recommendations/scriptquestions/${qigid}/${scriptid}`;
    return this.http
      .patch<string>(api, scripresponses, { headers: this.headers })
      .pipe(
        catchError((err) => {
          return throwError(err);
        }),
        map((result) => {
          return result;
        })
      );
  }

  UnrecommandedScripts(unrecommend: UnrecommandedScript) {
    let apiurl = `${this.endpoint}/api/${this.apiVersion}/projects/s1/recommendations/UnrecommandedScript`;
    return this.http.post<any>(apiurl, unrecommend, { headers: this.headers })
      .pipe(catchError((err) => {
        return throwError(err);
      }),
        map((result) => {
          return result;
        })
      )
  }

}
