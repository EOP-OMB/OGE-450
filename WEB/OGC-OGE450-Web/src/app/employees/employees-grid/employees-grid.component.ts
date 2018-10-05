import { Router } from '@angular/router';
import { Component, Input, Output, OnInit, OnChanges, SimpleChanges, ViewChild, EventEmitter } from '@angular/core';
import { SelectItem, DataTable } from 'primeng/primeng';

import { Lookups } from '../../common/constants';
import { Employee } from '../employee';
import { EmployeesService } from '../employees.service';

declare var $: any;

@Component({
    selector: 'employees-grid',
    templateUrl: './employees-grid.component.html',
    styleUrls: ['./employees-grid.component.css']
})

export class EmployeesGridComponent implements OnInit {
    @ViewChild(('dt')) dt: DataTable;

    @Input()
    employees: Employee[];

    @Output()
    editEmployee = new EventEmitter<number>();

    @Output()
    updated = new EventEmitter<any>();

    gridEmployees: Employee[];

    years: SelectItem[];
    reportingStatuses: SelectItem[];
    statuses: SelectItem[];
    filerTypes: SelectItem[];
    divisions: SelectItem[];
    employeeStatuses: SelectItem[];

    constructor(private employeeService: EmployeesService,
        private router: Router,
    ) {
        
    }

    ngOnInit(): void {
        this.years = Lookups.YEARS;
        this.reportingStatuses = Lookups.REPORTING_STATUSES;
        this.employeeStatuses = Lookups.EMPLOYEE_STATUSES;
        this.filerTypes = Lookups.FILER_TYPES;
        this.statuses = Lookups.FORM_STATUSES;
        this.divisions = Lookups.DIVISIONS;
    }

    ngOnChanges(): void {
        if (this.employees)
            this.filter("");
    }

    public filter(where: string, reset: boolean = false) {
        if (reset) {
            this.dt.reset();
            $("#ddlDivisions").val('null').change();
            $("#ddlFilerTypes").val('null').change();
            $("#ddlReportingStatus").val('null').change();
            $("#ddlFormStatus").val('null').change();
            $("#dtDateFilter").val('');
        }

        var showHidden = $('#chkHidden').is(':checked');

        this.gridEmployees = this.employees.filter(x => x.inactive == false || showHidden);

        if (where == "Not Assigned") {
            $("#ddlFilerTypes").val('Not Assigned').change();
            this.dt.filter(where, 'filerType', 'contains');
        }
    }

    gotoDetail(emp: Employee): void {
        // Edit Employee Pop up?
        this.editEmployee.emit(emp.id);
    }

    filterData(e, field, matchMode) {
        var value = e.target.value;

        if (value == 'null') value = null;

        this.dt.filter(value, field, matchMode);
    }

    ignoreEmployee(emp: Employee): void {
        if (emp.inactive || confirm("By proceeding with this action you will deactivate " + emp.displayName + ".  All pending OGE Form 450s and Extentions will be canceled.  This action cannot be undone except by your system administrator.  To proceed, click OK.")) {
            emp.inactive = !emp.inactive;

            this.employeeService.update(emp).then(response => {
                $("#success-alert").alert();

                $("#success-alert").fadeTo(2000, 500).slideUp(500, function () {
                    $("#success-alert").slideUp(500);
                });

                this.filter("");
                
                this.updated.emit();
            });
        }
    }
}