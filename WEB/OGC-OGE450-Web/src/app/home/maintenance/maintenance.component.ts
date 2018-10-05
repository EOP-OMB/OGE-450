import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';

import { UserService } from '../../user/user.service';

@Component({
    selector: 'maintenance',
    templateUrl: './maintenance.component.html',
    styleUrls: ['./maintenance.component.css']
})

export class MaintenanceComponent implements OnInit {
    get user() {
        return this.userService.user;
    }

    ngOnInit(): void {
        this.verifyMaintMode();
    }

    constructor(
        private userService: UserService,
        private router: Router) { }

    verifyMaintMode(): void { 
        this.userService.load().then(response => {
            console.log(response);
            if (!this.user.inMaintMode)
                this.router.navigate(['/home']);
        });
    }

    public retry() {
        this.verifyMaintMode();
    }
}