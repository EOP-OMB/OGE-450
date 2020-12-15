import { ErrorHandler, Injectable, Injector } from '@angular/core';
import { Logging } from '../common/logging'
import { Router } from '@angular/router'

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
    constructor(private injector: Injector) { }

    handleError(error) {
        const router = this.injector.get(Router);

        if (error.message && error.message.includes("JWT present or has expired")) {
            Logging.log('attempting to relogin');
            router.navigate(['/login']);
        }

        Logging.log("Global Error Handler: " + error.message || error.toString());

        // IMPORTANT: Rethrow the error otherwise it gets swallowed
        throw error;
    }

}