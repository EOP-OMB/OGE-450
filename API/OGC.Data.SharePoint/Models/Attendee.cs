using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGC.Data.SharePoint.Models
{
    public class Attendee
    {
        #region Properties
        public int EventRequestId { get; set; }
        public string EventName { get; set; }

        public string EmployeesName { get; set; } // Attendee
        public string Capacity { get; set; }
        public string EmployeeType { get; set; }
        public bool IsGivingRemarks { get; set; }
        public string Remarks { get; set; }
        public string ReasonForAttending { get; set; }

        public UserInfo Employee { get; set; }
        #endregion
    }
}
