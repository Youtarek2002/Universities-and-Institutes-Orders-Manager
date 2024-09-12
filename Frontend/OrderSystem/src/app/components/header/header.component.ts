import { Component } from '@angular/core';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { Router, RouterLink } from '@angular/router';
import { MenuItem, MessageService } from 'primeng/api';
import { AuthenticationService } from '../../services/authentication.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { jwtDecode } from "jwt-decode";
import {DialogModule} from 'primeng/dialog'
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [MenuModule,ButtonModule,ToastModule,RouterLink,FormsModule,ReactiveFormsModule,DialogModule,FloatLabelModule,InputTextModule,CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
  ,providers:[MessageService]
})
export class HeaderComponent {
  rolepicker: { [key: string]: number }= {
    'University-User' : 1,
    'Institute-User'  : 2,
    'University-Admin': 3,
    'Institute-Admin' : 4,
    'Super-Admin'     : 5
};


  gfg!: MenuItem[];
  id:string='' 
  editform!:FormGroup
  passwordform!:FormGroup
  info!:any
  token:any=""
  role!:any
  showpasswordbutton:boolean=false
  showmodal:boolean=false
  showpasswordmodal:boolean=false
  constructor(private router:Router,private auth:AuthenticationService,private fb:FormBuilder ,private service:UserService,private messageService:MessageService){

    this.editform = fb.group({
        userName:['',Validators.required],
        firstName:['',Validators.required],
        lastName:['',Validators.required],
        password:['',Validators.required]

    })
    this.passwordform = fb.group({
      password:['',Validators.required],
      newPassword:['',Validators.required],
      confirmpassword:['',Validators.required],
    },
    {validators:[this.passwordmatch,this.validpassword.bind(this)]}
  
  )
  }
  ngOnInit(){
    this.token=localStorage.getItem('data')
    this.info=jwtDecode(this.token)
    this.setData()
    this.role=this.rolepicker[this.info["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]]
    console.log(this.role)
    if(!localStorage.getItem('currentbutton'))
    {
      localStorage.setItem('currentbutton','myorders')
    var btn = document.getElementById("myorders")
    btn?.classList.add("selectedBtn")
    }
    else{
      const button = localStorage.getItem('currentbutton');
      if(button!=null)
      {
        this.id=button
      }
      var btn = document.getElementById(this.id)
    btn?.classList.add("selectedBtn")
    }
    this.gfg = [ 
      { 
        label: 'Settings', 
        items: [ 
          { 
            label: 'Edit Profile', command:() => {this.showmodal=true,this.setData()}
          }, 
          { 
            label: 'Change Password',command:() => {this.showpasswordmodal=true}
          } 
        ] 
      }, 
      { 
        label: 'Profile', 
  
        items: [ 
          { 
            label: "Logout", command:() => this.logout()
          } 
          
        ] 
      } 
    ]; 
  }

  setColor(event:any){
   
    document.getElementById("myorders")?.classList.remove("selectedBtn")
    document.getElementById("allorders")?.classList.remove("selectedBtn")
    document.getElementById("unis")?.classList.remove("selectedBtn")
    document.getElementById("insts")?.classList.remove("selectedBtn")
    var btn = document.getElementById(event.target.id)
    localStorage.setItem('currentbutton',event.target.id)
    btn?.classList.add("selectedBtn")
  }

  logout()
  {
    this.router.navigate(['/login'])
    this.auth.logout
  }
  togglepassword(){
    this.showpasswordbutton=true
  }
  onClose(){
    this.showmodal=false
    this.showpasswordbutton=false
    this.showpasswordmodal=false
  }
  setData(){
    this.editform.patchValue({userName:this.info.username,firstName:this.info.firstname,lastName:this.info.lastname})
  }
  editinfo(form:FormGroup)
  {
    this.service.editinfo(form.value).subscribe(
      response=>{
        if(response.success==true)
        {
          this.success(response.message)
          
        }
        else
        this.error(response.message)
    },
    error=>{
        this.error(error)
    }
    
    )
  }







  changepassword(form:FormGroup){

    this.service.changepassword(form.value).subscribe(
      response=>{
        if(response.success==true)
        {
          this.success(response.message)
          
        }
        else
        this.error(response.message)
    },
    error=>{
        this.error(error)
    }
    
    )
  }












  validpassword(signupform: FormGroup): {[key : string]: boolean} | null{
    const newPassword = signupform.get('newPassword')?.value;
    if( newPassword !== newPassword.toLowerCase() && newPassword === newPassword.toUpperCase())
    {    
      // this.errorr='Password must contain a lowercase letter'
      // console.log(this.errorr)
      return {notvalid : true}
    }
    if( newPassword === newPassword.toLowerCase() && newPassword !== newPassword.toUpperCase())
    {   
      // this.errorr='Password must contain an uppercase letter'
      // console.log(this.errorr)
      return {notvalid : true}
    }
    if(!(/\d/.test(newPassword)))
    {    
      // this.errorr='Password must contain a number'

      return {notvalid : true}
    }
    // this.errorr = ''
    return null;

  }


  passwordmatch(signupform: FormGroup): { [key: string]: boolean } | null{
    const newPassword = signupform.get('newPassword')?.value;
    const confirmpassword = signupform.get('confirmpassword')?.value;
    return newPassword === confirmpassword ? {mismatch:false} : {mismatch: true};
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
}
