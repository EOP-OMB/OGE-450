import { NgModule, APP_INITIALIZER, ModuleWithProviders } from '@angular/core';
import { Http, RequestOptions } from '@angular/http';
import { AuthHttp, AuthConfig } from 'angular2-jwt';
import { UserService } from './security/user.service';

// services (auth)
import { AdalService } from 'ng2-adal/core';
import { SecretService } from './security/secret.service';

import { Logging } from './common/logging';

export function authHttpServiceFactory(http: Http, options: RequestOptions) {
    return new AuthHttp(new AuthConfig({
        tokenName: 'token',
        tokenGetter: (() => sessionStorage.getItem('adal.idtoken')),
        globalHeaders: [{ 'Content-Type': 'application/json' }],
    }), http, options);
}

export function authenticateAdfs(adalService: AdalService, secretService: SecretService, userService: UserService) {
    return () => {
        adalService.init(secretService.adalConfig);
        adalService.handleWindowCallback();
        adalService.getUser();           
        
        if (adalService.userInfo.isAuthenticated) {
            Logging.log('authenticated!');

            return userService.load();
        }
    };
}

@NgModule({})
export class AuthModule {
    static forRoot(): ModuleWithProviders {
        return {
            ngModule: AuthModule,
            providers: [
                {
                    provide: AuthHttp,
                    useFactory: authHttpServiceFactory,
                    deps: [Http, RequestOptions]
                },
                {
                    provide: APP_INITIALIZER,
                    useFactory: authenticateAdfs,
                    multi: true,
                    deps: [AdalService, SecretService, UserService]
                },
                UserService,
                SecretService,
                AdalService,
            ]
        };
    }
}