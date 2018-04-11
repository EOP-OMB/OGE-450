import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { ExtensionRequest } from '../extension-request/extension-request';
import { Logging } from '../common/logging';

@Injectable()
export class ExtensionRequestService {
    private serviceUrl = '';
    private list = 'ExtensionRequest';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    getAll(): Promise<ExtensionRequest[]> {
        return this.http
            .get(this.serviceUrl)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    getPending(): Promise<ExtensionRequest[]> {
        var url = `${this.serviceUrl}?a=pending`;

        return this.http
            .get(url)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    get(id: number): Promise<ExtensionRequest> {
        const url = `${this.serviceUrl}/${id}`;

        return this.http
            .get(url)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    create(item: ExtensionRequest): Promise<ExtensionRequest> {
        return this.http
            .post(this.serviceUrl, JSON.stringify(item))
            .toPromise()
            .then(res => res.json())
            .catch(this.handleError);
    }

    update(item: ExtensionRequest): Promise<ExtensionRequest> {
        const url = `${this.serviceUrl}`;

        return this.http
            .put(url, JSON.stringify(item))
            .toPromise()
            .then(() => item)
            .catch(this.handleError);
    }

    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);
        return Promise.reject(error.message || error);
    }
}


