import { Component, OnInit } from '@angular/core';
import { SupportContactService } from '../../support-contacts/support-contacts.service';
import { SupportContact } from '../../support-contacts/support-contact';

declare var $: any;

@Component({
    selector: 'edit-contacts',
    templateUrl: './edit-contacts.component.html',
    styleUrls: ['./edit-contacts.component.css']
})

export class EditContactsComponent implements OnInit {
    private contacts: SupportContact[];
    private origContacts: SupportContact[];

    constructor(private contactService: SupportContactService) { }

    ngOnInit(): void {
        this.getLinks();
    }

    getLinks(): void {
        this.contactService
            .getAll()
            .then(response => {
                this.contacts = response;
                this.origContacts = JSON.parse(JSON.stringify(response));
            });
    }

    isDirty(): boolean {
        return JSON.stringify(this.origContacts) != JSON.stringify(this.contacts);
    }

    addRow(): void {
        this.contacts.push(new SupportContact());
    }

    saveLinks() {
        this.contactService.update(this.contacts).then(response => {
            this.contacts = response;
            this.origContacts = JSON.parse(JSON.stringify(response));
            
            $("#contacts-success").alert();

            $("#contacts-success").fadeTo(2000, 500).slideUp(500, function () {
                $("#contacts-success").slideUp(500);
            });
        });
    }
}

