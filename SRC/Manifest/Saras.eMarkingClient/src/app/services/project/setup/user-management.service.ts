import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Projectuserscount, ProjectUsersModel, Qiguserdataviewmodel, QigUsersViewByIdModel, BlankQigIds, UserCreationModel, RoleDetails, MoveMarkingTeamToEmptyQig, MoveMarkingTeamToEmptyQigs, UserBulkUploadModel, SaveMappedUsersModel, SearchFilterModel, UnMapAodetails, SuspendUserDetails, CreateEditProjectUserRoleChange } from 'src/app/model/project/setup/user-management';
import { BaseService } from 'src/app/services/base.service';
import { AlertService } from 'src/app/services/common/alert.service';


@Injectable({
  providedIn: 'root'
})
export class UserManagementService extends BaseService {
  readonly KeypersonnelApiUrl = `${this.endpoint}/api/${this.apiVersion}/projects/s1/setup/key-personnels`;
  readonly UsermanagementApiUrl = `${this.endpoint}/api/${this.apiVersion}/projects/setup/project-users`;

  constructor(
    private http: HttpClient, public Alert: AlertService) {
    super();
  }

  UploadFile(file: any): Observable<any> {
    const formData = new FormData();
    formData.append("file", file, file.name);
    return this.http.post(this.UsermanagementApiUrl + `/NPOI_upload`, formData)
  }

  ImportFile(file: any): Observable<any> {
    const formData = new FormData();
    formData.append("file", file, file.name);
    return this.http.post(this.UsermanagementApiUrl + `/file_upload`, formData)
  }

  QigUsersImportFile(file: any, QigId: number): Observable<any> {
    const formData = new FormData();
    formData.append("file", file, file.name);
    return this.http.post(this.UsermanagementApiUrl + `/qig/file_upload/${QigId}`, formData)
  }

  Userscount(QigId?: number): Observable<Projectuserscount> {
    return this.http.get<Projectuserscount>(this.UsermanagementApiUrl + `/${QigId}`, { headers: this.headers });
  }

  Projectuserview(): Observable<ProjectUsersModel[]> {
    return this.http.get<ProjectUsersModel[]>(this.UsermanagementApiUrl + `/project/userview`, { headers: this.headers });
  }

  QigdataorHierarchyview(qiguserdataviewmodel:Qiguserdataviewmodel): Observable<Qiguserdataviewmodel[]> {
    return this.http.post<Qiguserdataviewmodel[]>(this.UsermanagementApiUrl + `/dataorHierarchy_view`, qiguserdataviewmodel, { headers: this.headers });
    ////return this.http.get<Qiguserdataviewmodel[]>(this.UsermanagementApiUrl + `/qig/dataorHierarchy_view`, { headers: this.headers });
    ////return this.http.post<QigUsersViewByIdModel[]>(this.UsermanagementApiUrl + `/mappedusers`,Objsearchfilter, { headers: this.headers });

  }
  GetQiguserDataById(QigId: number, ProjectQIGTeamHierarchyID: number): Observable<QigUsersViewByIdModel> {
    return this.http.get<QigUsersViewByIdModel>(this.UsermanagementApiUrl + `/qig/${QigId}/${ProjectQIGTeamHierarchyID}`, { headers: this.headers });
  }

  UpdateQiguserDataById(QigId: number, ProjectQIGTeamHierarchyID: number, ReportingToId: number): Observable<any> {
    return this.http.post<any>(this.UsermanagementApiUrl + `/qig/${QigId}/${ProjectQIGTeamHierarchyID}/${ReportingToId}`, { headers: this.headers });
  }

  UserDelete(UserRoleId: number, QigId: number): Observable<any> {
    return this.http.post<any>(this.UsermanagementApiUrl + `/qig/delete-users/${UserRoleId}/${QigId}`, { headers: this.headers });
  }
  GetSuspendUsers(ProjectUserRoleID: number,SuspendOrUnmap:number): Observable<any> {
    return this.http.post<any>(this.UsermanagementApiUrl + `/qig/suspend-users/${ProjectUserRoleID}/${SuspendOrUnmap}`, { headers: this.headers });
  }
  SuspendUsers(suspenduser: SuspendUserDetails): Observable<any> {
    return this.http.post<any>(this.UsermanagementApiUrl + `/qig/suspend-save`, suspenduser, { headers: this.headers });
  }

  GetBlankQigIds(): Observable<BlankQigIds[]> {
    return this.http.get<BlankQigIds[]>(this.UsermanagementApiUrl + `/qig/BlankQigs`, { headers: this.headers });
  }

  GetUsersRoles(): Observable<RoleDetails[]> {
    return this.http.get<RoleDetails[]>(this.UsermanagementApiUrl + '/qig/user-roles', { headers: this.headers });
  }

  MoveMarkingTeamToEmptyQig(moveMarkingTeamToEmptyQig: MoveMarkingTeamToEmptyQig): Observable<any> {
    return this.http.post<any>(this.UsermanagementApiUrl + `/qig/MoveMarkingTeamToEmptyQig`, moveMarkingTeamToEmptyQig, { headers: this.headers });
  }

  CreateUser(UserCreation: UserCreationModel): Observable<any> {
    return this.http.post<any>(this.UsermanagementApiUrl + '/qig/createuser', UserCreation, { headers: this.headers });
  }

  CheckS1StartedOrLiveMarkingEnabled(QigId: number): Observable<any> {
    return this.http.get<any>(this.UsermanagementApiUrl + `/qig/s1orlivestarted/${QigId}`, { headers: this.headers });
  }

  GetEmptyQigIds(): Observable<BlankQigIds[]> {
    return this.http.get<BlankQigIds[]>(this.UsermanagementApiUrl + `/qig/EmptyQigs`, { headers: this.headers });
  }

  MoveMarkingTeamToEmptyQigs(moveMarkingTeamToEmptyQig: MoveMarkingTeamToEmptyQigs): Observable<any> {
    return this.http.post<any>(this.UsermanagementApiUrl + `/qig/MoveMarkingTeamToEmptyQigIds`, moveMarkingTeamToEmptyQig, { headers: this.headers });
  }

  UserBulkUpload(file: any): Observable<any> {
    const formData = new FormData();
    formData.append("file", file, file.name);
    return this.http.post(this.UsermanagementApiUrl + `/bulk-upload`, formData);
  }

  ValidateUserDetails(bulkUserUploadModel: UserBulkUploadModel[]): Observable<any> {
    return this.http.post<any>(this.UsermanagementApiUrl + '/validate-userdetails', bulkUserUploadModel, { headers: this.headers });
  }

  SaveBulkUsers(bulkUserUploadModel: UserBulkUploadModel[]): Observable<any> {
    return this.http.post<any>(this.UsermanagementApiUrl + '/save-bulkusers', bulkUserUploadModel, { headers: this.headers });
  }
  UnBlockUsers(UserId: number): Observable<any> {
    return this.http.post<any>(this.UsermanagementApiUrl + `/Unblocking/${UserId}`, { headers: this.headers });
  }
  BlockorUnblockUsers(UserRoleId: number, Isactive: boolean, QigId: number): Observable<any> {
    return this.http.post<any>(this.UsermanagementApiUrl + `/blockorunblock/${UserRoleId}/${QigId}/${Isactive}`, { headers: this.headers });
  }
  GetUnMappedUsers(Objsearchfilter: SearchFilterModel): Observable<any> {
    return this.http.post<QigUsersViewByIdModel[]>(this.UsermanagementApiUrl + `/Unmappedusers`, Objsearchfilter, { headers: this.headers });
  }
  GetMappedUsers(Objsearchfilter: SearchFilterModel): Observable<any> {
    return this.http.post<QigUsersViewByIdModel[]>(this.UsermanagementApiUrl + `/mappedusers`,Objsearchfilter, { headers: this.headers });
  }
  GetSelectedMappedUser(userid: number): Observable<any> {
    return this.http.get<QigUsersViewByIdModel>(this.UsermanagementApiUrl + `/selectedmappedusers/${userid}`, { headers: this.headers });
  }
  UnMapParticularUsers(Unmapaodetail : UnMapAodetails): Observable<any>{
    return this.http.post<any>(this.UsermanagementApiUrl + `/UnMappingParticularUsers`,Unmapaodetail, { headers: this.headers });
}
  GetRoles(): Observable<any> {
    return this.http.get<any>(this.UsermanagementApiUrl + `/Roles`, { headers: this.headers });
  }
  SaveMapUsers(mapdetails: SaveMappedUsersModel): Observable<any> {
    return this.http.post<SaveMappedUsersModel>(this.UsermanagementApiUrl + '/push-mappedusers', mapdetails, { headers: this.headers });
  }
  MapAO(mapdetails: SaveMappedUsersModel): Observable<any> {
    return this.http.post<SaveMappedUsersModel>(this.UsermanagementApiUrl + `/unmapped-ao`, mapdetails, { headers: this.headers });
  }

  GetUserWithdraw(ObjectsearchFilterModel:SearchFilterModel): Observable<any> {
    return this.http.post<any>(this.UsermanagementApiUrl + `/GetWithDrawUsers`,ObjectsearchFilterModel, { headers: this.headers });
  }
  
  UserWithdraw(Id: any): Observable<any> {
    return this.http.post<any>(this.UsermanagementApiUrl + `/WithDrawUsers`, Id, { headers: this.headers });
  }

  UntagqigUsers(suspenduser: SuspendUserDetails): Observable<any> {
    return this.http.post<any>(this.UsermanagementApiUrl + `/qig/un-tag-qig-users`, suspenduser, { headers: this.headers });
  }
  GetUpperHierarchyRole(RoleId:number, QigId:number){
    return this.http.get<any>(this.UsermanagementApiUrl + `/qig/un-tag-qig-users/${RoleId}/${QigId}`, { headers: this.headers });
  }
  ReTagQigUser(RoleId:number, QigId:number, ReportingTo:number, RoleCode:string){
    return this.http.post<any>(this.UsermanagementApiUrl + `/qig/re-tag-qig-users/${RoleId}/${QigId}/${ReportingTo}/${RoleCode}`, { headers: this.headers });
  }

  GetReTagUpperHierarchyRole(RoleId:number, QigId:number){
    return this.http.get<any>(this.UsermanagementApiUrl + `/qig/re-tag-qig-users/${RoleId}/${QigId}`, { headers: this.headers });
  }

  GetReportingToHierarchy(RoleId:number, QigId:number){
    return this.http.get<any>(this.UsermanagementApiUrl + `/qig/re-tag-reporting/${RoleId}/${QigId}`, { headers: this.headers });
  }

  Untaguserhaschildusers(RoleId:number, QigId:number){
    return this.http.get<any>(this.UsermanagementApiUrl + `/qig/tag-qig-users-exits/${RoleId}/${QigId}`, { headers: this.headers });
  }

  CheckActivityOfMP(ProjectUserRoleId:number){
    return this.http.get<any>(this.UsermanagementApiUrl + `/Checkactivity/${ProjectUserRoleId}`, { headers: this.headers });
  }

  CreateEditProjectUserRoleChange(userchangerolemodel:CreateEditProjectUserRoleChange){
    return this.http.post<any>(this.UsermanagementApiUrl + `/Checkactivity/userrolechange`,userchangerolemodel, { headers: this.headers });
  }

}
