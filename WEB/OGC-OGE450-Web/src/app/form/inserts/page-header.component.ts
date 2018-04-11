import { Component, Input} from '@angular/core';
import { Settings } from '../../admin/settings/settings';

@Component({
    selector: 'print-page-header',
    template: `
        <div id="pageHeader">
            <div class="row">
                <div class="col-xs-8">
                    OGE Form 450, 5 CFR Part 2634, Subpart I<br />
                    U.S. Office of Government Ethics ({{ settings.formVersion }})<br />
                    (Replaces {{ settings.replacesVersion }} edition)
                </div>
                <div class="col-xs-4" style="text-align: right;">
                    <br />
                    Form Approved<br />
                    OMB No. 3209-0006
                </div>
            </div>
        </div>
        <div class="row">
            <div class="outer-form-box">
                <div class="col-xs-10 form-box">
                    <div class="control-label">Employee's Name (Print last, first, middle initial)</div>
                    <input type="text" class="form-control" value="{{ employeesName }}" />
                </div>
                <div class="col-xs-2 form-box">
                    <div class="control-label">Page Number</div>
                    <input id="pageNumber" type="text" class="form-control"  />
                </div>
            </div>
        </div>
    `,
    styleUrls: ['../form.component.css']
})

export class PageHeaderComponent  {
    @Input()
    employeesName: string;

    @Input()
    settings: Settings;

    constructor() {
    }
}