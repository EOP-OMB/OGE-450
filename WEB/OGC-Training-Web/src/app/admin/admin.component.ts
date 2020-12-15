import { environment } from '../../environments/environment';
import { Router } from '@angular/router';
import { Component, OnInit, ViewChild } from '@angular/core';
import { TabsComponent } from '../common/tabs/tabs.component';
import { TabComponent } from '../common/tabs/tab.component';

import { Widget } from '../common/widget/widget';

import { Training } from '../training/training.model';
import { TrainingService } from '../training/training.service';
import { ChartService } from './charts/chart.service';
import { TrainingChartData, OGE450ChartData, EventsChartData, DataSet } from './charts/charts.model';

import { NotificationTemplateService } from './edit-templates/notification-template.service';
import { NotificationTemplate } from './edit-templates/notification-template';

import 'rxjs/Rx';
import { SelectItem } from 'primeng/primeng';
import { Lookups } from 'app/common/constants';

declare var $: any;

@Component({
    selector: 'admin',
    templateUrl: './admin.component.html',
    styleUrls: ['./admin.component.css']
})

export class AdminComponent implements OnInit {
    @ViewChild('tabs') tabs;

    templates: NotificationTemplate[];

    trainings: Training[];

    reload: boolean = false;

    oge450Data: any;
    oge450Options: any;

    empdata: any;
    empoptions: any;

    trainingdata: any;
    trainingChartData: TrainingChartData;
    trainingoptions: any;
    selectedTraining: Training;

    eventdata: any;
    eventoptions: any;

    employees: any[];

    selectedYear: number;
    years: SelectItem[];

    constructor(private trainingService: TrainingService,
        private templateService: NotificationTemplateService,
        private chartService: ChartService,
        private router: Router
    ) {
        var dt = new Date();
        this.selectedYear = dt.getFullYear();
        this.years = Lookups.YEARS.filter(x => x.value != null);
        
        //this.employees = [
        //    {
        //        employeesName: 'Washington, George',
        //        status: 'Current',
        //        filerType: '278-Filer',
        //        last450Date: 'N/A',
        //        lastTrainingDate: '1/21/2017',
        //        events: 3
        //    },
        //    {
        //        employeesName: 'Adams, John',
        //        status: 'Current',
        //        filerType: '450-Filer',
        //        last450Date: '10/11/2017',
        //        lastTrainingDate: '10/1/2017',
        //        events: 2
        //    },
        //    {
        //        employeesName: 'Jefferson, Thomas',
        //        status: 'New Employee',
        //        filerType: 'Not Assigned',
        //        last450Date: '',
        //        lastTrainingDate: '',
        //        events: 0
        //    },
        //    {
        //        employeesName: 'Madison, James',
        //        status: 'Current',
        //        filerType: '450-Filer',
        //        last450Date: '10/15/2017',
        //        lastTrainingDate: '4/1/2017',
        //        events: 1
        //    }
        //];


        //this.empdata = {
        //    labels: ['Not Assigned', '450-Filer', '278-Filer', 'Non-Filer', '450 Waived'],
        //    datasets: [
        //        {
        //            data: [7, 513, 128, 7, 2],
        //            backgroundColor: ['#f8f9fa', '#5bc0de', '#d9534f', '#337ab7', '#5cb85c'],
        //            hoverBackgroundColor: ['#e6e7de', '#46b8da', '#d43f3a', '#236ba7', '#4cae4c'],

        //        }]
        //};

        //this.empoptions = {
        //    title: {
        //        display: false,
        //        text: 'My Title',
        //        fontSize: 16
        //    },
        //    legend: {
        //        position: 'left'
        //    }
        //};
    }

    switchToYear(year: number) {
        this.selectedYear = year;
        this.loadTrainingChart();
    }

    ngOnInit(): void {
        this.loadTrainings();
        this.loadTrainingChart();
        this.loadOGE450Chart();
        this.loadEventsChart();
        // this.loadTemplates();
    }

    loadOGE450Chart(): void {
        this.oge450Options = {
            title: {
                display: false,
                text: 'My Title',
                fontSize: 16
            },
            legend: {
                position: 'right'
            }
        };

        this.chartService.getOGE450ChartData().then(response => {
            this.oge450Data = {
                labels: response.labels,
                datasets: [
                    {
                        data: response.data,
                        backgroundColor: ['#f8f9fa', '#5bc0de', '#f0ad4e', '#d9534f', '#337ab7', '#5cb85c'],
                        hoverBackgroundColor: ['#e6e7de', '#46b8da', '#ec971f', '#d43f3a', '#236ba7', '#4cae4c'],
                    }]
            };
        });;
    }

    loadTrainingChart(): void {
        this.trainingoptions = {
            title: {
                display: false,
                text: 'My Title',
                fontSize: 16
            },
            legend: {
                position: 'bottom'
            },
            scales: {
                xAxes: [{ stacked: true, percent: true }],
                yAxes: [{ stacked: true, percent: true }]
            }
        };

        this.chartService
            .getTrainingChartData(this.selectedYear)
            .then(response => {
                this.trainingChartData = response;

                //var percentComplete = parseFloat((this.trainingChartData.completedTraining / this.trainingChartData.totalEmployees).toFixed(2));

                //percentComplete *= 100;

                //var percentMissing = 100.0 - percentComplete;

                this.trainingdata = {
                    labels: ['Employees (' + this.trainingChartData.totalEmployees + ')'],
                    datasets: [
                        {
                            label: 'Complete',
                            backgroundColor: '#5cb85c',
                            bordercolor: '#4cae4c',
                            data: [this.trainingChartData.completedTraining]
                        },
                        {
                            label: 'Missing',
                            backgroundColor: '#d9534f',
                            bordercolor: '#d43f3a',
                            data: [this.trainingChartData.totalEmployees - this.trainingChartData.completedTraining]
                        }
                    ]
                };
            });
    }

    loadEventsChart(): void {
        this.chartService
            .getEventsChartData()
            .then(response => {
                //response.datasets.forEach(x => { 
                //    if (x.label == 'Cleared') {

                //    }
                //    else if (x.label == 'Events') {

                //    }
                //    //label: ;
                //    //data: [6, 9, 15, 12, 20, 10, 9, 0, 8, 10, 15, 0],
                //    //    backgroundColor: '#5cb85c',
                //    //        borderColor: '#4cae4c',
                //    //            fill: false
                //    //label: 'Events',
                //    //data: [6, 9, 15, 12, 23, 13, 16, 2, 10, 11, 20, 0],
                //    //borderColor: '#999',
                //    //backgroundColor: '#aaa',
                //    //fill: false
                //});

                this.eventdata = {
                    labels: response.labels,
                    datasets: response.datasets
                };
            });

        this.eventoptions = {
            title: {
                display: false,
                fontSize: 16
            },
            legend: {
                position: 'top'
            }
        };
    }

    loadTrainings(): void {
        this.trainingService
            .getAll()
            .then(response => {
                this.trainings = response;
            });
    }

    runReport(): void {
        //window.open(environment.apiUrl + '/training?a=missingtrainingreport', '_self');
        this.trainingService
            .getMissingTrainingReport(this.selectedYear)
            .then(response => {
                var type = response.headers.get('content-type');
                var filename = response.headers.get('content-disposition');
                var start = filename.indexOf('filename=') + 10;

                filename = filename.substr(start, filename.length - start - 1);

                var blob = new Blob([response._body], { type: type });

                var url = window.URL.createObjectURL(blob);

                //window.open(url, '_self');

                var a = document.createElement("a");
                a.href = url;
                a.download = filename;

                // start download
                a.click();
            });
    }

    loadTemplates(): void {
        this.templateService
            .getAll()
            .then(response => {
                this.templates = response;
            });
    }

    editTrainingClose(save: boolean): void {
        $('#edit-training').modal('hide');

        if (save) {
            this.loadTrainings();
        }
    }

    gotoDetail(training: Training): void {
        this.selectedTraining = training;
        $('#edit-training').modal();
    }

    refresh(widget: string): void {
        if (widget == "training")
            this.loadTrainingChart();
        else if (widget == "oge450")
            this.loadOGE450Chart();
        else if (widget == "events")
            this.loadEventsChart();
    }

    goto(where: string, id: number = 0): void {
        var url = "";
        if (where == "Events") {
            url = environment.eventClearanceUrl;
        }
        else if (where == "OGE450") {
            url = environment.oge450Url;
            if (id > 0)
                url += "/form/" + id;
        }

        if (url != "")
            window.open(url, '_self');
    }
}
