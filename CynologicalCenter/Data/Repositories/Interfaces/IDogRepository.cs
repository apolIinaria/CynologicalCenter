using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Models;
using CynologicalCenter.Models.ViewModels;

namespace CynologicalCenter.Data.Repositories.Interfaces
{
    public interface IDogRepository
    {
        Task<List<Dog>> GetAllAsync();
        Task<List<Dog>> GetByOwnerIdAsync(int ownerId);
        Task<Dog?> GetByIdAsync(int id);
        Task AddAsync(Dog dog);
        Task UpdateAsync(Dog dog);
        Task UpdatePhotoAsync(int dogId, string relativePath);
        Task<List<DogProfileViewModel>> GetProfilesViewAsync();
        Task<DogProfileViewModel?> GetProfileViewAsync(int dogId);
        Task<List<ExpiredVaccinationViewModel>> GetExpiredVaccinationViewAsync();
    }
}
