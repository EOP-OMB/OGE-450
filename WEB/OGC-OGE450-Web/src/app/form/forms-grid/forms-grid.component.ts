import { Router } from '@angular/router';
import { Component, Input, OnInit, OnChanges, SimpleChanges, ViewChild, Output, EventEmitter } from '@angular/core';
import { SelectItem, DataTable } from 'primeng/primeng';

import { Switch } from '../../common/switch';
import { Lookups, FormStatus, FormFlags } from '../../common/constants';
import { OGEForm450 } from '../oge-form-450';

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

    @Output()
    onEdit = new EventEmitter<any>();

    gridForms: OGEForm450[];
    date: Date;

    years: SelectItem[];
    reportingStatuses: SelectItem[];
    statuses: SelectItem[];
    formFlags: SelectItem[];

    overdueFilter: Switch = new Switch();

    constructor(private router: Router,
    ) {
        this.overdueFilter.value = false;
        this.overdueFilter.onText = "Overdue";
        this.overdueFilter.color = "danger";
        this.overdueFilter.offText = "All";
        this.overdueFilter.offColor = "default";
        this.overdueFilter.size = "normal";
    }

    ngOnInit(): void {
        this.years = Lookups.YEARS;
        this.reportingStatuses = Lookups.REPORTING_STATUSES;
        this.statuses = Lookups.FORM_STATUSES;
        this.formFlags = Lookups.FORM_FLAGS;
    }

    public filter(where: string) {
        if (where == "Submitted") {
            $("#ddlStatus").val('Submitted').change();
            this.dt.filter(where, 'formStatus', 'contains');
        }
        else if (where == "Overdue") {
            this.overdueFilter.value = true;
            $("#ddlFlags").val('Overdue').change();
            this.dt.filter(where, 'formFlags', 'contains');
        }
    }

    gotoDetail(id: number): void {
        this.router.navigate(['/form', id]);
    }

    filterData(e, field, matchMode) {
        var value = e.target.value;

        if (value == 'null') value = null;
        
        this.dt.filter(value, field, matchMode);
    }

    onSwitchChange(e) {
        var value = e.currentValue ? true : null;
        
        this.dt.filter(value, 'isOverdue', 'equals');
    }

    isOverdue(form: OGEForm450): boolean {
        var today = new Date();
        
        return (new Date(form.dueDate) < today && form.formStatus != FormStatus.CERTIFIED && form.formStatus != FormStatus.CANCELED);
    }

    rowClick(form: OGEForm450) {
        this.onEdit.emit(form);
    }
}