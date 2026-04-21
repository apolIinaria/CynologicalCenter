using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CynologicalCenter.Models.ViewModels
{
    public class ActiveClientViewModel
    {
        public int OwnerId { get; set; }
        public string FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int DogsCount { get; set; }
        public DateTime? LastVisit { get; set; }
    }
}
