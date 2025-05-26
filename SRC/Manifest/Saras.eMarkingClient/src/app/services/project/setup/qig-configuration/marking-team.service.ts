import { BaseService } from 'src/app/services/base.service';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';


@Injectable({
    providedIn: 'root'
  })

  export class MarkingTeamService extends BaseService {

    constructor(private http:HttpClient) {
        super();
      }
      readonly apiurl = `${this.endpoint}/api/${this.apiVersion}/projects/setup/qig-config/marking-teams`;
      GetMarkingTeamDetails(QigId:number):Observable<any>{
        return this.http.get<any>(this.apiurl + `/${QigId}`,{headers:this.headers});
      }
    
  }