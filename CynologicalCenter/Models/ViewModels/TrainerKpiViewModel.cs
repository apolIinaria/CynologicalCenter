using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CynologicalCenter.Models.ViewModels
{
    public class TrainerKpiViewModel
    {
        public int TrainerId { get; set; }
        public string TrainerName { get; set; }
        public int TotalSessions { get; set; }
        public decimal? AvgGrade { get; set; }
        public long Rating { get; set; }
    }
}
