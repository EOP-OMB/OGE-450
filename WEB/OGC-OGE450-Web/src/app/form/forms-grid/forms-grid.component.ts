import { Router } from '@angular/router';
import { Component, Input, OnInit, OnChanges, SimpleChanges, ViewChild, Output, EventEmitter, SimpleChange } from '@angular/core';
import { SelectItem, DataTable } from 'primeng/primeng';

import { Switch } from '../../common/switch';
import { Lookups, FormStatus, FormFlags } from '../../common/constants';
import { OGEForm450 } from '../oge-form-450';
import { OGEForm450Service } from '../oge-form-450.service';
import { GridPersistence } from '../../common/grid-options';

declare var $: any;

@Component({
    selector: 'forms-grid',
    templateUrl: './forms-grid.component.html',
    styleUrls: ['./forms-grid.component.css']
})

export class FormsGridComponent implements OnInit {
    @ViewChild(('dt')) dt: DataTable;
    
    @Input()
    forms: OGEForm450[];

    @Input()
    edit: boolean = false;

    @Input()
    gridId: string;

    @Output()
    onEdit = new EventEmitter<any>();

    gridForms: OGEForm450[];
    date: Date;

    years: SelectItem[];
    reportingStatuses: SelectItem[];
    statuses: SelectItem[];
    formFlags: SelectItem[];

    overdueFilter: Switch = new Switch();

    selectedForm: OGEForm450;

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
    
    constructor(private router: Router,
        private formService: OGEForm450Service) {

        this.gridState = new GridPersistence();

        this.overdueFilter.value = false;
        this.overdueFilter.onText = "Overdue";
        this.overdueFilter.color = "danger";
        this.overdueFilter.offText = "All";
        this.overdueFilter.offColor = "default";
        this.overdueFilter.size = "normal";
    }

    ngOnInit(): void {
        this.selectedForm = new OGEForm450();

        this.years = Lookups.YEARS;
        this.reportingStatuses = Lookups.REPORTING_STATUSES;
        this.statuses = Lookups.FORM_STATUSES;
        this.formFlags = Lookups.FORM_FLAGS;
    }

    ngOnChanges(changes: { [propKey: string]: SimpleChange }): void {
        if (changes["gridId"])
            this.gridState.key = this.gridId;

        this.loadPersistantGridSettings();
    }

    loadPersistantGridSettings() {
        this.gridState.load(this.gridId);

        if (this.gridState.gridOptions) {
            setTimeout(() => {
                this.gridState.gridOptions.controls.forEach(x => {
                    $('#' + x.id).val(x.value).change();
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
            $("#ddlStatus").val('null').change();
            $("#ddlFlags").val('null').change();
            $("#ddlYear").val('null').change();
            $("#ddlReportingStatus").val('null').change();
            $("#dtDateFilter").val('');

            this.gridState.resetControls();
        }

        if (where == "Submitted") {
            $("#ddlStatus").val('Submitted').change();
            this.gridState.setGridControl('formStatus', where, 'ddlStatus');
            this.dt.filter(where, 'formStatus', 'contains');

            this.gridState.save();
        }
        else if (where == "Overdue") {
            this.overdueFilter.value = true;
            $("#ddlFlags").val('Overdue').change();
            this.gridState.setGridControl('formFlags', where, 'ddlFlags');
            this.dt.filter(where, 'formFlags', 'contains');

            this.gridState.save();
        }
    }

    gotoDetail(id: number): void {
        this.router.navigate(['/form', id]);
    }

    filterData(e, field, matchMode) {
        var value = e.target.value;

        if (value == 'null') value = null;

        this.gridState.setGridControl(field, value, e.target.id);

        this.dt.filter(value, field, matchMode);
    }

    isOverdue(form: OGEForm450): boolean {
        var today = new Date();
        
        return (new Date(form.dueDate) < today && form.formStatus != FormStatus.CERTIFIED && form.formStatus != FormStatus.CANCELED);
    }

    rowClick(form: OGEForm450) {
        this.onEdit.emit(form);
    }

    confirmCancel(form: OGEForm450) {
        this.selectedForm = form;
        $('#confirm-cancel').modal('show');
    }

    cancelForm(form: OGEForm450) {
        form.formStatus = FormStatus.CANCELED;
        this.formService.update(form).then(response => {
            $("#success-alert").alert();

            $("#success-alert").fadeTo(2000, 500).slideUp(500, function () {
                $("#success-alert").slideUp(500);
            });
        });
    }

    canCancel(form: OGEForm450) {
        return form.formStatus != FormStatus.CANCELED && form.formStatus != FormStatus.EXPIRED && form.formStatus != FormStatus.CERTIFIED;
    }
}
