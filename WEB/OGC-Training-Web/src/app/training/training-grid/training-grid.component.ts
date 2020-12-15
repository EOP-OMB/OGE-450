import { Router } from '@angular/router';
import { Component, Input, Output, OnInit, OnChanges, SimpleChanges, ViewChild, EventEmitter } from '@angular/core';
import { SelectItem, DataTable } from 'primeng/primeng';
import { Lookups } from '../../common/constants';

import { Training } from '../training.model';
import { TrainingService } from '../training.service';

declare var $: any;

@Component({
    selector: 'training-grid',
    templateUrl: './training-grid.component.html',
    styleUrls: ['./training-grid.component.css']
})

export class TrainingGridComponent implements OnInit {
    @ViewChild(('dt')) dt: DataTable;

    @Input()
    showFilters: boolean;

    @Input()
    showPages: boolean;

    @Input()
    trainings: Training[];

    @Output()
    updated = new EventEmitter<any>();

    @Output()
    trainingClick = new EventEmitter<Training>();

    gridTraining: Training[];

    years: SelectItem[];
    divisions: SelectItem[];
    trainingTypes: SelectItem[];
    
    constructor(private trainingService: TrainingService,
        private router: Router,
    ) {
        this.years = Lookups.YEARS;
        this.divisions = Lookups.DIVISIONS;
        this.trainingTypes = Lookups.TRAINING_TYPES;
    }

    ngOnInit(): void {
        
    }

    ngOnChanges(): void {
        if (this.trainings)
            this.filter("");
    }

    public filter(where: string) {
        var showHidden = $('#chkHidden').is(':checked');

        this.gridTraining = this.trainings.filter(x => x.inactive == false || showHidden);
    }

    gotoDetail(tra: Training): void {
        this.trainingClick.emit(tra);
    }

    filterData(e, field, matchMode) {
        var value = e.target.value;

        if (value == 'null') value = null;

        this.dt.filter(value, field, matchMode);
    }
}
