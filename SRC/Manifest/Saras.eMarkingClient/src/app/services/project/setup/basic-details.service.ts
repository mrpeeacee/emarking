import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BasicDetailsModel } from 'src/app/model/project/setup/basic-details-model';
import { BaseService } from '../../base.service';
import { AlertService } from 'src/app/services/common/alert.service';

@Injectable({
    providedIn: 'root'
})

export class BasicDetailsService extends BaseService {
    readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/setup/basic-details`;

    constructor(
        private http: HttpClient, public Alert: AlertService) {
        super();
    }

    GetBasicDetails(navigate:number): Observable<BasicDetailsModel> {
        return this.http.get<BasicDetailsModel>(`${this.APIUrl}/${navigate}`, { headers: this.headers });
    }
    UpdateBasicDetails(_project: any): Observable<any> {
        return this.http.post<any>(this.APIUrl, _project, { headers: this.headers });
    }
}