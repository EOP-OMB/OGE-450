export class EventStatus
{
    public static DRAFT: string = "Draft";
    public static UNASSIGNED: string = "Open - Unassigned";
    public static OPEN: string = "Open";
    public static CLOSED: string = "Closed - Other";
    public static APPROVED: string = "Closed - Approved";
    public static CANCELED: string = "Closed - Canceled";
    public static WITHDRAWN: string = "Closed - Withdrawn";
    public static DENIED: string = "Closed - Denied";
}

export class EventDateFilter {
    public static PAST_EVENTS: string = "Past Events";
    public static NEXT_WEEK: string = "Next 7 Days";
    public static SEVEN_TO_THIRTY: string = "7 to 30 Days";
    public static MONTH_PLUS: string = "30+ Days";
}

export class FilerTypes {
    public static NOT_ASSIGNED: string = "Not Assigned";
    public static FORM_450_FILER: string = "450 Filer";
    public static FORM_278_FILER: string = "278 Filer";
    public static NON_FILER: string = "Non-Filer";
}

export class RecipientTypes {
    public static USER: string = "User";
    public static GROUP: string = "Group";
}

export class AttachmentTypes {
    public static TRAVEL_FORMS: string = "Travel Form";
    public static INVIATIONS: string = "Invitation";
    public static OTHER: string = "Other";
}

import { SelectItem } from 'primeng/primeng';

export class Lookups {
    public static YEARS: SelectItem[];
    public static EXTENSION_STATUSES: SelectItem[];
    public static FILER_TYPES: SelectItem[];
    public static REPORTING_STATUSES: SelectItem[];
    public static EVENT_STATUSES: SelectItem[];
    public static RECIPIENT_TYPES: SelectItem[];
    public static EVENT_DATE_FILTER: SelectItem[];
    
    public static initialize() {
        Lookups.YEARS = [];

        var year = new Date().getFullYear();

        Lookups.YEARS.push({ label: 'All', value: null });

        for (var i = year; i >= year - 5; i--) {
            Lookups.YEARS.push({ label: i.toString(), value: i.toString() });
        }

        Lookups.FILER_TYPES = [];
        Lookups.FILER_TYPES.push({ label: 'All', value: null });
        Lookups.FILER_TYPES.push({ label: FilerTypes.NOT_ASSIGNED, value: FilerTypes.NOT_ASSIGNED });
        Lookups.FILER_TYPES.push({ label: FilerTypes.FORM_450_FILER, value: FilerTypes.FORM_450_FILER });
        Lookups.FILER_TYPES.push({ label: FilerTypes.FORM_278_FILER, value: FilerTypes.FORM_278_FILER });
        Lookups.FILER_TYPES.push({ label: FilerTypes.NON_FILER, value: FilerTypes.NON_FILER });

        Lookups.EVENT_STATUSES = [];
        Lookups.EVENT_STATUSES.push({ label: 'All', value: null });
        Lookups.EVENT_STATUSES.push({ label: EventStatus.DRAFT, value: EventStatus.DRAFT });
        Lookups.EVENT_STATUSES.push({ label: EventStatus.UNASSIGNED, value: EventStatus.UNASSIGNED });
        Lookups.EVENT_STATUSES.push({ label: EventStatus.OPEN, value: EventStatus.OPEN });
        Lookups.EVENT_STATUSES.push({ label: EventStatus.CLOSED, value: EventStatus.CLOSED });
        Lookups.EVENT_STATUSES.push({ label: EventStatus.APPROVED, value: EventStatus.APPROVED });
        Lookups.EVENT_STATUSES.push({ label: EventStatus.CANCELED, value: EventStatus.CANCELED });
        Lookups.EVENT_STATUSES.push({ label: EventStatus.WITHDRAWN, value: EventStatus.WITHDRAWN });
        Lookups.EVENT_STATUSES.push({ label: EventStatus.DENIED, value: EventStatus.DENIED });

        Lookups.RECIPIENT_TYPES = [];
        Lookups.RECIPIENT_TYPES.push({ label: RecipientTypes.USER, value: RecipientTypes.USER });
        Lookups.RECIPIENT_TYPES.push({ label: RecipientTypes.GROUP, value: RecipientTypes.GROUP });

        Lookups.EVENT_DATE_FILTER = [];

        Lookups.EVENT_DATE_FILTER.push({ label: 'All', value: null });
        Lookups.EVENT_DATE_FILTER.push({ label: EventDateFilter.PAST_EVENTS, value: EventDateFilter.PAST_EVENTS });
        Lookups.EVENT_DATE_FILTER.push({ label: EventDateFilter.NEXT_WEEK, value: EventDateFilter.NEXT_WEEK });
        Lookups.EVENT_DATE_FILTER.push({ label: EventDateFilter.SEVEN_TO_THIRTY, value: EventDateFilter.SEVEN_TO_THIRTY });
        Lookups.EVENT_DATE_FILTER.push({ label: EventDateFilter.MONTH_PLUS, value: EventDateFilter.MONTH_PLUS });
    }
}
Lookups.initialize();
