export class FormStatus
{
    public static NOT_AVAILABLE: string = "Not Available";
    public static NOT_STARTED: string = "Not-Started";
    public static DRAFT: string = "Draft";
    public static MISSING_INFORMATION: string = "Missing Information";
    public static RE_SUBMITTED: string = "Re-submitted";
    public static SUBMITTED: string = "Submitted";
    public static CERTIFIED: string = "Certified";
    public static CANCELED: string = "Canceled";
    public static EXPIRED: string = "Expired";
}

export class ExtensionStatus {
    public static PENDING: string = "Pending";
    public static APPROVED: string = "Approved";
    public static REJECTED: string = "Rejected";
    public static CANCELED: string = "Canceled";
}

export class ReportingStatus {
    public static NEW_ENTRANT: string = "New Entrant";
    public static ANNUAL: string = "Annual";
}

export class FilerTypes {
    public static NOT_ASSIGNED: string = "Not Assigned";
    public static FORM_450_FILER: string = "450 Filer";
    public static FORM_278_FILER: string = "278 Filer";
    public static _450_WAIVED: string = "450 Waived";
    public static NON_FILER: string = "Non-Filer";
}

export class EmployeeStatus {
    public static ACTIVE: string = "Active";
    public static ON_DETAIL: string = "On Detail";
    public static ON_LEAVE: string = "On Leave";
    public static DETAILEE: string = "Detailee";
    public static INACTIVE: string = "Inactive";
}

export class RecipientTypes {
    public static USER: string = "User";
    public static GROUP: string = "Group";
}

export class FormFlags {
    public static EXTENDED: string = "Extended";
    public static PAPER_COPY: string = "Paper Copy";
    public static BLANK_SUBMISSION: string = "Blank";
    public static OVERDUE: string = "Overdue";
    public static UNCHANGED: string = "Unchanged";
}

import { SelectItem } from 'primeng/primeng';

export class Lookups {
    public static YEARS: SelectItem[];
    public static EXTENSION_STATUSES: SelectItem[];
    public static FILER_TYPES: SelectItem[];
    public static EMPLOYEE_STATUSES: SelectItem[];
    public static REPORTING_STATUSES: SelectItem[];
    public static FORM_STATUSES: SelectItem[];
    public static RECIPIENT_TYPES: SelectItem[];
    public static DIVISIONS: SelectItem[];
    public static FORM_FLAGS: SelectItem[];

    public static initialize() {
        Lookups.YEARS = [];

        var year = new Date().getFullYear();

        Lookups.YEARS.push({ label: 'All', value: null });

        for (var i = year; i >= year-5; i--) {
            Lookups.YEARS.push({ label: i.toString(), value: i.toString() });
        }

        Lookups.FORM_FLAGS = [];
        Lookups.FORM_FLAGS.push({ label: '', value: null });
        Lookups.FORM_FLAGS.push({ label: FormFlags.EXTENDED, value: FormFlags.EXTENDED });
        Lookups.FORM_FLAGS.push({ label: FormFlags.BLANK_SUBMISSION, value: FormFlags.BLANK_SUBMISSION });
        Lookups.FORM_FLAGS.push({ label: FormFlags.PAPER_COPY, value: FormFlags.PAPER_COPY });
        Lookups.FORM_FLAGS.push({ label: FormFlags.OVERDUE, value: FormFlags.OVERDUE });
        Lookups.FORM_FLAGS.push({ label: FormFlags.UNCHANGED, value: FormFlags.UNCHANGED });

        Lookups.EXTENSION_STATUSES = [];
        Lookups.EXTENSION_STATUSES.push({ label: 'All', value: null });
        Lookups.EXTENSION_STATUSES.push({ label: ExtensionStatus.PENDING, value: ExtensionStatus.PENDING });
        Lookups.EXTENSION_STATUSES.push({ label: ExtensionStatus.REJECTED, value: ExtensionStatus.REJECTED });
        Lookups.EXTENSION_STATUSES.push({ label: ExtensionStatus.APPROVED, value: ExtensionStatus.APPROVED });
        Lookups.EXTENSION_STATUSES.push({ label: ExtensionStatus.CANCELED, value: ExtensionStatus.CANCELED });

        Lookups.FILER_TYPES = [];
        Lookups.FILER_TYPES.push({ label: 'All', value: null });
        Lookups.FILER_TYPES.push({ label: FilerTypes.NOT_ASSIGNED, value: FilerTypes.NOT_ASSIGNED });
        Lookups.FILER_TYPES.push({ label: FilerTypes.FORM_450_FILER, value: FilerTypes.FORM_450_FILER });
        Lookups.FILER_TYPES.push({ label: FilerTypes.FORM_278_FILER, value: FilerTypes.FORM_278_FILER });
        Lookups.FILER_TYPES.push({ label: FilerTypes.NON_FILER, value: FilerTypes.NON_FILER });
        Lookups.FILER_TYPES.push({ label: FilerTypes._450_WAIVED, value: FilerTypes._450_WAIVED });

        Lookups.EMPLOYEE_STATUSES = [];
        Lookups.EMPLOYEE_STATUSES.push({ label: 'All', value: null });
        Lookups.EMPLOYEE_STATUSES.push({ label: EmployeeStatus.ACTIVE, value: EmployeeStatus.ACTIVE });
        Lookups.EMPLOYEE_STATUSES.push({ label: EmployeeStatus.ON_DETAIL, value: EmployeeStatus.ON_DETAIL });
        Lookups.EMPLOYEE_STATUSES.push({ label: EmployeeStatus.ON_LEAVE, value: EmployeeStatus.ON_LEAVE });
        Lookups.EMPLOYEE_STATUSES.push({ label: EmployeeStatus.DETAILEE, value: EmployeeStatus.DETAILEE });
        Lookups.EMPLOYEE_STATUSES.push({ label: EmployeeStatus.INACTIVE, value: EmployeeStatus.INACTIVE });

        Lookups.REPORTING_STATUSES = [];
        Lookups.REPORTING_STATUSES.push({ label: 'All', value: null });
        Lookups.REPORTING_STATUSES.push({ label: ReportingStatus.NEW_ENTRANT, value: ReportingStatus.NEW_ENTRANT });
        Lookups.REPORTING_STATUSES.push({ label: ReportingStatus.ANNUAL, value: ReportingStatus.ANNUAL });

        Lookups.FORM_STATUSES = [];
        Lookups.FORM_STATUSES.push({ label: 'All', value: null });
        Lookups.FORM_STATUSES.push({ label: FormStatus.NOT_AVAILABLE, value: FormStatus.NOT_AVAILABLE });
        Lookups.FORM_STATUSES.push({ label: FormStatus.NOT_STARTED, value: FormStatus.NOT_STARTED });
        Lookups.FORM_STATUSES.push({ label: FormStatus.DRAFT, value: FormStatus.DRAFT });
        Lookups.FORM_STATUSES.push({ label: FormStatus.SUBMITTED, value: FormStatus.SUBMITTED });
        Lookups.FORM_STATUSES.push({ label: FormStatus.MISSING_INFORMATION, value: FormStatus.MISSING_INFORMATION });
        Lookups.FORM_STATUSES.push({ label: FormStatus.RE_SUBMITTED, value: FormStatus.RE_SUBMITTED });
        Lookups.FORM_STATUSES.push({ label: FormStatus.CERTIFIED, value: FormStatus.CERTIFIED });
        Lookups.FORM_STATUSES.push({ label: FormStatus.CANCELED, value: FormStatus.CANCELED });
        Lookups.FORM_STATUSES.push({ label: FormStatus.EXPIRED, value: FormStatus.EXPIRED });

        Lookups.RECIPIENT_TYPES = [];
        Lookups.RECIPIENT_TYPES.push({ label: RecipientTypes.USER, value: RecipientTypes.USER });
        Lookups.RECIPIENT_TYPES.push({ label: RecipientTypes.GROUP, value: RecipientTypes.GROUP });

        Lookups.DIVISIONS = [];
        Lookups.DIVISIONS.push({ label: 'All', value: null });
        Lookups.DIVISIONS.push({ label: 'N/A', value: 'N/A' });
        Lookups.DIVISIONS.push({ label: 'BR', value: 'BR' });
        Lookups.DIVISIONS.push({ label: 'DO', value: 'DO' });
        Lookups.DIVISIONS.push({ label: 'ECON', value: 'ECON' });
        Lookups.DIVISIONS.push({ label: 'EIMLP', value: 'EIMLP' });
        Lookups.DIVISIONS.push({ label: 'GGP', value: 'GGP' });
        Lookups.DIVISIONS.push({ label: 'HEALTH', value: 'HEALTH' });
        Lookups.DIVISIONS.push({ label: 'IAD', value: 'IAD' });
        Lookups.DIVISIONS.push({ label: 'IPEC', value: 'IPEC' });
        Lookups.DIVISIONS.push({ label: 'LRD', value: 'LRD' });
        Lookups.DIVISIONS.push({ label: 'MOD', value: 'MOD' });
        Lookups.DIVISIONS.push({ label: 'NRP', value: 'NRP' });
        Lookups.DIVISIONS.push({ label: 'NSD', value: 'NSD' });
        Lookups.DIVISIONS.push({ label: 'OFCIO', value: 'OFCIO' });
        Lookups.DIVISIONS.push({ label: 'OFFM', value: 'OFFM' });
        Lookups.DIVISIONS.push({ label: 'OPPM', value: 'OPPM' });
        Lookups.DIVISIONS.push({ label: 'OFPP', value: 'OFPP' });
        Lookups.DIVISIONS.push({ label: 'OGC', value: 'OGC' });
        Lookups.DIVISIONS.push({ label: 'OIRA', value: 'OIRA' });
        Lookups.DIVISIONS.push({ label: 'USDS', value: 'USDS' });
    }
}
Lookups.initialize();