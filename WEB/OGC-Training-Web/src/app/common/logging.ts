import { environment } from '../../environments/environment'

export class Logging {
    static log(val: any) {
        if (environment.debug)
            console.log(val);
    }
}