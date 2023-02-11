import { RegisterServiceService } from './../services/register-service.service';
import { Component } from '@angular/core';
import { FormControl,FormGroup, Validators } from '@angular/forms';
import { Route, Router } from '@angular/router';
@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  title = 'Cinema Movies';
  constructor(private service :RegisterServiceService,private route:Router){}

  Logout(){
      this.service.Logout().subscribe(success=>{
        localStorage.removeItem('email');
        localStorage.removeItem('expire');
        localStorage.removeItem('role');
        console.log("Logout !!")
        this.route.navigate(['home']); // after login go to home page

      },err=>{
        console.log(err.error);
        localStorage.removeItem('email');
        localStorage.removeItem('expire');
        localStorage.removeItem('role');
        console.log("errorrrrrrr");
      });
  }
  isUserRegistered()
  {
    const email=!!localStorage.getItem('email');
    const exp=!!localStorage.getItem('expire');
    const role=!!localStorage.getItem('role');
    if(email&&exp&&role){
      return true;
    }
    return false;
  }



}
