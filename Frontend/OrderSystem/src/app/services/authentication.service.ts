import { HttpClient, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Environment } from '../environment/environment';
@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(private http : HttpClient) {}



  login(event:any)
  {
    
    return this.http.post<any>(Environment.url+"/Users/Login",event);

  }
  logout(){
    localStorage.clear()
  }
  signup(event:any)
  {
    delete event.confirmpassword
    return this.http.post<any>(Environment.url+"/Users/Register",event);
  }
  checkLogin():boolean
  {
    return !!localStorage.getItem('data')
    
  }
}


