using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Models.ViewModels;
using CynologicalCenter.Models;

namespace CynologicalCenter.Services.Interfaces
{
    public interface IOwnerService
    {
        Task<List<Owner>> GetAllAsync();
        Task<List<Owner>> GetActiveAsync();
        Task<Owner?> GetByIdAsync(int id);
        Task<(bool Success, string Message)> AddAsync(Owner owner);
        Task<(bool Success, string Message)> UpdateAsync(Owner owner);
        Task<(bool Success, string Message)> DeactivateAsync(int ownerId);
        Task<List<ActiveClientViewModel>> GetActiveClientsViewAsync();
    }
}
