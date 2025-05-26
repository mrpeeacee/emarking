import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable,BehaviorSubject  } from 'rxjs';
import { CreateEditUser, GetAllApplicationUsersModel, GetAllUsersModel, GetCreateEditUserModel, SearchFilterModel, UserCreations } from 'src/app/model/Global/UserManagement/UserManagementModel';
import { BaseService } from 'src/app/services/base.service';
import { AlertService } from 'src/app/services/common/alert.service';


@Injectable({
  providedIn: 'root'
})
export class GlobalUserManagementService extends BaseService {
  readonly GlobalUserManagementApiUrl = `${this.endpoint}/api/${this.apiVersion}/global/user-management`;

  constructor(
    private http: HttpClient, public Alert: AlertService) {
    super();
  }

  GetAllUsers(searchFilterModel: SearchFilterModel) {
    return this.http.post<GetAllApplicationUsersModel>(this.GlobalUserManagementApiUrl + `/allusers`, searchFilterModel, { headers: this.headers })
  }

  GetCreateEditUserdetails(UserId: number) {
    return this.http.get<GetCreateEditUserModel>(this.GlobalUserManagementApiUrl + `/roleschool/${UserId}`, { headers: this.headers })
  }

  Createuser(createuserobj: CreateEditUser) {
    return this.http.post<CreateEditUser>(this.GlobalUserManagementApiUrl + `/CreateEditUser`, createuserobj, { headers: this.headers })
  }

  Resetpwd(UserId: CreateEditUser) {
    return this.http.post<CreateEditUser>(this.GlobalUserManagementApiUrl + `/Resetpwd`,UserId, { headers: this.headers })
  }

  UserCreations(file: any, type: number): Observable<any> {
    const formData = new FormData();
    formData.append("file", file, file.name);
    return this.http.post<UserCreations>(this.GlobalUserManagementApiUrl + `/UserCreation/${type}`, formData);
  }

  GetBlockedUsers(): Observable<GetAllUsersModel> {
    return this.http.get<GetAllUsersModel>(this.GlobalUserManagementApiUrl + `/BlockedUsers`, { headers: this.headers });
  }

  unBlockSubmit(blockUsers: any): Observable<any> {
    return this.http.post<any>(this.GlobalUserManagementApiUrl + `/unblockUser`, blockUsers, { headers: this.headers });
  }
  GetPassPhrase(): Observable<any> {
    return this.http.get<any>(this.GlobalUserManagementApiUrl + `/PassPhrase`, { headers: this.headers });
  }
  AddPassPhraseUsers(importUser: any): Observable<any> {
    return this.http.post<any>(this.GlobalUserManagementApiUrl + `/AddPassPhrase`, importUser, { headers: this.headers });
  }
  
  ScriptExists(UserId: any, TypeId: number): Observable<any> {
    return this.http.get<any>(this.GlobalUserManagementApiUrl + `/ScriptExists/${UserId}/${TypeId}`, { headers: this.headers });
  }
  
  ChangeStatusUsers(UserId: any, TypeId: number, loginName:string): Observable<any> {
    return this.http.post<any>(this.GlobalUserManagementApiUrl + `/ChangeStatusUsers/${UserId}/${TypeId}/${loginName}`, { headers: this.headers });
  }
  StatusOfRemove(UserId: number): Observable<any> {
    return this.http.get<any>(this.GlobalUserManagementApiUrl + `/RemoveUser/${UserId}`, { headers: this.headers });
  }

  getMyprofileDetails(): Observable<any> {
    return this.http.get<any>(this.GlobalUserManagementApiUrl + `/my-profile`, { headers: this.headers });
  }

  getMyprofileDetailsProject(ProjectuserroleId:number): Observable<any> {
    return this.http.get<any>(this.GlobalUserManagementApiUrl + `/my-profile/${ProjectuserroleId}`, { headers: this.headers });
  }

  GetUserdataCompelteReport(): Observable<any> {

    const requestOptions: Object = {
      headers:  this.headers,
      responseType: 'blob'
    }
   
    return this.http.get<any>(this.GlobalUserManagementApiUrl +`/usermanagement-complete-report`, requestOptions);
  }

  GetApplicationLevelUserRoles(): Observable<any> {
    return this.http.get<any>(this.GlobalUserManagementApiUrl + `/applicationlevel-user-role`, { headers: this.headers });
  }

  private dataSubject = new BehaviorSubject<any>(null);
  public data$ = this.dataSubject.asObservable();

  updateData(data: any) {
    this.dataSubject.next(data);
  }

}
