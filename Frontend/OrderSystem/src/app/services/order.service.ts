import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Environment } from '../environment/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  token:any=""
  constructor(private http:HttpClient) { 
    this.token=localStorage.getItem('data')
  }

  GetOrdersbyID():Observable<any>{
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
    });
    return this.http.get(Environment.url+"/Orders/GetOrdersbyID",{headers})
  }
  GetOrdersbyRole():Observable<any>{
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
    });
    return this.http.get(Environment.url+"/Orders/GetOrdersbyRole",{headers})
  }

  addOrder(event:FormData):Observable<any>{
    console.log(event.getAll)
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
    });
    return this.http.post(Environment.url+"/Orders/AddOrder",event,{headers})
  }
  deleteOrder(id:number):Observable<any>
  {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
    });
    return this.http.delete(Environment.url+`/Orders/DeleteOrder/id?id=${id}`,{headers})
  }
  approveOrder(id:number):Observable<any>
  {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
    });
    return this.http.post(Environment.url+`/Orders/ApproveOrder/id?id=${id}`,{},{headers})
  }

  downloadPDF(id:number):Observable<any>
  {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
    });
    return this.http.get(Environment.url+`/Orders/download/id?id=${id}`,{headers:headers, responseType:'blob'})
  }
  FilterOrder(body:any):Observable<any>
  {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`  // Include the token in the Authorization header
    });
    return this.http.post(Environment.url+"/Orders/FilterOrder",body,{headers})
  }
}
