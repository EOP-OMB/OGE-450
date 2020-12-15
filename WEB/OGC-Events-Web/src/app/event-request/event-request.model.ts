import { AppUser } from '../security/app-user';
import { Helper, Guid } from '../common/helper';

export class EventRequest {
    constructor() {
        this.typeOfOrg = 0;
        this.typeOfHost = 0;
    }

    id: number;
    title: string;

    submittedBy: string; // UPN of submitter
    submitter: string;  // Display Name of submitter
    eventName: string;
    guestInvited: boolean;
    eventStartDate: Date;
    eventEndDate: Date;
    eventContactName: string;
    eventContactPhone: string;
    individualExtendingInvite: string;
    isIndividualLobbyist: boolean;
    orgExtendingInvite: string;
    isOrgLobbyist: boolean;
    typeOfOrg: number;
    orgOther: string;
    orgHostingEvent: string;
    isHostLobbyist: boolean;
    typeOfHost: number;
    hostOther: string;
    isFundraiser: boolean;
    whoIsPaying: string;
    fairMarketValue: number;
    requiresTravel: boolean;
    internationalTravel: boolean;
    additionalInformation: string;
    eventLocation: string;
    crowdDescription: string;
    approximateAttendees: number;
    isOpenToMedia: boolean;
    guidanceGiven: string;
    assignedTo: string;
    assignedToUpn: string;
    status: string;
    closedReason: string;
    closedBy: string;
    closedDate: Date;
    contactNumber: string;
    contactEmail: string;
    contactComponent: string;
    attachmentGuid: string;

    attendees: Attendee[];
    attachments: Attachment[];

    attendeesString: string;
    eventDates: string;
    dateValue: number;
    dateFilter: string;
}

export class Attendee {
    constructor(reqId: number) {
        this.eventRequestId = reqId;
        this.id = 0;
        this.employee = new AppUser();
        this.guid = Guid.newGuid();
    }

    id: number;
    eventRequestId: number;
    eventName: string;
    employee: AppUser;
    capacity: string;
    employeeType: string;
    isGivingRemarks: boolean;
    remarks: string;
    reasonForAttending: string;

    informedSupervisor: boolean;
    nameOfSupervisor: string;

    // UI Only
    selected: boolean;
    guid: string;
}

export class Attachment {
    eventRequestId: number;
    fileName: string;
    size: number;
    typeOfAttachment: string;
    attachmentGuid: string;
    id: number;
}
