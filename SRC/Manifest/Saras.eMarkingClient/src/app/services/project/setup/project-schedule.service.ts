import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DayWiseScheduleModel, ProjectScheduleModel } from 'src/app/model/project/setup/project-schedule-model';
import { BaseService} from 'src/app/services/base.service'
import { AlertService} from 'src/app/services/common/alert.service';

@Injectable({
    providedIn: 'root'
})
export class ProjectScheduleService extends BaseService {
    readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/setup/project-schedules`;

    constructor(
        private http: HttpClient, public Alert: AlertService) {
        super();
    }

    GetProjectSchedule(): Observable<any> {
        return this.http.get<any>(this.APIUrl, { headers: this.headers });
    }

    CreateProjectSchedule(ProjectSchedule: any): Observable<ProjectScheduleModel> {
        return this.http.post<any>(this.APIUrl, ProjectSchedule, { headers: this.headers });
    }

    UpdateProjectSchedule(ProjectSchedule: any): Observable<ProjectScheduleModel> {
        return this.http.patch<any>(this.APIUrl, ProjectSchedule, { headers: this.headers });
    }

    GetDayWiseConfig(objDayWiseScheduleModel: DayWiseScheduleModel): Observable<any> {
        return this.http.post<any>(this.APIUrl + `/daywise-get`, objDayWiseScheduleModel, { headers: this.headers });
    }

    UpdateDayWiseConfig(ProjectScheduleCalendar: any): Observable<any> {
        return this.http.post<any>(this.APIUrl + `/daywise`, ProjectScheduleCalendar, { headers: this.headers });
    }
}