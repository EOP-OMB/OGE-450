import { Component, OnInit } from '@angular/core';
import { TrainingVideo } from './training.model';
import { VideoService } from './video.service';


@Component({
    selector: 'training',
    templateUrl: './training.component.html',
    styleUrls: ['./training.component.css']
})

export class TrainingComponent implements OnInit {
    public files: TrainingVideo[];

    public selectedFile: TrainingVideo;

    constructor(
        private videoService: VideoService) {

    }

    ngOnInit(): void {
        this.videoService
            .getVideos()
            .then(response => {
                this.files = response;
                //this.loadingComplete = true;
            });
    }

    public showVideo(id: number): void {
        this.selectedFile = this.files.filter(x => x.id == id)[0];
    }
}
