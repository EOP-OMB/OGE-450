import { environment } from '../../environments/environment'
import { Component, Input, OnInit, AfterViewInit, ViewChild, Output, EventEmitter, OnChanges } from '@angular/core';
import { FormStatus, ReportingStatus } from '../common/constants';

import 'rxjs/add/operator/switchMap';

import { OGEForm450 } from './oge-form-450';
import { Settings } from '../admin/settings/settings';
import { ReportableInformation } from './reportable-information';
import { UserService } from '../user/user.service';
import { SettingsService } from '../admin/settings/settings.service';
import { Roles } from '../security/roles';
import { Helper } from '../common/helper';

declare var $: any;

@Component({
    selector: 'oge-form-450',
    templateUrl: './form.component.html',
    styleUrls: ['./form.component.css']
})

export class FormComponent implements OnInit, OnChanges, AfterViewInit {
    @Input()
    form: OGEForm450;

    @Input()
    prevForm: OGEForm450;

    compareForm: OGEForm450;

    @Input()
    settings: Settings;

    @Output()
    onSave = new EventEmitter<any>();

    @Output()
    onClose = new EventEmitter<any>();

    @Output()
    onCompare = new EventEmitter<any>();

    @Input()
    mode: string = 'EDIT';  // EDIT - Regular mode for filers, REVIEW - Highlight changes can still edit/review as normal, COMPARE - Side by Side mode, read only

    @ViewChild(('topDiv')) topDiv: any;

    origForm: OGEForm450;

    printView: boolean = false;
    isReviewer: boolean;
    isAdmin: boolean;

    showWatermark: boolean = false;

    assetsAndIncome: ReportableInformation[];
    liabilities: ReportableInformation[];
    outsidePositions: ReportableInformation[];
    agreementsOrArrangements: ReportableInformation[];
    giftsOrTravelReimbursements: ReportableInformation[];

    compareAssets: ReportableInformation[];
    compareLiabilities: ReportableInformation[];
    comparePositions: ReportableInformation[];
    compareArrangements: ReportableInformation[];
    compareGifts: ReportableInformation[];

    tempAppointmentDate: Date;
    tempReviewDate: Date;

    constructor(
        private userService: UserService
    ) {
        this.isReviewer = this.userService.isInRole(Roles.Reviewer);
        this.isAdmin = this.userService.isInRole(Roles.Admin);
    }

    ngOnInit(): void {
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

    ngOnChanges() {
        if (this.mode != "EDIT" && this.prevForm != null)
            this.compareForm = this.prevForm;

        this.tempReviewDate = this.form.dateOfSubstantiveReview;

        this.initializePopovers();
        this.assetsAndIncome = this.form.reportableInformationList.filter(x => x.infoType === "AssetsAndIncome");
        this.liabilities = this.form.reportableInformationList.filter(x => x.infoType === "Liabilities");
        this.outsidePositions = this.form.reportableInformationList.filter(x => x.infoType === "OutsidePositions");
        this.agreementsOrArrangements = this.form.reportableInformationList.filter(x => x.infoType === "AgreementsOrArrangements");
        this.giftsOrTravelReimbursements = this.form.reportableInformationList.filter(x => x.infoType === "GiftsOrTravelReimbursements");

        if (this.compareForm && this.compareForm.reportableInformationList) {
            this.compareAssets = this.compareForm.reportableInformationList.filter(x => x.infoType === "AssetsAndIncome");
            this.compareLiabilities = this.compareForm.reportableInformationList.filter(x => x.infoType === "Liabilities");
            this.comparePositions = this.compareForm.reportableInformationList.filter(x => x.infoType === "OutsidePositions");
            this.compareArrangements = this.compareForm.reportableInformationList.filter(x => x.infoType === "AgreementsOrArrangements");
            this.compareGifts = this.compareForm.reportableInformationList.filter(x => x.infoType === "GiftsOrTravelReimbursements");
        }

        this.origForm = JSON.parse(JSON.stringify(this.form));
        this.tempAppointmentDate = Helper.getDate(this.form.dateOfAppointment);

        this.disableForm();

        if (this.form.submittedPaperCopy && (this.form.formStatus == FormStatus.SUBMITTED || this.form.formStatus == FormStatus.RE_SUBMITTED || this.form.formStatus == FormStatus.CERTIFIED)) {
            this.showWatermark = true;
        }
    }

    ngAfterViewInit() {
        var headerHtml = $('#pageHeader').html();
        $('.header').html(headerHtml);
    }

    public get newEntrant(): string {
        return ReportingStatus.NEW_ENTRANT;
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

    inputDisabled: boolean = false;
    reviewCommentsDisabled: boolean = true;
    showReviewSection: boolean = true;

    disableForm(): void {
        if (!this.canSave()) {
            this.inputDisabled = true;
        }
        if (!this.isReviewer || this.mode == "COMPARE")
            this.reviewCommentsDisabled = true;
        else if (!this.isCertified())
            this.reviewCommentsDisabled = false;

        // Hide reviewer section until certified or if it's returned to user
        if (!(this.isReviewer || this.isCertified() || this.isMissingInformation()))
            this.showReviewSection = false;
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
        this.onClose.emit();
    }


    print(): void {
        window.print();
    }

    save(submitting: boolean, close: boolean = false): void {
        this.form.closeAfterSaving = close;

        if (this.canSave() || submitting || this.canReview()) {
            this.onSave.emit(this.form);
        }
    }

    compare() {
        this.onCompare.emit();
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
            this.form.isSubmitting = true;
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
        var divs = $('#oge-form').find('.required');

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

    reviewComplete(): void {
        if (this.canReview()) {
            this.form.substantiveReviewer = this.userService.user.displayName;
            this.save(false, false);
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
        var ret = (this.form.formStatus == FormStatus.NOT_STARTED || this.form.formStatus == FormStatus.DRAFT || this.form.formStatus == FormStatus.MISSING_INFORMATION) && (this.form.filer == this.userService.user.upn) && this.mode != "COMPARE";

        return ret;
    }

    isSubmitted(): boolean {
        return (this.form.formStatus == FormStatus.SUBMITTED || this.form.formStatus == FormStatus.RE_SUBMITTED);
    }

    canReview(): boolean {
        return !this.canSubmit() && !this.isCertified() && ((this.isReviewer && (this.form.filer != this.userService.user.upn || environment.debug)) || this.isAdmin) && this.mode != "COMPARE";
    }

    canCertify(): boolean {
        return this.isSubmitted() && ((this.isReviewer && (this.form.filer != this.userService.user.upn || environment.debug)) || this.isAdmin) && this.mode != "COMPARE";
    }

    canSubmitPaper(): boolean {
        return (this.isReviewer || this.isAdmin) && !this.isSubmitted() && !this.isCertified() && this.mode != "COMPARE";
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
        var ret = (this.canCertify() || !(this.form.formStatus == FormStatus.SUBMITTED || this.form.formStatus == FormStatus.RE_SUBMITTED || this.form.formStatus == FormStatus.CERTIFIED)) && this.mode != "COMPARE";

        return ret;
    }
}


