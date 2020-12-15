export class NotificationTemplate {
    id: number;
    sharePointList: string;
    viewName: string;
    recipientType: string;
    recipientColumn: string;
    subject: string;
    frequency: string;
    body: string;

    nextRunDate: Date;
    lastRunDate: Date;

    lastRunStatus: string;
    includeCc: boolean;

    active: boolean;

    templateFields: Dictionary;
}

interface Dictionary {
    [index: string]: string
}