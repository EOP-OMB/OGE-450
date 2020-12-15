import { Component, OnInit, Input } from '@angular/core';
import { Helper } from '../common/helper'
import { EventStatus } from '../common/constants';

import { Router } from '@angular/router';

import { UserService } from '../security/user.service';
import { Data } from './data.model';

import { Widget } from '../common/widget';

import { EventRequest } from '../event-request/event-request.model';
import { EventRequestService } from '../event-request/event-request.service';

declare var $: any;

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})

export class HomeComponent implements OnInit {
    pendingEventsWidget: Widget;
    newEventWidget: Widget;

    events: EventRequest[];
    pendingEvents: EventRequest[];
    pastEvents: EventRequest[];
    submittedEvents: EventRequest[];

    hasDrafts: boolean = false;

    get user() {
        return this.userService.user;
    }

    ngOnInit(): void {
        this.newEventWidget = new Widget();
        this.newEventWidget.title = "New";
        this.newEventWidget.text = "Event Request";
        this.newEventWidget.actionText = "click to enter a new event";
        this.newEventWidget.color = "success";

        this.eventService.getMyEvents().then(response => {
            this.events = response;

            //var myevents = this.events.filter(x => x.attendeesString.includes(this.user.displayName));
            this.hasDrafts = this.events.find(x => x.status == EventStatus.DRAFT) != null;
            this.pendingEvents = this.events.filter(x => x.status.indexOf('Open') >= 0 || x.status == EventStatus.DRAFT);
            this.pastEvents = this.events.filter(x => x.status.indexOf('Closed') >= 0);

            //this.submittedEvents = this.events.filter(x => x.submittedBy == this.user.displayName && !x.attendeesString.includes(this.user.displayName));

            this.updatePendingEventsWidget();
        });
    }

    constructor(
        private userService: UserService,
        private eventService: EventRequestService,
        private router: Router) { }


    updatePendingEventsWidget() {
        this.pendingEventsWidget = new Widget();
        this.pendingEventsWidget.title = this.pendingEvents.length.toString();
        this.pendingEventsWidget.text = "Pending";
        this.pendingEventsWidget.actionText = "see events below";
        this.pendingEventsWidget.color = "info";
    }

    filter(): void {

    }

    gotoEvent(id: number) {
        this.router.navigate(['/event', id]);
    }
}
