import { Component, OnInit } from '@angular/core';
import { SupportContactService } from './support-contacts.service';
import { SupportContact } from './support-contact';

@Component({
    selector: 'support-contacts',
    templateUrl: './support-contacts.component.html',
    styleUrls: ['./support-contacts.component.css']
})

export class SupportContactsComponent implements OnInit {
    private contacts: SupportContact[];

    constructor(private contactsService: SupportContactService) { }

    ngOnInit(): void {
        this.getContacts();
    }

    getContacts(): void {
        this.contactsService
            .getAll()
            .then(contacts => {
                this.contacts = contacts;
            });
    }
}

