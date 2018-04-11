import { Router } from '@angular/router';
import { Component, Input, Output, OnInit, OnChanges, SimpleChanges, ViewChild, EventEmitter } from '@angular/core';
import { SelectItem, DataTable } from 'primeng/primeng';
import { ExtensionStatus, Lookups } from '../../common/constants';

import { ExtensionRequest } from '../extension-request';
import { ExtensionRequestService } from '../extension-request.service';

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

    @Output()
    updated = new EventEmitter<any>();

    gridExtensions: ExtensionRequest[];

    years: SelectItem[];
    extensionStatuses: SelectItem[];
    filingTypes: SelectItem[];
    

    pending: string = ExtensionStatus.PENDING;

    constructor(private extensionService: ExtensionRequestService,
        private router: Router,
    ) {

    }

    ngOnInit(): void {
        this.years = Lookups.YEARS;
        this.extensionStatuses = Lookups.EXTENSION_STATUSES;
        this.filingTypes = Lookups.FILER_TYPES;
    }

    ngOnChanges(): void {
        if (this.extensions)
            this.filter("");
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

        this.dt.filter(value, field, matchMode);
    }
}