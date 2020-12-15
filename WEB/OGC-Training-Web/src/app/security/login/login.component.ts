import { Component, OnInit } from '@angular/core';
import { Router } from "@angular/router";
import { AdalService } from 'ng2-adal/core';
//import { UserService } from '../services/user.service';

@Component({
    selector: 'login',
    templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {

    constructor(
        private router: Router,
        private adalService: AdalService,
        //private userService: UserService,
    ) {
        if (this.adalService.userInfo.isAuthenticated) {
            this.router.navigate(['/home']);
        }

        this.logIn(); 
    }

    public ngOnInit() {
       
    }

    public logIn() {
        this.adalService.login();
    }
}
