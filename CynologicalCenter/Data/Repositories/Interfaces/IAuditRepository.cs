using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Models;

namespace CynologicalCenter.Data.Repositories.Interfaces
{
    public interface IAuditRepository
    {
        Task<List<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<List<AuditLog>> GetByTableAsync(string tableName);
        Task<List<AuditLog>> GetErrorsAsync();
    }
}
