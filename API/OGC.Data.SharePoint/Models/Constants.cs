using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGC.Data.SharePoint.Models
{
    public class Constants
    {
        public class ApplicationName
        {
            public const string OGE_FORM_450 = "OGE450";
            public const string EVENT_CLEARANCE = "Events";
        }

        public class FormFlags
        {
            public const string EXTENDED = "Extended";
            public const string PAPER_COPY = "Paper Copy";
            public const string BLANK_SUBMISSION = "Blank";
            public const string OVERDUE = "Overdue";
            public const string UNCHANGED = "Unchanged";
        }

        public class FormStatus
        {
            public const string NOT_STARTED = "Not-Started";
            public const string DRAFT = "Draft";
            public const string MISSING_INFORMATION = "Missing Information";
            public const string RE_SUBMITTED = "Re-submitted";
            public const string SUBMITTED = "Submitted";
            public const string CERTIFIED = "Certified";
            public const string CANCELED = "Canceled";
            public const string EXPIRED = "Expired";
        }

        public class ExtensionStatus
        {
            public const string PENDING = "Pending";
            public const string APPROVED = "Approved";
            public const string REJECTED = "Rejected";
            public const string CANCELED = "Canceled";
        }

        public class ReportingStatus
        {
            public const string NEW_ENTRANT = "New Entrant";
            public const string ANNUAL = "Annual";
        }

        public class FilerType
        {
            public const string NOT_ASSIGNED = "Not Assigned";
            public const string NON_FILER = "Non-Filer";
            public const string _278_FILER = "278 Filer";
            public const string _450_FILER = "450 Filer";
        }

        public class EmployeeStatus
        {
            public const string ACTIVE = "Active";
            public const string ON_DETAIL = "On Detail";
            public const string ON_LEAVE = "On Leave";
            public const string DETAILEE = "Detailee";
            public const string INACTIVE = "Inactive";
        }

        public class TrainingType
        {
            public const string ANNUAL = "Annual";
            public const string INITIAL = "Initial";
        }

        public class EventRequestStatus
        {
            public const string DRAFT = "Draft";
            public const string UNASSIGNED = "Open - Unassigned";
            public const string OPEN = "Open";
            public const string CLOSED = "Closed - Other";
            public const string APPROVED = "Closed - Approved";
            public const string CANCELED = "Closed - Canceled";
            public const string WITHDRAWN = "Closed - Withdrawn";
        }

        public class AttachmentType
        {
            public const string TRAVEL_FORMS = "Travel Form";
            public const string INVITATIONS = "Invitation";
            public const string OTHER = "Other";
        }
    }
}
