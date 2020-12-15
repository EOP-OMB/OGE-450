import { environment } from '../../environments/environment';
import { Helper, Guid } from '../common/helper';
import { Lookups, EventStatus, AttachmentTypes } from '../common/constants';
import { FileItem } from 'ng2-file-upload';

import { Component, OnInit, Input, ViewChild, AfterViewInit } from '@angular/core';

import { Router, ActivatedRoute, CanDeactivate } from '@angular/router';

import { UserService } from '../security/user.service';
import { EventRequestService } from './event-request.service';
import { AppUser } from '../security/app-user';

import { EventRequest, Attendee, Attachment } from './event-request.model';

declare var $: any;

@Component({
    selector: 'event-request',
    templateUrl: './event-request.component.html',
    styleUrls: ['./event-request.component.css']
})

export class EventRequestComponent implements OnInit, AfterViewInit {
    event: EventRequest;
    origEvent: EventRequest;

    hostOrgType: OrgType;
    orgOrgType: OrgType;

    @ViewChild('uplTravelForm') uplTravelForm;
    @ViewChild('uplInvitation') uplInvitation;
    @ViewChild('uplOther') uplOther;

    travelForms: Attachment[];
    invitations: Attachment[];
    otherAttachments: Attachment[];

    invalidControls: any[];
    invalidMessage: string;
    tempStartDate: Date;
    tempEndDate: Date;

    public results: AppUser[];

    constructor(
        private eventService: EventRequestService,
        private userService: UserService,
        private route: ActivatedRoute,
        private router: Router) {
    }

    public isDirty(): boolean {
        return JSON.stringify(this.origEvent) != JSON.stringify(this.event);
    }

    get user() {
        return this.userService.user;
    }

    ngOnInit(): void {
        this.route.data.subscribe((data: { event: EventRequest }) => {
            this.initialize(data.event);

            localStorage.setItem('dirtyOvervide', "0");
            localStorage.setItem('goto', '');
        });
    }

    ngAfterViewInit() {
        if (!this.event.status || this.event.status == '')
            $('#intro-popup').modal();
    }

    initialize(event: EventRequest) {
        this.event = event;

        if (event.id == 0) {
            this.event.attachmentGuid = Guid.newGuid();
            this.event.submittedBy = this.userService.user.upn;
            this.event.submitter = this.userService.user.displayName;
            this.event.contactEmail = this.userService.user.email;
            this.event.contactNumber = this.userService.user.phoneNumber;
            this.event.contactComponent = this.userService.user.branch;
        }

        this.tempStartDate = this.event.eventStartDate;
        this.tempEndDate = this.event.eventEndDate;

        if (!this.event.attendees || this.event.attendees.length == 0) {
            this.event.attendees = [];
            this.newAttendee();
        }

        if (!this.event.attachments) {
            this.event.attachments = [];
        } else {
            this.initializeAttachments();
        }

        this.setOrgTypes();
        this.origEvent = JSON.parse(JSON.stringify(this.event));
    }

    initializeAttachments() {
        this.travelForms = this.event.attachments.filter(x => x.typeOfAttachment == AttachmentTypes.TRAVEL_FORMS);
        this.invitations = this.event.attachments.filter(x => x.typeOfAttachment == AttachmentTypes.INVIATIONS);
        this.otherAttachments = this.event.attachments.filter(x => x.typeOfAttachment == AttachmentTypes.OTHER);
    }

    amIGoing(): boolean {
        return this.event.attendees ? this.event.attendees.find(x => x.employee && x.employee.upn == this.user.upn) != undefined : false;
    }

    canSubmit(): boolean {
        return this.event.status == EventStatus.DRAFT || this.event.id == 0;
    }

    canSave(): boolean {
        return this.event ? this.event.status ? !this.event.status.includes('Closed') : true : true;
    }

    newAttendee(): void {
        var attendee = new Attendee(this.event.id);
        attendee.employee = new AppUser();
        attendee.employee.displayName = "";
        this.event.attendees.push(attendee);
    }

    goBack(): void {
        if (this.canSave() && this.isDirty())
            $('#confirm-close').modal();
        else
            this.confirmClose();
    }

    delete(att: Attendee): void {
        this.event.attendees = this.event.attendees.filter(x => x != att);

        if (this.event.attendees.length == 0)
            this.newAttendee();
    }

    setOrgTypes() {
        this.orgOrgType = new OrgType();

        this.orgOrgType.nonProfit = this.checkFlag(this.event.typeOfOrg, 1);
        this.orgOrgType.mediaOrg = this.checkFlag(this.event.typeOfOrg, 2);
        this.orgOrgType.LobbyingOrg = this.checkFlag(this.event.typeOfOrg, 4);
        this.orgOrgType.usgEntity = this.checkFlag(this.event.typeOfOrg, 8);
        this.orgOrgType.foreignGov = this.checkFlag(this.event.typeOfOrg, 16);

        this.hostOrgType = new OrgType();

        this.hostOrgType.nonProfit = this.checkFlag(this.event.typeOfHost, 1);
        this.hostOrgType.mediaOrg = this.checkFlag(this.event.typeOfHost, 2);
        this.hostOrgType.LobbyingOrg = this.checkFlag(this.event.typeOfHost, 4);
        this.hostOrgType.usgEntity = this.checkFlag(this.event.typeOfHost, 8);
        this.hostOrgType.foreignGov = this.checkFlag(this.event.typeOfHost, 16);
    }

    checkFlag(flag: number, check: number): boolean {
        return ((flag & check) == check);
    }

    confirmClose() {
        localStorage.setItem('dirtyOvervide', "1");
        $('#confirm-close').modal('hide');

        // Check to see if we were trying to go somewhere other than previous, if not go back to previous
        var prev = localStorage.getItem('goto') ? localStorage.getItem('goto') : '';

        if (prev == '') {
            prev = localStorage.getItem('prev') ? localStorage.getItem('prev') : '/home';
            if (prev.includes('event'))
                prev = '/';
        }

        this.router.navigate([prev]);
    }

    scroll(hash: string): void {
        // animate
        $('html, body').animate({
            scrollTop: $(hash).offset().top - 222
        }, 500, function () {

        });
    }

    submitRequest() {
        var valid = this.validateForm();

        if (valid && this.canSubmit()) {
            this.event.status = EventStatus.UNASSIGNED;
            this.save(true, true);
            this.disableForm();
        }
        else {
            this.displayInvalid();
        }
    }

    displayInvalid() {
        $("#invalid-alert").alert();
        $("#invalid-alert").fadeTo(5000, 500).slideUp(500, function () {
            $("#invalid-alert").slideUp(500);
        });

        this.scroll('#eventFormContainer');
    }

    disableForm(): void {
        if (!this.canSave()) {
            $('input,textarea').prop('disabled', true);
            $('.add-row').hide();
        }
    }

    validateForm(): boolean {
        var isValid = false;

        $(':input[required]:visible').focus();
        $(':input[required]:visible').blur();

        this.invalidMessage = "";
        var invalidControls = [];
        $(':input[required]:visible.ng-invalid').each(function() {
            var title = $(this)[0].title;

            if (title)
                invalidControls.push($(this));
        });

        if (invalidControls.length > 0)
            this.invalidControls = invalidControls;

        var invalid = $(':input[required]:visible.ng-invalid:first');

        if (invalid.length > 0)
            invalid.focus();
        else
            isValid = true;

        return isValid;
    }

    hasAttachments(upl: any): boolean {
        var files: FileItem[] = upl.getFiles();

        return files.length > 0;
    }

    saveAndClose() {
        this.save(false, true);
    }

    save(submitting: boolean, close: boolean = false): void {
        this.loadAttachments(this.uplTravelForm);
        this.loadAttachments(this.uplInvitation);
        //this.loadAttachments(this.uplOther);
        this.event.typeOfOrg = this.orgOrgType.value;
        this.event.typeOfHost = this.hostOrgType.value;

        var valid = true;

        if (submitting)
            this.event.status = EventStatus.UNASSIGNED;
        else if (this.event.status && this.event.status.includes('Open'))
            valid = this.validateForm();

        if (valid) {
            if (this.canSave() || submitting) {
                //this.form.dateOfAppointment = Helper.formatDate(this.tempAppointmentDate);
                if (this.event.id > 0) {
                    this.eventService.update(this.event)
                        .then(response => {
                            this.saveComplete(close, response);
                        });
                }
                else {
                    this.eventService.create(this.event)
                        .then(response => {
                            this.saveComplete(close, response);
                        });
                }
            }
        }
        else {
            this.displayInvalid();
        }
    }

    loadAttachments(upl: any) {
        var files: FileItem[] = upl.getFiles();
        var i: number = 0;

        while (i < files.length) {
            var att = new Attachment();

            att.fileName = files[i].file.name;
            att.size = files[i].file.size;
            att.typeOfAttachment = upl.type;
            att.attachmentGuid = this.event.attachmentGuid;

            this.event.attachments.push(att);

            i++;
        }
    }

    saveComplete(close: boolean, eventRequest: EventRequest): void {
        if (close) {
            this.confirmClose();
        }
        else {
            this.initialize(eventRequest);

            $("#success-alert").alert();

            $("#success-alert").fadeTo(2000, 500).slideUp(500, function () {
                $("#success-alert").slideUp(500);
            });
        }
    }

    startDateChange(e) {
        if (!this.event.eventEndDate) {
            this.tempEndDate = e.target.value;
            this.event.eventEndDate = this.tempEndDate;
        }
    }

    print(): void {
        window.print();
    }

    search(event, att) {
        this.userService.search(event.query).then(data => {
            this.results = data;
        });
    }

    removeAttachment(att: Attachment) {
        this.event.attachments = this.event.attachments.filter(x => x.id != att.id);
        this.initializeAttachments();
    }

    assignMe(att: Attendee) {
        att.employee = this.userService.user;
    }

    employeeSelect(event) {
        //console.log(event);
    }
}

export class OrgType {
    public nonProfit: boolean;
    public mediaOrg: boolean;
    public LobbyingOrg: boolean;
    public usgEntity: boolean;
    public foreignGov: boolean;

    get value(): number {
        return (this.nonProfit == true ? 1 : 0) + (this.mediaOrg == true ? 2 : 0) + (this.LobbyingOrg == true ? 4 : 0) + (this.usgEntity == true ? 8 : 0) + (this.foreignGov == true ? 16 : 0);
    }
}

export class PreventUnsavedChangesGuard implements CanDeactivate<EventRequestComponent> {
    canDeactivate(component: EventRequestComponent) {
        var override = localStorage.getItem('dirtyOvervide') ? localStorage.getItem('dirtyOvervide') == "1" : false;

        if (component.canSave() && !override && component.isDirty()) {
            $('#confirm-close').modal();
            return false;
        }

        return true;
    }
}
