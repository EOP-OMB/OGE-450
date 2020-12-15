import { environment } from '../../environments/environment'
import { Router } from '@angular/router';
import { Component, OnInit, ViewChild } from '@angular/core';
import { TabsComponent } from '../common/tabs/tabs.component';
import { TabComponent } from '../common/tabs/tab.component';

import { Widget } from '../common/widget';

import { UserService } from '../security/user.service';

import { NotificationTemplateService } from './edit-templates/notification-template.service';
import { NotificationTemplate } from './edit-templates/notification-template';

import { EventRequest } from '../event-request/event-request.model';
import { EventRequestService } from '../event-request/event-request.service';

import { AdminService } from './admin.service';

declare var $: any;

@Component({
    selector: 'admin',
    templateUrl: './admin.component.html',
    styleUrls: ['./admin.component.css']
})

export class AdminComponent implements OnInit {
    //@ViewChild('tabAppSettings') tabAppSettings;
    @ViewChild('tabNotifications') tabNotifications;
    @ViewChild('tabEvents') tabEvents;
    @ViewChild('tabs') tabs;

    @ViewChild('dtEvents') dtEvents;

    templates: NotificationTemplate[];
    
    openWidget: Widget = new Widget();
    upcomingWidget: Widget = new Widget();
    notificationWidget: Widget = new Widget();
    
    reload: boolean = false;

    eventRequests: EventRequest[];
    eventId: number;

    isFlipped: boolean;
    showClosed: boolean;
    needsRefresh: boolean;  

    constructor(private templateService: NotificationTemplateService,
        private eventService: EventRequestService,
        private router: Router,
        private userService: UserService,
        private adminService: AdminService
    ) {
        this.showClosed = false;
        this.needsRefresh = false;
    }

    ngOnInit(): void {
        if (this.isAdmin())
            this.loadTemplates();

        this.loadEvents();

        this.isFlipped = false;
    }

    removeClosedEvents() {
        this.showClosed = false;
        this.eventRequests = this.eventRequests.filter(x => x.status.includes('Open'));
    }

    loadClosedEvents() {
        this.showClosed = true;

        return this.eventService
            .getAll()
            .then(response => {
                this.eventRequests = response;

                this.updateOpenWidget();
                this.updateUpcomingWidget();
            });
    }

    toggleClosed() {
        this.showClosed = !this.showClosed;

        if (this.showClosed)
            this.loadClosedEvents();
        else
            this.removeClosedEvents();
            
    }

    loadEvents(): Promise<void> {
        return this.eventService
            .getAllOpen()
            .then(response => {
                this.eventRequests = response;

                this.updateOpenWidget();
                this.updateUpcomingWidget();
            });
    }

    loadTemplates(): void {
        this.templateService
            .getAll()
            .then(response => {
                this.templates = response;
                this.updateNotificationsWidget();
            });
    }

    updateUpcomingWidget() {
        if (this.eventRequests) {
            var thisWeek = new Date();
            var today = new Date();
            today.setHours(0, 0, 0, 0);
            thisWeek.setDate(thisWeek.getDate() + 7);

            var upcommingEvents = this.eventRequests.filter(x => x.status.indexOf('Open') >= 0 && new Date(x.eventStartDate).getTime() >= today.getTime() && new Date(x.eventStartDate).getTime() <= thisWeek.getTime());

            var upcomingCount = upcommingEvents.length;
                        
            this.upcomingWidget.color = upcomingCount == 0 ? 'success' : upcomingCount <= 10 ? 'warning' : 'danger';
            this.upcomingWidget.title = upcomingCount.toString();
            this.upcomingWidget.text = 'Upcoming Event' + ((upcomingCount != 1) ? 's' : '');
            this.upcomingWidget.actionText = 'click to review';
        }
    }

    updateOpenWidget() {
        if (this.eventRequests) {
            var openEvents = this.eventRequests.filter(x => x.status.indexOf('Open') >= 0);

            var openCount = openEvents.length;

            this.openWidget.color = 'success';
            this.openWidget.title = openCount.toString();
            this.openWidget.text = 'Open Event' + ((openCount != 1) ? 's' : '');
            this.openWidget.actionText = 'click to review';
        }
    }

    updateNotificationsWidget() {
        if (this.templates) {
            this.notificationWidget.title = this.templates.length.toString();
            this.notificationWidget.text = "Notification Templates";
            this.notificationWidget.actionText = "click to modify";
            this.notificationWidget.color = "primary";
        }
    }

    onOpenClick() {
        this.tabs.selectTab(this.tabEvents);

        this.dtEvents.filterTable("Open", true);

        if (this.isFlipped)
            this.toggleCard();
    }

    onUpcomingClick() {
        this.tabs.selectTab(this.tabEvents);

        this.dtEvents.filterTable("Upcoming", true);

        if (this.isFlipped)
            this.toggleCard();
    }

    onEmailsClick() {
        this.tabs.selectTab(this.tabNotifications);
    }

    gotoDetail(id: number): void {
        this.router.navigate(['/event', id]);
    }

    editRequest(id: number): void {
        this.eventId = id;

        this.toggleCard();
    }

    detailClose(): void {
        this.toggleCard();
    }

    toggleCard() {
        this.isFlipped = !this.isFlipped;

        if (this.needsRefresh && !this.isFlipped) {
            this.loadEvents().then(() => {
                $('#eventCard').toggleClass("flip");

                this.needsRefresh = false;
            });
        }
        else {
            $('#eventCard').toggleClass("flip");
        }
    }

    saveDetail(er: EventRequest) {
        this.loadEvents().then(() => {
            this.toggleCard();
        });
    }

    updated() {
        this.needsRefresh = true;
    }

    isAdmin() {
        return this.userService.user.isAdmin;
    }
    //onSettingsClick() {
    //    this.tabs.selectTab(this.tabAppSettings);
    //}

    //onEmailsClick() {
    //    this.tabs.selectTab(this.tabNotifications);
    //}
}
