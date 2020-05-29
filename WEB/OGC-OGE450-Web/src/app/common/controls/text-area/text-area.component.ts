import { Component, OnInit, Input, forwardRef } from '@angular/core';
import { ControlBaseComponent } from '../control-base/control-base.component';
import { NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
    selector: 'app-text-area',
    templateUrl: './text-area.component.html',
    styleUrls: ['./text-area.component.css', '../control-base/control-base.component.css'],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => TextAreaComponent),
            multi: true
        }
    ]
})
export class TextAreaComponent extends ControlBaseComponent implements OnInit {

    @Input()
    rows: number;

    @Input()
    showLabel: boolean;

    @Input()
    customStyle: string;

    constructor() {
        super();
    }

    ngOnInit() {
    }

}
