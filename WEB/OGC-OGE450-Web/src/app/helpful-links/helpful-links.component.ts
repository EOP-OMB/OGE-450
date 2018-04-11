import { Component, OnInit } from '@angular/core';
import { HelpfulLinkService } from './helpful-links.service';
import { HelpfulLink } from './helpful-link';

@Component({
    selector: 'helpful-links',
    templateUrl: './helpful-links.component.html',
    styleUrls: ['./helpful-links.component.css']
})

export class HelpfulLinksComponent implements OnInit {
    private links: HelpfulLink[];

    constructor(private linkService: HelpfulLinkService) { }

    ngOnInit(): void {
        this.getLinks();
    }

    getLinks(): void {
        this.linkService
            .getAll()
            .then(links => {
                this.links = links;
        });
    }
}

