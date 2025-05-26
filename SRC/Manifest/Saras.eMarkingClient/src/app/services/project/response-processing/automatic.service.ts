import { Injectable } from '@angular/core';
import { BaseService } from '../../base.service';
import { HttpClient } from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { AutomaticQuestionsModel, ModeratescoreModel } from 'src/app/model/project/response-processing/automatic-model';


@Injectable({
  providedIn: 'root'
})
export class AutomaticQuestionsService extends BaseService {

  constructor(private http: HttpClient) {
    super();
  }

  GetViewAllAutomaticQuestions() {
    let api = `${this.endpoint}/api/${this.apiVersion}/projects/response-processing/automatic/questions`;
    return this.http.get<AutomaticQuestionsModel[]>(api, { headers: this.headers }).pipe(
      catchError((err) => {
        return throwError(err);
      }), map((res: AutomaticQuestionsModel[]) => {
        return res || {};
      })
    );
  }


  GetViewModerateAutomaticQuestions(parentQuestionId:number) {
    let api = `${this.endpoint}/api/${this.apiVersion}/projects/response-processing/automatic/questions/${parentQuestionId}`;
    return this.http.get<AutomaticQuestionsModel[]>(api, { headers: this.headers }).pipe(
      catchError((err) => {
        return throwError(err);
      }), map((res: AutomaticQuestionsModel[]) => {
        return res || {};
      })
    );
  }

  UpdateModeratescore(objModeratescoreModel: ModeratescoreModel) {
    let api = `${this.endpoint}/api/${this.apiVersion}/projects/response-processing/automatic/moderate-score`;
    return this.http.post<any>(api, objModeratescoreModel, { headers: this.headers }).pipe(
        catchError((err) => {
            return throwError(err);
        }), map(result => {
            return result;
        })
    );
}

}
