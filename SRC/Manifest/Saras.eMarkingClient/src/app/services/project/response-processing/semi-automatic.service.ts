import { Injectable } from '@angular/core';
import { BaseService } from '../../base.service';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ViewFrequencyDistributionModel, EnableManualMarkigModel, ViewAllBlankSummaryModel, ModerateMarks, FibDiscrepencyReportModel, DiscrepencyNormalizeScoreModel } from 'src/app/model/project/response-processing/semi-automatic-model';
import { QigQuestionModel } from 'src/app/model/project/qig';
import { ProjectClosureModel } from 'src/app/model/project/setup/project-closure-model';


@Injectable({
    providedIn: 'root'
})
export class SemiAutomaticQuestionsService extends BaseService {

    constructor(private http: HttpClient) {
        super();
    }
    //readonly apiurl = `${this.endpoint}/api/${this.apiVersion}/projects/response-processing/semi-automatic/frequency-distributions`;
    readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/setup/projectclosure`;
    GetAllViewQuestions() {
        let api = `${this.endpoint}/api/${this.apiVersion}/projects/response-processing/semi-automatic/frequency-distributions/allviewquestion`;
        return this.http.get<QigQuestionModel[]>(api, { headers: this.headers }).pipe(
            catchError((err) => {
                return throwError(err);
            }), map((res: QigQuestionModel[]) => {
                return res || {};
            })
        );
    }

    GetFrequencyDistribution(QuestionId: number) {
        let api = `${this.endpoint}/api/${this.apiVersion}/projects/response-processing/semi-automatic/frequency-distributions/view-frequency-distributions/${QuestionId}`;
        return this.http.get<ViewFrequencyDistributionModel[]>(api, { headers: this.headers }).pipe(
            catchError((err) => {
                return throwError(err);
            }), map((res: ViewFrequencyDistributionModel[]) => {
                return res || {};
            })
        );
    }

    UpdateModerateMarks(objModerateMarksModel: ModerateMarks) {
        let api = `${this.endpoint}/api/${this.apiVersion}/projects/response-processing/semi-automatic/frequency-distributions/moderate-marks`;
        return this.http.post<any>(api, objModerateMarksModel, { headers: this.headers }).pipe(
            catchError((err) => {
                return throwError(err);
            }), map(result => {
                return result;
            })
        );
    }

    UpdateOverallModerateMarks(ProjectQuestionId: number): Observable<any> {
        let api = `${this.endpoint}/api/${this.apiVersion}/projects/response-processing/semi-automatic/frequency-distributions/moderate-marks`;
        return this.http.post<any>(api + `/${ProjectQuestionId}`, { headers: this.headers });
    }


    UpdateManualMarkig(objEnableManualMarkigModel: EnableManualMarkigModel) {
        let api = `${this.endpoint}/api/${this.apiVersion}/projects/response-processing/semi-automatic/frequency-distributions/enable-manual-markig`;
        return this.http.post<any>(api, objEnableManualMarkigModel, { headers: this.headers }).pipe(
            catchError((err) => {
                return throwError(err);
            }), map(result => {
                return result;
            })
        );
    }

    GetAllBlankSummary(ParentQuestionId: number) {
        let api = `${this.endpoint}/api/${this.apiVersion}/projects/response-processing/semi-automatic/frequency-distributions/view-all-blank-summary/${ParentQuestionId}`;
        return this.http.get<ViewAllBlankSummaryModel[]>(api, { headers: this.headers }).pipe(
            catchError((err) => {
                return throwError(err);
            }), map((res: ViewAllBlankSummaryModel[]) => {
                return res || {};
            })
        );
    }

    GetDiscrepancyReportFIB(CandidateAnswer: string,ProjectQuestionId:number) {
        let api = `${this.endpoint}/api/${this.apiVersion}/projects/response-processing/semi-automatic/frequency-distributions/discrepancy-report-fib/${CandidateAnswer}/${ProjectQuestionId}`;
        return this.http.get<FibDiscrepencyReportModel>(api, { headers: this.headers }).pipe(
            catchError((err) => {
                return throwError(err);
            }), map((res: FibDiscrepencyReportModel) => {
                return res || {};
            })
        );
    }

    UpdateNormaliseScore(objNormalizedscoreModel: DiscrepencyNormalizeScoreModel) {
        let api = `${this.endpoint}/api/${this.apiVersion}/projects/response-processing/semi-automatic/frequency-distributions/normalise-score`;
        return this.http.post<any>(api, objNormalizedscoreModel, { headers: this.headers }).pipe(
            catchError((err) => {
                return throwError(err);
            }), map(result => {
                return result;
            })
        );
    }

    CheckDiscrepancy(projectQuestionId: number): any {
        let api = `${this.APIUrl}/discrepancy/${projectQuestionId}`;
        return this.http
          .get<ProjectClosureModel[]>(api, { headers: this.headers })
          .pipe(
            map((res: any) => {
              return res || {};
            })
          );
      }

      UpdateAllResponsestoManualMarkig(ParentQuestionId: number) {
        let api = `${this.endpoint}/api/${this.apiVersion}/projects/response-processing/semi-automatic/frequency-distributions/all-responses-to-manual-markig/${ParentQuestionId}`;
        return this.http.post<any>(api, { headers: this.headers }).pipe(
            catchError((err) => {
                return throwError(err);
            }), map(result => {
                return result;
            })
        );
    }

    UpdateAcceptDescrepancy(objAcceptDecrepancyModel: DiscrepencyNormalizeScoreModel) {
        let api = `${this.endpoint}/api/${this.apiVersion}/projects/response-processing/semi-automatic/frequency-distributions/accept-descrepancy`;
        return this.http.post<any>(api, objAcceptDecrepancyModel, { headers: this.headers }).pipe(
            catchError((err) => {
                return throwError(err);
            }), map(result => {
                return result;
            })
        );
    }
}
