import { Component, OnInit, forwardRef, Input } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { ControlBaseComponent } from '../control-base/control-base.component';

@Component({
    selector: 'app-short-text',
    templateUrl: './short-text.component.html',
    styleUrls: ['./short-text.component.css', '../control-base/control-base.component.css'],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => ShortTextComponent),
            multi: true
        }
    ]
})
export class ShortTextComponent extends ControlBaseComponent implements OnInit {

    @Input()
    type: string = "text";

    constructor() {
        super();
    }

    ngOnInit() {

    }
}
