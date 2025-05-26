import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedService {

  private objectListSource = new BehaviorSubject<any[]>([]);
  objectList$ = this.objectListSource.asObservable();

  setObjectList(objectList: any[]) {
    this.objectListSource.next(objectList);
  }
}



