using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Models;

namespace CynologicalCenter.Data.Repositories.Interfaces
{
    public interface IBreedRepository
    {
        Task<List<Breed>> GetAllAsync();
        Task<Breed?> GetByIdAsync(int id);
        Task AddAsync(Breed breed);
        Task UpdateAsync(Breed breed);
        Task DeleteAsync(int id);
    }
}
