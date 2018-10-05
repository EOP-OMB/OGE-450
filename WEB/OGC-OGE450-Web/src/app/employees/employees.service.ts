import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { Employee } from './employee';
import { Logging } from '../common/logging';

@Injectable()
export class EmployeesService {
    private serviceUrl = '';
    private list = 'employees';  // URL to web api

    constructor(private http: AuthHttp) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    getAll(): Promise<Employee[]> {
        return this.http.get(this.serviceUrl)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    get(id: number): Promise<Employee> {
        var url = `${this.serviceUrl}/${id}`;

        return this.http.get(url)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    update(emp: Employee): Promise<Employee> {
        const url = `${this.serviceUrl}`;

        return this.http
            .put(url, JSON.stringify(emp))
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);
        return Promise.reject(error.message || error);
    }

    public copy(emp: Employee): Employee {
        var newEmp = new Employee();

        newEmp.id = emp.id;
        newEmp.title = emp.title;
        newEmp.accountName = emp.accountName;
        newEmp.inactive = emp.inactive;
        newEmp.filerType = emp.filerType;
        newEmp.reportingStatus = emp.reportingStatus;
        newEmp.last450Date = emp.last450Date;
        newEmp.currentFormId = emp.currentFormId;
        newEmp.position = emp.position;
        newEmp.emailAddress = emp.emailAddress;
        newEmp.displayName = emp.displayName;
        newEmp.workPhone = emp.workPhone;
        newEmp.agency = emp.agency;
        newEmp.branch = emp.branch;
        newEmp.profileUrl = emp.profileUrl;
        newEmp.pictureUrl = emp.pictureUrl;
        newEmp.generateForm = emp.generateForm;
        newEmp.appointmentDate = emp.appointmentDate;
        newEmp.employeeStatus = emp.employeeStatus;

        newEmp.newEntrantEmailText = emp.newEntrantEmailText;
        newEmp.annualEmailText = emp.annualEmailText;

        return newEmp;
    }
}


