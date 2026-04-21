using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Models.ViewModels;
using CynologicalCenter.Models;

namespace CynologicalCenter.Services.Interfaces
{
    public interface ISessionService
    {
        Task<(bool Success, string Message)> CompleteAsync(
            int sessionId, decimal grade, string comment);
        Task<(bool Success, string Message)> CancelAsync(int sessionId);
        Task<List<Session>> GetAllAsync();
        Task<List<Session>> GetByStatusAsync(string status);
        Task<List<Session>> GetByDogIdAsync(int dogId);
        Task<List<Session>> GetByTrainerIdAsync(int trainerId);
        Task<List<PublicScheduleViewModel>> GetPublicScheduleAsync();
    }
}
