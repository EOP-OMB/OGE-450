import { AppUser } from '../security/app-user';

export class EventRequest {
    constructor() {
        this.typeOfOrg = 0;
        this.typeOfHost = 0;
    }

    id: number;
    title: string;

    submittedBy: string;
    submitter: string;
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

        this.employee = new AppUser();
    }

    eventRequestId: number;
    eventName: string;
    employee: AppUser;
    capacity: string;
    employeeType: string;
    isGivingRemarks: boolean;
    remarks: string;
    reasonForAttending: string;

    // UI Only
    selected: boolean;
}

export class Attachment {
    eventRequestId: number;
    fileName: string;
    size: number;
    typeOfAttachment: string;
    attachmentGuid: string;
    id: number;
}
