import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { environment } from '../../environments/environment';

import 'rxjs/add/operator/toPromise';

import { TrainingVideo } from './training.model';
import { Logging } from '../common/logging';

@Injectable()
export class VideoService {

    constructor(private http: Http) {
        
    }

    getVideos(): Promise<TrainingVideo[]> {
        var url = 'assets/json/training-videos.json';

        return this.http
            .get(url)
            .toPromise()
            .then(response => {
                var videos = response.json();

                return videos.filter(x => x.environment == environment.envName);
            })
            .catch(this.handleError);
    }

    private handleError(error: Response | any): Promise<any> {
        Logging.log(error);

        let check: TrainingVideo[] = [];

        return Promise.resolve(check);
    }
}


