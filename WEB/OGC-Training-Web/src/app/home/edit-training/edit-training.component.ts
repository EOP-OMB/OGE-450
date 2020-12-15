import { Helper } from '../../common/helper'
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BadRequestResponse } from '../../common/bad-request-response.model';

import { TrainingService } from '../../training/training.service';
import { Training } from '../../training/training.model';

import { SelectItem } from 'primeng/primeng';
import { Lookups } from '../../common/constants';

declare var $: any;

@Component({
    selector: 'edit-training',
    templateUrl: './edit-training.component.html',
    styleUrls: ['./edit-training.component.css']
})

export class EditTrainingComponent implements OnInit {
    @Input()
    private training: Training;

    @Output()
    close = new EventEmitter<any>();

    tempTrainingDateAndTime: Date;
    minDate: Date;
    maxDate: Date;

    errorMessage: string;
    
    private divisions: SelectItem[];

    ngOnInit(): void {

    }

    constructor(private trainingService: TrainingService) {
        this.divisions = Lookups.DIVISIONS;
    }

    ngOnChanges(): void {
        if (this.training) {
            this.tempTrainingDateAndTime = Helper.getDate(this.training.dateAndTime);

            this.maxDate = new Date();
        }
    }

    saveClicked() {
        this.training.year = this.tempTrainingDateAndTime.getFullYear();
        this.training.title = this.training.year.toString() + ' ' + this.training.trainingType + ' Ethics Training';

        if (this.isValid()) {
            $('#invalid').hide();
            $('#badRequest').hide();
            this.training.dateAndTime = Helper.formatDate(this.tempTrainingDateAndTime, true);

            this.trainingService.save(this.training).then(() => {
                this.close.emit(true);
            })
                .catch(reason => {
                    var br = JSON.parse(reason.text());
                    this.errorMessage = br.message;
                    $('#badRequest').show();
            });
        }
        else
            $('#invalid').show();
    }

    cancel() {
        this.close.emit();
    }

    isValid() {
        var valid = true;

        var divs = $('.required');

        var valid = true;

        divs.each(function () {
            var input = $(this).next(':input');

            if (input.val() == "") {
                //input.addClass("invalid");
                $(this).parent('div').addClass("has-error");
                valid = false;
            } else {
                //input.removeClass("invalid");
                $(this).parent('div').removeClass("has-error");
            }
        });

        var ddl = $('#ddlDateOfTraining');

        if (!this.tempTrainingDateAndTime) {
            ddl.parent('div').addClass("has-error");
            valid = false;
        } else {
            ddl.parent('div').removeClass("has-error");
        }
        
        return valid;
    }
}
