using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Models;
using CynologicalCenter.Models.ViewModels;

namespace CynologicalCenter.Data.Repositories.Interfaces
{
    public interface IOwnerRepository
    {
        Task<List<Owner>> GetAllAsync();
        Task<List<Owner>> GetActiveAsync();
        Task<Owner?> GetByIdAsync(int id);
        Task AddAsync(Owner owner);
        Task UpdateAsync(Owner owner);
        Task<List<ActiveClientViewModel>> GetActiveClientsViewAsync();
    }
}
