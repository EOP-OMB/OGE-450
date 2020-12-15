export class Training {
    constructor() {
        this.id = 0;
    }

    id: number;
    employee: string;
    employeesName: string;
    title: string;
    dateAndTime: string;
    location: string;
    ethicsOfficial: string;
    division: string;
    trainingType: string;
    year: number;
    inactive: boolean;
}

export class TrainingVideo {
    constructor() {

    }

    id: number;
    url: string;
}
