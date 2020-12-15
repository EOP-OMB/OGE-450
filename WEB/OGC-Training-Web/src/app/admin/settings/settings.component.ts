import { Component, OnInit } from '@angular/core';
import { SettingsService } from './settings.service';
import { Settings } from './settings';

declare var $: any;

@Component({
    selector: 'settings',
    templateUrl: './settings.component.html',
    styleUrls: ['./settings.component.css']
})

export class SettingsComponent implements OnInit {
    private settings: Settings;
    private origSettings: Settings;

    constructor(private settingsService: SettingsService) { }

    ngOnInit(): void {
        this.getSettings();
       
    }

    getSettings(): void {
        this.settingsService
            .get()
            .then(response => {
                this.origSettings = JSON.parse(JSON.stringify(response));
                this.settings = response;
            });
    }

    public isDirty(): boolean {
        return JSON.stringify(this.origSettings) != JSON.stringify(this.settings);
    }

    saveSettings() {
        this.settingsService.update(this.settings).then(response => {
            this.settings = response;
            this.origSettings = JSON.parse(JSON.stringify(response));
            
            $("#settings-success").alert();

            $("#settings-success").fadeTo(2000, 500).slideUp(500, function () {
                $("#settings-success").slideUp(500);
            });
        });
    }
}

