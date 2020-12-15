import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { EthicsTeam } from './ethics-team.model';
import { Logging } from '../common/logging';

@Injectable()
export class EthicsTeamService {
    private serviceUrl = '';
    private list = 'ethicsteam';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    getAll(): Promise<EthicsTeam[]> {
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

    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);

        let check: EthicsTeam[] = [];

        return Promise.resolve(check);
    }
}


