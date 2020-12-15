using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OGC.Training.API.Models
{
    public class TrainingChartData
    {
        public int CompletedTraining { get; set; }
        public int TotalEmployees { get; set; }

        public TrainingChartData()
        {
        }
    }
}