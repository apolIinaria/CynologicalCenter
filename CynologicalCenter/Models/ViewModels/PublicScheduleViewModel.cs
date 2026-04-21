using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CynologicalCenter.Models.ViewModels
{
    public class PublicScheduleViewModel
    {
        public string SessionTime { get; set; } = string.Empty;
        public string Trainer { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string Dog { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
    }
}
