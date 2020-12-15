import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { Data } from './data.model';
import { Logging } from '../common/logging';

@Injectable()
export class DataService {
    private serviceUrl = '';
    private list = 'data';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    getAll(): Promise<Data[]> {
        return this.http
            .get(this.serviceUrl)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }
    
    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);
        return Promise.reject(error.message || error);
    }
}


