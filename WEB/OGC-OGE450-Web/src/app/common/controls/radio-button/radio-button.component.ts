import { Component, OnInit, forwardRef, Input } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { ControlBaseComponent } from '../control-base/control-base.component';

@Component({
    selector: 'app-radio-button',
    templateUrl: './radio-button.component.html',
    styleUrls: ['./radio-button.component.css', '../control-base/control-base.component.css'],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => RadioButtonComponent),
            multi: true
        }
    ]
})
export class RadioButtonComponent extends ControlBaseComponent implements OnInit {

    @Input()
    selectedValue: any;

    constructor() {
        super();
    }

    ngOnInit() {
    }

}
