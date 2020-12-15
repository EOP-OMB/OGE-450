import { environment } from '../../../environments/environment';
import { Helper } from '../../common/helper';
import { Lookups, AttachmentTypes, EventStatus } from '../../common/constants';
import { SelectItem } from 'primeng/primeng';

import { Component, OnInit, Input, Output, EventEmitter, SimpleChange } from '@angular/core';

import { Router } from '@angular/router';

import { UserService } from '../../security/user.service';
import { EventRequestService } from '../event-request.service';
import { AttachmentService } from './attachment.service';
import { AppUser } from '../../security/app-user';

import { EventRequest, Attendee, Attachment } from '../event-request.model';
import { AdminService } from '../../admin/admin.service';

declare var $: any;
declare var tinymce: any;

@Component({
    selector: 'event-request-detail',
    templateUrl: './event-request-detail.component.html',
    styleUrls: ['./event-request-detail.component.css']
})

export class EventRequestDetailComponent implements OnInit {
    @Input()
    eventId: number;

    @Output()
    close = new EventEmitter<any>();

    @Output()
    updated = new EventEmitter<any>();

    editEvent: EventRequest;
    
    travelForms: Attachment[];
    invitations: Attachment[];

    closedReason: string;
    closeStatus: string;

    reviewers: SelectItem[];
    statuses: SelectItem[];

    constructor(
        private eventService: EventRequestService,
        private userService: UserService,
        private attachmentService: AttachmentService,
        private router: Router,
        private adminService: AdminService) {

        this.adminService.getReviewers().then(response => {
            this.reviewers = [];
            this.reviewers.push({ label: 'Unassigned', value: null });

            response.forEach(x => {
                this.reviewers.push({ label: x.displayName, value: x.upn });
            });
        });
    }

    ngOnInit(): void {
        localStorage.setItem('dirtyOvervide', "0");
        localStorage.setItem('goto', '');
        
        this.statuses = Lookups.EVENT_STATUSES.filter(x => x.value && x.value.indexOf('Closed -') >= 0);
    }

    ngOnChanges(changes: { [propKey: string]: SimpleChange }): void {
        if (changes["eventId"]) {
            if (this.eventId) {
                this.closedReason = "";
                this.eventService.get(this.eventId).then(response => {
                    this.editEvent = response;

                    //var editor = tinymce.get('txtGuidance');
                    //if (editor)
                    //    editor.setContent(this.editEvent.guidanceGiven);

                    this.travelForms = this.editEvent.attachments.filter(x => x.typeOfAttachment == AttachmentTypes.TRAVEL_FORMS);
                    this.invitations = this.editEvent.attachments.filter(x => x.typeOfAttachment == AttachmentTypes.INVIATIONS);
                });
            }
        }
    }

    ngOnDestroy() {
        $("body>#confirm-close").remove();
    }

    getAttachment(id: number) {
        window.open(environment.apiUrl + '/attachment/' + id, '_self');
        //this.attachmentService.get(id).subscribe(data => {
        //    console.log('file acquired?');
        //    console.log(data);
        //    this.downloadFile(data);
        //});
    }

    getOrgTypeDesc(orgFlag: number, other: string) {
        var desc = '';

        desc += ((orgFlag & 1) == 1 ? 'Non-Profit/501(c)(3), ' : '');
        desc += ((orgFlag & 2) == 2 ? 'Media Org., ' : '');
        desc += ((orgFlag & 4) == 4 ? 'Lobbying Org., ' : '');
        desc += ((orgFlag & 8) == 8 ? 'USG Entity, ' : '');
        desc += ((orgFlag & 16) == 16 ? 'Foreign Gov., ' : '');
        desc += other ? other : '';

        desc = desc.trim();

        if (desc.endsWith(','))
            desc = desc.substr(0, desc.length - 1);

        return desc;
    }

    downloadFile(blob: Blob) {
        var url = window.URL.createObjectURL(blob);
        window.open(url);
    }

    keyupHandlerFunction(content: string) {
        this.editEvent.guidanceGiven = content;
    }

    cancel(): void {
        this.close.emit();
    }

    save(close: boolean = false): void {
        this.updated.emit();

        this.eventService.update(this.editEvent).then(response => {
            this.editEvent = response;
            if (close)
                this.close.emit();
        });
    }

    showModal() {
        if (this.editEvent.guidanceGiven) {
            this.closeStatus = EventStatus.APPROVED;
        }
        
        $("#confirm-close").modal("show");
        $("#confirm-close").appendTo("body");
    }

    closeModal() {
        $("#confirm-close").modal("hide");
    }

    touchControl(id: string) {
        $('#' + id).focus();
        $('#' + id).blur();
    }

    validateForm(): boolean {
        var isValid = false;

        this.touchControl('status');
        //this.touchControl('reason');

        var invalid = $(':input[required]:visible.ng-invalid:first');

        if (invalid.length > 0)
            invalid.focus();
        else
            isValid = true;

        return isValid;
    }

    closeRequest() {
        if (this.validateForm()) {
            this.closeModal();

            this.editEvent.closedReason = this.closedReason;
            this.editEvent.status = this.closeStatus;

            this.save(true);
        }
    }

    resendAssignEmail() {
        this.eventService.resendAssignedToEmail(this.eventId);
    }

    canAssign() {
        return this.userService.user.isAdmin && this.editEvent.status.includes('Open');
    }

    canEdit() {
        return (this.userService.user.isAdmin || (this.userService.user.isReviewer && this.editEvent.assignedTo == this.userService.user.displayName)) && this.editEvent.status != EventStatus.DRAFT;
    }

    canClose() {
        return (this.userService.user.isAdmin || (this.userService.user.isReviewer && this.editEvent.assignedTo == this.userService.user.displayName)) && this.editEvent.status.includes('Open');
    }

    isClosed() {
        return this.editEvent.status.includes('Closed');
    }
}
