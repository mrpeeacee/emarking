import { HttpClient, HttpHeaders, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import {
  CourseValidationModel,
  ParamStdDetails,
  StudentsResult,
  StudentsResultStatistics,
} from 'src/app/model/reports/studentreports';
import { BaseService } from '../base.service';
import { AlertService } from '../common/alert.service';
import { SearchFilterModel } from 'src/app/model/project/setup/user-management';

@Injectable({
  providedIn: 'root',
})
export class StudentReportsService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/reports/students-result`;

  private readonly ExportAPIUrl = `${this.endpoint}/api/public/${this.apiVersion}/media/export`;
  
  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }

  GetStudentResultDetails(
    ExamYear: number,
    paraDtls: ParamStdDetails
  ): Observable<any> {
    let api = `${this.APIUrl}/count/${ExamYear}`;
    return this.http
      .post<StudentsResultStatistics>(api, paraDtls, { headers: this.headers })
      .pipe(
        map((res: any) => {
          return res || {};
        })
      );
  }

  GetStudentsResult(
    ExamYear: number,
    paraDtls: ParamStdDetails
  ): Observable<any> {
    let api = `${this.APIUrl}/${ExamYear}`;
    return this.http
      .post<StudentsResult[]>(api, paraDtls, { headers: this.headers })
      .pipe(
        map((res: any) => {
          return res || {};
        })
      );
  }

  GetCourseValidationResult(): Observable<any> {
    let api = `${this.APIUrl}/course-validation`;
    return this.http
      .get<CourseValidationModel[]>(api, { headers: this.headers })
      .pipe(
        map((res: CourseValidationModel[]) => {
          return res || {};
        })
      );
  }
  GetUserResponse(_scheduleid: any,Isfrommarkingplayer:boolean): Observable<any> {
    return this.http.get<any>(
      `${this.endpoint}/api/${this.apiVersion}/projects/reports/userresponse/${_scheduleid}/${Isfrommarkingplayer}/0/true`,
      { headers: this.headers }
    );
  }

  ExportHtmlToPdf(htmlcontent: string, fileName: string) {
    this._ExportHtmlToPdf(htmlcontent).subscribe({
      next: (response: any) => { 
        if (response.status === 200) {
          const blob = new Blob([response.body], { type: response.headers.get('Content-Type') });
          this.triggerFileDownload(blob, fileName);
        } else {
          console.error('Unexpected response status:', response.status);
        }
      },
      error: (ad: any) => { 
        throw ad;
      }
    });
  }

  private _ExportHtmlToPdf(htmlcontent: string): Observable<any> {
    let api = `${this.ExportAPIUrl}/pdf`;

    // Prepare base request without specific headers for potential reuse
    const baseRequest = new HttpRequest<any>('POST', api, JSON.stringify(htmlcontent), {
      responseType: 'blob'
    });

    // Flag for conditional header addition
    let addSpecificHeaders = true;

    // Clone the request if specific headers need to be added
    const requestOptions: HttpRequest<any> = addSpecificHeaders ? baseRequest.clone({
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }), // Add your specific headers here
      // Other modifications if needed
    }) : baseRequest;

    return this.http.request(requestOptions)
      .pipe(
        map((res: any) => {
          return res || {};
        })
      );
  }

  private triggerFileDownload(blob: Blob, fileName: string) {
    var link = document.createElement('a');
    if (link.download !== undefined) {
      var url = URL.createObjectURL(blob);
      link.setAttribute('href', url);
      link.setAttribute('download', fileName);
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  }

  GetStudentCompleteReport(): Observable<any> {
    const requestOptions: Object = {
      headers: this.headers,
      responseType: 'blob',
    };

    return this.http.get<any>(
      `${this.APIUrl}/student-complete-report`,
      requestOptions
    );
  }

  GetStudentCompleteArchiveReport(): Observable<any> {
    const requestOptions: Object = {
      headers: this.headers,
      responseType: 'blob',
    };
    return this.http.get<any>(
      `${this.APIUrl}/student-complete-report-archive`,
      requestOptions
    );   
  }

  GetStudentCompleteTextArchiveReport(): Observable<any> {
    const requestOptions: Object = {
      headers: this.headers,
      responseType: 'blob',
    };
    return this.http.get<any>(
      `${this.APIUrl}/student-complete-textreport-Archive`,
      requestOptions
    );
    
  }

  GetStudentCompleteTextReport(): Observable<any> {
    const requestOptions: Object = {
      headers: this.headers,
      responseType: 'blob',
    };
    return this.http.get<any>(
      `${this.APIUrl}/student-complete-textreport`,
      requestOptions
    );
  }

  GetAllUseresponses(ObjectsearchFilterModel:SearchFilterModel): Observable<any> {
    return this.http.post<any>(
      `${this.endpoint}/api/${this.apiVersion}/projects/reports/userresponse/`,ObjectsearchFilterModel,
      { headers: this.headers }
    );
  }

  GetSchoolSDetails(): Observable<any> {
    return this.http.get<any>(
      `${this.endpoint}/api/${this.apiVersion}/projects/reports/schools`,
      { headers: this.headers }
    );
  }
}
