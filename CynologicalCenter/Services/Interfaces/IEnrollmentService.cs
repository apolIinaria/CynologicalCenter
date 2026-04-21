using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CynologicalCenter.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<(bool Success, string Message)> EnrollAsync(
            int dogId, int trainerId, int courseId, DateTime sessionTime);
    }
}
