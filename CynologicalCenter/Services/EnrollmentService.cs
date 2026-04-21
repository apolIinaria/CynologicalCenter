using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Data.Repositories.Interfaces;
using CynologicalCenter.Services.Interfaces;
using CynologicalCenter.Data.StoredProcedures;

namespace CynologicalCenter.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly ProcedureCaller _procedures;
        private readonly IDogRepository _dogs;
        public EnrollmentService(ProcedureCaller procedures, IDogRepository dogs)
        {
            _procedures = procedures;
            _dogs = dogs;
        }

        public async Task<(bool Success, string Message)> EnrollAsync(
            int dogId, int trainerId, int courseId, DateTime sessionTime)
        {
            if (sessionTime < DateTime.Now)
                return (false, "Не можна записати на минулу дату");

            var dog = await _dogs.GetByIdAsync(dogId);
            if (dog == null)
                return (false, "Собаку не знайдено");

            // Основна логіка — в процедурі sp_enroll_dog
            // Вона сама перевірить: власника, вік, вакцинацію, допуск тренера (тригер), перетин розкладу (тригер)
            try
            {
                string msg = await _procedures.EnrollDogAsync(
                    dogId, trainerId, courseId, sessionTime);
                bool success = msg.StartsWith("Успішно");
                return (success, msg);
            }
            catch (Exception ex)
            {
                return (false, $"Помилка БД: {ex.Message}");
            }
        }
    }
}
