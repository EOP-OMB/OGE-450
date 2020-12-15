import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { HealthCheck } from './health-check';
import { Logging } from '../common/logging';

@Injectable()
export class HealthCheckService {
    private serviceUrl = '';
    private list = 'HealthCheck';  // URL to web api

    constructor(private http: Http) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    getAll(): Promise<HealthCheck[]> {
        return this.http.get(this.serviceUrl)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);
        
        let check: HealthCheck[] = [];

        var h = new HealthCheck();
        h.description = "Ethics Portal API";
        h.status = "Error";
        check.push(h);

        h = new HealthCheck();
        h.description = "SharePoint List Access";
        h.status = "Unknown";
        h.detail = "Status unknown, when API is availalbe, can recheck";

        check.push(h);

        h = new HealthCheck();
        h.description = "Check ADFS availability";
        h.status = "Unknown";
        h.detail = "Status unknown, when API is availalbe, can recheck";
        
        check.push(h);

        return Promise.resolve(check);
    }
}


