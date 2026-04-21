using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Models;
using CynologicalCenter.Models.ViewModels;

namespace CynologicalCenter.Data.Repositories.Interfaces
{
    public interface ISessionRepository
    {
        Task<List<Session>> GetAllAsync();
        Task<Session?> GetByIdAsync(int id);
        Task<List<Session>> GetByDogIdAsync(int dogId);
        Task<List<Session>> GetByTrainerIdAsync(int trainerId);
        Task<List<Session>> GetByStatusAsync(string status);
        Task UpdateStatusAsync(int sessionId, string status);
        Task<List<PublicScheduleViewModel>> GetPublicScheduleViewAsync();
    }
}
