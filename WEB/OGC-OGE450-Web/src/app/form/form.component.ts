import { Helper } from '../common/helper'
import { environment } from '../../environments/environment'
import { Component, Input, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params, CanDeactivate } from '@angular/router';
import { FormStatus, ReportingStatus } from '../common/constants';

import 'rxjs/add/operator/switchMap';

import { OGEForm450Service } from './oge-form-450.service';
import { OGEForm450 } from './oge-form-450';
import { Settings } from '../admin/settings/settings';
import { ReportableInformation } from './reportable-information';
import { IntroComponent } from './intro/intro.component';
import { UserService } from '../user/user.service';
import { SettingsService } from '../admin/settings/settings.service';
import { Roles } from '../security/roles';

declare var $: any;

@Component({
    selector: 'oge-form-450',
    templateUrl: './form.component.html',
    styleUrls: ['./form.component.css']
})

export class FormComponent implements OnInit, AfterViewInit {
    @Input() form: OGEForm450;
    origForm: OGEForm450;

    settings: Settings = new Settings();
    printView: boolean = false;
    isReviewer: boolean;
    isAdmin: boolean;

    assetsAndIncome: ReportableInformation[];
    liabilities: ReportableInformation[];
    outsidePositions: ReportableInformation[];
    agreementsOrArrangements: ReportableInformation[];
    giftsOrTravelReimbursements: ReportableInformation[];

    tempAppointmentDate: Date;

    constructor(
        private formService: OGEForm450Service,
        private route: ActivatedRoute,
        private router: Router,
        private userService: UserService,
        private settingsService: SettingsService,
    ) {
        this.settingsService.get().then(response => {
            this.settings = response;
        });

        this.isReviewer = this.userService.isInRole(Roles.Reviewer);
        this.isAdmin = this.userService.isInRole(Roles.Admin);
    }

    ngOnInit(): void {
        this.route.data.subscribe((data: { form: OGEForm450 }) => {
            this.initialize(data.form);
            localStorage.setItem('dirtyOvervide', "0");
            localStorage.setItem('goto', '');
        });

        $("a[href^='#']").on('click', function (e) {
            // prevent default anchor click behavior
            e.preventDefault();
            // store hash
            var hash = this.hash;

            if (!$(hash).is(":visible")) {
                hash = "#steps";
                $("#section-off-alert").alert();
                $("#section-off-alert").fadeTo(5000, 500).slideUp(500, function () {
                    $("#section-off-alert").slideUp(500);
                });
                
            }

            scroll(hash);
        });

        function scroll(hash: string): void {
            // animate
            $('html, body').animate({
                scrollTop: $(hash).offset().top - 222
            }, 500, function () {

            });
        }
    }

    initialize(form: OGEForm450) {
        this.form = form;

        this.initializePopovers();
        this.assetsAndIncome = this.form.reportableInformationList.filter(x => x.infoType === "AssetsAndIncome");
        this.liabilities = this.form.reportableInformationList.filter(x => x.infoType === "Liabilities");
        this.outsidePositions = this.form.reportableInformationList.filter(x => x.infoType === "OutsidePositions");
        this.agreementsOrArrangements = this.form.reportableInformationList.filter(x => x.infoType === "AgreementsOrArrangements");
        this.giftsOrTravelReimbursements = this.form.reportableInformationList.filter(x => x.infoType === "GiftsOrTravelReimbursements");

        this.origForm = JSON.parse(JSON.stringify(this.form));
        this.tempAppointmentDate = Helper.getDate(this.form.dateOfAppointment);
        if (this.form.reportingStatus == ReportingStatus.NEW_ENTRANT) {
            $('#dtDateOfAppointment').show();
            $('#dtNoDate').hide();
        }
        else {
            $('#dtDateOfAppointment').hide();
            $('#dtNoDate').show();
        }
    }

    ngAfterViewInit() {
        if (this.form.formStatus == FormStatus.NOT_STARTED && this.form.filer == this.userService.user.upn)
            $('#intro-popup').modal();
        else if (this.form.submittedPaperCopy && (this.form.formStatus == FormStatus.SUBMITTED || this.form.formStatus == FormStatus.RE_SUBMITTED || this.form.formStatus == FormStatus.CERTIFIED))
        {
            $('#watermark').show();
            $('#steps').hide();
        }
        else
            this.disableForm();

        var headerHtml = $('#pageHeader').html();
        $('.header').html(headerHtml);
    }

    scroll(hash: string): void {
        // animate
        $('html, body').animate({
            scrollTop: $(hash).offset().top - 185
        }, 500, function () {

        });
    }

    addRow(infoType: string): void {
        var newRow = new ReportableInformation();

        newRow.infoType = infoType;
        newRow.name = "";
        newRow.additionalInfo = "";
        newRow.description = "";
        newRow.noLongerHeld = false;
        switch (infoType) {
            case "AssetsAndIncome":
                this.assetsAndIncome.push(newRow);
                break;
            case "Liabilities":
                this.liabilities.push(newRow);
                break;
            case "OutsidePositions":
                this.outsidePositions.push(newRow);
                break;
            case "AgreementsOrArrangements":
                this.agreementsOrArrangements.push(newRow);
                break;
            case "GiftsOrTravelReimbursements":
                this.giftsOrTravelReimbursements.push(newRow);
                break;
        }

        this.form.reportableInformationList.push(newRow);
    };

    disableForm(): void {
        if (!this.canSave()) {
            $('input,textarea').prop('disabled', true);
            $('.add-row').hide();
        }

        // Hide reviewer section until certified or if it's returned to user
        if (!(this.isReviewer || this.isCertified() || this.isMissingInformation()))
            $('#sectionR').hide();

        if (!this.isReviewer)
            $('#txtReviewerComments').prop('disabled', true);
        else if (!this.isCertified())
            $('#txtReviewerComments').prop('disabled', false);
    }

    initializePopovers(): void {
        $('#lnkPartI').popover({ container: 'body', html: true, content: function () { return $('#partI_Instructions').html(); } });
        $('#lnkPartII').popover({ container: 'body', html: true, content: function () { return $('#partII_Instructions').html(); } });
        $('#lnkPartIII').popover({ container: 'body', html: true, content: function () { return $('#partIII_Instructions').html(); } });
        $('#lnkPartIV').popover({ container: 'body', html: true, content: function () { return $('#partIV_Instructions').html(); } });
        $('#lnkPartV').popover({ container: 'body', html: true, content: function () { return $('#partV_Instructions').html(); } });

        $('#lnkPartIExample').popover({ container: 'body', html: true, content: function () { return $('#partIExample').html(); } });
        $('#lnkPartIIExample').popover({ container: 'body', html: true, content: function () { return $('#partIIExample').html(); } });
        $('#lnkPartIIIExample').popover({ container: 'body', html: true, content: function () { return $('#partIIIExample').html(); } });
        $('#lnkPartIVExample').popover({ container: 'body', html: true, content: function () { return $('#partIVExample').html(); } });
        $('#lnkPartVExample').popover({ container: 'body', html: true, content: function () { return $('#partVExample').html(); } });

        $('#partI_Examples').html($('#partIExample').html());
        $('#partII_Examples').html($('#partIIExample').html());
        $('#partIII_Examples').html($('#partIIIExample').html());
        $('#partIV_Examples').html($('#partIVExample').html());
        $('#partV_Examples').html($('#partVExample').html());
    };

    public isDirty(): boolean {

        return JSON.stringify(this.origForm) != JSON.stringify(this.form);
    }

    goBack(): void {
        if (this.canSave() && this.isDirty())
            $('#confirm-close').modal();
        else
            this.confirmClose();
    }

    saveAndClose() {
        this.save(false, true);
    }

    confirmClose() {
        localStorage.setItem('dirtyOvervide', "1");
        $('#confirm-close').modal('hide');

        // Check to see if we were trying to go somewhere other than previous, if not go back to previous
        var prev = localStorage.getItem('goto') ? localStorage.getItem('goto') : '';

        if (prev == '') {
            prev = localStorage.getItem('prev') ? localStorage.getItem('prev') : '/home';
            if (prev.includes('form'))
                prev = '/';
        }

        this.router.navigate([prev]);
    }

    print(): void {
       window.print();
    }

    //getAppointmentDate(): string {
    //    var dt = $('#dtDateOfAppointment');

    //    this.tempAppointmentDate = Helper.getDate(dt.val());

    //    return Helper.formatDate(this.tempAppointmentDate);
    //}

    save(submitting: boolean, close: boolean = false): void {
        if (this.canSave() || submitting) {
            //console.log(this.tempAppointmentDate);
            //this.form.dateOfAppointment = this.getAppointmentDate();
            //console.log(this.form.dateOfAppointment);
            this.formService.update(this.form)
                .then(response => {
                    this.userService.user.currentFormStatus = response.formStatus;

                    if (close) {
                        this.confirmClose();
                    }
                    else {
                        this.initialize(response);

                        $("#success-alert").alert();

                        $("#success-alert").fadeTo(2000, 500).slideUp(500, function () {
                            $("#success-alert").slideUp(500);
                        });
                    }
                });
        }
    }

    signClicked() {
        var valid = this.validateForm();

        if (valid && this.canSubmit()) {
            $("#confirm-sign").modal();
        }
        else {
            $("#invalid-alert").alert();
            $("#invalid-alert").fadeTo(5000, 500).slideUp(500, function () {
                $("#invalid-alert").slideUp(500);
            });

            this.scroll('#oge-form');
        }
    }

    submit(): void {
        // Filer sign & submit button
        var valid = this.validateForm();

        if (valid && this.canSubmit()) {
            this.form.employeeSignature = this.userService.user.displayName;
            this.form.dateOfEmployeeSignature = new Date().toDateString();
            this.form.formStatus = this.form.formStatus == FormStatus.MISSING_INFORMATION ? FormStatus.RE_SUBMITTED : FormStatus.SUBMITTED;
            this.save(true, true);
            this.disableForm();
        }
        else {
            $("#invalid-alert").alert();
            $("#invalid-alert").fadeTo(5000, 500).slideUp(500, function () {
                $("#invalid-alert").slideUp(500);
            });

            this.scroll('#oge-form');
        }
    }

    cancel(): void {
        this.form.submittedPaperCopy = false;
    }

    validateForm(): boolean {
        var divs = $('.required');

        var valid = true;

        divs.each(function () {
            var input = $(this).next(':input');

            if (input.val() == "") {
                $(this).parent('div').addClass("has-error");
                valid = false;
            } else {
                $(this).parent('div').removeClass("has-error");
            }
        });

        return valid;
    }

    certify(): void {
        if (this.canCertify()) {
            this.form.reviewingOfficialSignature = this.userService.user.displayName;
            this.save(false, true);
        }
    }

    certifyPaper(): void {
        this.form.reviewingOfficialSignature = this.userService.user.displayName;
        this.form.commentsOfReviewingOfficial = "Certification based on filer’s paper submission.\n" + this.form.commentsOfReviewingOfficial;
        this.form.submittedPaperCopy = true;
        this.save(true, true);
    }

    reject(): void {
        if (this.canCertify()) {
            this.form.isRejected = true;
            this.save(false, true);
        }
    }

    canSubmit(): boolean {
        var ret = (this.form.formStatus == FormStatus.NOT_STARTED || this.form.formStatus == FormStatus.DRAFT || this.form.formStatus == FormStatus.MISSING_INFORMATION) && this.form.filer == this.userService.user.upn;

        return ret;
    }

    isSubmitted(): boolean {
        return (this.form.formStatus == FormStatus.SUBMITTED || this.form.formStatus == FormStatus.RE_SUBMITTED);
    }

    canCertify(): boolean {
        return this.isSubmitted() && ((this.isReviewer && (this.form.filer != this.userService.user.upn || environment.debug)) || this.isAdmin) ;
    }

    canSubmitPaper(): boolean {
        return (this.isReviewer || this.isAdmin) && !this.isSubmitted() && !this.isCertified();
    }

    isCertified(): boolean {
        return (this.form.formStatus == FormStatus.CERTIFIED);
    }

    isMissingInformation(): boolean {
        return (this.form.formStatus == FormStatus.MISSING_INFORMATION);
    }

    canSave(): boolean {
        return this.canSubmit() || this.canCertify();
    }

    canEdit(): boolean {
        var ret = this.canCertify() || !(this.form.formStatus == FormStatus.SUBMITTED || this.form.formStatus == FormStatus.RE_SUBMITTED || this.form.formStatus == FormStatus.CERTIFIED);

        return ret;
    }
}

export class PreventUnsavedChangesGuard implements CanDeactivate<FormComponent> {
    canDeactivate(component: FormComponent) {
        var override = localStorage.getItem('dirtyOvervide') ? localStorage.getItem('dirtyOvervide') == "1" : false;
        
        if (component.canSave() && !override && component.isDirty()) {
            $('#confirm-close').modal();
            return false;
        }

        return true;
    }
}