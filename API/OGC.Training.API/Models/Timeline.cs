using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OGC.Training.API.Models
{
    public class Timeline
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public int Year
        {
            get
            {
                return Date == null ? 0 : Date.Value.Year;
            }
        }
    }
}