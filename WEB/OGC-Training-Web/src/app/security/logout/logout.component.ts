import { Component, OnInit } from '@angular/core';
import { AdalService } from 'ng2-adal/core';

@Component({
    selector: 'logout',
    templateUrl: 'logout.component.html'
})
export class LogoutComponent implements OnInit {

    constructor(private adalService: AdalService) {

    }

    public ngOnInit() {
        if (this.adalService && this.adalService.userInfo && this.adalService.userInfo.isAuthenticated)
            this.adalService.logOut();
    }
}
