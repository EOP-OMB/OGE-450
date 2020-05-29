import { Component, OnInit, Input } from '@angular/core';
import { OGEForm450 } from '../../form/oge-form-450';
import { Settings } from '../../admin/settings/settings';

@Component({
    selector: 'app-form-compare',
    templateUrl: './form-compare.component.html',
    styleUrls: ['./form-compare.component.css']
})
export class FormCompareComponent implements OnInit {

    @Input()
    form: OGEForm450;

    @Input()
    prevForm: OGEForm450;

    @Input()
    settings: Settings;

    constructor() { }

    ngOnInit() {
    }
}
