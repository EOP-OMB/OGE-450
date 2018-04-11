import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { AdalService } from 'ng2-adal/core';
import { UserService } from '../user/user.service';

@Injectable()
export class LoggedInGuard implements CanActivate {
    constructor(private adalService: AdalService,
        private userService: UserService,
        private router: Router) { }

    canActivate() {
        
        if (this.adalService.userInfo.isAuthenticated && this.userService.user) {
            
            return true;
            
        } else {
            this.router.navigate(['/login']);
            return false;
        }
    }
}
