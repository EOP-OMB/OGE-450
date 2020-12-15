import { FileSelectDirective, FileDropDirective, FileUploader, FileItem, FileUploaderOptions } from 'ng2-file-upload';
import { Attachment } from '../../event-request/event-request.model';
import { environment } from '../../../environments/environment';

import { Component, OnInit, OnChanges, Input, Output, EventEmitter } from '@angular/core';

declare var $: any;

@Component({
    selector: 'file-upload',
    templateUrl: './upload.component.html'
})

export class UploadComponent implements OnInit, OnChanges {
    public uploader: FileUploader;
    public hasBaseDropZoneOver: boolean = false;
    public hasAnotherDropZoneOver: boolean = false;

    @Input()
    uploadText: string;

    @Input()
    id: number;

    @Input()
    fileQueue: Attachment[];

    @Input()
    guid: string;

    @Input()
    type: string;

    @Output()
    attachmentRemoved: EventEmitter<Attachment> = new EventEmitter<Attachment>();

    errorMessage: string;

    constructor() {
        
    }

    ngOnInit(): void {
        this.initializeUploader();

        this.uploader.onWhenAddingFileFailed = (item, filter, options) => {
            $('#divErrorMessage').show();

            if (filter.name == 'fileSize')
                this.errorMessage = "File exceeds the maximum file size of 5MB";
            else if (filter.name == 'mimeType')
                this.errorMessage = "File type is not allowed.  Allowed file types are: .doc, .docx, .csv, .msg, .pdf, .ppt, .pptx, .xls, .xlsx";
        }

        this.uploader.onAfterAddingFile = (file) => {
            var dupe = this.uploader.queue.filter(x => x.file.name == file.file.name);

            if (dupe && dupe.length > 1) {
                var x = dupe[0];

                if (x.progress < 100)
                    x.cancel();

                x.remove();
            }
        };
    }

    ngOnChanges(): void {
        this.initializeUploader();

        if (this.fileQueue) {
            
        }
    }

    initializeUploader() {
        var URL = environment.apiUrl + 'attachment';

        this.uploader = new FileUploader({
            url: URL,
            isHTML5: true,
            autoUpload: true,
            maxFileSize: 5242880,
            additionalParameter:
            {
                id: this.id,
                type: this.type,
                guid: this.guid
            },
            allowedMimeType: ['application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'application/msword', 'text/csv', 'application/pdf', 'application/vnd.ms-powerpoint', 'application/vnd.openxmlformats-officedocument.presentationml.presentation', 'application/vnd.ms-excel', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'application/vnd.ms-outlook']
        });
    }

    public fileOverBase(e: any): void {
        this.hasBaseDropZoneOver = e;
    }

    public triggerUpload(): void {
        $('#hdnFileUpload').trigger('click');
    }

    public uploadFiles(): void {
        this.uploader.uploadAll();
    }

    public getFiles(): FileItem[] {
        return this.uploader.queue;
    }

    removeAttachment(att: Attachment) {
        this.fileQueue = this.fileQueue.filter(x => x.id != att.id);
        this.attachmentRemoved.emit(att);
    }
}
