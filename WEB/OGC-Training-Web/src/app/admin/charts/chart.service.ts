import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { TrainingChartData, OGE450ChartData, EventsChartData } from './charts.model';
import { Logging } from '../../common/logging';

@Injectable()
export class ChartService {
    private serviceUrl = '';
    private list = 'chart';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    getTrainingChartData(selectedYear: number): Promise<TrainingChartData> {
        return this.http
            .get(this.serviceUrl + '?a=training-' + selectedYear.toString())
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    getOGE450ChartData(): Promise<OGE450ChartData> {
        return this.http
            .get(this.serviceUrl + '?a=oge450')
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    getEventsChartData(): Promise<EventsChartData> {
        return this.http
            .get(this.serviceUrl + '?a=events')
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);
        return Promise.reject(error.message || error);
    }
}


