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
        return this.getFormattedArray(this.serviceUrl);
    }

    private getFormattedArray(url: string) {
        return this.http.get(url)
            .toPromise()
            .then(response => {
                var data: OGEForm450[] = [];

                var obj = response.json();

                obj.forEach(x => {
                    var obj = this.formatResponse(x);
                    data.push(obj);
                });

                return data;
            })
            .catch(this.handleError);
    }

    getMyForms(): Promise<OGEForm450[]> {
        var url = `${this.serviceUrl}?a=myforms`;

        return this.getFormattedArray(url);
    }

    getReviewableForms(): Promise<OGEForm450[]> {
        var url = `${this.serviceUrl}?a=reviewer`;

        return this.getFormattedArray(url);
    }

    certifyForms(action: string): Promise<OGEForm450[]> {
        var url = `${this.serviceUrl}?a=certify` + action;

        return this.getFormattedArray(url);
    }

    get(id: number): Promise<OGEForm450> {
        var url = `${this.serviceUrl}/${id}`;

        return this.getFormattedObject(url);
    }

    private getFormattedObject(url: string) {
        return this.http.get(url)
            .toPromise()
            .then(response => {
                var obj = response.json();
                if (obj != null)
                    obj = this.formatResponse(obj);
                return obj;
            })
            .catch(this.handleError);
    }

    getPrevious(id: number): Promise<OGEForm450> {
        var url = `${this.serviceUrl}?a=prev&id=` + id;

        return this.getFormattedObject(url);
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

    formatResponse(data: OGEForm450): OGEForm450 {
        return Object.assign(new OGEForm450(), data);
    }
}


