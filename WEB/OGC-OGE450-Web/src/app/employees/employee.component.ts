import { Helper } from '../common/helper'
import { Component, OnInit, Input, Output, EventEmitter, OnChanges, ViewChild } from '@angular/core';
import { EmployeesService } from './employees.service';
import { Employee } from './employee';
import { OGEForm450 } from '../form/oge-form-450';
import { OGEForm450Service } from '../form/oge-form-450.service';
import { SelectItem } from 'primeng/primeng';
import { Logging } from '../common/logging';
import { Lookups, ReportingStatus } from '../common/constants';
import { Switch } from '../common/switch';

declare var $: any;
declare var tinymce: any;

@Component({
    selector: 'employee',
    templateUrl: './employee.component.html',
    styleUrls: ['./employee.component.css']
})

export class EmployeeComponent implements OnInit {
    @Input() employee: Employee;
    @Output()
    close = new EventEmitter<void>();

    @Output()
    save = new EventEmitter<any>();

    @Output()
    updated = new EventEmitter<void>();

    currentForm: OGEForm450;

    employeeStatuses: SelectItem[];
    filingTypes: SelectItem[];
    editEmp: Employee;

    activeText: string;
    activeIcon: string;
    activeColor: string;
    activeAction: string;

    tempAppointmentDate: Date;
    tempDueDate: Date;

    createFormSwitch: Switch = new Switch();

    isNewEntrant: boolean = true;

    emailText: string;

    constructor(private employeeService: EmployeesService,
        private formService: OGEForm450Service) {
        
    }

    ngOnInit(): void {
        this.filingTypes = Lookups.FILER_TYPES;
        this.employeeStatuses = Lookups.EMPLOYEE_STATUSES;
        console.log(this.employeeStatuses);
    }

    ngOnChanges(): void {
        this.currentForm = null;

        this.createFormSwitch.value = false;
        this.createFormSwitch.onText = "Yes";
        this.createFormSwitch.color = "success";
        this.createFormSwitch.offText = "No";
        this.createFormSwitch.offColor = "default";
        this.createFormSwitch.size = "normal";

        console.log(this.employee);
        
        if (this.employee) {
            
            this.editEmp = this.employeeService.copy(this.employee);

            this.updateNewEntrant();

            this.tempAppointmentDate = Helper.getDate(this.editEmp.appointmentDate);
            
            this.setActiveStatus(this.editEmp);

            if (this.editEmp.currentFormId > 0) {
                this.formService.get(this.editEmp.currentFormId).then(response => {
                    this.currentForm = response;
                });
            }
            else {
                this.currentForm = new OGEForm450();
                this.currentForm.id = 0;

                this.updateDueDate();
            }
        }
    }

    updateDueDate() {
        var newDate = Helper.addDays(this.editEmp.reportingStatus == ReportingStatus.NEW_ENTRANT ? Helper.getDate(this.editEmp.appointmentDate, true) : new Date(), 30);

        this.editEmp.dueDate = Helper.formatDate(newDate);
        this.tempDueDate = Helper.getDate(this.editEmp.dueDate);
    }

    setActiveStatus(emp: Employee) {
        if (emp.inactive) {
            this.activeText = "inactive";
            this.activeColor = "danger";
            this.activeIcon = "remove-circle";
            this.activeAction = "click to activate";
        }
        else {
            this.activeText = "active";
            this.activeColor = "success";
            this.activeIcon = "ok-circle";
            this.activeAction = "click to deactivate";
        }
    }

    onSwitchChange(e) {
        var value = e.currentValue ? true : false;

        this.editEmp.generateForm = value;
    }

    flipActiveStatus(emp: Employee) {
        if (emp.inactive || confirm("By proceeding with this action you will deactivate " + emp.displayName + ".  All pending OGE Form 450s and Extentions will be canceled.  This action cannot be undone except by your system administrator.  To proceed, click OK.")) {
            emp.inactive = !emp.inactive;

            this.employeeService.update(emp).then(response => {
                this.setActiveStatus(emp);
                this.employee = this.editEmp;
                this.updated.emit();
            });
        }
    }

    reportingStatusChange() {
        this.updateDueDate();

        this.updateNewEntrant();
    }

    updateNewEntrant() {
        this.isNewEntrant = this.editEmp.reportingStatus == ReportingStatus.NEW_ENTRANT;
        this.emailText = this.isNewEntrant ? this.editEmp.newEntrantEmailText : this.editEmp.annualEmailText;

        var editor = tinymce.get('txtEmail');
        if (editor)
            editor.setContent(this.emailText);
    }

    cancel() {
        this.close.emit();
    }

    saveClicked() {
        if (this.isValid()) {
            this.editEmp.appointmentDate = Helper.formatDate(this.tempAppointmentDate);

            this.editEmp.dueDate = Helper.formatDate(this.tempDueDate);

            this.employeeService.update(this.editEmp).then(response => {
                this.save.emit(response);
            });
        }
    }

    isValid() {
        var valid = true;

        if (this.editEmp.generateForm) {
            var ddl = $('#ddlDueDate');

            if (!this.tempDueDate) {
                ddl.parent('div').addClass("has-error");
                valid = false;
            } else {
                ddl.parent('div').removeClass("has-error");
            }
        }

        return valid;
    }

    keyUpHandler(content: string) {
        if (this.isNewEntrant)
            this.editEmp.newEntrantEmailText = content;
        else
            this.editEmp.annualEmailText = content;
    }
}

