import { environment } from '../../environments/environment';

import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../security/user.service';

@Component({
    selector: 'banner',
    templateUrl: './banner.component.html',
    styleUrls: ['./banner.component.css']
})

export class BannerComponent implements OnInit {
    get user() {
        return this.userService.user;
    }

    ngOnInit(): void {

    }

    constructor(
        private userService: UserService,
        private router: Router) { }


    showTab(tabName: string): boolean {
        return true;
    }

    launchPortal(): void {
        var url = environment.portalUrl;

        window.open(url, '_self');
    }
}
