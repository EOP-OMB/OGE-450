import { Helper } from '../../common/helper'
import { Component, Input, Output, OnInit, EventEmitter, OnChanges } from '@angular/core';
import { OGEForm450Service } from '../../form/oge-form-450.service';
import { OGEForm450 } from '../../form/oge-form-450';

@Component({
    selector: 'form-mini',
    templateUrl: './form-mini.component.html',
    styleUrls: ['./form-mini.component.css']
})

export class FormMiniComponent implements OnInit {
    private tempDueDate: Date = null;

    @Input()
    form: OGEForm450;

    @Output()
    close = new EventEmitter<any>();

    constructor(private formService: OGEForm450Service) { }

    ngOnInit(): void {
        
    }

    ngOnChanges(): void {
        if (this.form)
            this.tempDueDate = Helper.getDate(this.form.dueDate);
    }

    saveClicked() {
        this.form.dueDate = Helper.formatDate(this.tempDueDate);
        this.form.isOverdue = this.tempDueDate < new Date();
        this.formService.update(this.form).then(() => {
            this.close.emit(true);
        });
    }

    cancel() {
        this.close.emit();
    }
}

