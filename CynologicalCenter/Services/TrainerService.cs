using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Data.Repositories.Interfaces;
using CynologicalCenter.Helpers;
using CynologicalCenter.Models;
using CynologicalCenter.Models.ViewModels;
using CynologicalCenter.Services.Interfaces;

namespace CynologicalCenter.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly ITrainerRepository _trainers;
        public TrainerService(ITrainerRepository trainers)
        {
            _trainers = trainers;
        }

        public Task<List<Trainer>> GetAllAsync()
            => _trainers.GetAllAsync();

        public Task<Trainer?> GetByIdAsync(int id)
            => _trainers.GetByIdAsync(id);

        public Task<List<TrainerKpiViewModel>> GetKpiAsync()
            => _trainers.GetKpiViewAsync();

        public Task<List<int>> GetCourseIdsAsync(int trainerId)
            => _trainers.GetCourseIdsAsync(trainerId);

        public Task SetCoursesAsync(int trainerId, List<int> courseIds)
            => _trainers.SetCoursesAsync(trainerId, courseIds);

        public async Task<(bool Success, string Message)> AddAsync(Trainer trainer)
        {
            if (string.IsNullOrWhiteSpace(trainer.FullName))
                return (false, "ПІБ є обов'язковим полем");

            if (trainer.ExperienceYears.HasValue && trainer.ExperienceYears < 0)
                return (false, "Стаж не може бути від'ємним");

            if (trainer.HireDate.HasValue && trainer.HireDate > DateTime.Today)
                return (false, "Дата найму не може бути в майбутньому");

            try
            {
                await _trainers.AddAsync(trainer);
                return (true, "Тренера успішно додано");
            }
            catch (Exception ex)
            {
                return (false, $"Помилка: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Trainer trainer)
        {
            if (string.IsNullOrWhiteSpace(trainer.FullName))
                return (false, "ПІБ є обов'язковим полем");

            if (trainer.ExperienceYears.HasValue && trainer.ExperienceYears < 0)
                return (false, "Стаж не може бути від'ємним");

            try
            {
                await _trainers.UpdateAsync(trainer);
                return (true, "Дані тренера оновлено");
            }
            catch (Exception ex)
            {
                return (false, $"Помилка: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var trainer = await _trainers.GetByIdAsync(id);
            if (trainer == null)
                return (false, "Тренера не знайдено");

            try
            {
                // Видалення тренера автоматично очистить trainer_courses через ON DELETE CASCADE у БД
                // Тригер trg_audit_trainers_delete запише в AuditLog
                PhotoHelper.DeletePhoto(trainer.PhotoPath);
                await _trainers.DeleteAsync(id);
                return (true, $"Тренера {trainer.FullName} видалено");
            }
            catch (Exception ex)
            {
                return (false, $"Помилка: {ex.Message}");
            }
        }

        public async Task<string> UploadPhotoAsync(int trainerId, string sourceFilePath)
        {
            var trainer = await _trainers.GetByIdAsync(trainerId);
            if (trainer?.PhotoPath != null)
                PhotoHelper.DeletePhoto(trainer.PhotoPath);

            string relativePath = PhotoHelper.SaveTrainerPhoto(trainerId, sourceFilePath);
            await _trainers.UpdatePhotoAsync(trainerId, relativePath);
            return relativePath;
        }
    }
}
