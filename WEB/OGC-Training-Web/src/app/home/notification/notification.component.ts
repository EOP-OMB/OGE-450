import { Component, OnInit, Input, Output, EventEmitter, SimpleChange } from '@angular/core';

import { Notifications } from '../../notifications/notifications.model';

import { SelectItem } from 'primeng/primeng';
import { Lookups } from '../../common/constants';

@Component({
    selector: 'notification',
    templateUrl: './notification.component.html',
    styleUrls: ['./notification.component.css']
})

export class NotificationComponent implements OnInit {
    @Input()
    private message: Notifications;

    @Output()
    close = new EventEmitter<any>();

    public isVideo: boolean = false;
    public vidSrc: string;
    public vidType: string;

    ngOnInit(): void {

    }

    constructor() {
        
    }

    ngOnChanges(changes: { [propKey: string]: SimpleChange }): void {
        if (changes["message"]) {
            if (this.message && this.message.body.includes('[VIDEO]')) {
                this.isVideo = true;

                this.vidSrc = this.message.errorMessage;
            }
        }
    }

    cancel() {
        this.close.emit();
    }
}
