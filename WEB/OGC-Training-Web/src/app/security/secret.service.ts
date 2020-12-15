import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable()
export class SecretService {

    public get adalConfig(): any {
        return {
            instance: 'https://adfs.omb.gov/',
            tenant: 'adfs',
            clientId: environment.clientId,
            redirectUri: window.location.origin + environment.base,
            postLogoutRedirectUri: window.location.origin + environment.base + 'logout',
            extraQueryParameter: 'resource=' + environment.clientId,
        };
    }
}
