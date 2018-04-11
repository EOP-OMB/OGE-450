import { Component, OnInit } from '@angular/core';
import { UserInfo } from './user/user-info';
import { Router, NavigationEnd, NavigationStart } from "@angular/router";
import { SecretService } from "./security/secret.service";
import { UserService } from "./user/user.service";
import { AdalService } from "ng2-adal/services/adal.service";
import { environment } from '../environments/environment'
import { FormStatus } from './common/constants';

import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/pairwise';

import { Idle, DEFAULT_INTERRUPTSOURCES } from '@ng-idle/core';
import { Keepalive } from '@ng-idle/keepalive';

declare var $: any;

@Component({
    selector: 'my-app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
})

export class AppComponent implements OnInit {
    idleState = '';
    countdownColor = '';
    timedOut = false;
    lastPing?: Date = null;

    ngOnInit(): void {
        
    }

    constructor(
        private adalService: AdalService,
        private secretService: SecretService,
        private userService: UserService,
        private router: Router,
        private idle: Idle,
        private keepalive: Keepalive
    )
    {
        this.router.events
            .filter(e => e instanceof NavigationEnd)
            .pairwise().subscribe((e: any[]) => {
                var url: string = e[0].url;

                if (url.includes('client-request-id') || url.includes('id_token'))
                    url = '/';

                localStorage.setItem('prev', url);
            });

        this.router.events
            .filter(e => e instanceof NavigationStart)
            .pairwise().subscribe((e: any[]) => {
                localStorage.setItem('goto', e[1].url);
            });

        // sets an idle timeout of 5 seconds, for testing purposes.
        idle.setIdle(environment.idleTimeout);
        // sets a timeout period of 5 seconds. after 10 seconds of inactivity, the user will be considered timed out.
        idle.setTimeout(environment.idleCountdown);
        // sets the default interrupts, in this case, things like clicks, scrolls, touches to the document
        idle.setInterrupts(DEFAULT_INTERRUPTSOURCES);

        idle.onIdleEnd.subscribe(() => {
            this.showIdle(false);
        });
        idle.onTimeout.subscribe(() => {
            this.timedOut = true;
            this.router.navigate(['/logout']);
        });
        idle.onIdleStart.subscribe(() => {
            this.showIdle(true);
        });
        idle.onTimeoutWarning.subscribe((countdown) => {
            this.idleState = countdown;

            if (countdown > 30)
                this.countdownColor = '';
            else if (countdown > 10)
                this.countdownColor = 'text-warning';
            else
                this.countdownColor = 'text-danger';

        });

        // sets the ping interval to 15 seconds
        keepalive.interval(15);

        keepalive.onPing.subscribe(() => this.lastPing = new Date());

        this.reset();
    }

    showIdle(isIdle: boolean) {
        $('#idleTimeoutDialog').modal(isIdle ? "show" : "hide");
    }

    reset() {
        this.idle.watch();
        this.timedOut = false;
    }
}

