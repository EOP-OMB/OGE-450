export class ExtensionRequest {
    constructor() {
        this.id = 0;
    }

    id: number;

    year: string;
    filerName: string;
    created: Date;

    ogeForm450Id: number;
    reason: string;
    daysRequested: number;
    status: string;
    extensionDate: Date;
    reviewerComments: string;

    dueDate: Date;
}