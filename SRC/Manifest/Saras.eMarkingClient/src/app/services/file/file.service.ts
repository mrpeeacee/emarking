import { Injectable } from '@angular/core';
import { BaseService } from '../base.service';
import { HttpClient } from '@angular/common/http';
import { AlertService } from '../common/alert.service';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FileService extends BaseService {
  readonly FileAPIUrl = `${this.endpoint}/api/${this.apiVersion}/file`;

  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }

  upload(file: any): Observable<any> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    let api = `${this.FileAPIUrl}/upload`;
    return this.http.post<any>(api, formData).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  GetFilesLists(schemeid: number) {
    let api = `${this.FileAPIUrl}/filelist/${schemeid}`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );
  }

  Download(key: string) {
    let api = `${this.FileAPIUrl}/${key}`;
    return this.http
      .get<Blob>(api, {
        responseType: 'blob' as 'json',
        observe: 'response',
        headers: this.headers,
      })
      .pipe(
        map((res: any) => {
          return res || {};
        })
      );
  }
}
