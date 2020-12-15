export class EthicsForm {
    constructor() {
        this.id = 0;
    }

    id: number;
    url: string;
    doctype: string;
    formType: string; // Form or Guidance
    title: string;
    modifiedBy: string;
    modified: Date;
    description: string;
}
