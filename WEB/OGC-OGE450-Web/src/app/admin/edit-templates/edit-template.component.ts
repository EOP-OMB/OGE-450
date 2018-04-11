import { Component, OnInit, Input } from '@angular/core';
import { NotificationTemplateService } from './notification-template.service';
import { SettingsService } from '../settings/settings.service';
import { NotificationTemplate } from './notification-template';
import { Settings } from '../settings/settings';
import { Lookups } from '../../common/constants';
import { SelectItem } from 'primeng/primeng';

declare var $: any;
declare var tinymce: any;

@Component({
    selector: 'edit-templates',
    templateUrl: './edit-template.component.html',
    styleUrls: [ './edit-template.component.css']
})

export class EditTemplateComponent implements OnInit {
    @Input()
    templates: NotificationTemplate[];

    private selectedTemplate: NotificationTemplate;
    private origTemplate: NotificationTemplate;

    private settings: Settings;

    recipientTypes: SelectItem[];

    constructor(private templateService: NotificationTemplateService,
                private settingsService: SettingsService) { }

    ngOnInit(): void {
        this.getSettings();

        this.recipientTypes = Lookups.RECIPIENT_TYPES;
    }

    getSettings(): void {
        this.settingsService.get().then(response => {
            this.settings = response;
        });
    }

    public isDirty(): boolean {
        return JSON.stringify(this.origTemplate) != JSON.stringify(this.selectedTemplate);
    }

    templateSelected(template: NotificationTemplate) {
        // deactivate all templates
        this.templates.forEach(template => template.active = false);

        // activate the template the user has clicked on.
        template.active = true;

        this.templateService.get(template.id).then(response => {
            this.origTemplate = JSON.parse(JSON.stringify(response));
            this.selectedTemplate = response;
            var editor = tinymce.get('txtBody');
            if (editor)
                editor.setContent(this.selectedTemplate.body);
        });
    }

    saveTemplate() {
        this.templateService.update(this.selectedTemplate).then(response => {
            this.selectedTemplate = response;
            this.origTemplate = JSON.parse(JSON.stringify(response));
            
            $("#template-success").alert();

            $("#template-success").fadeTo(2000, 500).slideUp(500, function () {
                $("#template-success").slideUp(500);
            });
        });
    }

    keys(): Array<string> {
        return Object.keys(this.selectedTemplate.templateFields);
    }

    keyupHandlerFunction(content: string) {
        this.selectedTemplate.body = content;
    }
}

