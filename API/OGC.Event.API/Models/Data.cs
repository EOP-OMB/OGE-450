using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OGC.Event.API.Models
{
    public class Data
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Info { get; set; }
        public DateTime DateCreated { get; set; }

        public static List<Data> GetAll()
        {
            var data = new List<Data>();

            data.Add(new Models.Data() { Id = 1, Name = "Element 1", Description = "This is the first element", Info = "Some info about element 1", DateCreated = new DateTime(2017, 11, 1) });
            data.Add(new Models.Data() { Id = 2, Name = "Element 2", Description = "This is the second element", Info = "Some info about element 2", DateCreated = new DateTime(2017, 11, 1) });
            data.Add(new Models.Data() { Id = 3, Name = "Element 3", Description = "This is the third element", Info = "Some info about element 3", DateCreated = new DateTime(2017, 11, 1) });

            return data;
        }
    }
}