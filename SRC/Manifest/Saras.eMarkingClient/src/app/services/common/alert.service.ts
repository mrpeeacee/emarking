import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { Alert, AlertType } from 'src/app/model/alert';

@Injectable({ providedIn: 'root' })
export class AlertService {
  private subject = new Subject<Alert>();
  private defaultId = 'default-alert'; 

  getAlert(): Observable<any> {
    return this.subject.asObservable();
  }

  onAlert(id = this.defaultId): Observable<Alert> {
    return this.subject.asObservable().pipe(filter(x => x && x.id === id));
  }

  success(message: string, autoClose = true, keepAfterRouteChange = true) { 
    this.alert(new Alert({ type: AlertType.Success, message, autoClose: autoClose, id: this.defaultId, keepAfterRouteChange: keepAfterRouteChange }));
  }


  error(message: string, autoClose = true, keepAfterRouteChange = false) { 
    this.alert(new Alert({ type: AlertType.Error, message, autoClose: autoClose, id: this.defaultId, keepAfterRouteChange: keepAfterRouteChange }));
  }

  warning(message: string, autoClose = true, keepAfterRouteChange = false) {  
    this.alert(new Alert({ type: AlertType.Warning, message, autoClose: autoClose, id: this.defaultId, keepAfterRouteChange: keepAfterRouteChange }));
  }

  info(message: string, autoClose = true, keepAfterRouteChange = false) { 
    this.alert(new Alert({ type: AlertType.Info, message, autoClose: autoClose, id: this.defaultId, keepAfterRouteChange: keepAfterRouteChange }));
  }

  alert(alert: Alert) {
    alert.id = alert.id || this.defaultId;
    this.subject.next(alert);
  }

  clear(id = this.defaultId) {
    this.subject.next(new Alert({ id }));
  }
}
