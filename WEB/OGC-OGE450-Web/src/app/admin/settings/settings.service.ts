import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { Settings } from './settings';
import { Logging } from '../../common/logging';

@Injectable()
export class SettingsService {
    private serviceUrl = '';
    private list = 'settings';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    create(settings: Settings): Promise<Settings> {
        return this.http
            .post(this.serviceUrl, JSON.stringify(settings))
            .toPromise()
            .then(res => res.json().data as Settings)
            .catch(this.handleError);
    }

    get(): Promise<Settings> {
        var url = `${this.serviceUrl}`;

        return this.http.get(url)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    initiateAnnualRollover(): Promise<Settings> {
        var url = `${this.serviceUrl}?a=annual`;

        return this.http.get(url)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    update(settings: Settings): Promise<Settings> {
        const url = `${this.serviceUrl}`;

        return this.http
            .put(url, JSON.stringify(settings))
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);
        return Promise.reject(error.message || error);
    }
}


