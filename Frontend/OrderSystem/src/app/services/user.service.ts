import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Environment } from '../environment/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  token:any=""
  constructor(private http:HttpClient) { 
    this.token=localStorage.getItem('data')
  }

  editinfo(event:any):Observable<any>{
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
    });
    return this.http.post(Environment.url+"/Users/EditInfo",event,{headers})
  }
  changepassword(event:any):Observable<any>{
    delete event.confirmpassword
    console.log(event)
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
    });
    return this.http.post(Environment.url+"/Users/EditPassword",event,{headers})
  }


}
