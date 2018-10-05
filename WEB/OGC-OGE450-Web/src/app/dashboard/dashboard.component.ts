import { Router } from '@angular/router';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ReviewerDashboard } from '../dashboard/reviewer-dashboard';
import { TabsComponent } from '../tabs/tabs.component';
import { TabComponent } from '../tabs/tab.component';
import { SelectItem } from 'primeng/primeng';
import { FormStatus } from '../common/constants';

import { OGEForm450 } from '../form/oge-form-450';
import { OGEForm450Service } from '../form/oge-form-450.service';

import { ExtensionRequest } from '../extension-request/extension-request';
import { ExtensionRequestService } from '../extension-request/extension-request.service';

import { Widget } from '../common/widget';

@Component({
    selector: 'my-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls:  ['./dashboard.component.css']
})

export class DashboardComponent implements OnInit {
    @ViewChild('dtForms') dtForms;
    @ViewChild('tabExtensions') tabExt;
    @ViewChild('tabForms') tabForms;
    @ViewChild('tabs') tabs;

    private dashboard: ReviewerDashboard;
    forms: OGEForm450[];
    extensions: ExtensionRequest[];

    numberOfBlankForms: number = 0;
    numberOfUnchangedForms: number = 0;
    
    submittedWidget: Widget = new Widget();
    extensionWidget: Widget = new Widget();
    overdueWidget: Widget = new Widget();

    constructor(private formService: OGEForm450Service,
        private extensionService: ExtensionRequestService,
        private router: Router) {
    }

    ngOnInit(): void {
        this.loadForms();

        this.extensionService.getPending()
            .then(response => {
                this.extensions = response;

                this.extensionWidget.title = this.extensions.length.toString();
                this.extensionWidget.text = "Extension Request" + ((this.extensions.length == 1) ? "" : "s");
                this.extensionWidget.actionText = "click to review";

                if (this.extensions.length > 0)
                    this.extensionWidget.color = "warning";
                else
                    this.extensionWidget.color = "success";
            });
    }

    loadForms() {
        this.formService
            .getReviewableForms()
            .then(forms => {
                this.forms = forms;

                this.submittedWidget.title = this.forms.filter(x => x.formStatus == FormStatus.SUBMITTED || x.formStatus == FormStatus.RE_SUBMITTED).length.toString();
                this.submittedWidget.text = "Submitted";
                this.submittedWidget.actionText = "require reviewer action";

                if (this.submittedWidget.title == "0")
                    this.submittedWidget.color = "success";
                else
                    this.submittedWidget.color = "info";

                this.overdueWidget.title = this.forms.filter(x => x.isOverdue == true).length.toString();
                this.overdueWidget.text = "Overdue";
                this.overdueWidget.actionText = "click to view";

                if (this.overdueWidget.title == "0")
                    this.overdueWidget.color = "success";
                else
                    this.overdueWidget.color = "danger";

                var blankForms = forms.filter(x => x.isBlank);
                this.numberOfBlankForms = blankForms.length;

                var unchangedForms = forms.filter(x => x.isUnchanged);
                this.numberOfUnchangedForms = unchangedForms.length;
            });
    }

    gotoDetail(id: number): void {
        this.router.navigate(['/extension', id]);
    }

    onSubmittedClick() {
        this.tabs.selectTab(this.tabForms);
        this.dtForms.filter("Submitted", true);
    }

    onExtensionClick() {
        this.tabs.selectTab(this.tabExt);
    }

    onOverdueClick() {
        this.tabs.selectTab(this.tabForms);
        this.dtForms.filter("Overdue", true);
    }
    
    certifyBlankForms() {
        if (confirm("Proceeding with this action will auto certify all 'blank' forms.  All submitted forms where the filer answered 'no' to all 5 sections will be certified.  Are you sure you want to continue?")) {
            this.formService.certifyForms('blank').then(forms => {
                this.loadForms();
            });
        }
    }

    certifyUnchangedForms() {
        if (confirm("Proceeding with this action will auto certify all 'unchanged' forms.  All submitted forms where the filer made no changes to their previous year's certified form.  Are you sure you want to continue?")) {
            this.formService.certifyForms('unchanged').then(forms => {
                this.loadForms();
            });
        }
    }
}