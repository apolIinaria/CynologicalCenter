using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CynologicalCenter.Services.Interfaces
{
    public interface ISecurityService
    {
        Task<(bool Success, string Role)> LoginAsync(string username, string password);
    }
}
