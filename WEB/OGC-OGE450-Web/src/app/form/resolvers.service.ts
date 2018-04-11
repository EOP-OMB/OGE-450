import { Injectable } from '@angular/core';
import { Router, Resolve, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';

import { OGEForm450Service } from './oge-form-450.service';
import { OGEForm450 } from './oge-form-450';

@Injectable()
export class OGEForm450Resolver implements Resolve<OGEForm450> {
    constructor(private formService: OGEForm450Service, private router: Router) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<OGEForm450> {
        let id = route.params['id'];

        return this.formService.get(id).then(form => {
            if (form) {
                return form;
            } else { // id not found
                this.router.navigate(['/']);
                return null;
            }
        });
    }
}