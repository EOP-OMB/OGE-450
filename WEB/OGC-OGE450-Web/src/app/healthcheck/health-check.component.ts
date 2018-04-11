import { Component, OnInit } from '@angular/core';
import { HealthCheckService } from './health-check.service';
import { HealthCheck } from './health-check';

@Component({
    selector: 'health-check',
    templateUrl: './health-check.component.html',
    styleUrls: ['./health-check.component.css']
})

export class HealthCheckComponent implements OnInit {
    private health: HealthCheck[];
    private status: string = "Success";

    constructor(private healthService: HealthCheckService) { }

    ngOnInit(): void {
        this.getHealth();
    }

    getHealth(): void {
        this.healthService
            .getAll()
            .then(health => {
                this.health = health;
                var broke = health.filter(x => x.status != "Success");

                if (broke.length > 0)
                    this.status = "Error";
                else
                    this.status = "Success";
            });
    }
}

