import { AuthService } from './../../services/auth.service';
import { RegisterServiceService } from './../../services/register-service.service';
import { LoginModel } from './../../Models/Login-model';
import { FormControl,FormGroup, Validators } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { Route, Router } from '@angular/router';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit{

  constructor(private service :RegisterServiceService,private route:Router,private auth:AuthService ){}
  ngOnInit(): void {
    //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
    //Add 'implements OnInit' to the class.
    this.message='';
    this.log={
      Email:'',
      Password:'',
      RememberMe:false
    }


  }

  message:string='';
  log:LoginModel=new LoginModel();
  LoginForm:FormGroup=new FormGroup({
      'Email':new FormControl('',[Validators.required]),
      'Password':new FormControl('',[Validators.required]),
      'RememberMe':new FormControl(false)
  });


  Login(){
    if(this.LoginForm.valid)
    {
      this.FullLoginModel();
      this.service.Login(this.log).subscribe(success=>{
        const rem=!!this.LoginForm.value.RememberMe;
        const email=this.LoginForm.value.Email;
        this.auth.installStorage(rem,email);

        this.route.navigate(['home']); // after login go to home page
        this.message="Success Login";
        console.log("Login !!")
        this.LoginForm.reset(); // to empty the form input
      },err=>{
        console.log(err.error);
        this.message=err.error;
      });
    }
  }

  FullLoginModel() {
    this.log.Email=this.LoginForm.value.Email;
    this.log.Password=this.LoginForm.value.Password;
    this.log.RememberMe=this.LoginForm.value.RememberMe;
  }
}
