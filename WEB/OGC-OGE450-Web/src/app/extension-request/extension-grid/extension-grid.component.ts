import { Router } from '@angular/router';
import { Component, Input, Output, OnInit, OnChanges, SimpleChanges, ViewChild, EventEmitter, SimpleChange } from '@angular/core';
import { SelectItem, DataTable } from 'primeng/primeng';
import { ExtensionStatus, Lookups } from '../../common/constants';

import { ExtensionRequest } from '../extension-request';
import { ExtensionRequestService } from '../extension-request.service';
import { GridPersistence } from '../../common/grid-options';

declare var $: any;

@Component({
    selector: 'extension-grid',
    templateUrl: './extension-grid.component.html',
    styleUrls: ['./extension-grid.component.css']
})

export class ExtensionGridComponent implements OnInit {
    @ViewChild(('dt')) dt: DataTable;

    @Input()
    extensions: ExtensionRequest[];

    @Input()
    gridId: string;

    @Output()
    updated = new EventEmitter<any>();

    gridExtensions: ExtensionRequest[];

    years: SelectItem[];
    extensionStatuses: SelectItem[];
    filingTypes: SelectItem[];
    
    pending: string = ExtensionStatus.PENDING;

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

    constructor(private extensionService: ExtensionRequestService,
        private router: Router,
    ) {
        this.gridState = new GridPersistence();
    }

    ngOnInit(): void {
        this.years = Lookups.YEARS;
        this.extensionStatuses = Lookups.EXTENSION_STATUSES;
        this.filingTypes = Lookups.FILER_TYPES;
    }

    ngOnChanges(changes: { [propKey: string]: SimpleChange }): void {
        if (changes["gridId"])
            this.gridState.key = this.gridId;

        this.loadPersistantGridSettings();

        if (changes["extensions"])
            this.filter("");
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

    public filter(where: string) {
        
    }

    gotoDetail(ext: ExtensionRequest): void {
        if (ext.status == ExtensionStatus.PENDING)
            this.router.navigate(['/extension/' + ext.ogeForm450Id]);
    }

    filterData(e, field, matchMode) {
        var value = e.target.value;

        if (value == 'null') value = null;

        this.gridState.setGridControl(field, value, e.target.id);
        this.dt.filter(value, field, matchMode);
    }
}
