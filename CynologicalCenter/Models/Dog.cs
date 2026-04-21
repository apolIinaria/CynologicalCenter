using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CynologicalCenter.Models
{
    public class Dog
    {
        public int DogId { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public int? BreedId { get; set; }
        public string? BreedName { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public DateTime? LastVaccination { get; set; }
        public string? PhotoPath { get; set; }
    }
}
