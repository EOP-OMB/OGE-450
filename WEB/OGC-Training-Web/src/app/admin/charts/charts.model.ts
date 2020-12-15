export class OGE450ChartData {
    labels: string[];
    data: number[];

    constructor() {
    }
}

export class TrainingChartData {
    completedTraining: number;
    totalEmployees: number;

    constructor() {
    }
}

export class EventsChartData {
    labels: string[];
    datasets: DataSet[];

    constructor() {
    }
}

export class DataSet {
    label: string;
    data: number[];
    backgroundColor: string;
    borderColor: string;
    fill: boolean;
}