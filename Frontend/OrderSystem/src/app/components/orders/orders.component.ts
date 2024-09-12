import { Component, importProvidersFrom } from '@angular/core';
import { OrderService } from '../../services/order.service';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { FloatLabelModule } from 'primeng/floatlabel';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { ClientService } from '../../services/client.service';
import { DialogModule } from 'primeng/dialog';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import {ToastModule} from 'primeng/toast'
import {MessageService} from 'primeng/api';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule,RouterLink,FormsModule, ReactiveFormsModule,InputTextModule,FloatLabelModule,CalendarModule,DropdownModule,DialogModule,TableModule,TooltipModule,ToastModule],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss',
  providers:[MessageService]
})
export class OrdersComponent {
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

  orderform!: FormGroup
  orders!: any[]
  mainorders!: any[]
  showmodal:boolean=false
  modalmessage:string='Add Order'
  clientslist!:any[]
  mode:number=1
  showxml:boolean=false
  clientsnameslist!:string[]
  clientstype:any[]=[{name:'University',value:1},{name:'Institute',value:2}]
  statustype:any[]=[{name:'Pending Approval',value:'Pending Approval'},{name:'Approved',value:'Approved'}]
  clientfilter:any=null
  clientfiltertype!:any
  statusfilter:any=null 
  showbutton:boolean=true
  fileupload!:File
  xmlupload!:File
  constructor(private service : OrderService,private router : Router,private route:ActivatedRoute,private fb:FormBuilder,private clientservice:ClientService,private messageService:MessageService){    
  }
 
  
  ngOnInit(){
    if(!localStorage.getItem('data'))
    {
      
    this.router.navigate(["/login"])
    }
    // this.clientslist=this.clients.GetUniClients().map((client) => client.clientName);
    this.token=localStorage.getItem('data')
    this.info=jwtDecode(this.token)
    this.role=this.rolepicker[this.info["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]]
    console.log(this.role)
    if(this.role%2==1)
    {
        this.GetUniClients()
    }
    else if(this.role%2==0||this.role==5){
      this.GetInstsClients()
    }
    this.getOrdersbyID()
    // this.getOrders();
    this.getOrderByParams()
    this.orderform =  this.fb.group({
      orderName:['',Validators.required],
      orderDate:['',Validators.required],
      numberOfCopies:['',Validators.required],
      clientID:['',Validators.required],
      OrgID:['',Validators.required]
    },
    
)
  }
  handleModeChange(event:any)
  {
    if(event.value==1)
    {
      this.GetUniClients()
      this.showxml=true
    }
    else{
      this.GetInstsClients()
      this.showxml=false
    }
  }
  handleFilterChange()
  {
    this.orders=this.mainorders
    
    if(this.clientfilter != null || this.statusfilter != null)
    {
      this.service.FilterOrder({orderList:this.orders,clientName:this.clientfilter,statusName:this.statusfilter}).subscribe(
        response=>{
          if(response.success)
        {
          console.log(response)
          this.orders=response.data
        }

        else{
          this.orders=[]
          this.warn(response.message)
        }
        },
        error=>{
          console.log(this.orders)
          console.log(error)
        }
      )
    }
    else if(this.clientfilter == null && this.statusfilter == null)
    {
      this.getOrderByParams()
    }
  }
  getOrderByParams()
  {
    this.route.queryParams.subscribe(params => {
      const action = params['action'];
      if (action === 'myorders') {
        this.mode=1
        // Invoke a certain thing for Button 1
        this.showbutton=true
        this.getOrdersbyID()
      } else if (action === 'allorders') {
        this.mode=2
        // Invoke a certain thing for Button 2
        // setInterval(()=>{this.GetOrdersbyRole()},5)
        setTimeout(() => {
          this.GetOrdersbyRole()
          
        }, 5);
        this.showbutton=false
      }
    });
  }
  onPDFChange(event: any) {
    if (event.target.files.length > 0) {
      this.fileupload = event.target.files[0];
    }
  }
  onXMLChange(event: any) {
    if (event.target.files.length > 0) {
      this.xmlupload = event.target.files[0];
    }
  }


  getOrdersbyID(){
    this.service.GetOrdersbyID().subscribe(
      response=>{
        if(response.success)
        {
          this.orders=response.data
          this.mainorders=this.orders
        }
        else{
          this.orders=[]
          this.warn(response.message)
        }

      },
      error =>{

      }
    )
    
  }

  GetOrdersbyRole(){
    this.service.GetOrdersbyRole().subscribe(
      response=>{
        if(response.success)
        {
          console.log(response.data)
          console.log(response['data'])
          
          this.orders=response.data
          this.mainorders=this.orders
          console.log(this.orders)

        }
        else{
          this.warn(response.message)
        }

      },
      error =>{

      }
    )
    
  }


  GetUniClients(){
    this.clientservice.GetUniClients().subscribe(
      response =>{
        if(response.success==true)
        {
          // this.clients=response.data;
          this.clientslist=response.data
          this.clientsnameslist=response.data.map((client:any) => ({name:client.name,id:client.id}))
          
        }
        else{
          if(response.data==null)
          {
          this.clientslist=[]
          this.clientsnameslist=[]
          }
          this.warn(response.message)
        }

      },error=>{

      }

    )
      
  }
  GetInstsClients(){
    this.clientservice.GetInstClients().subscribe(
      response =>{
        if(response.success==true)
        {
          this.clientslist=response.data.map((client:any) => client.clientName)
          this.clientsnameslist=response.data.map((client:any) => ({name:client.name,id:client.id}))

        }
        else{
          if(response.data==null)
          {
          this.clientslist=[]
          this.clientsnameslist=[]
          }
          
          this.warn(response.message)
        }

      },error=>{

      }
    )
  }
  togglemodal()
  {
    this.showmodal=true
  }
  AddOrder(formgroup:FormGroup){
      //add
      console.log(formgroup.get('orderDate')?.value)

      const datePipe = new DatePipe('en-US');
      formgroup.patchValue({orderDate:datePipe.transform(formgroup.get('orderDate')?.value, 'yyyy-MM-dd')})
      console.log(formgroup.get('orderDate')?.value)

      console.log(formgroup.get('orderDate')?.value)
      const formData = new FormData();
      formData.append('orderName', formgroup.get('orderName')?.value);
      formData.append('orderDate',formgroup.get('orderDate')?.value);
      formData.append('numberOfCopies', formgroup.get('numberOfCopies')?.value);
      formData.append('clientID', formgroup.get('clientID')?.value);
      formData.append('OrgID', formgroup.get('OrgID')?.value);
      formData.append('pdf', this.fileupload);
      formData.append('xml', this.xmlupload);
      this.service.addOrder(formData).subscribe(
        response=>{
          if(response.success===true)
          {
              this.getOrdersbyID()
         
            setTimeout(() => {
              this.handleFilterChange()
              
            }, 80);
            this.success(response.message)

          }
          

        },
        error=>{
            console.log(error)
        }
      )
      this.showmodal=false

    
  }
  deleteOrder(event:any){
    console.log(event)
    this.service.deleteOrder(event.orderID).subscribe(
      response=>{
        if(response.success===true)
        {
          this.getOrdersbyID()
          this.success(response.message)
        }

      },
      error=>{
        console.log(error)

      }
    )


  }

  approveOrder(event:any)
  { console.log(event)
    this.service.approveOrder(event.orderID).subscribe(
      response=>{
        if(response.success===true)
        {
          this.getOrdersbyID()
          // alert(response.message)
          this.success(response.message)
        }
        else{
          this.error(response.message)
        }

      },
      error=>{
        console.log(error)

      }
    )
  }

  downloadPDF(order:any)
  {
    this.service.downloadPDF(order.orderID).subscribe((blob: Blob) => {
      const downloadURL = URL.createObjectURL(blob);
      const link = document.createElement('a');
      console.log(blob)
      link.href = downloadURL;
      window.open(downloadURL)
      link.download = 'ModifiedDocument.pdf';
      link.click();
    });

   
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
  
//   addMultiple() {
//     this.messageService.addAll([{severity:'success', summary:'Service Message', detail:'Via MessageService'},
//                                 {severity:'error', summary:'error Message', detail:'Via MessageService'}]);
// }

// clear() {
//     this.messageService.clear();
// }
  // handleEdit(order:any)
  // {
  //   this.mode=2
  // this.modalmessage='Edit Order'
  //  this.orderform.setValue({orderName:order.orderName,orderAmount:order.orderAmount,numberOfCopies:order.numberOfCopies,orderDate:order.orderDate,clientID:1,OrgID:2})
  //  this.showmodal=true
  // }
}


