import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ErrorCodes } from './error-info';

@Component({
    template: ''
})
export class PageNotFoundComponent {
    constructor(private router: Router) {
        localStorage.setItem(ErrorCodes.Key, ErrorCodes.NoRouteDefined);
        router.navigate(['/error']);
    };
}