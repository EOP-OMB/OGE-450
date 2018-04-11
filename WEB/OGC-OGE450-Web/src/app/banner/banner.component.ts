import { environment } from '../../environments/environment';

import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../user/user.service';
import { FormStatus } from '../common/constants';

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


    showExtensionTab(): boolean {
        return this.userService.user ? this.userService.user.currentFormId > 0 && (this.userService.user.currentFormStatus == FormStatus.NOT_STARTED || this.userService.user.currentFormStatus == FormStatus.DRAFT || this.userService.user.currentFormStatus == FormStatus.MISSING_INFORMATION) : false;
    }

    launchPortal(): void {
        var url = environment.portalUrl;

        window.open(url, '_self');
    }
}