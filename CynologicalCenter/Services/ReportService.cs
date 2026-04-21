using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Data.StoredProcedures;
using CynologicalCenter.Services.Interfaces;
using System.Data;

namespace CynologicalCenter.Services
{
    public class ReportService : IReportService
    {
        private readonly ProcedureCaller _procedures;
        public ReportService(ProcedureCaller procedures)
        {
            _procedures = procedures;
        }

        // Розклад тренера на день
        public Task<DataSet> GetTrainerScheduleAsync(int trainerId, DateTime date)
            => _procedures.GetTrainerScheduleAsync(trainerId, date);

        // Повний профіль собаки
        public Task<DataSet> GetDogReportAsync(int dogId)
            => _procedures.GenerateDogReportAsync(dogId);

        // Аналіз журналу аудиту
        public Task<DataSet> GetAuditAnalysisAsync(DateTime from, DateTime to)
            => _procedures.AnalyzeLogsAsync(from, to);

        // Перевірка інцидентів безпеки
        public Task<(DataSet Data, int TotalIncidents)> GetSecurityReportAsync(int hoursBack)
            => _procedures.CheckSecurityAsync(hoursBack);
    }
}
