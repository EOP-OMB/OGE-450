import { Router } from '@angular/router';
import { Component, Input, OnInit, OnChanges, SimpleChanges, ViewChild, Output, EventEmitter, SimpleChange } from '@angular/core';
import { SelectItem, DataTable } from 'primeng/primeng';

import { Helper } from '../../common/helper';
import { Lookups, EventStatus, EventDateFilter } from '../../common/constants';
import { EventRequest } from '../event-request.model';

import { AdminService } from '../../admin/admin.service';
import { AppUser } from '../../security/app-user';

declare var $: any;

@Component({
    selector: 'event-grid',
    templateUrl: './event-grid.component.html',
    styleUrls: ['./event-grid.component.css']
})

export class EventGridComponent implements OnInit {
    @ViewChild(('dt')) dt: DataTable;

    @Input()
    eventRequests: EventRequest[];

    @Input()
    includeSubmitted: boolean;

    selectedEvents: EventRequest[];

    @Input()
    edit: boolean = false;

    @Output()
    onEdit = new EventEmitter<any>();

    date: Date;

    //years: SelectItem[];
    dateFilters: SelectItem[];
    statuses: SelectItem[];
    reviewers: SelectItem[];

    //overdueFilter: Switch = new Switch();

    constructor(private router: Router,
        private adminService: AdminService,
    ) {
        this.reviewers = [];
        this.reviewers.push({ label: 'All', value: '' });

        this.adminService.reviewers.forEach(x => {
            this.reviewers.push({ label: x.displayName, value: x.displayName });
        });
    }

    ngOnInit(): void {
        this.statuses = Lookups.EVENT_STATUSES;
        this.dateFilters = Lookups.EVENT_DATE_FILTER;
    }

    ngOnChanges(changes: { [propKey: string]: SimpleChange }): void {
        if (changes["eventRequests"]) {

            if (this.eventRequests) {
                var currTime = new Date().getTime();

                var today = currTime - (currTime % (60 * 24 * 60 * 1000));
                var nextWeek = currTime + 7 * 1000 * 60 * 60 * 24;
                var nextMonth = currTime + 30 * 1000 * 60 * 60 * 24;

                this.eventRequests = this.eventRequests.map(event => {
                    if (event.eventStartDate) {
                        var dt = Helper.getDate(event.eventStartDate.toString());
                        var dv = dt.getTime();
                        if (dv < today)
                            event.dateFilter = EventDateFilter.PAST_EVENTS;
                        else if (dv >= today && dv < nextWeek)
                            event.dateFilter = EventDateFilter.NEXT_WEEK;
                        else if (dv >= nextWeek && dv < nextMonth)
                            event.dateFilter = EventDateFilter.SEVEN_TO_THIRTY;
                        else if (dv >= nextMonth)
                            event.dateFilter = EventDateFilter.MONTH_PLUS;
                        else
                            event.dateFilter = null;
                    }

                    return event;
                });
            }
        }
    }
    
    gotoDetail(id: number): void {
        // this.router.navigate(['/event', id]);
        this.onEdit.emit(id);
    }

    public filterTable(where: string, reset: boolean = false) {
        if (reset) {
            this.dt.reset();
            $("#ddlDates").val('null').change();
            $("#ddlStatus").val('null').change();
            $("#ddlAssignedTo").val('null').change();
        }

        if (where == "Open") {
            $("#ddlStatus").val('Open').change();
            this.dt.filter(where, 'status', 'contains');
        } else if (where == "Upcoming") {
            $("#ddlStatus").val('Open').change();
            $("#ddlDates").val(EventDateFilter.NEXT_WEEK).change();

            this.dt.filter(EventDateFilter.NEXT_WEEK, 'dateFilter', 'contains')
            this.dt.filter('Open', 'status', 'contains');
        }
    }

    filterData(e, field, matchMode) {
        var value = e.target.value;

        if (value == 'null') value = null;

        this.dt.filter(value, field, matchMode);
    }

    handleSelect(e: EventRequest[]) {
        if (e.length > 0)
            this.selectedEvents = e.filter(x => x.status.includes('Open'));
    }
}
