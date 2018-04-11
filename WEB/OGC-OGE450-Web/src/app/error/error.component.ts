import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from "@angular/router";
import { ErrorInfo, ErrorInfoList, ErrorCodes } from './error-info';
import { Logging } from '../common/logging';

@Component({
    selector: 'error',
    templateUrl: './error.component.html'
})

export class ErrorComponent implements OnInit {
    private _errorInfo: ErrorInfo;

    constructor(
        private router: Router, route: ActivatedRoute
    ) {
        let status = ErrorCodes.UnexpectedError;

        Logging.log('in errorComponent constructor');
        Logging.log(route.snapshot.data);

        if (localStorage.getItem(ErrorCodes.Key))
            status = localStorage.getItem(ErrorCodes.Key);

        this._errorInfo = ErrorInfoList.find(x => x.status == status);
    }

    public ngOnInit() {
        
    }

    public retry() {
        this.router.navigate(['/']);
    }
}