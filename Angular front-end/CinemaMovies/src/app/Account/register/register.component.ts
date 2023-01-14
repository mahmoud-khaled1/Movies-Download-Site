import { RegisterServiceService } from './../../services/register-service.service';
import { RegisterModel } from './../../Models/register-model';
import { Component, OnInit } from '@angular/core';
import {FormControl,FormGroup,FormBuilder,Validators} from '@angular/forms';
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{
    constructor(private service:RegisterServiceService){}
    ngOnInit(): void {
      //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
      //Add 'implements OnInit' to the class.
      this.reg={
        UserName:'',
        Email:'',
        Password:''
      }
    }

    reg:RegisterModel=new RegisterModel();

    registerForm:FormGroup=new FormGroup({
      'Email':new FormControl(null,[Validators.required,Validators.pattern('([a-z]||[0-9])+@gmail.com')]),
      'UserName':new FormControl(null,[Validators.required,Validators.minLength(6)]),
      'Password':new FormControl(null,[Validators.required,Validators.minLength(6)])
    })

    register(){
      // console.log(this.registerForm)
      if(this.registerForm.valid)
      {
        this.validateRegisterModel();
        this.service.Register(this.reg).subscribe(success=>{
          alert("Regstration Success");
        },err=>console.log(err));
      }
    }
  validateRegisterModel() {
    this.reg.UserName=this.registerForm.value.UserName;
    this.reg.Email=this.registerForm.value.Email;
    this.reg.Password=this.registerForm.value.Password;
  }




}
