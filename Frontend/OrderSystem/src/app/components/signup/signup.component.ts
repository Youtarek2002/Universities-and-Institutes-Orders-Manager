import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { DividerModule } from 'primeng/divider';

import { FloatLabelModule } from 'primeng/floatlabel';
import { AuthenticationService } from '../../services/authentication.service';
import { CommonModule } from '@angular/common';
type ErrorMessageFunction = (params: any) => string;

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [DividerModule,PasswordModule,CommonModule,RouterLink,RouterOutlet,FormsModule,InputTextModule,FloatLabelModule,ReactiveFormsModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss'
})

export class SignupComponent {
  errorMessage: { [key: string]: ErrorMessageFunction }= {
    'required'  : (params:any)  => `This field is required`,
    'maxlength' : (params:any)  => `Maximum ${params.requiredLength} characters are allowed`,
    'minlength' : (params:any)  => `Minimum ${params.requiredLength} characters are required`,
    'email'     : (params:any)  => 'This field must be an email',
};
  signupform!: FormGroup;
  errorr!: string;
  constructor(private auth : AuthenticationService, private fb: FormBuilder, private router: Router){}
  ngOnInit(): void {
    this.errorr = "hi"
    console.log(this.errorr);
    
    this.signupform = this.fb.group({
      email: ['', [Validators.required,Validators.email,Validators.minLength(15),Validators.maxLength(30)]],
      username: ['',[Validators.required,Validators.minLength(6),Validators.maxLength(15)]],
      firstname: ['',[Validators.required,Validators.minLength(3),Validators.maxLength(12)]],
      lastname: ['',[Validators.required,Validators.minLength(3),Validators.maxLength(12)]],
      password: ['', [Validators.required,Validators.minLength(8),Validators.maxLength(20)]],
      confirmpassword:['',[Validators.required]]
    },
    {validators: [this.passwordmatch,this.validpassword.bind(this)]}
    )
  }
  
  signup()
  {
    this.auth.signup(this.signupform.value).subscribe(
        response => {
          if(response.success)
          {
            this.router.navigate(["/login"]);
          }
          else
          {
            alert(response.message);
          }


        },
        error=>{
          console.log(error);
        }

    )
  }
  
  passwordmatch(signupform: FormGroup): { [key: string]: boolean } | null{
      const password = signupform.get('password')?.value;
      const confirmpassword = signupform.get('confirmpassword')?.value;
      return password === confirmpassword ? {mismatch:false} : {mismatch: true};
  }
  returnerror():String|null{
    return this.errorr
  }
  validpassword(signupform: FormGroup): {[key : string]: boolean} | null{
    const password = signupform.get('password')?.value;
    if( password !== password.toLowerCase() && password === password.toUpperCase())
    {    
      this.errorr='Password must contain a lowercase letter'
      console.log(this.errorr)
      return {notvalid : true}
    }
    if( password === password.toLowerCase() && password !== password.toUpperCase())
    {   
      this.errorr='Password must contain an uppercase letter'
      console.log(this.errorr)
      return {notvalid : true}
    }
    if(!(/\d/.test(password)))
    {    
      this.errorr='Password must contain a number'

      return {notvalid : true}
    }
    this.errorr = ''
    return null;

  }

  listerrors(event:any)
  {
    console.log(event)
  }
  getErrorMessage(control: any): String | null {

    if (control.errors) {
      const errorKey = Object.keys(control.errors)[0]; // Get the first error key
      const errorParams = control.errors[errorKey]; // Get the error parameters

      // Check if the error message function exists and return the message
      if (this.errorMessage[errorKey]) {
        return this.errorMessage[errorKey](errorParams);
      }
    }
    return null;
}
}
