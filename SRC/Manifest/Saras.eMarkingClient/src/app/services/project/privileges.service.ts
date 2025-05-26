import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs'; 
import { BaseService } from '../base.service';
import { IUserprivileges } from 'src/app/model/project/UserPrivileges';
import { CreateEditUser } from 'src/app/model/Global/UserManagement/UserManagementModel';
import {map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})

export class UserprivilegeService extends BaseService {
  readonly ApiUrl = `${this.endpoint}/api/${this.apiVersion}/projects/privileges/`;

  constructor(
    private http: HttpClient) {
    super();
  } 
  
  GetUserPrivileges(Type : number = 1): Observable<[IUserprivileges]> { 
    return this.http.get<any>(this.ApiUrl + Type, { headers: this.headers });
  }
  getMyprofileDetails(): Observable<any> {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/authenticate/my-profile`;
    return this.http.get<CreateEditUser>(api, { headers: this.headers }).pipe(
      map((res: CreateEditUser) => {
        return res || {}
      })
    )
  }
}
