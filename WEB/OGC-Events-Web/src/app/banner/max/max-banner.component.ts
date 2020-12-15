import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../../security/user.service';

@Component({
    selector: 'max-banner',
    templateUrl: './max-banner.component.html',
    styleUrls: ['./max-banner.component.css']
})

export class MaxBannerComponent implements OnInit {
    get user() {
        return this.userService.user;
    }

    ngOnInit(): void {

    }

    constructor(
        private userService: UserService,
        private router: Router) { }

}