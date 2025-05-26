import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../base.service';
import { AlertService } from '../../common/alert.service';
import {
  MarkScheme,
  QuestionTag,
  QuestionText,
} from 'src/app/model/project/mark-scheme/mark-scheme-model';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class MarkSchemeService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/mark-schemes`;
  readonly FileAPIUrl = `${this.endpoint}/api/${this.apiVersion}/file`;
  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }

  getAllMarkSchemes() {
    let api = `${this.APIUrl}`;
    return this.http.get<MarkScheme[]>(api, { headers: this.headers }).pipe(
      map((res: MarkScheme[]) => {
        return res || {};
      })
    );
  }

  deleteMarkScheme(marksch: number[]) {
    let api = `${this.APIUrl}/delete`;
    return this.http.post<any>(api, marksch, { headers: this.headers }).pipe(
      map((res: any) => {
        return res;
      })
    );
  }

  getMarkSchemeAndBand(schemeId: number) {
    let api = `${this.APIUrl}/${schemeId}/view`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  getQuestions(maxMark: any, schmeId: number) {
    let api = `${this.APIUrl}/${schmeId}/${maxMark}`;
    return this.http.get<QuestionTag[]>(api, { headers: this.headers }).pipe(
      map((res: QuestionTag[]) => {
        return res;
      })
    );
  }

  saveMarkSchemeAndBands(marksch: MarkScheme): Observable<any> {
    ///markschemecreation
    let api = `${this.APIUrl}`;
    return this.http.post<any>(api, marksch, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  updateMarkSchemeAndBands(marksch: MarkScheme, schemeId: number) {
    let api = `${this.APIUrl}/${schemeId}`;
    return this.http.patch<any>(api, marksch, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }
  TagQnsToMarkScheme(questions: QuestionTag[]) {
    let api = `${this.APIUrl}/map`;
    return this.http.patch<any>(api, questions, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }
  getQuestionText(questionId: number) {
    let api = `${this.APIUrl}/view/${questionId}`;
    return this.http.get<QuestionText>(api, { headers: this.headers }).pipe(
      map((res: QuestionText) => {
        return res;
      })
    );
  }

  GetQuestionsUnderProject(pagenumber: number) {
    let api = `${this.APIUrl}/project-questions/${pagenumber}`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  Download(key: string){
    let api = `${this.FileAPIUrl}/${key}`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  public download(fileUrl: string) {
    return this.http.get(`${this.APIUrl}/download?fileUrl=${fileUrl}`, {
      reportProgress: true,
      observe: 'events',
      responseType: 'blob',
    });
  }
}
