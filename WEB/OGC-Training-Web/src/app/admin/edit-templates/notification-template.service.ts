import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { NotificationTemplate } from './notification-template';
import { Logging } from '../../common/logging';

@Injectable()
export class NotificationTemplateService {
    private serviceUrl = '';
    private list = 'notificationtemplate';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    get(id: number): Promise<NotificationTemplate> {
        var url = `${this.serviceUrl}/${id}`;

        return this.http.get(url)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    getAll(): Promise<NotificationTemplate[]> {
        return this.http
            .get(this.serviceUrl)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    update(entity: NotificationTemplate): Promise<NotificationTemplate> {
        const url = `${this.serviceUrl}`;

        return this.http
            .put(url, JSON.stringify(entity))
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);
        return Promise.reject(error.message || error);
    }
}


