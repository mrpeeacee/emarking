import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { BaseService } from '../../base.service';
import { HttpClient } from '@angular/common/http';
import { AlertService } from '../../common/alert.service';
import { ScoreComponentLibraryName } from 'src/app/model/project/Scoring-Component/Scoring-Component.model';
@Injectable({
  providedIn: 'root',
})
export class ScoringComponentService extends BaseService {
  readonly APIUrl = `${this.endpoint}/api/${this.apiVersion}/projects/ScoringComponentLibrary`;
  
  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }

  GetAllScoringComponentLibrary(){
    let api = `${this.APIUrl}`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );

  }


  SaveScoringCompponent(ScoreComponentLibraryName:ScoreComponentLibraryName) {

    let api = `${this.APIUrl}/markschemecreation`;
    return this.http.post<any>(api, ScoreComponentLibraryName, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );

  }


  ViewScoringComponentLibrary(ScoreComponentID:any){

    let api = `${this.APIUrl}/${ScoreComponentID}/View`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {};
      })
    );

  }
}