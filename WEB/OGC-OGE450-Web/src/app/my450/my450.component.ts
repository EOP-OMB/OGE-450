import { Router } from '@angular/router';
import { Component, OnInit, Input } from '@angular/core';
import { Helper } from '../common/helper'

import { FormVM } from '../form/form-vm';
import { OGEForm450Service } from '../form/oge-form-450.service';
import { UserService } from '../user/user.service';
import { ExtensionRequest } from '../extension-request/extension-request';

import { Widget } from '../common/widget';
import { Settings } from '../admin/settings/settings';
import { SettingsService } from '../admin/settings/settings.service';

declare var $: any;

@Component({
    selector: 'my-450',
    templateUrl: './my450.component.html',
    styleUrls: ['./my450.component.css']
})

export class My450Component implements OnInit {
    vm: FormVM;

    noFilingWidget: Widget = new Widget();
    settings: Settings;

    constructor(private router: Router,
                private formService: OGEForm450Service,
                private settingsService: SettingsService,
                private userService: UserService) {

        this.vm = new FormVM();
        this.loadSettings();
    }

    loadSettings(): Promise<void> {
        return this.settingsService.get()
            .then(response => {
                this.settings = response;
                this.updateSettingsWidget();
            });
    }

    getLatestForm(): void {

        if (this.userService.user && this.userService.user.currentFormId != 0) {
            this.formService
                .get(this.userService.user.currentFormId)
                .then(form => {
                    this.vm.form = form;
                    this.vm.setStatus();
                });
        }
    }

    updateSettingsWidget() {
        if (this.settings) {
            this.noFilingWidget.title = this.settings.currentFilingYear.toString();
            this.noFilingWidget.text = "No OGE 450";
            this.noFilingWidget.actionText = "please await instructions";
            this.noFilingWidget.color = "success";
        }
    }

    ngOnInit(): void {
        this.getLatestForm();
    }

    closeExtensionRequest(evt) {
        if (evt) {
            // close modal
            $('#extension-modal').modal('hide');
        }
    }

    onFilingClick() {
        this.router.navigate(['/form', this.vm.form.id]);
    }

    onDaysClick() {
        this.router.navigate(['/extension', this.vm.form.id]);
    }
}
