import { RegisterModel } from './../Models/register-model';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient,HttpHeaders } from '@angular/common/http';
@Injectable({
  providedIn: 'root'
})
export class RegisterServiceService {

  constructor(private _httpClient:HttpClient) { }

  baseUrl='https://localhost:7188/api/Account/';
  headers={
    headers:new HttpHeaders({
      'Content-Type':'application/json'
    })
  };

   Register(reg:RegisterModel):Observable<any>{
     return this._httpClient.post(this.baseUrl+'Register',reg,this.headers).pipe();
   }
}
