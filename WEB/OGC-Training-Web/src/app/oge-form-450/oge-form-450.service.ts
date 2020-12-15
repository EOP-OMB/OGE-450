import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { OGEForm450 } from './oge-form-450.model';
import { Logging } from '../common/logging';

@Injectable()
export class OGEForm450Service {
    private serviceUrl = '';
    private list = 'ogeform450';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    get(): Promise<OGEForm450> {
        return this.http.get(this.serviceUrl)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    private handleError(error: Response | any): Promise<any> {
        return Promise.reject(error.message || error);
    }
}


