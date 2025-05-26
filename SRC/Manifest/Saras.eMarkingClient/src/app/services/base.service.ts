import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BaseService {

  endpoint: string = environment.apiUrl;
  apiVersion: string = environment.apiVersion;
  headers = new HttpHeaders().set('Content-Type', 'application/json');
 
}
