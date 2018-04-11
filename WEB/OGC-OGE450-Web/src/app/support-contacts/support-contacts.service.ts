import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { SupportContact } from './support-contact';
import { Logging } from '../common/logging';

@Injectable()
export class SupportContactService {
    private serviceUrl = '';
    private list = 'SupportContacts';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    getAll(): Promise<SupportContact[]> {
        return this.http.get(this.serviceUrl)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    update(contacts: SupportContact[]): Promise<SupportContact[]> {
        const url = `${this.serviceUrl}`;

        return this.http
            .put(url, JSON.stringify(contacts))
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);
        return Promise.reject(error.message || error);
    }
}


