import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { FloatLabelModule } from 'primeng/floatlabel';
import { AuthenticationService } from '../../services/authentication.service';
import { ChangeDetectorRef } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { MenuModule } from 'primeng/menu';
import {ButtonModule} from 'primeng/button'
import {ToastModule} from 'primeng/toast'

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterOutlet, RouterLink,RouterModule, FormsModule, ReactiveFormsModule,InputTextModule,FloatLabelModule,MenuModule,ButtonModule,ToastModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  gfg!: MenuItem[]; 

  loginForm!: FormGroup
  constructor( private auth : AuthenticationService,private fb: FormBuilder, private router: Router){}
  ngOnInit(): void {
    setTimeout(() => 
    { 
      this.auth.logout()  
    },
    1);
    this.loginForm = this.fb.group({
      email: ['', [Validators.required,Validators.email]],
      password: ['', Validators.required]
    })

    this.gfg = [
      { label: 'Item 1', icon: 'pi pi-fw pi-plus' },
      { label: 'Item 2', icon: 'pi pi-fw pi-trash' },
    ];
  }
  login()
  {
    this.auth.login(this.loginForm.value).subscribe(
      response=>{
        if(response.success)
        {
           localStorage.setItem("data", response.data);
           this.router.navigate(["/orders"]);
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

  

}
