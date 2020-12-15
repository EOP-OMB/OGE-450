import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions, ResponseContentType } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { AppUser } from '../security/app-user';
import { Logging } from '../common/logging';

@Injectable()
export class AdminService {
    private serviceUrl = '';
    private list = 'admin';  // URL to web api

    private _reviewers: AppUser[];

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    public get reviewers(): AppUser[] {
        return this._reviewers;
    }

    load(): Promise<AppUser[]> {
        var url = `${this.serviceUrl}?a=reviewers`;

        return this.http.get(url)
            .toPromise()
            .then(response => {
                this._reviewers = response.json();
                return response.json();
            })
            .catch(this.handleError);
    }
   
    getReviewers(): Promise<AppUser[]> {
        var url = `${this.serviceUrl}?a=reviewers`;

        return this.http.get(url)
            .toPromise()
            .then(response => {
                return response.json();
            })
            .catch(this.handleError);
    }

    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);
        return Promise.reject(error.message || error);
    }
}


