import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions, ResponseContentType } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';


import { EthicsForm } from './ethics-form.model';
import { Logging } from '../common/logging';

@Injectable()
export class EthicsFormService {
    private serviceUrl = '';
    private list = 'ethicsform';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    getAll(): Promise<EthicsForm[]> {
        //var url = 'assets/json/guidance.json';
        var url = this.serviceUrl;

        return this.http
            .get(url)
            .toPromise()
            .then(response => {
                var guidance = response.json();

                return guidance;
            })
            .catch(this.handleError);
    }

    get(id: number) {
        var url = `${this.serviceUrl}/${id}`;

        return this.http.get(url, { responseType: ResponseContentType.Blob })
            .map((res) => {
                return res;
            })
    }

    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);

        let check: EthicsForm[] = [];

        return Promise.resolve(check);
    }
}


