import { Injectable, Input } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { AppUser } from '../security/app-user';
import { UserService } from '../security/user.service';
import { Roles } from '../security/roles';
import { ErrorCodes } from '../error/error-info';


@Injectable()
export class AdminGuard implements CanActivate {
    constructor(private userService: UserService, private router: Router) { }

    canActivate() {

        if (this.userService.isInRole(Roles.Admin)) {
            return true;
        } else {
            localStorage.setItem(ErrorCodes.Key, ErrorCodes.AccessRequired);
            this.router.navigate(['/error']);
            return false;
        }
    }
}