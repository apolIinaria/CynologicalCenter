using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CynologicalCenter.Models.ViewModels
{
    public class ExpiredVaccinationViewModel
    {
        public int DogId { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string BreedName { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime? LastVaccination { get; set; }
        public int DaysSinceVaccination { get; set; }
    }
}
