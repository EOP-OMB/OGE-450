import { Component, OnDestroy, AfterViewInit, EventEmitter, Input, Output } from '@angular/core';

declare var tinymce: any;

@Component({
    selector: 'tinymce-editor',
    template: `<textarea id="{{elementId}}">{{content}}</textarea>`
})

export class TinyMceEditorComponent implements AfterViewInit, OnDestroy {
    @Input() elementId: String;
    @Input() content: String;
    @Input() height: number;
    @Output() onEditorKeyup = new EventEmitter<any>();

    editor;

    ngAfterViewInit() {
        tinymce.init({
            selector: '#' + this.elementId,
            plugins: ['link', 'paste', 'table'],
            skin_url: 'assets/skins/lightgray',
            setup: editor => {
                this.editor = editor;
                editor.on('keyup', () => {
                    const content = editor.getContent();
                    this.onEditorKeyup.emit(content);
                });
            },
            branding: false,
            height: this.height,
            statusbar: false,
        });
    }

    ngOnDestroy() {
        tinymce.remove(this.editor);
    }
}