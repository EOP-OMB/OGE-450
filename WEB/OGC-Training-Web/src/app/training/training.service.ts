import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { Training } from './training.model';
import { Logging } from '../common/logging';

@Injectable()
export class TrainingService {
    private serviceUrl = '';
    private list = 'training';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }


    getAll(): Promise<Training[]> {
        return this.http
            .get(this.serviceUrl)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    getMyTrainings(): Promise<Training[]> {
        var url = `${this.serviceUrl}?a=mytraining`;

        return this.http.get(url)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    getMissingTrainingReport(year: number): Promise<any> {
        var url = `${this.serviceUrl}?a=missingtrainingreport-` + year.toString();

        return this.http.get(url)
            .toPromise()
            .then(response => response)
            .catch(this.handleError);
    }

    get(id: number): Promise<Training> {
        const url = `${this.serviceUrl}/${id}`;

        return this.http
            .get(url)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    save(item: Training): Promise<Training> {
        if (item.id > 0)
            return this.update(item);
        else
            return this.create(item);
    }

    create(item: Training): Promise<Training> {
        return this.http
            .post(this.serviceUrl, JSON.stringify(item))
            .toPromise()
            .then(res => res.json())
            .catch(this.handleError);
    }

    update(item: Training): Promise<Training> {
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


