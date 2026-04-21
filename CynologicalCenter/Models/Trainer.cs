using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CynologicalCenter.Models
{
    public class Trainer
    {
        public int TrainerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? ExperienceYears { get; set; }
        public DateTime? HireDate { get; set; }
        public string? PhotoPath { get; set; }
    }
}
