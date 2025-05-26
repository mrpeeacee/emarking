import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Globalconst } from 'src/app/model/globalconst';

@Injectable({
  providedIn: 'root'
})
export class CommonService {
  IsKp: boolean = false;
  //#region pageheader
  public pageheader = new BehaviorSubject<string>("Dashboard");
  get GetPageHeader() {
    return this.pageheader.asObservable();
  }
  setHeaderName(data: string) {
    this.pageheader.next(data)
  }


  private infoText = new BehaviorSubject<string>("Dashboard");
  get GetPageDescription() {
    return this.infoText.asObservable();
  }
  setPageDescription(data: string) {
    this.infoText.next(data)
  }

  getProjectId() {
    return Globalconst.projectId;
  }

  //#endregion pageheader

  //#region HeaderMenu
  // public headerMenu = new BehaviorSubject<any[]>();
  // get GetPageHeader() {
  //   return this.pageheader.asObservable();
  // }
  // setHeaderName(data: string) {
  //   this.pageheader.next(data)
  // }
  //#endregion HeaderMenu



}
