import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { IBandingScriptResponse } from 'src/app/model/standardisation/recommendation';
import { BaseService } from '../base.service';
import { AlertService } from '../common/alert.service';
import { MarkScheme } from 'src/app/model/project/mark-scheme/mark-scheme-model';

@Injectable({
  providedIn: 'root',
})
export class BandingService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/public/${this.apiVersion}/banding`;
  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }

  getScriptQuestions(scriptid: number, QigId: number): any {
    let api = `${this.APIUrl}/scriptquestions/${scriptid}/${QigId}`;
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
    let api = `${this.APIUrl}/questionresponse/${scriptid}/${projectpuestionid}/${isdefault}`;
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
    let api = `${this.APIUrl}/scriptquestions/${qigid}/${scriptid}`;
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
  getAllMarkSchemeLibrary(totalmarks?: any) {
    let api = `${this.APIUrl}/markscheme/${totalmarks}/getall`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }
  getMarkScheme(schemeid: number) {
    let api = `${this.APIUrl}/markscheme/${schemeid}/get`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  createMarkSchemeAndBands(
    QuestionId: number,
    marksch: MarkScheme
  ) {
    let api = `${this.APIUrl}/markscheme/${QuestionId}/create`;
    return this.http.post<any>(api, marksch, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  getAllQuestion() {
    let api = `${this.APIUrl}/markscheme/GetAllQuestions`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }
  getQuestion(questionid: number) {
    let api = `${this.APIUrl}/markscheme/${questionid}/GetQuestionWithId`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  TagMarkScheme(
    projectQuestionID: number,
    projectMarkSchemeID: number,
    marksch: MarkScheme
  ) {
    let api = `${this.APIUrl}/${projectQuestionID}/markscheme/${projectMarkSchemeID}/MarkSchemeMapping`;
    return this.http.patch<any>(api, marksch, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  GetQuestionsUnderProject(pagenumber: number) {
    let api = `${this.APIUrl}/markscheme/questions/${pagenumber}`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }
}
