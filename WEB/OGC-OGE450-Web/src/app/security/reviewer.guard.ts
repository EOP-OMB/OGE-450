import { Injectable, Input } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { UserInfo } from '../user/user-info';
import { UserService } from '../user/user.service';
import { Roles } from '../security/roles';
import { ErrorCodes } from '../error/error-info';

@Injectable()
export class ReviewerGuard implements CanActivate {
    constructor(private userService: UserService, private router: Router) { }

    canActivate() {
        
        if (this.userService.isInRole(Roles.Reviewer)) {
            return true;
        } else {
            localStorage.setItem(ErrorCodes.Key, ErrorCodes.AccessRequired);
            this.router.navigate(['/error']);
            return false;
        }
    }
}

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