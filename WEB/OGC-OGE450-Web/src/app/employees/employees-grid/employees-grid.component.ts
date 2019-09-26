import { Router } from '@angular/router';
import { Component, Input, Output, OnInit, OnChanges, SimpleChanges, ViewChild, EventEmitter, SimpleChange } from '@angular/core';
import { SelectItem, DataTable } from 'primeng/primeng';

import { Lookups } from '../../common/constants';
import { Employee } from '../employee';
import { EmployeesService } from '../employees.service';
import { GridOptions, GridPersistence } from '../../common/grid-options';

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

    @Input()
    gridId: string;

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

    public gridState: GridPersistence;

    onSort(e: { field: string, order: number }) {
        this.gridState.setSort(e.field, e.order);
    }

    onPage(e: { first: number, rows: number }) {
        this.gridState.setPage(e.rows, e.first);
    }

    onFilter(e: any) {
        this.gridState.setGridFilters(e.filters);
    }

    constructor(private employeeService: EmployeesService,
        private router: Router,
    ) {
        this.gridState = new GridPersistence();
    }

    ngOnInit(): void {
        this.years = Lookups.YEARS;
        this.reportingStatuses = Lookups.REPORTING_STATUSES;
        this.employeeStatuses = Lookups.EMPLOYEE_STATUSES;
        this.filerTypes = Lookups.FILER_TYPES;
        this.statuses = Lookups.FORM_STATUSES;
        this.divisions = Lookups.DIVISIONS;
    }

    ngOnChanges(changes: { [propKey: string]: SimpleChange }): void {
        if (changes["gridId"])
            this.gridState.key = this.gridId;

        this.loadPersistantGridSettings();

        if (changes["employees"])
            this.filter("");
    }

    loadPersistantGridSettings() {
        this.gridState.load(this.gridId);

        if (this.gridState.gridOptions) {
            setTimeout(() => {
                this.gridState.gridOptions.controls.forEach(x => {
                    if (x.key == "showHidden") {
                        $('#chkHidden').prop('checked', x.value);
                    } else {
                        $('#' + x.id).val(x.value).change();
                    }
                });

                if (this.gridState.gridOptions.filters) {
                    this.dt.filters = this.gridState.gridOptions.filters;
                }

                this.dt.sortField = this.gridState.gridOptions.sortField;
                this.dt.sortOrder = this.gridState.gridOptions.sortOrder;
                this.dt.sortSingle();
                this.dt.first = this.gridState.gridOptions.first;
                this.dt.rows = this.gridState.gridOptions.rows;
                this.dt.paginate();
            }, 0);
        }
    }

    public filter(where: string, reset: boolean = false) {
        if (reset) {
            this.dt.reset();
            $("#ddlDivisions").val('null').change();
            $("#ddlFilerTypes").val('null').change();
            $("#ddlReportingStatus").val('null').change();
            $("#ddlFormStatus").val('null').change();
            $("#dtDateFilter").val('');

            this.gridState.resetControls();
        }

        var showHidden = $('#chkHidden').is(':checked');

        this.gridState.setGridControl("showHidden", showHidden, 'chkHidden');

        this.gridEmployees = this.employees.filter(x => x.inactive == false || showHidden);

        if (where == "Not Assigned") {
            $("#ddlFilerTypes").val('Not Assigned').change();
            this.gridState.setGridControl('filerType', where, 'ddlFilerTypes');
            this.dt.filter(where, 'filerType', 'contains');

            this.gridState.save();
        }
    }

    filterData(e, field, matchMode) {
        var value = e.target.value;
        if (value == 'null') value = null;

        this.gridState.setGridControl(field, value, e.target.id);

        this.dt.filter(value, field, matchMode);
    }
    gotoDetail(emp: Employee): void {
        // Edit Employee Pop up?
        this.editEmployee.emit(emp.id);
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
