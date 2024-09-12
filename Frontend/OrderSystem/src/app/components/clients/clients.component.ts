import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ClientService } from '../../services/client.service';
import { ButtonModule } from 'primeng/button';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { FloatLabelModule } from 'primeng/floatlabel';
import {TableModule} from 'primeng/table'
import {ToastModule} from 'primeng/toast'
import { MessageService } from 'primeng/api';
import { jwtDecode } from 'jwt-decode';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-clients',
  standalone: true,
  imports: [CommonModule,ButtonModule,FormsModule,ReactiveFormsModule,DialogModule,InputTextModule,FloatLabelModule,TableModule,ToastModule,TooltipModule],
  templateUrl: './clients.component.html',
  styleUrl: './clients.component.scss',
  providers:[MessageService]
})
export class ClientsComponent {

  rolepicker: { [key: string]: number }= {
    'University-User' : 1,
    'Institute-User'  : 2,
    'University-Admin': 3,
    'Institute-Admin' : 4,
    'Super-Admin'     : 5
    };

  role!:any
  token!:any
  info!:any
  clientinfo!:any
  clients!:any[]
  currentclient:string=""
  clientform!:FormGroup
  modalmessage:string='Add University'
  showmodal:boolean=false
  showinfo:boolean=false

  mode!:number
  editid:number=0
  constructor(private router:Router,private route:ActivatedRoute,private service:ClientService,private fb:FormBuilder,private messageService:MessageService){
    this.clientform= this.fb.group(
      {
        
        name:['',Validators.required],
        fixedPart:['',Validators.required],
        orgID:['',Validators.required]
      }
    )
  }
  ngOnInit(){
    if(!localStorage.getItem('data'))
    {
    this.router.navigate(["/login"])
    }
    this.GetClients()
    
    this.token=localStorage.getItem('data')
    this.info=jwtDecode(this.token)
    this.role=this.rolepicker[this.info["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]]
    console.log(this.role)

  }

  GetUniClients(){
    this.service.GetUniClients().subscribe(
      response =>{
        if(response.success==true)
        {
          this.clients=response.data;
        }
        else{
          this.warn(response.message)
        }

      },error=>{

      }

    )
      
  }
  GetClients()
  {
    this.route.queryParams.subscribe(params => {
      const action = params['action'];
      if (action === 'unis') {
        // Invoke a certain thing for Button 1
        this.clients=[]
        this.GetUniClients()
        this.modalmessage='Add University'
        this.clientform.patchValue({orgID:1})
        console.log("i'm here")
      } else if (action === 'insts') {
        // Invoke a certain thing for Button 2
        this.clients=[]
        this.GetInstsClients()
        this.modalmessage='Add Institute'
        this.clientform.patchValue({orgID:2})

        console.log("thereeee")
      }
    });
  }

  GetInstsClients(){
    this.service.GetInstClients().subscribe(
      response =>{
        if(response.success==true)
        {
          this.clients=response.data;
        }
        else{
          this.warn(response.message)
        }

      },error=>{

      }
    )
  }
  GetClientInfo(client:any)
  {
    this.service.GetClientInfo(client.id).subscribe(
      response=>{
        this.showinfo=true
        this.currentclient = client.name
        console.log(response)
        this.clientinfo = response.data
        console.log(this.clientinfo)
      },
      error=>{

      }
    )
  }

  handleClient(formgroup:FormGroup)
  {
    if(this.mode==1)
    {
      this.addClient(formgroup)
    }
    else{
      this.editClient(this.editid,formgroup)
    }
    this.showmodal=false
  }

  addClient(formgroup:FormGroup){
    console.log(formgroup.value)
    this.service.addClient(formgroup.value).subscribe(
      response=>{
        if(response.success==true)
        {
          this.success(response.message)
          this.GetClients()
        }
        else{
          this.error(response.message)
          this.GetClients()

        }

      },
      error=>{
        this.error(error)

      }
    )

  }
  editClient(id:number,formgroup:FormGroup)
  {
    this.service.editClient(id,formgroup.value).subscribe(
      response=>{
        if(response.success==true)
        {
          this.success(response.message)
          this.GetClients()

        }
        else{
          this.error(response.message)
          this.GetClients()

        }

      },
      error=>{
        this.error(error)

      }
    )
  }
  deleteClient(client:any)
  {
    console.log(client)
    this.service.deleteClient(client.id).subscribe(
      response=>{
        if(response.success===true)
        {
          this.GetClients()
          this.success(response.message)
        }

      },
      error=>{
        console.log(error)

      }
    )
  }
  handleEdit(client:any)
  {
    this.mode=2
    this.clientform.patchValue({name:client.name})
    this.clientform.patchValue({fixedPart:client.fixedPart})
    this.editid=client.id
    this.modalmessage='Edit Client'
    this.showmodal=true
  }

  togglemodal()
  {
    this.showmodal=true
    this.mode=1
  }
  success(message:string){
    this.messageService.add({severity:'success', summary:message});
  }
  error(message:string){
    this.messageService.add({severity:'error', summary:message});
  }
  warn(message:string){
    this.messageService.add({severity:'warn', summary:message});
  }
  onClose()
  {
    this.clientform.reset()
    this.modalmessage="Add Client"
  }
}
