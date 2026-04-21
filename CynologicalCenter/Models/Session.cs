using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CynologicalCenter.Models
{
    public class Session
    {
        public int SessionId { get; set; }
        public int? DogId { get; set; }
        public string? DogNickname { get; set; }
        public int? TrainerId { get; set; }
        public string? TrainerName { get; set; }
        public int? CourseId { get; set; }
        public string? CourseName { get; set; }
        public DateTime SessionDatetime { get; set; }
        public string Status { get; set; } = "Заплановано";
        public decimal? Grade { get; set; }
        public string? Comment { get; set; }
        public int DurationMinutes { get; set; } = 60;
    }
}
