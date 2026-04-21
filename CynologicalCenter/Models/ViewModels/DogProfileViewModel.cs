using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CynologicalCenter.Models.ViewModels
{
    public class DogProfileViewModel
    {
        public int DogId { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int AgeMonths { get; set; }
        public string Owner { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public decimal? AvgGrade { get; set; }
        public DateTime? LastSession { get; set; }
        public DateTime? LastVaccination { get; set; }
        public int DaysSinceVaccination { get; set; }
    }
}
