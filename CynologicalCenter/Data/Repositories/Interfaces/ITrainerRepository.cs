using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Models;
using CynologicalCenter.Models.ViewModels;

namespace CynologicalCenter.Data.Repositories.Interfaces
{
    public interface ITrainerRepository
    {
        Task<List<Trainer>> GetAllAsync();
        Task<Trainer?> GetByIdAsync(int id);
        Task AddAsync(Trainer trainer);
        Task UpdateAsync(Trainer trainer);
        Task DeleteAsync(int id);
        Task UpdatePhotoAsync(int trainerId, string relativePath);
        Task<List<int>> GetCourseIdsAsync(int trainerId);
        Task SetCoursesAsync(int trainerId, List<int> courseIds);
        Task<List<TrainerKpiViewModel>> GetKpiViewAsync();
    }
}
