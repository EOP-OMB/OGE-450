import { Component, OnInit } from '@angular/core';
import { HelpfulLinkService } from '../../helpful-links/helpful-links.service';
import { HelpfulLink } from '../../helpful-links/helpful-link';

declare var $: any;

@Component({
    selector: 'edit-links',
    templateUrl: './edit-links.component.html',
    styleUrls: ['./edit-links.component.css']
})

export class EditLinksComponent implements OnInit {
    private links: HelpfulLink[];
    private origLinks: HelpfulLink[];

    constructor(private linkService: HelpfulLinkService) { }

    ngOnInit(): void {
        this.getLinks();
    }

    getLinks(): void {
        this.linkService
            .getAll()
            .then(response => {
                this.links = response;
                this.origLinks = JSON.parse(JSON.stringify(response));
            });
    }

    isDirty(): boolean {
        return JSON.stringify(this.origLinks) != JSON.stringify(this.links);
    }

    addRow(): void {
        this.links.push(new HelpfulLink());
    }

    saveLinks() {
        this.linkService.update(this.links).then(response => {
            this.links = response;
            this.origLinks = JSON.parse(JSON.stringify(response));
            
            $("#links-success").alert();

            $("#links-success").fadeTo(2000, 500).slideUp(500, function () {
                $("#links-success").slideUp(500);
            });
        });
    }
}

