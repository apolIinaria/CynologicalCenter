using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Models.ViewModels;
using CynologicalCenter.Models;

namespace CynologicalCenter.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<List<Trainer>> GetAllAsync();
        Task<Trainer?> GetByIdAsync(int id);
        Task<(bool Success, string Message)> AddAsync(Trainer trainer);
        Task<(bool Success, string Message)> UpdateAsync(Trainer trainer);
        Task<(bool Success, string Message)> DeleteAsync(int id);
        Task<string> UploadPhotoAsync(int trainerId, string sourceFilePath);
        Task<List<int>> GetCourseIdsAsync(int trainerId);
        Task SetCoursesAsync(int trainerId, List<int> courseIds);
        Task<List<TrainerKpiViewModel>> GetKpiAsync();
    }
}
