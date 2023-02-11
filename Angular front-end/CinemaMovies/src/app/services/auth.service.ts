import { CryptService } from './crypt.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RegisterServiceService } from './register-service.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {


  constructor(private _httpClient:HttpClient,private crypt: CryptService) { }

  baseUrl='https://localhost:7188/api/Account/';
  headers={
    headers:new HttpHeaders({
      'Content-Type':'application/json'
    })
  };
  public installStorage(rem :boolean,Email:string){
    const day =new Date();
    if(rem)
    {
        day.setDate(day.getDay()+10);
    }
    else
    {
        day.setMinutes(day.getMinutes()+30);
    }
    // save user data in local storage
    localStorage.setItem('email',this.crypt.encryptData(Email));
    localStorage.setItem('expire',this.crypt.encryptData(day.toString()));
    this.GetRoleName(Email).subscribe(succ=>{
      localStorage.setItem('role',this.crypt.encryptData(succ.toString()));
    },err=>console.log("error"));
  }

  public GetRoleName(email:string){
    return this._httpClient.get(this.baseUrl+'GetRoleName/'+email,{responseType:'text'}).pipe();
  }
}
