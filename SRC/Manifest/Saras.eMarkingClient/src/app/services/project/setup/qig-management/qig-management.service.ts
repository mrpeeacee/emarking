import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { QigUserModel } from 'src/app/model/project/qig';
import { UserLogin } from 'src/app/modules/auth/user';
import { BaseService } from 'src/app/services/base.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { ManageQigs, QigDetails, QigQuestionModel, QigQuestionsDetails, Tagqigdetails, CreateQigsModel, FinalRemarks } from '../../../../model/project/setup/qig-management/qig-management-model';


@Injectable({
  providedIn: 'root'
})
export class QigManagementService extends BaseService {
  readonly QigMappingApiUrl = `${this.endpoint}/api/${this.apiVersion}/projects/setup/qig-management`;

  constructor(
    private http: HttpClient, public Alert: AlertService) {
    super();
  }

  GetCurrentProjectQigs() {
    return this.http.get<any>(this.QigMappingApiUrl + `/Qigs`, { headers: this.headers });
  }
  GetQigQuestions(QuestionType: number) {
    return this.http.get<any>(this.QigMappingApiUrl + `/QigQuestions/${QuestionType}`, { headers: this.headers });
  }
  GetQigDetails(QigId: number) {
    return this.http.get<any>(this.QigMappingApiUrl + `/QigDetails/${QigId}`, { headers: this.headers });
  }
  SaveQigDetails(qigdetails: QigDetails) {
    return this.http.post<any>(this.QigMappingApiUrl + `/QigDetails`, qigdetails, { headers: this.headers });
  }
  GetManagedQigDetails() {
    return this.http.get<ManageQigs>(this.QigMappingApiUrl + `/QigmanagedDetails`, { headers: this.headers });
  }
  GetQuestionText(ProjectQigId: number, ProjectQuestionID: number) {
    return this.http.get<QigQuestionModel>(this.QigMappingApiUrl + `/Qnsxml/${ProjectQigId}/${ProjectQuestionID}`, { headers: this.headers });
  }
  GetQuestionDetails(QigType: number, ProjectQigId: number, QnsType: number) {
    return this.http.get<QigQuestionsDetails>(this.QigMappingApiUrl + `/Qnsdetails/${QigType}/${ProjectQigId}/${QnsType}`, { headers: this.headers });
  }
  MoveorTagQIG(tagqigdetails: Tagqigdetails) {
    return this.http.post<any>(this.QigMappingApiUrl + `/moveqig`, tagqigdetails, { headers: this.headers });
  }
  getQigs(Qigtype?: number): Observable<QigUserModel[]> {
    return this.http.get<QigUserModel[]>(this.QigMappingApiUrl + `/getqigs/${Qigtype}`, { headers: this.headers });
  }
  CreateQigs(createqigsdetails: CreateQigsModel) {
    return this.http.post<CreateQigsModel>(this.QigMappingApiUrl + `/Qiginsertion`, createqigsdetails, { headers: this.headers });
  }
  SaveQigQuestions(remarks:FinalRemarks) {
    return this.http.post<any>(this.QigMappingApiUrl  + `/finaliseQig`,remarks, { headers: this.headers });
  }
  SaveQigQuestionsDetails(createqigsModel: CreateQigsModel){
    return this.http.post<any>(this.QigMappingApiUrl + `/QigQuestions`,createqigsModel, { headers: this.headers });
  }
  GetUntaggedQuestionsDetails() {
    return this.http.get<ManageQigs>(this.QigMappingApiUrl + `/UntaggedQuestions`, { headers: this.headers });
  }
  DeleteQig(TempManageQigObject: ManageQigs) {
    return this.http.post<ManageQigs>(this.QigMappingApiUrl + `/Deleteqig`,TempManageQigObject, { headers: this.headers });
  }
  QigReset(){
    return this.http.post<any>(this.QigMappingApiUrl + '/qigreset',{headers:this.headers});
  }


  Login(user: UserLogin) {
    return this.http.post<any>(this.QigMappingApiUrl + '/SaveUser',user,{headers:this.headers});
  }


}
