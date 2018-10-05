import { environment } from '../../environments/environment';
import { Router } from '@angular/router';
import { Component, OnInit, ViewChild } from '@angular/core';
import { TabsComponent } from '../tabs/tabs.component';
import { TabComponent } from '../tabs/tab.component';

import { Widget } from '../common/widget';

import { OGEForm450 } from '../form/oge-form-450';
import { OGEForm450Service } from '../form/oge-form-450.service';
import { Employee } from '../employees/employee';
import { EmployeesService } from '../employees/employees.service';
import { ExtensionRequestService } from '../extension-request/extension-request.service';
import { ExtensionRequest } from '../extension-request/extension-request';
import { Settings } from './settings/settings';
import { SettingsService } from './settings/settings.service';

import { NotificationTemplateService } from './edit-templates/notification-template.service';
import { NotificationTemplate } from './edit-templates/notification-template';

declare var $: any;

@Component({
    selector: 'admin',
    templateUrl: './admin.component.html',
    styleUrls: ['./admin.component.css']
})

export class AdminComponent implements OnInit {
    @ViewChild('tabEmployees') tabEmployees;
    @ViewChild('tabAppSettings') tabAppSettings;
    @ViewChild('tabFormSettings') tabFormSettings;
    @ViewChild('tabNotifications') tabNotifications;
    @ViewChild('tabs') tabs;

    @ViewChild('dtEmployees') dtEmployees;

    forms: OGEForm450[];
    selectedForm: OGEForm450 = null;
    employees: Employee[];
    templates: NotificationTemplate[];

    extensions: ExtensionRequest[];
    settings: Settings;

    filersWidget: Widget = new Widget();
    appSettingsWidget: Widget = new Widget();
    notificationWidget: Widget = new Widget();

    formUrl: string;
    linksUrl: string;
    emailsUrl: string;
    templateUrl: string;
    settingsUrl: string;
    contactsUrl: string;

    employeeToEdit: Employee;
    reload: boolean = false;

    constructor(private formService: OGEForm450Service,
        private employeesService: EmployeesService,
        private extensionService: ExtensionRequestService,
        private settingsService: SettingsService,
        private templateService: NotificationTemplateService,
        private router: Router
    ) {
        this.formUrl = environment.sharePointUrl + 'Lists/OGEForm450/AllItems.aspx';
        this.templateUrl = environment.sharePointUrl + 'Lists/NotificationTemplates/AllItems.aspx';
        this.linksUrl = environment.sharePointUrl + 'Lists/Helpful%20Links/AllItems.aspx';
        this.emailsUrl = environment.sharePointUrl + 'Lists/Notifications/AllItems.aspx';
        this.settingsUrl = environment.sharePointUrl + 'Lists/Settings/AllItems.aspx';
        this.contactsUrl = environment.sharePointUrl + 'Lists/SupportContacts/AllItems.aspx';
    }

    ngOnInit(): void {
        this.loadForms();
        this.loadEmployees();
        this.loadExtensions();
        this.loadSettings();
        this.loadTemplates();
    }

    loadTemplates(): void {
        this.templateService
            .getAll()
            .then(response => {
                this.templates = response;
                this.updateNotificationsWidget();
            });
    }

    loadForms(): Promise<void> {
        return this.formService
            .getAll()
            .then(forms => {
                this.forms = forms;
            });
    }

    loadEmployees(): Promise<void> {
        return this.employeesService
            .getAll()
            .then(employees => {
                this.employees = employees;
                this.updateFilingWidget();
            });
    }

    loadExtensions(): Promise<void> {
        return this.extensionService.getAll()
            .then(response => {
                this.extensions = response;
            });
    }

    loadSettings(): Promise<void> {
        return this.settingsService.get()
            .then(response => {
                this.settings = response;
                this.updateSettingsWidget();
            });
    }
    
    updateFilingWidget() {
        if (this.employees) {
            var newEmps = this.employees.filter(x => x.filerType == "Not Assigned" && x.inactive == false);
            var count = newEmps ? newEmps.length : 0;

            this.filersWidget.title = count.toString();
            this.filersWidget.text = "New Employees";
            this.filersWidget.actionText = "click to review";
            this.filersWidget.color = count >= 25 ? "danger" : count >= 10 ? "warning" : count > 0 ? "primary" : "success";
        }
    }

    updateSettingsWidget() {
        if (this.settings) {
            this.appSettingsWidget.title = this.settings.currentFilingYear.toString();
            this.appSettingsWidget.text = "Current Year";
            this.appSettingsWidget.actionText = "click to change settings";

            var today = new Date();
            
            this.appSettingsWidget.color = today.getFullYear() == this.settings.currentFilingYear ? "primary" : "danger";
        }
    }

    updateNotificationsWidget() {
        if (this.templates) {
            this.notificationWidget.title = this.templates.length.toString();
            this.notificationWidget.text = "Notifications";
            this.notificationWidget.actionText = "click to modify";
            this.notificationWidget.color = "primary";
        }
    }

    gotoDetail(id: number): void {
        this.router.navigate(['/extension', id]);
    }

    onFilersClick() {
        this.tabs.selectTab(this.tabEmployees);
        this.dtEmployees.filter("Not Assigned", true);
    }

    onSettingsClick() {
        this.tabs.selectTab(this.tabAppSettings);
    }

    onEmailsClick() {
        this.tabs.selectTab(this.tabNotifications);
    }

    editEmployee(employeeId: number) {
        this.toggleCard();
        this.employeeToEdit = new Employee();
        this.employeesService.get(employeeId).then(employee => {
            this.employeeToEdit = employee;
        });
    }

    toggleCard() {
        this.employeeToEdit = null;
        $('#employeeCard').toggleClass("flip");
    }

    editForm(form: OGEForm450) {
        this.selectedForm = form;
        $('#edit-form').modal();
    }

    editFormClose(isSaved: boolean) {
        $('#edit-form').modal('hide');
    }

    saveEmployee(emp: Employee) {
        this.loadEmployees().then(() => {
            this.toggleCard();
        });
    }

    updated() {
        this.reload = true;
    }

    closeEmployee() {
        if (this.reload) {
            this.reload = false;
            this.saveEmployee(null);
        }
        else
            this.toggleCard();
    }
}