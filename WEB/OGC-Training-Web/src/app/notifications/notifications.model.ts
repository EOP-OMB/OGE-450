export class Notifications {
    id: number;

    recipient: string;
    cc: string;
    subject: string;
    body: string;
    sentDateTime: Date;
    status: string;
    errorMessage: string;
    application: string;
    isAnnouncement: boolean;

    cssClass: string;

    created: Date;
    createdBy: string;
    listName: string;
    modified: Date;
    modifiedBy: string;
    title: string;
    year: number;
    
    constructor() {
    }
}