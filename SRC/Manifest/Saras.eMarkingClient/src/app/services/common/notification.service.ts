import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../base.service';
import { AlertService } from './alert.service';
import { BuildVersion } from './notification-build';
import { BehaviorSubject } from 'rxjs';
import { BrandingModel } from 'src/app/model/globalconst';

@Injectable({
  providedIn: 'root'
})
export class NotificationService extends BaseService {

  constructor(private http: HttpClient, public Alert: AlertService) {
    super();
  }
  public CurrentDateTime$ = new BehaviorSubject<Date>(new Date());
  public brand$ = new BehaviorSubject<BrandingModel>(new BrandingModel());
  private serverTime = new Date();
  getServerDatetime(): any {
    this.getServertime().subscribe((_servertime: Date) => {
      if (_servertime != null) {
        this.serverTime = new Date(_servertime);
        this.CurrentDateTime$.next(this.serverTime);
        clearInterval(this.refinterval);
        this.startservertime();
      }
      return _servertime;
    });
  }
  private refinterval: any;
  private startservertime() {
    this.refinterval = setInterval(() => {
      this.serverTime = new Date(this.serverTime.getTime() + 1000);
      this.CurrentDateTime$.next(this.serverTime);
    }, 1000);
  }

  private getServertime(): any {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/notifications/serverdatetime`;
    return this.http.get<any>(api, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {}
      })
    )
  }
  private buildnumber(): any {
    let api = `${this.endpoint}/api/public/${this.apiVersion}/notifications/buildnumber`;
    return this.http.get<BuildVersion>(api, { headers: this.headers }).pipe(
      map((res: BuildVersion) => {
        return res || {}
      })
    )
  }
  setBuildnumber(): any {
    this.buildnumber().subscribe((_brand: BrandingModel) => {
      if (_brand != null) {
        this.brand$.next(_brand); 
      }
      return _brand;
    });
  } 
}
