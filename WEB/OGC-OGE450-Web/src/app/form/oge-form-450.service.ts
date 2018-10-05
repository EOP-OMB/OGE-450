import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { OGEForm450 } from './oge-form-450';
import { Logging } from '../common/logging';

@Injectable()
export class OGEForm450Service {
    private serviceUrl = '';
    private list = 'ogeform450';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    getAll(): Promise<OGEForm450[]> {
        return this.http.get(this.serviceUrl)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    getMyForms(): Promise<OGEForm450[]> {
        var url = `${this.serviceUrl}?a=myforms`;

        return this.http.get(url)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    getReviewableForms(): Promise<OGEForm450[]> {
        var url = `${this.serviceUrl}?a=reviewer`;

        return this.http.get(url)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    certifyForms(action: string): Promise<OGEForm450[]> {
        var url = `${this.serviceUrl}?a=certify` + action;

        return this.http.get(url)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    get(id: number): Promise<OGEForm450> {
        var url = `${this.serviceUrl}/${id}`;

        return this.http.get(url)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

     create(name: string): Promise<OGEForm450> {
        return this.http
            .post(this.serviceUrl, JSON.stringify({ name: name }))
            .toPromise()
            .then(res => res.json().data as OGEForm450)
            .catch(this.handleError);
    }

    update(form: OGEForm450): Promise<OGEForm450> {
        const url = `${this.serviceUrl}`;

        return this.http
            .put(url, JSON.stringify(form))
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    delete(id: number): Promise<void> {
        const url = `${this.serviceUrl}/${id}`;
        return this.http.delete(url)
            .toPromise()
            .then(() => null)
            .catch(this.handleError);
    }

    private handleError(error: Response | any): Promise<any> {
        return Promise.reject(error.message || error);
    }
}


