import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Widget } from './widget';

@Component({
    selector: 'widget',
    templateUrl: './widget.component.html',
    styleUrls: ['../widget/widget.component.css'],
})

export class WidgetComponent implements OnInit {
    @Input() data: Widget;
    @Input() icon: string;

    @Output()
    widgetClick: EventEmitter<void> = new EventEmitter<void>();

    constructor() { }

    ngOnInit(): void {
     
    }

    onClick(): void {
        this.widgetClick.emit();
    }
}

