import { Injectable, Injector } from '@angular/core';
import { Headers, Response, RequestOptions } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';
import { environment } from '../../environments/environment';

import { UserInfo } from './user-info';
import { Roles } from '../security/roles';

import 'rxjs/add/operator/toPromise';

@Injectable()
export class UserService {
    private serviceUrl = '';
    private list = 'user';  // URL to web api

    private _user: UserInfo;
    
    constructor(private injector: Injector/*private http: AuthHttp*/) {
        this.serviceUrl = environment.apiUrl + this.list;
    }

    public get user(): UserInfo {
        return this._user;
    }

    public get http(): AuthHttp {
        return this.injector.get(AuthHttp);
    }

    load(): Promise<UserInfo> {
        return this.http.get(this.serviceUrl)
                .toPromise()
                .then(response => {
                    this._user = response.json();
                    return response.json();
                })
                .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {

        return Promise.reject(error.message || error);
    }

    public isInRole(role: string): boolean {
       
        return (role != Roles.Reviewer || this.user.isReviewer) && (role != Roles.Admin || this.user.isAdmin);
    }
}


