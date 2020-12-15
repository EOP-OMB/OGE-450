import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../security/user.service';

@Component({
    selector: 'welcome',
    templateUrl: './welcome.component.html',
    styleUrls: ['./welcome.component.css']
})

export class WelcomeComponent implements OnInit {
    get user() {
        return this.userService.user;
    }

    ngOnInit(): void {

    }

    constructor(
        private userService: UserService,
        private router: Router) { }

}