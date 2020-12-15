import { Injectable } from '@angular/core';
import { Helper } from '../common/helper';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { EventRequest } from './event-request.model';
import { Logging } from '../common/logging';

@Injectable()
export class EventRequestService {
    private serviceUrl = '';
    private list = 'eventRequest';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    getAll(): Promise<EventRequest[]> {
        return this.http.get(this.serviceUrl)
            .toPromise()
            .then(response => {
                var data = response.json();
                this.formatResponse(data);
                return data;
            })
            .catch(this.handleError);
    }

    formatResponse(response: EventRequest[]) {
        
        response.forEach(x => {
            var i: number = 0;
            x.attendeesString = "";

            for (i = 0; i < x.attendees.length; i++) {
                x.attendeesString += x.attendees[i].employee.displayName + ', ';
            }

            if (x.attendeesString.length > 0)
                x.attendeesString = x.attendeesString.substr(0, x.attendeesString.length - 2);

            x.eventDates = "";

            x.eventDates = Helper.formatDate(x.eventStartDate);

            if (x.eventEndDate && x.eventEndDate != x.eventStartDate)
                x.eventDates += ' - ' + Helper.formatDate(x.eventEndDate);

        });
    }

    get(id: number): Promise<EventRequest> {
        var url = `${this.serviceUrl}/${id}`;

        return this.http.get(url)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    getMyEvents(): Promise<EventRequest[]> {
        var url = `${this.serviceUrl}?a=myevents`;

        return this.http.get(url)
            .toPromise()
            .then(response => {
                var data = response.json();
                this.formatResponse(data);
                return data;
            })
            .catch(this.handleError);
    }

    getAllOpen(): Promise<EventRequest[]> {
        var url = `${this.serviceUrl}?a=open`;

        return this.http.get(url)
            .toPromise()
            .then(response => {
                var data = response.json();
                this.formatResponse(data);
                return data;
            })
            .catch(this.handleError);
    }

    resendAssignedToEmail(id): Promise<any> {
        var url = `${this.serviceUrl}?a=assignedTo:` + id.toString();

        return this.http.get(url)
            .toPromise()
            .then()
            .catch(this.handleError);
    }

    update(event: EventRequest): Promise<EventRequest> {
        const url = `${this.serviceUrl}`;

        return this.http
            .put(url, JSON.stringify(event))
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    create(event: EventRequest): Promise<EventRequest> {
        const url = `${this.serviceUrl}`;

        return this.http
            .post(url, JSON.stringify(event))
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);
        return Promise.reject(error.message || error);
    }

    public copy(event: EventRequest): EventRequest {
        var newEvent = new EventRequest();

        newEvent.id = event.id;
        newEvent.title = event.title;



        return newEvent;
    }
}


