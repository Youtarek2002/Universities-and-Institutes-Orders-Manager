import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { SignupComponent } from './components/signup/signup.component';
import { NotfoundComponent } from './components/notfound/notfound.component';
import { OrdersComponent } from './components/orders/orders.component';
import { AllordersComponent } from './components/allorders/allorders.component';
import { ClientsComponent } from './components/clients/clients.component';
import { InstitutesComponent } from './components/institutes/institutes.component';

export const routes: Routes = [
    {
        path:'',
        redirectTo:'login',
        pathMatch:'full'
    },
    {
        path:'login',
        component:LoginComponent,
        title:'login'
    },
    {
        path:'signup',
        component:SignupComponent,
        title:'signup'
    },
    {
        path:'orders',
        component:OrdersComponent,
        title:'orders'   
    },
     {
        path:'clients',
        component:ClientsComponent,
        title:'clients'   
    },
    {
        path:'**',
        component:NotfoundComponent,
        title:'notfound'
    }

];
