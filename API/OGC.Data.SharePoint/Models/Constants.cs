using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGC.Data.SharePoint.Models
{
    public class Constants
    {
        public class FormFlags
        {
            public const string EXTENDED = "Extended";
            public const string PAPER_COPY = "Paper Copy";
            public const string BLANK_SUBMISSION = "Blank";
            public const string OVERDUE = "Overdue";
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

        public class TrainingType
        {
            public const string ANNUAL = "Annual";
            public const string INITIAL = "Initial";
        }
    }
}
