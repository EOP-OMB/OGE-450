import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { UserService } from '../user/user.service';

@Injectable()
export class MaintenanceGuard implements CanActivate {
    constructor(private userService: UserService,
        private router: Router) { }

    canActivate() {

        if (this.userService.user && !this.userService.user.inMaintMode) {

            return true;

        } else {
            this.router.navigate(['/maintenance']);
            return false;
        }
    }
}
