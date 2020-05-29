import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router, CanDeactivate } from '@angular/router';
import { OGEForm450 } from '../form/oge-form-450';
import { OGEForm450Service } from '../form/oge-form-450.service';
import { FormComponent } from '../form/form.component';
import { UserService } from '../user/user.service';
import { SettingsService } from '../admin/settings/settings.service';
import { FormStatus } from '../common/constants';
import { Settings } from '../admin/settings/settings';

declare var $: any;

@Component({
    selector: 'app-form-view',
    templateUrl: './form-view.component.html',
    styleUrls: ['./form-view.component.css']
})
export class FormViewComponent implements OnInit {
    form: OGEForm450;
    prevForm: OGEForm450;
    settings: Settings;
    mode: string = "EDIT";

    @ViewChild('ogeForm450')
    ogeForm450: FormComponent

    constructor(
        private userService: UserService,
        private settingsService: SettingsService,
        private formService: OGEForm450Service,
        private route: ActivatedRoute,
        private router: Router
    ) {
        this.settingsService.get().then(response => {
            this.settings = response;
        });
    }

    ngOnInit(): void {
        this.route.data.subscribe((data: { form: OGEForm450, prev: OGEForm450 }) => {
            this.form = data.form;
            this.prevForm = data.prev;

            if ((this.userService.user.isReviewer || this.userService.user.isAdmin) && (this.form.formStatus == FormStatus.SUBMITTED || this.form.formStatus == FormStatus.RE_SUBMITTED || this.form.formStatus == FormStatus.CERTIFIED)) {
                this.mode = "REVIEWER";
            }
            localStorage.setItem('dirtyOvervide', "0");
            localStorage.setItem('goto', '');
        });
    }

    ngAfterViewInit() {
        if (this.form.formStatus == FormStatus.NOT_STARTED && this.form.filer == this.userService.user.upn)
            $('#intro-popup').modal();
    }
    
    saveForm(form: OGEForm450) {
        //console.log(this.tempAppointmentDate);
        //this.form.dateOfAppointment = this.getAppointmentDate();
        //console.log(this.form.dateOfAppointment);
        this.formService.update(form)
            .then(response => {
                this.form = new OGEForm450();
                this.form = JSON.parse(JSON.stringify(form));;

                this.userService.user.currentFormStatus = response.formStatus;

                if (form.closeAfterSaving) {
                    this.confirmClose();
                }
                else {
                    $("#success-alert").alert();

                    $("#success-alert").fadeTo(2000, 500).slideUp(500, function () {
                        $("#success-alert").slideUp(500);
                    });
                }
            });
    }

    closeForm() {
        if (this.ogeForm450.canSave() && this.ogeForm450.isDirty())
            $('#confirm-close').modal();
        else
            this.confirmClose();
    }

    saveAndClose() {
        this.ogeForm450.save(false, true);
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

    compareForms() {
        $('#compare-modal').modal();
    }
}

export class PreventUnsavedChangesGuard implements CanDeactivate<FormViewComponent> {
    canDeactivate(component: FormViewComponent) {
        var override = localStorage.getItem('dirtyOvervide') ? localStorage.getItem('dirtyOvervide') == "1" : false;

        if (component.ogeForm450 && component.ogeForm450.canSave() && !override && component.ogeForm450.isDirty()) {
            $('#confirm-close').modal();
            return false;
        }

        return true;
    }
}
