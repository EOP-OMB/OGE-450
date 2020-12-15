using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OGC.Data.SharePoint.Models;

namespace OGC.Training.API.Models
{
    public class NotificationsVm
    {
        public List<Notifications> notifications;
        public int Records;
        public int TotalRecords;
    }
}