import { RegisterServiceService } from './../../services/register-service.service';
import { RegisterModel } from './../../Models/register-model';
import { Component, OnInit } from '@angular/core';
import {FormControl,FormGroup,FormBuilder,Validators} from '@angular/forms';
import { Users } from 'src/app/Models/user';
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
        Password:'',
        PasswordConfirm:''
      }
      this.allUsers();
      this.message='';

    }

    reg:RegisterModel=new RegisterModel();
    users:Users[]=[];
    message:string='';
    registerForm:FormGroup=new FormGroup({
      'Email':new FormControl(null,[Validators.required,Validators.pattern('([a-z]||[0-9])+@gmail.com')]),
      'UserName':new FormControl(null,[Validators.required,Validators.minLength(6)]),
      'Password':new FormControl(null,[Validators.required,Validators.minLength(6)]),
      'PasswordConfirm':new FormControl(null,[Validators.required])
    })

    register(){
      // console.log(this.registerForm)
      if(this.registerForm.valid)
      {
        this.FullRegisterModel();
        this.service.Register(this.reg).subscribe(success=>{
          this.message="Completed Registration";
          this.registerForm.reset(); // to empty the form input
        },err=>console.log(err));
      }
    }
  FullRegisterModel() {
    this.reg.UserName=this.registerForm.value.UserName;
    this.reg.Email=this.registerForm.value.Email;
    this.reg.Password=this.registerForm.value.Password;
    this.reg.PasswordConfirm=this.registerForm.value.PasswordConfirm;
  }

   isPasswordMatch(){
     if(this.registerForm.value.Password !==this.registerForm.value.PasswordConfirm)
          return true;
     else
         return false;
   }
   allUsers(){
    this.service.GetAllUsers().subscribe(list=>{
      this.users=list;
      // console.log(this.users);
    },err=>console.log(err));
   }










}
