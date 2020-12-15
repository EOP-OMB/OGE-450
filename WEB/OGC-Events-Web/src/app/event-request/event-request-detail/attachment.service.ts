import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions, ResponseContentType } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { Attachment } from '../event-request.model';
import { Logging } from '../../common/logging';

@Injectable()
export class AttachmentService {
    private serviceUrl = '';
    private list = 'attachment';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    //get(id: number): Promise<any> {
    //    var url = `${this.serviceUrl}/${id}`;

    //    return this.http.get(url)
    //        .toPromise()
    //        .then(response => response.json())
    //        .catch(this.handleError);
    //}
    get(id: number) {
        var url = `${this.serviceUrl}/${id}`;

        return this.http.get(url, { responseType: ResponseContentType.Blob })
        .map((res) => {
            return new Blob([res.blob()]);
        })
    }

    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);
        return Promise.reject(error.message || error);
    }
}


