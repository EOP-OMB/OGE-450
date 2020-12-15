import { environment } from '../../environments/environment';
import { Helper } from '../common/helper';
import { Lookups, Divisions } from '../common/constants';

import { Component, OnInit, Input } from '@angular/core';

import { Router } from '@angular/router';

import { UserService } from '../security/user.service';

import { TimelineService } from '../timeline/timeline.service';
import { Timeline } from '../timeline/timeline.model';
import { TimelineVm } from '../timeline/timeline-vm';

import { NotificationService } from '../notifications/notifications.service';
import { Notifications } from '../notifications/notifications.model';
import { NotificationsVm } from '../notifications/notifications-vm';

import { TrainingService } from '../training/training.service';
import { Training } from '../training/training.model';

import { OGEForm450Service } from '../oge-form-450/oge-form-450.service';
import { OGEForm450 } from '../oge-form-450/oge-form-450.model';

import { EventRequestService } from '../event-request/event-request.service';
import { EventRequest } from '../event-request/event-request.model';
import { constants } from 'fs';
import { EthicsFormService } from '../ethics-form/ethics-form.service';
import { EthicsForm } from '../ethics-form/ethics-form.model';
import { EthicsTeam } from '../ethics-team/ethics-team.model';
import { EthicsTeamService } from '../ethics-team/ethics-team.service';

declare var $: any;

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})

export class HomeComponent implements OnInit {
    private trainings: Training[];
    private selectedTraining: Training;

    private currentYear: number;

    private annualTraining: Training;
    private initialTraining: Training;

    text: string;
    results: string[];

    tempId: number;
    tempTrainingStatus: string;
    tempTrainingBorder: string;
    tempTrainingText: string;
    tempTrainingStatusColor: string;

    tempFormId: number;
    tempFormStatus: string;
    tempFormBorder: string;
    tempFormText: string;
    tempFormStatusColor: string;

    tempEventText: string;
    tempEventColor: string;

    notificationVm: NotificationsVm;
    notifications: Notifications[];
    maxNotifications: number = 5;
    selectedNotification: Notifications;

    guidanceFiles: EthicsForm[];
    ethicsFormFiles: EthicsForm[];
    ethicsTeam: EthicsTeam[] = [];

    timelineVm: TimelineVm;
    timeline: Timeline[];
    maxTimeline: number = 5;

    currentForm: OGEForm450;

    // TODO:  Make each list a component
    // Filters
    showOGE450: boolean = true;
    showTraining: boolean = true;
    showEvents: boolean = true;

    showCurrentYear: boolean = true;
    showLastYear: boolean = true;
    showOlder: boolean = true;

    // Notification Filters
    nfShowOGE450: boolean = true;
    nfShowTraining: boolean = true;
    nfShowEvents: boolean = true;
    nfShowOther: boolean = true;

    nfShowCurrentYear: boolean = true;
    nfShowLastYear: boolean = true;
    nfShowOlder: boolean = true;

    constructor(
        private userService: UserService,
        private trainingService: TrainingService,
        private timelineService: TimelineService,
        private notificationService: NotificationService,
        private formService: OGEForm450Service,
        private eventService: EventRequestService,
        private ethicsFormService: EthicsFormService,
        private ethicsTeamService: EthicsTeamService,
        private router: Router) {

        var dt = new Date();
        this.currentYear = dt.getFullYear();
    }

    get user() {
        return this.userService.user;
    }

    ngOnInit(): void {
        this.getTrainings();
        this.getTimeline();
        //this.getNotifications();
        this.getGuidance();
        this.getEthicsTeam();
        this.getForm();
        this.getEvents();
    }

    getForm(): void {
        this.formService
            .get()
            .then(form => {
                this.currentForm = form;
                this.set450Status();
                //this.loadingComplete = true;
            });
    }

    getEvents(): void {
        this.eventService.getMyEvents().then(response => {
            var pending = response.filter(x => x.status.includes('Open') == true);

            var numPending = pending.length;

            if (numPending == 0)
                this.tempEventColor = "text-success";
            else if (numPending < 3)
                this.tempEventColor = "text-info";
            else if (numPending < 6)
                this.tempEventColor = "text-warning";
            else 
                this.tempEventColor = "text-danger";

            this.tempEventText = numPending.toString() + ((numPending == 1) ? " Event" : " Events");
        });
    }

    getGuidance(): void {
        this.ethicsFormService.getAll()
            .then(result => {
                var allForms = result;

                this.guidanceFiles = allForms.filter(x => x.formType == 'Guidance');
                this.ethicsFormFiles = allForms.filter(x => x.formType == 'Form');
            });
    }

    getEthicsTeam(): void {
        this.ethicsTeamService.getAll()
            .then(result => {
                this.ethicsTeam = result;
            });
    }

    getNotifications(): void {
        this.notificationService.get(this.maxNotifications)
            .then(result => {
                // Use the Type from the API to assign CssClass and Icon
                this.notificationVm = result;
                
                if (this.notificationVm.notifications) {

                    for (let n of this.notificationVm.notifications) {
                        if (n.application == "OGE450") {
                            n.cssClass = "oge450Class";
                        }
                        else if (n.application == "Training") {
                            n.cssClass = "trainingClass";
                        }
                        else if (n.application == "Event") {
                            n.cssClass = "eventClass";
                        }
                        else {
                            n.cssClass = "otherClass";
                        }
                    }
                }

                this.doFilterNotifications();
            });
    }

    getTimeline(): void {
        this.timelineService.get(this.maxTimeline)
            .then(result => {
                // Use the Type from the API to assign CssClass and Icon
                this.timelineVm = result;

                if (this.timelineVm.timeline) {

                    for (let tl of this.timelineVm.timeline) {
                        if (tl.type == "OGEForm450") {
                            tl.cssClass = "oge450Class";
                            tl.icon = "fa-file-text";
                        }
                        else if (tl.type == "Training") {
                            tl.cssClass = "trainingClass";
                            tl.icon = "fa-balance-scale";
                        }
                        else if (tl.type == "Event") {
                            tl.cssClass = "eventClass";
                            tl.icon = "fa-calendar-check-o";
                        }
                    }
                }

                this.doFilterTimeline();
            });
    }

    getTrainings(): void {
        this.trainingService.getMyTrainings()
            .then(result => {
                this.trainings = result;

                var thisYearTraining = result.filter(x => x.year == this.currentYear);

                for (let tra of thisYearTraining) {
                    if (tra.trainingType == "Annual")
                        this.annualTraining = tra;
                    else if (tra.trainingType == "Initial") {
                        this.initialTraining = tra;
                        if (tra.division == Divisions.OGC)
                            this.annualTraining = tra;
                    }
                }

                if (!this.initialTraining) {
                    var initials = result.filter(x => x.trainingType == "Initial");
                    this.initialTraining = initials.length > 0 ? initials[0] : null;
                }

                this.tempTrainingText = this.currentYear + " Annual";

                if (this.annualTraining) {
                    this.tempId = this.annualTraining.id;
                    this.tempTrainingStatus = "COMPLETE";
                    this.tempTrainingBorder = "";
                    this.tempTrainingStatusColor = "text-success";
                }
                else {
                    this.tempTrainingStatus = "MISSING";
                    this.tempTrainingBorder = "danger";
                    this.tempTrainingStatusColor = "text-danger";
                }
            });
    }
    
    gotoDetail(type: string, id: number): void {

        if (type == "OGEForm450") {
            this.goto("OGE450", id);
        }
        else if (type == "Training") {
            if (id > 0) {
                // show modal for selected training
                var training = this.trainings.filter(x => x.id == id);
                if (training) {
                    this.selectedTraining = training[0];
                    $('#edit-training').modal();
                }
            }
            else {
                this.newTraining("Annual");
            }
        }
    }

    goto(where: string, id: number = 0): void {
        var url = "";
        if (where == "Events") {
            url = environment.eventClearanceUrl;
        }
        else if (where == "OGE450") {
            url = environment.oge450Url;
            if (id > 0)
                url += "/form/" + id;
        }

        if (url != "")
            window.open(url, '_self');
    }

    newTraining(type: string): void {
        // show modal for new training

        this.selectedTraining = new Training();
        this.selectedTraining.trainingType = type;
        var initialDateTime = new Date();
        initialDateTime.setHours(12, 0, 0);
        this.selectedTraining.dateAndTime = Helper.formatDate(initialDateTime, true);
        $('#edit-training').modal();
    }

    editTrainingClose(save: boolean): void {
        $('#edit-training').modal('hide');

        if (save) {
            this.getTrainings();
            this.getTimeline();
        }
    }

    notificationClose(): void {
        $('#view-notification').modal('hide');
    }

    showMore(list: string) {
        if (list == "timeline") {
            this.maxTimeline += 5;
            this.getTimeline();
        }
        else if (list == "notifications") {
            this.maxNotifications += 5;
            this.getNotifications();
        }
    }

    showNotification(notification: Notifications) {
        
        this.selectedNotification = notification;
        $('#view-notification').modal();
    }

    refresh(list: string) {
        if (list == "timeline") {
            this.maxTimeline = 5;
            this.getTimeline();
        }
        else if (list == "notifications") {
            this.maxNotifications = 5;
            this.getNotifications();
        }
    }

    filterTimeline(how: string) {
        if (how == "allCats") {
            this.showOGE450 = true;
            this.showTraining = true;
            this.showEvents = true;
        }
        else if (how == "OGE450") {
            this.showOGE450 = !this.showOGE450;
        }
        else if (how == "Training") {
            this.showTraining = !this.showTraining;
        }
        else if (how == "Events") {
            this.showEvents = !this.showEvents;
        } else if (how == "allYears") {
            this.showCurrentYear = true;
            this.showLastYear = true;
            this.showOlder = true;
        } else if (how == "current") {
            this.showCurrentYear = !this.showCurrentYear;
        } else if (how == "last") {
            this.showLastYear = !this.showLastYear;
        } else if (how == "older") {
            this.showOlder = !this.showOlder;
        }
        
        this.doFilterTimeline();
    }

    doFilterTimeline() {
        this.timeline = this.timelineVm.timeline.filter(x => ((this.showOGE450 && x.type == "OGEForm450") || (this.showTraining && x.type == "Training") || (this.showEvents && x.type == "Event")) && 
                                                             ((this.showCurrentYear && x.year == this.currentYear) || (this.showLastYear && x.year == this.currentYear - 1) || (this.showOlder && x.year <= this.currentYear -2)));
    }

    filterNotifications(how: string) {
        if (how == "allCats") {
            this.nfShowOGE450 = true;
            this.nfShowTraining = true;
            this.nfShowEvents = true;
            this.nfShowOther = true;
        }
        else if (how == "OGE450") {
            this.nfShowOGE450 = !this.nfShowOGE450;
        }
        else if (how == "Training") {
            this.nfShowTraining = !this.nfShowTraining;
        }
        else if (how == "Other") {
            this.nfShowOther = !this.nfShowOther;
        }
        else if (how == "Events") {
            this.nfShowEvents = !this.nfShowEvents;
        } else if (how == "allYears") {
            this.nfShowCurrentYear = true;
            this.nfShowLastYear = true;
            this.nfShowOlder = true;
        } else if (how == "current") {
            this.nfShowCurrentYear = !this.nfShowCurrentYear;
        } else if (how == "last") {
            this.nfShowLastYear = !this.nfShowLastYear;
        } else if (how == "older") {
            this.nfShowOlder = !this.nfShowOlder;
        }
        
        this.doFilterNotifications();
    }

    doFilterNotifications() {
        this.notifications = this.notificationVm.notifications.filter(x => ((this.nfShowOGE450 && x.application == "OGE450") || (this.nfShowTraining && x.application == "Training") || (this.nfShowEvents && x.application == "Events") || (this.nfShowOther && x.application == "Portal")) &&
            ((this.nfShowCurrentYear && x.year == this.currentYear) || (this.nfShowLastYear && x.year == this.currentYear - 1) || (this.nfShowOlder && x.year <= this.currentYear - 2)));
    }

    set450Status(): void {
        this.tempFormText = "click to launch app";

        if (this.currentForm) {
            this.tempFormId = this.currentForm.id;

            if (this.currentForm.formStatus == 'Certified') {
                this.tempFormStatus = "CERTIFIED";
                this.tempFormBorder = "";
                this.tempFormStatusColor = "text-success";
            }
            else if (this.currentForm.formStatus == 'Submitted' || this.currentForm.formStatus == 'Re-submitted') {
                this.tempFormStatus = "SUBMITTED";
                this.tempFormBorder = "";
                this.tempFormStatusColor = "text-primary";
            }
            else if (this.currentForm.isOverdue) {
                this.tempFormStatus = "OVERDUE";
                this.tempFormBorder = "danger";
                this.tempFormStatusColor = "text-danger";
                this.tempFormText = "<i style='margin-right: 10px; font-size: 1.1em;' class='fa fa-warning text-danger'></i> Please submit your OGE 450 or request an extension";
            }
            else {
                this.tempFormStatus = "IN PROGRESS";
                this.tempFormBorder = "";
                this.tempFormStatusColor = "text-info";
            }
        }
        else {
            this.tempFormStatus = "NOT ASSIGNED";
            this.tempFormBorder = "";
            this.tempFormStatusColor = "text-success";
        }
    }

    mailTo(email: string) {
        var mail = document.createElement("a");
        mail.href = "mailto:" + email;
        mail.click();
    }

    showFile(id: number) {
        this.ethicsFormService.get(id).subscribe((response) => {
            var type = response.blob().type;

            var blob = new Blob([response.blob()], { type });

            var filename = response.headers.get('content-disposition');
            var start = filename.indexOf('filename=') + 10;

            filename = filename.substr(start, filename.length - start - 1);

            //if (type == 'application/pdf') {
            //    var fileURL = URL.createObjectURL(blob);

            //    window.open(fileURL, filename);
            //} else {
                var url = window.URL.createObjectURL(blob);
                
                var a = document.createElement("a");
                a.href = url;
                a.target = '_blank';
                a.download = filename;

                a.click();
            //}            
        });
    }
}

