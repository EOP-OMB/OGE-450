import { Component, OnInit, Input } from '@angular/core';

import { Router } from '@angular/router';

import { OGEForm450 } from '../form/oge-form-450';

import { UserService } from '../user/user.service';
import { OGEForm450Service } from '../form/oge-form-450.service';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})

export class HomeComponent implements OnInit {
    get user() {
        return this.userService.user;
    }

    ngOnInit(): void {
        this.getForms();
    }

    forms: OGEForm450[];

    constructor(
        private userService: UserService,
        private router: Router,
        private formService: OGEForm450Service) { }
    
    getForms(): void {
        this.formService
            .getMyForms()
            .then(forms => {
                this.forms = forms;
            });
    }

    gotoDetail(id: number): void {
        this.router.navigate(['/form', id]);
    }
}