import {     Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Router } from '@angular/router';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/finally';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import { Http, XHRBackend, RequestOptions, Request, RequestOptionsArgs, Response, Headers } from '@angular/http';
import { Logging } from '../common/logging';
import { ErrorCodes } from '../error/error-info';
import { BadRequestResponse } from './bad-request-response.model';

declare var $: any;

@Injectable()
export class HttpService extends Http {
    public pendingRequests: number = 0;
    public showLoading: boolean = false;
    public timeoutId: NodeJS.Timer;

    constructor(backend: XHRBackend, defaultOptions: RequestOptions, private router: Router) {
        super(backend, defaultOptions);
    }

    // ***
    // Loading intercept
    // ***
    request(url: string | Request, options?: RequestOptionsArgs): Observable<Response> {
        // If we're the healthcheck page, don't handle the error
        var checkUrl = (url as Request).url.toLowerCase();
        if (checkUrl.includes('healthcheck') || checkUrl.includes('training'))
            return this.intercept(super.request(url, options).catch((res) => { return Observable.throw(res); }));
        else
            return this.intercept(super.request(url, options).catch(this.handleError));
    }

    intercept(observable: Observable<Response>): Observable<Response> {
        this.pendingRequests++;
        this.turnOnModal();
        return observable
            .finally(() => {
                this.turnOffModal();
            });
    }

    private handleError = (res: Response) => {
        localStorage.setItem(ErrorCodes.Key, res.status.toString());
        Logging.log(res.text());

        var br = new BadRequestResponse();
        if (res.status == 400) {
            br = JSON.parse(res.text());
        }

        if (!br.message.startsWith('Cannot'))
            this.router.navigate(['/error']);
        
        //if (res.status === 401 || res.status === 403) {
        //    //handle authorization errors
        //    //in this example I am navigating to logout route which brings the login screen
        //    this.router.navigate(['logout']);
        //}
        return Observable.throw(res);
    }

    private turnOnModal() {
        if (!this.showLoading) {
            this.showLoading = true;
            this.timeoutId = setTimeout(this.showAlert, 750);
        }
        this.showLoading = true;
    }

    private showAlert()
    {
        $('#pleaseWaitDialog').modal();
    }

    private turnOffModal() {
        this.pendingRequests--;
        if (this.pendingRequests <= 0) {
            if (this.showLoading) {
                $('#pleaseWaitDialog').modal('hide');
                clearTimeout(this.timeoutId);
            }
            this.showLoading = false;
        }
    }
}
