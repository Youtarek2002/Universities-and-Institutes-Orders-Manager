import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Environment } from '../environment/environment';
import { FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class ClientService {
  token:any=""

  constructor(private http : HttpClient) {
    this.token=localStorage.getItem('data')
   }

  GetUniClients():Observable<any>{
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
    });
   return this.http.get(Environment.url+'/Clients/GetUniClients',{headers})  
  }
  GetInstClients():Observable<any>{
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
    });
    return this.http.get(Environment.url+'/Clients/GetInstClients',{headers})  
   }

   addClient(event:any):Observable<any>
   {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
    });
    return this.http.post(Environment.url+'/Clients/AddClient',event,{headers})  
   }

   editClient(id:number,event:any):Observable<any>
   {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
    });
    return this.http.post(Environment.url+`/Clients/EditClient/id?id=${id}`,event,{headers})  
   }
   deleteClient(id:number):Observable<any>
   {
     const headers = new HttpHeaders({
       'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
     });
     return this.http.delete(Environment.url+`/Clients/DeleteClient/id?id=${id}`,{headers})
   }
   GetClientInfo(id:number):Observable<any>
   {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
    });
    return this.http.get(Environment.url+`/Clients/GetClientInfo/id?id=${id}`,{headers})

   }
}
