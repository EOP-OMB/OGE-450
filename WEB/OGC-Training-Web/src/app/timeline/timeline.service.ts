import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { TimelineVm } from './timeline-vm';
import { Logging } from '../common/logging';

@Injectable()
export class TimelineService {
    private serviceUrl = '';
    private list = 'Timeline';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    get(max: number): Promise<TimelineVm> {
        var url = `${this.serviceUrl}/${max}`;

        return this.http.get(url)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    getAll(): Promise<TimelineVm> {
        return this.http.get(this.serviceUrl)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);
        
        return Promise.reject(error.message || error);
    }
}


