import { Injectable } from '@angular/core';
import { BaseService } from '../../base.service';
import { HttpClient } from '@angular/common/http';
import { ResolutionOfCoiModel, CoiSchoolModel } from 'src/app/model/project/setup/resolution-of-coi';
import { throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';


@Injectable({
    providedIn: 'root'
})
export class ResolutionofCoiService extends BaseService {

    constructor(private http: HttpClient) {
        super();
    }

    GetResolutionCOI() {
        let api = `${this.endpoint}/api/${this.apiVersion}/projects/setup/resolution-of-coi/markers`;
        return this.http.get<ResolutionOfCoiModel[]>(api, { headers: this.headers }).pipe(
            catchError((err) => {
                return throwError(err);
            }), map((res: ResolutionOfCoiModel[]) => {
                return res || {};
            })
        );
    }

    GetSchoolsCOI() {
        let api = `${this.endpoint}/api/${this.apiVersion}/projects/setup/resolution-of-coi/schools`;
        return this.http.get<CoiSchoolModel[]>(api, { headers: this.headers }).pipe(
            catchError((err) => {
                return throwError(err);
            }), map((res: CoiSchoolModel[]) => {
                return res || {};
            })
        );
    }

    updateResolutionCOI(ProjectUserRoleID: number, objCoiSchoolModel: CoiSchoolModel[]) {
        let api = `${this.endpoint}/api/${this.apiVersion}/projects/setup/resolution-of-coi/markers/${ProjectUserRoleID}`;
        return this.http.post<any>(api, objCoiSchoolModel, { headers: this.headers }).pipe(
            catchError((err) => {
                return throwError(err);
            }), map(result => {
                return result;
            })
        );
    }
}
