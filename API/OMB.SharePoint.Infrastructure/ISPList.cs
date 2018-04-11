using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMB.SharePoint.Infrastructure
{
    public interface ISPList
    {
        int Id { get; set; }
        string ListName { get; set; }
        string Title { get; set; }

        void MapFromList(ListItem item, bool includeChildren = false);

        void MapToList(ListItem item);
    }
}
