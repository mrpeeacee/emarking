import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/services/base.service';
import { HttpClient } from '@angular/common/http';
import { Observable,throwError } from 'rxjs';
import { QigModel } from 'src/app/model/project/setup/qig-configuration/random-check-setting-model';
import { catchError, map } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
  })
  export class RandomCheckSetting extends BaseService {
  
    constructor(private http:HttpClient) {
      super();
    }
    readonly apiurl = `${this.endpoint}/api/${this.apiVersion}/projects/setup/qig-config/rc-settings`;
    GetRandomcheckQIGs(QigId:number): Observable<QigModel> {      
        return this.http.get<any>(this.apiurl + `/${QigId}` , { headers: this.headers });
      }

      patchRandomcheckQIGs(qig: QigModel) {
        return this.http.patch<boolean>(this.apiurl, qig, { headers: this.headers }).pipe(
          catchError((err) => {
            return throwError(err);
          }), map(result => {
            return result;
          })
        );
      }
}