import { Injectable } from '@angular/core';
import { Router, Resolve, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';

import { EventRequestService } from './event-request.service';
import { EventRequest } from './event-request.model';

@Injectable()
export class EventRequestResolver implements Resolve<EventRequest> {
    constructor(private eventService: EventRequestService, private router: Router) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<EventRequest> {
        let id = route.params['id'];

        if (id == 0) {
            var prom = new Promise<EventRequest>((resolve, reject) => {
                var event = new EventRequest();

                event.id = 0;

                if (event)
                    resolve(event);
                else
                    reject(null);
            });

            return prom;
        }
        else {
            return this.eventService.get(id).then(form => {
                if (form) {
                    return form;
                } else { // id not found
                    this.router.navigate(['/']);
                    return null;
                }
            });
        }
    }
}