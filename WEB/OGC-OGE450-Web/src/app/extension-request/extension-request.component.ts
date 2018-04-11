import { environment } from '../../environments/environment'
import { Helper } from '../common/helper'
import { FormStatus, ExtensionStatus } from '../common/constants'

import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
    
import { FormVM } from '../form/form-vm';
import { Widget } from '../common/widget';
import { OGEForm450 } from '../form/oge-form-450';
import { OGEForm450Service } from '../form/oge-form-450.service';

import { ExtensionRequest } from './extension-request';
import { UserService } from '../user/user.service';
import { ExtensionRequestService } from './extension-request.service';

import { Roles } from '../security/roles';

declare var $: any;

@Component({
    selector: 'extension-request',
    templateUrl: './extension-request.component.html',
    styleUrls: ['./extension-request.component.css']
})

export class ExtensionRequestComponent implements OnInit {
    vm: FormVM;
    currentExtension: ExtensionRequest;

    status: string;
    days: number[];
    newDueDate: string;

    message: string;

    daysRemainingWidget: Widget;
    pendingRequestsWidget: Widget;

    isReviewer: boolean = false;
    showComments: boolean = false;
    disableEdits: boolean = false;

    daysRemaining: number;
    successMessage: string;

    constructor(private router: Router,
        private route: ActivatedRoute,
        private formService: OGEForm450Service,
        private userService: UserService,
        private extensionService: ExtensionRequestService,
    ) {
        this.vm = new FormVM();
    }

    ngOnInit(): void {
        this.route.data.subscribe((data: { form: OGEForm450 }) => {
            this.vm.form = data.form;
            this.getExtensions(this.vm.form.id);

            this.newDueDate = this.vm.form.dueDate;

            this.vm.setStatus();

            // Should not be here
            if (this.vm.form.formStatus != FormStatus.NOT_STARTED && this.vm.form.formStatus != FormStatus.DRAFT && this.vm.form.formStatus != FormStatus.MISSING_INFORMATION && this.vm.form.formStatus != FormStatus.SUBMITTED)
                this.close();
        });
    }

    getExtensions(ogeForm450Id: number) {
        this.extensionService.get(ogeForm450Id).then(response => {
            this.currentExtension = response;

            var newDate = Helper.addDays(new Date(this.vm.form.dueDate), this.currentExtension.daysRequested);
            this.newDueDate = Helper.formatDate(newDate);
            this.initialize();
        });
    }

    onDayChange(event: Event) {
        var day = 0;
        var val = (event.target as HTMLSelectElement).value;

        var vals = val.split(':');

        if (vals.length > 0)
            day = +vals[1];

        var newDate = Helper.addDays(new Date(this.vm.form.dueDate), day);

        this.newDueDate = Helper.formatDate(newDate);
    }

    save(): void {
        if (this.validateForm()) {
            this.extensionService.create(this.currentExtension).then(response => {
                this.successMessage = "Extension Request successfully submitted";

                $("#success-alert").alert();

                $("#success-alert").fadeTo(2000, 500).slideUp(500, function () {
                    $("#success-alert").slideUp(500);
                });

                this.currentExtension = response;
                this.initialize();
            });
        }
    }

    validateForm(): boolean {
        var valid = true;

        $("textarea.required").each(function () {
            var input = $(this);
                
            if (input.val() == "") {
                $(this).parent('div').addClass("has-error");
                valid = false;
            } else {
                $(this).parent('div').removeClass("has-error");
            }
        });
        
        
        var selectedDay = $('#ddlDays').val();

        if (!selectedDay)
        {
            $('#ddlDays').parent('div').addClass("has-error");
            valid = false;
        }
        else
            $('#ddlDays').parent('div').removeClass("has-error");

        return valid;
    }

    update(approved: boolean) {
        this.currentExtension.status = approved ? ExtensionStatus.APPROVED : ExtensionStatus.REJECTED;

        this.extensionService.update(this.currentExtension).then(response => {
            this.successMessage = "Extension Request successfully " + this.currentExtension.status;

            $("#success-alert").alert();

            $("#success-alert").fadeTo(2000, 500).slideUp(500, function () {
                $("#success-alert").slideUp(500);
            });

            this.currentExtension = response;
            this.initialize();
        });
    }
    
    close(): void {
        this.router.navigate(['/home']);
    }

    initialize() {
        this.daysRemainingWidget = new Widget();
        this.pendingRequestsWidget = new Widget();

        this.days = new Array<number>(0);

        this.daysRemaining = 90 - this.vm.form.daysExtended;
        var maxDays = this.daysRemaining > 45 ? 45 : this.daysRemaining;

        for (var i = 1; i <= maxDays; i++)
            this.days.push(i);

        var today = new Date();
        var dueDate = new Date(this.vm.form.dueDate);

        if (dueDate < today) {
            this.status = "overdue";
        }
        else {
            var days = Helper.daysBetween(today, dueDate);
            this.status = "due in " + days + " days";
        }

        if (this.currentExtension.id == 0) {
            this.message = 'Your <b>' + this.vm.form.year + '</b> ' + this.vm.form.reportingStatus + ' filing is <b>' + this.status + '</b>.  If you require more time to finish, please choose number of days and provide a reason and your request will be reviewed.  Individual extension requests of up to 45 days will be considered <b>for good cause shown</b>.  Multiple extensions can be requested up to a maximum of 90 days.';
        }
        else if (this.currentExtension.status == 'Pending') {
            this.disableEdits = true;

            if (this.canReview()) {
                this.message = "Please review this request and use the buttons below to accept or reject the request.  Leave a comment to be included in the resulting email to the filer.";
                this.isReviewer = true;
                this.showComments = true;
            }
            else
                this.message = 'Your request for a ' + this.currentExtension.daysRequested + ' day extension has been submitted and is awaiting approval.  If approved, your due date will be updated accordingly.  You will receive an email with the reviewer\'s decision when it is made.';
        }

        if (this.vm.form.daysExtended >= 75)
            this.daysRemainingWidget.color = "danger";
        else if (this.vm.form.daysExtended >= 15)
            this.daysRemainingWidget.color = "warning";
        else
            this.daysRemainingWidget.color = "success";

        this.daysRemainingWidget.text = "Days Remaining";
        this.daysRemainingWidget.actionText = "maximum of 90 days";
        this.daysRemainingWidget.title = this.daysRemaining.toString();

        if (this.currentExtension.id == 0) {
            this.pendingRequestsWidget.color = "success";
            this.pendingRequestsWidget.title = "0";
            this.pendingRequestsWidget.text = "Pending Requests";
            this.pendingRequestsWidget.actionText = "request an extension below";
        }
        else {
            this.pendingRequestsWidget.color = "primary";
            this.pendingRequestsWidget.title = "1";
            this.pendingRequestsWidget.text = "Pending Request";
            this.pendingRequestsWidget.actionText = "see below for details";
        }
    }

    canReview(): boolean {
        return (this.userService.isInRole(Roles.Reviewer) && (this.vm.form.filer != this.userService.user.upn || environment.debug)) || this.userService.isInRole(Roles.Admin);
    }

    canApprove(): boolean {
        return this.isReviewer && this.currentExtension.status == ExtensionStatus.PENDING;
    }
}