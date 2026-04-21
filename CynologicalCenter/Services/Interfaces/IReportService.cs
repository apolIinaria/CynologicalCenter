using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace CynologicalCenter.Services.Interfaces
{
    public interface IReportService
    {
        Task<DataSet> GetTrainerScheduleAsync(int trainerId, DateTime date);
        Task<DataSet> GetDogReportAsync(int dogId);
        Task<DataSet> GetAuditAnalysisAsync(DateTime from, DateTime to);
        Task<(DataSet Data, int TotalIncidents)> GetSecurityReportAsync(int hoursBack);
    }
}
