using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Data.Repositories.Interfaces;
using CynologicalCenter.Data.StoredProcedures;
using CynologicalCenter.Models;
using CynologicalCenter.Models.ViewModels;
using CynologicalCenter.Services.Interfaces;

namespace CynologicalCenter.Services
{
    public class SessionService : ISessionService
    {
        private readonly ProcedureCaller _procedures;
        private readonly ISessionRepository _sessions;
        public SessionService(ProcedureCaller procedures, ISessionRepository sessions)
        {
            _procedures = procedures;
            _sessions = sessions;
        }

        public async Task<(bool Success, string Message)> CompleteAsync(
            int sessionId, decimal grade, string comment)
        {
            if (grade < 1 || grade > 10)
                return (false, "Оцінка має бути від 1 до 10");
            try
            {
                string msg = await _procedures.CompleteSessionAsync(
                    sessionId, grade, comment ?? string.Empty);
                bool success = msg.StartsWith("Успішно");
                return (success, msg);
            }
            catch (Exception ex)
            {
                return (false, $"Помилка БД: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> CancelAsync(int sessionId)
        {
            var session = await _sessions.GetByIdAsync(sessionId);
            if (session == null)
                return (false, "Заняття не знайдено");
            if (session.Status == "Виконано")
                return (false, "Не можна скасувати вже виконане заняття");
            if (session.Status == "Скасовано")
                return (false, "Заняття вже скасовано");

            try
            {
                await _sessions.UpdateStatusAsync(sessionId, "Скасовано");
                return (true, "Заняття успішно скасовано");
            }
            catch (Exception ex)
            {
                return (false, $"Помилка: {ex.Message}");
            }
        }

        public Task<List<Session>> GetAllAsync()
            => _sessions.GetAllAsync();

        public Task<List<Session>> GetByStatusAsync(string status)
            => _sessions.GetByStatusAsync(status);

        public Task<List<Session>> GetByDogIdAsync(int dogId)
            => _sessions.GetByDogIdAsync(dogId);

        public Task<List<Session>> GetByTrainerIdAsync(int trainerId)
            => _sessions.GetByTrainerIdAsync(trainerId);

        public Task<List<PublicScheduleViewModel>> GetPublicScheduleAsync()
            => _sessions.GetPublicScheduleViewAsync();
    }
}
